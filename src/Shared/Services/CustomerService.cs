using Application.Constants;
using Application.Dtos.Customer;
using Application.DTOs.Authorization.Accounts;
using Application.DTOs.Common;
using Application.DTOs.Pagination;
using Application.DTOs.Verification;
using Application.Enums;
using Application.Extensions;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IVerificationService _verificationService;
        private readonly IOrderRepository _orderRepository;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, IConfiguration configuration, IVerificationService verificationService, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _configuration = configuration;
            _verificationService = verificationService;
            _orderRepository = orderRepository;
        }

        public async Task<CustomerDto> GetLoginResultAsync(string username, string password)
        {
            password = Utils.ComputeHash(password);

            var customer = await _customerRepository.FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == password);

            var customerDto = _mapper.Map<CustomerDto>(customer);
            if (customerDto == null)
                throw new Exception(CustomResponseMessage.InvalidUserNameOrPassword);

            customerDto.AvatarUrl = !string.IsNullOrEmpty(customer.Avatar)
                ? S3Path.GetS3Url(_configuration, customer.Avatar, BucketType.UserAvatar)
                : "";

            return customerDto;
        }

        public async Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input)
        {
            var user = await _customerRepository.FirstOrDefaultAsync(u => u.Email.Equals(input.EmailAddress));

            if (user == null)
                throw new Exception(UserAccountMessage.EmailNotExist);

            return await _verificationService.SaveAndSendVerificationByEmail(new SendVerificationEmailModel
            {
                Email = user.Email,
                Mode = UserVerificationMode.ForgotPassword
            }, user.Id);
        }

        public async Task<ResetPasswordOutput> ValidResetPasswordCode(ValidateResetPasswordCodeInput input)
        {
            if (string.IsNullOrEmpty(input.c) && string.IsNullOrEmpty(input.ResetCode))
            {
                throw new Exception(CustomResponseMessage.InvalidParams);
            }

            EmailConfirmVerificationOutput emailConfirm;

            if (!string.IsNullOrEmpty(input.c))
            {
                var verificationTokenInput = new VerifyUserEmailTokenInput(input.c);

                verificationTokenInput.ResolveParameters();

                if (verificationTokenInput.Mode != UserVerificationMode.ForgotPassword)
                {
                    throw new Exception(CustomResponseMessage.NotAllowed);
                }

                emailConfirm = await _verificationService.VerifyUserEmailByToken(new EmailVerificationModel
                {
                    Email = verificationTokenInput.Email,
                    Token = verificationTokenInput.Token
                });
                if (!emailConfirm.VerifiedEmail)
                    throw new Exception(emailConfirm.Message);
            }
            else if (!string.IsNullOrEmpty(input.ResetCode) && !string.IsNullOrEmpty(input.Email))
            {
                emailConfirm = await _verificationService.VerifyUserEmailByCode(new EmailVerificationModel
                {
                    Email = input.Email,
                    Code = input.ResetCode
                }, UserVerificationMode.ForgotPassword);

                if (!emailConfirm.VerifiedEmail)
                    throw new Exception(emailConfirm.Message);
            }
            else
            {
                throw new Exception(CustomResponseMessage.InvalidParams);
            }

            if (!emailConfirm.UserId.HasValue)
            {
                throw new Exception(CustomResponseMessage.UserDoesNotExist);
            }

            var token = await this.SetNewPasswordResetToken(emailConfirm.UserId.Value);

            var output = new ResetPasswordOutput
            {
                Token = token,
                Email = emailConfirm.Email
            };
            return output;
        }

        public async Task<string> ResetPassword(ResetPasswordInput input)
        {
            var user = await _customerRepository.FirstOrDefaultAsync(u => u.Email == input.Email && u.PasswordResetToken == input.ResetToken);

            if (user == null)
                throw new Exception(UserAccountMessage.EmailNotExist);

            if (user.PasswordResetExpiration < DateTime.UtcNow)
            {
                throw new Exception(CustomResponseMessage.TokenExpired);
            }

            await this.ChangePasswordAsync(user.Id, input.NewPassword);
            // await _emailService.ChangePasswordSuccessfullyAsync(input.Email, user.FirstName);

            return user.Email;
        }

        private async Task<string> SetNewPasswordResetToken(int userId)
        {
            var token = Guid.NewGuid().ToString("N").Truncate(328);
            var expiration = DateTime.UtcNow.AddMinutes(15);
            var user = await _customerRepository.FirstOrDefaultAsync(x => x.Id == userId);
            user.PasswordResetToken = token;
            user.PasswordResetExpiration = expiration;
            await _customerRepository.UpdateAsync(user);
            return token;
        }

        public async Task ChangePasswordAsync(int userId, string password)
        {
            password = Utils.ComputeHash(password);

            var user = await _customerRepository.FirstOrDefaultAsync(x => x.Id == userId);
            user.PasswordHash = password;
            await _customerRepository.UpdateAsync(user);
        }

        public async Task<CustomerDto> Register(CustomerDto input)
        {
            return await Insert(input);
        }

        public async Task<CustomerDto> GetById(int id)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(u => u.Id == id);

            if (customer == null)
                throw new Exception("CUSTOMER_DOES_NOT_EXIST");

            customer.Orders = await _orderRepository
                .FindByIncludes(x => x.CustomerId == customer.Id, x => x.OrderDetails).ToListAsync();

            var customerDto = _mapper.Map<CustomerDto>(customer);

            foreach (var customerDtoOrder in customerDto.Orders)
            {
                customerDtoOrder.OrderDetails.ForEach(od =>
                {
                    // todo: Remove test data
                    if (od.ProductImage.Contains("http"))
                    {
                        od.ProductImageUrl = od.ProductImage;
                        return;
                    }

                    od.ProductImageUrl = !string.IsNullOrEmpty(od.ProductImage)
                        ? S3Path.GetS3Url(_configuration, od.ProductImage)
                        : "";
                });
            }

            customerDto.AvatarUrl = !string.IsNullOrEmpty(customer.Avatar)
                ? S3Path.GetS3Url(_configuration, customer.Avatar, BucketType.UserAvatar)
                : "";
            return customerDto;
        }

        public async Task<PagedResultDto<CustomerDto>> GetAll(GetCustomerInput input)
        {
            var customers = await _customerRepository
                .AsQueryable()
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Where(x => string.IsNullOrEmpty(input.KeySearch)
                            || ((x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(input.KeySearch.Trim())
                                || x.Code.ToLower().Contains(input.KeySearch.ToLower())
                                || x.PhoneNumber.Contains(input.KeySearch)))
                .ToListAsync();
            if (customers is not { Count: > 0 })
            {
                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
                return new PagedResultDto<CustomerDto>
                {
                    Items = customerDtos,
                    TotalCount = customerDtos.Count
                };
            }

            var customerDto = _mapper.Map<List<CustomerDto>>(customers);

            foreach (var customer in customerDto)
            {
                customer.AvatarUrl = !string.IsNullOrEmpty(customer.Avatar)
                    ? S3Path.GetS3Url(_configuration, customer.Avatar, BucketType.UserAvatar)
                    : "";
            }

            return new PagedResultDto<CustomerDto>
            {
                Items = customerDto,
                TotalCount = string.IsNullOrEmpty(input.KeySearch)
                    ? _customerRepository.Count()
                    : _customerRepository.Count(x => string.IsNullOrEmpty(input.KeySearch)
                                                      || ((x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(input.KeySearch.Trim())
                                                          || x.Code.ToLower().Contains(input.KeySearch.ToLower())
                                                          || x.PhoneNumber.Contains(input.KeySearch)))
            };
        }

        public async Task<List<CustomerDto>> GetCustomersForSelect(string? keySearch, int pageSize)
        {
            if (!string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            var customers = await
                _customerRepository
                    .AsQueryable()
                    .Where(x => string.IsNullOrEmpty(keySearch)
                                || ((x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(keySearch.Trim())
                                    || x.Code.ToLower().Contains(keySearch.ToLower())
                                    || x.PhoneNumber.Contains(keySearch)))
                    .Take(pageSize)
                    .ToListAsync();

            var customerDto = _mapper.Map<List<CustomerDto>>(customers);
            foreach (var customer in customerDto)
            {
                customer.AvatarUrl = !string.IsNullOrEmpty(customer.Avatar)
                    ? S3Path.GetS3Url(_configuration, customer.Avatar, BucketType.UserAvatar)
                    : "";
            }

            return customerDto;
        }

        public async Task DeleteById(int key)
        {
            var attribute = await _customerRepository.FirstOrDefaultAsync(x => x.Id == key);

            if (attribute == null)
                throw new ArgumentException("ATTRIBUTE_DOES_NOT_EXIST");
            await _customerRepository.DeleteAsync(attribute);
        }

        public async Task<CustomerDto> Insert(CustomerDto input)
        {
            var isExistUserName = await _customerRepository.AnyAsync(customer => customer.NormalizedEmail == input.UserName.ToLower());

            if (isExistUserName)
                throw new Exception(CustomResponseMessage.UserNameAlreadyExists);

            if (!string.IsNullOrEmpty(input.Email))
            {
                var isExistEmail = await _customerRepository.AnyAsync(customer => customer.NormalizedEmail == input.Email);
                if (isExistEmail)
                    throw new Exception(CustomResponseMessage.UserNameAlreadyExists);
            }

            input.Code = Guid.NewGuid().ToString().Truncate(8).ToUpper();

            var customer = _mapper.Map<Customer>(input);
            var password = Utils.ComputeHash(input.Password ?? Guid.NewGuid().ToString().Truncate(8));
            customer.PasswordHash = password;
            if (string.IsNullOrEmpty(input.Email))
                customer.NormalizedEmail = Utils.NormalizeEmail(input.Email);

            customer.NormalizedUserName = Utils.NormalizeUserName(input.UserName);

            await _customerRepository.AddAsync(customer);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> Update(int key, CustomerDto entity)
        {
            var customer = await _customerRepository
                .FirstOrDefaultAsync(x => x.Id == key);
            if (customer == null)
                throw new Exception(CustomResponseMessage.CustomerDoesNotExist);

            if (await _customerRepository.AnyAsync(x => x.Id != entity.Id && x.Email == entity.Email))
                throw new ArgumentException("Email đã được đăng ký");

            if (await _customerRepository.AnyAsync(x => x.Id != entity.Id && x.PhoneNumber == entity.PhoneNumber))
                throw new ArgumentException("Số điện thoại đã được đăng ký");

            customer.Avatar = entity.Avatar;
            customer.FirstName = entity.FirstName;
            customer.LastName = entity.LastName;
            customer.Email = entity.Email;
            customer.PhoneNumber = entity.PhoneNumber;
            customer.Address = entity.Address;
            customer.DateOfBirth = entity.DateOfBirth;
            customer.Gender = entity.Gender;

            await _customerRepository.UpdateAsync(customer);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> UpdateFull(int key, CustomerDto entity)
        {
            var customer = await _customerRepository
                .FirstOrDefaultAsync(x => x.Id == key);

            if (customer == null)
                throw new Exception(CustomResponseMessage.CustomerDoesNotExist);

            if (await _customerRepository.AnyAsync(x => x.Id != entity.Id && x.Email == entity.Email))
                throw new ArgumentException("Email đã được đăng ký");

            if (await _customerRepository.AnyAsync(x => x.Id != entity.Id && x.PhoneNumber == entity.PhoneNumber))
                throw new ArgumentException("Số điện thoại đã được đăng ký");

            customer.Avatar = entity.Avatar;
            customer.FirstName = entity.FirstName;
            customer.LastName = entity.LastName;
            customer.Email = entity.Email;
            customer.PhoneNumber = entity.PhoneNumber;
            customer.Address = entity.Address;
            customer.DateOfBirth = entity.DateOfBirth;
            customer.Gender = entity.Gender;

            if (!string.IsNullOrEmpty(entity.Password))
            {
                var password = Utils.ComputeHash(entity.Password);
                customer.PasswordHash = password;
            }

            await _customerRepository.UpdateAsync(customer);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> UpdatePassword(int key, UpdateCustomerPasswordInput input)
        {
            var customer = await _customerRepository
                .FirstOrDefaultAsync(x => x.Id == key);

            if (customer == null)
                throw new Exception(CustomResponseMessage.CustomerDoesNotExist);

            var oldPassword = Utils.ComputeHash(input.OldPassword);

            if (oldPassword != customer.PasswordHash)
            {
                throw new Exception(CustomResponseMessage.OldPasswordIncorrect);
            }

            if (!string.IsNullOrEmpty(input.NewPassword))
            {
                var password = Utils.ComputeHash(input.NewPassword);
                customer.PasswordHash = password;
            }

            await _customerRepository.UpdateAsync(customer);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetUserByIdAsync(int userId)
        {
            var user = await _customerRepository.FirstOrDefaultAsync(x => x.Id == userId);

            var userDto = _mapper.Map<CustomerDto>(user);
            userDto.AvatarUrl = !string.IsNullOrEmpty(user.Avatar)
                ? S3Path.GetS3Url(_configuration, user.Avatar, BucketType.UserAvatar)
                : "";

            return userDto;
        }
    }
}
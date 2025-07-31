using Application.Constants;
using Application.Dtos.Role;
using Application.Dtos.User;
using Application.Dtos.UserClaim;
using Application.Dtos.UserRole;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationService _verificationService;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserClaimRepository _userClaimRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(
            IMapper mapper,
            IVerificationService verificationService,
            IConfiguration configuration,
            IUserRepository userRepository, IUserClaimRepository userClaimRepository, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _userClaimRepository = userClaimRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _verificationService = verificationService;
            _configuration = configuration;
        }

        public async Task<CheckingItemExistModel> CheckEmailExisted(string email)
        {
            if (!Utils.CheckEmailIsValid(email))
                throw new Exception("Wrong email format");

            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email);

            return new CheckingItemExistModel(user != null, user?.IsVerifiedPhone ?? false, email.Trim());
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception(CustomResponseMessage.UserRoleDoesNotExist);

            var listRoleByUser = await _userRoleRepository.AsQueryable()
                .Where(x => user.Id == x.UserId).Include(x => x.Role).ToListAsync();

            var listUserClaims = await _userClaimRepository.AsQueryable()
                .Where(x => user.Id == x.UserId).ToListAsync();

            var userDto = _mapper.Map<UserDto>(user);

            userDto.Roles = _mapper.Map<List<RoleDto>>(listRoleByUser.Select(x => x.Role));
            userDto.UserClaims = _mapper.Map<List<UserClaimDto>>(listUserClaims);

            userDto.AvatarUrl = !string.IsNullOrEmpty(user.Avatar)
                ? S3Path.GetS3Url(_configuration, user.Avatar, BucketType.UserAvatar)
                : "";

            return userDto;
        }

        public async Task<UserDto> GetLoginResultAsync(string username, string password)
        {
            password = Utils.ComputeHash(password);

            var user = await _userRepository.FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == password);

            var userDto = _mapper.Map<UserDto>(user);
            if (userDto == null)
                throw new Exception(CustomResponseMessage.InvalidUserNameOrPassword);

            var listRoleByUser = await _userRoleRepository.AsQueryable()
                .Where(x => user.Id == x.UserId).Include(x => x.Role).ToListAsync();

            var listUserClaims = await _userClaimRepository.AsQueryable()
                .Where(x => user.Id == x.UserId).ToListAsync();

            userDto.Roles = _mapper.Map<List<RoleDto>>(listRoleByUser.Select(x => x.Role));
            userDto.UserClaims = _mapper.Map<List<UserClaimDto>>(listUserClaims);

            userDto.AvatarUrl = !string.IsNullOrEmpty(user.Avatar)
                ? S3Path.GetS3Url(_configuration, user.Avatar, BucketType.UserAvatar)
                : "";

            return userDto;
        }

        public async Task ChangePasswordAsync(int userId, string password)
        {
            password = Utils.ComputeHash(password);

            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            user.PasswordHash = password;
            await _userRepository.UpdateAsync(user);
        }

        private async Task<string> SetNewPasswordResetToken(int userId)
        {
            var token = Guid.NewGuid().ToString("N").Truncate(328);
            var expiration = DateTime.UtcNow.AddMinutes(15);
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            user.PasswordResetToken = token;
            user.PasswordResetExpiration = expiration;
            await _userRepository.UpdateAsync(user);
            return token;
        }

        public async Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email.Equals(input.EmailAddress));

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
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == input.Email && u.PasswordResetToken == input.ResetToken);

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

        public async Task SetVerificationEmail(string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email);
            user.IsVerifiedEmail = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<PagedResultDto<UserDto>> GetAll(GetUserInput input)
        {
            var query = _userRepository
                .AsQueryable()
                .Where(x => string.IsNullOrEmpty(input.KeySearch) || ((x.FirstName + " " + x.LastName).Contains(input.KeySearch.Trim())));

            var sources = await query.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (sources is not { Count: > 0 })
            {
                return new PagedResultDto<UserDto>
                {
                    Items = new List<UserDto>(),
                    TotalCount = 0
                };
            }

            var items = _mapper.Map<List<UserDto>>(sources);

            var userIds = items.Select(x => x.Id);

            var listRoleByUser = await _userRoleRepository.AsQueryable()
                .Where(x => userIds.Any(id => id == x.UserId)).Include(x => x.Role).ToListAsync();

            var listUserClaims = await _userClaimRepository.AsQueryable()
                       .Where(x => userIds.Any(id => id == x.UserId)).ToListAsync();

            foreach (var userDto in items)
            {
                userDto.Roles = _mapper.Map<List<RoleDto>>(listRoleByUser.Where(x => x.UserId == userDto.Id).Select(x => x.Role));
                userDto.UserClaims = _mapper.Map<List<UserClaimDto>>(listUserClaims.Where(x => x.UserId == userDto.Id));
            }

            return new PagedResultDto<UserDto>
            {
                Items = items,
                TotalCount = string.IsNullOrEmpty(input.KeySearch)
                    ? _userRepository.Count()
                    : await query.CountAsync()
            };
        }

        public async Task DeleteById(int key)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == key);

            if (user == null)
                throw new Exception(CustomResponseMessage.UserDoesNotExist);

            user.IsDeleted = true;
            await _userRepository.DeleteAsync(user);
        }

        public async Task UpdateUserRoles(UserRolesDtoInput input)
        {
            var listRoleIdByUser = await _userRoleRepository.AsQueryable()
                .Where(x => input.UserId == x.UserId).Include(x => x.Role)
                .Select(x => x.Role!.Id).ToListAsync();

            var deleteUserRoleIds = listRoleIdByUser
                .Except(input.RoleIds).ToList();

            var addUserRoleIds = input.RoleIds
                .Except(listRoleIdByUser).ToList();

            if (deleteUserRoleIds.Count > 0)
            {
                var deleteUserRoles = await _userRoleRepository.AsQueryable()
                    .Where(ur => ur.UserId == input.UserId && deleteUserRoleIds.Any(id => id == ur.RoleId))
                    .ToListAsync();

                await _userRoleRepository
                    .DeleteRangeAsync(deleteUserRoles);
            }
            if (addUserRoleIds.Count > 0)
            {
                await _userRoleRepository.AddRangeAsync(addUserRoleIds.Select(id => new UserRole
                {
                    UserId = input.UserId,
                    RoleId = id
                }));
            }
        }

        public async Task<UserDto> Insert(UserDto input)
        {
            var isExistUserName = await _userRepository.AnyAsync(customer => customer.NormalizedEmail == input.UserName.ToLower());

            if (isExistUserName)
                throw new Exception(CustomResponseMessage.UserNameAlreadyExists);

            if (!string.IsNullOrEmpty(input.Email))
            {
                var isExistEmail = await _userRepository.AnyAsync(customer => customer.NormalizedEmail == input.Email);
                if (isExistEmail)
                    throw new Exception(CustomResponseMessage.UserNameAlreadyExists);
            }

            var customer = _mapper.Map<User>(input);
            var password = Utils.ComputeHash(input.Password ?? Guid.NewGuid().ToString().Truncate(8));
            customer.PasswordHash = password;
            if (string.IsNullOrEmpty(input.Email))
                customer.NormalizedEmail = Utils.NormalizeEmail(input.Email);

            customer.NormalizedUserName = Utils.NormalizeUserName(input.UserName);

            await _userRepository.AddAsync(customer);
            return _mapper.Map<UserDto>(customer);
        }

        public async Task<UserDto> Update(int key, UserDto entity)
        {
            var customer = await _userRepository
                .FirstOrDefaultAsync(x => x.Id == key);
            if (customer == null)
                throw new Exception(CustomResponseMessage.UserDoesNotExist);

            if (await _userRepository.AnyAsync(x => x.Id != entity.Id && x.Email == entity.Email))
                throw new ArgumentException("Email đã được đăng ký");

            if (await _userRepository.AnyAsync(x => x.Id != entity.Id && x.PhoneNumber == entity.PhoneNumber))
                throw new ArgumentException("Số điện thoại đã được đăng ký");

            customer.Avatar = entity.Avatar;
            customer.FirstName = entity.FirstName;
            customer.LastName = entity.LastName;
            customer.Email = entity.Email;
            customer.PhoneNumber = entity.PhoneNumber;

            await _userRepository.UpdateAsync(customer);
            return _mapper.Map<UserDto>(customer);
        }

        public async Task<UserDto> UpdateFull(int key, UserDto entity)
        {
            var user = await _userRepository
                .FirstOrDefaultAsync(x => x.Id == key);

            if (user == null)
                throw new Exception(CustomResponseMessage.UserDoesNotExist);

            if (await _userRepository.AnyAsync(x => x.Id != entity.Id && x.Email == entity.Email))
                throw new ArgumentException("Email đã được đăng ký");

            if (await _userRepository.AnyAsync(x => x.Id != entity.Id && x.PhoneNumber == entity.PhoneNumber))
                throw new ArgumentException("Số điện thoại đã được đăng ký");

            user.Avatar = entity.Avatar;
            user.FirstName = entity.FirstName;
            user.LastName = entity.LastName;
            user.Email = entity.Email;
            user.PhoneNumber = entity.PhoneNumber;

            if (!string.IsNullOrEmpty(entity.Password))
            {
                var password = Utils.ComputeHash(entity.Password);
                user.PasswordHash = password;
            }

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdatePassword(int key, UpdateUserPasswordInput input)
        {
            var customer = await _userRepository
                .FirstOrDefaultAsync(x => x.Id == key);

            if (customer == null)
                throw new Exception(CustomResponseMessage.UserDoesNotExist);

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

            await _userRepository.UpdateAsync(customer);
            return _mapper.Map<UserDto>(customer);
        }
    }
}
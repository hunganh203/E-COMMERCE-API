using Application.Constants;
using Application.Constants.EmailTemplate;
using Application.DTOs.Configuration;
using Application.DTOs.Verification;
using Application.Extensions;
using Application.Interfaces.CloudService.Google;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Interfaces.Service.Email;
using Application.Utility;
using Domain.Entities;
using System.Web;

namespace Shared.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IGoogleApiIdentityToolkitCloudService _apiIdentityToolkitCloudService;

        private readonly AppConfiguration _appConfig;
        private readonly VerificationConfiguration _verificationConfig;
        private readonly IUserVerificationRepository _userVerificationRepository;
        private readonly IEmailService _emailService;
        private readonly ICustomerRepository _customerRepository;

        public VerificationService(IGoogleApiIdentityToolkitCloudService apiIdentityToolkitCloudService,
            AppConfiguration appConfig, VerificationConfiguration verificationConfig, IUserVerificationRepository userVerificationRepository, IEmailService emailService, ICustomerRepository customerRepository)
        {
            _apiIdentityToolkitCloudService = apiIdentityToolkitCloudService;

            _appConfig = appConfig;
            _verificationConfig = verificationConfig;
            _userVerificationRepository = userVerificationRepository;
            _emailService = emailService;
            _customerRepository = customerRepository;
        }

        public async Task<PhoneConfirmVerificationOutput> VerifyUserPhoneNumber(PhoneVerificationInput input)
        {
            var response = await _apiIdentityToolkitCloudService.VerifyUserPhoneNumber(input);
            return response;
        }

        public async Task<SendVerificationEmailOutputModel> SaveAndSendVerificationByEmail(SendVerificationEmailModel input, int? userId = null)
        {
            // trigger will delete the records has DeletedDate > CurrentDate
            // Link has disable on system Except the Forgot password

            input.Locale = this.NormalizeLocale(input.Locale);

            var userVerification = await this.PrepareDataUserVerification(input.Email, input.Mode, userId);

            var userInfo = await _customerRepository.FirstOrDefaultAsync(u => u.Email == input.Email);

            switch (input.Mode)
            {
                case UserVerificationMode.VerificationForSignin:
                    {
                        userVerification.DeletedDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForSignIn.ExpireTimeRecord);
                        userVerification.CodeExpirationDate =
                            DateTime.UtcNow.AddMinutes(_verificationConfig.Email.TimeForSignIn.ExpireTimeCode);

                        userVerification.Mode = UserVerificationMode.VerificationForSignin;
                        //_emailService.SendVerificationCodeAsync()
                        //deliverData = new
                        //{
                        //    locale = input.Locale,
                        //    mode = UserVerificationMode.VerificationForSignin,
                        //    code = userVerification.VerificationCode,
                        //    email = input.Email,
                        //    username = userInfo?.FirstName
                        //};
                        break;
                    }
                case UserVerificationMode.VerificationForSignUp:
                    {
                        userVerification.DeletedDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForSignUp.ExpireTimeRecord);
                        userVerification.CodeExpirationDate =
                            DateTime.UtcNow.AddMinutes(_verificationConfig.Email.TimeForSignUp.ExpireTimeCode);

                        userVerification.Mode = UserVerificationMode.VerificationForSignUp;
                        //deliverData = new
                        //{
                        //    locale = input.Locale,
                        //    mode = UserVerificationMode.VerificationForSignUp,
                        //    code = userVerification.VerificationCode,
                        //    email = input.Email,
                        //    username = userInfo?.FirstName
                        //};
                        break;
                    }
                case UserVerificationMode.ForgotPassword:
                    {
                        var token = Guid.NewGuid().ToString("N").Truncate(328);

                        var codeForgotExpirationMinutes = _verificationConfig.Email.ForgotPassword.ExpireTimeCode;
                        var tokenForgotExpirationMinutes = _verificationConfig.Email.ForgotPassword.ExpireTimeToken;
                        var recordForgotExpirationMinutes = _verificationConfig.Email.ForgotPassword.ExpireTimeRecord;

                        userVerification.CodeExpirationDate = DateTime.UtcNow.AddMinutes(codeForgotExpirationMinutes);
                        userVerification.Mode = UserVerificationMode.ForgotPassword;
                        userVerification.Token = token;
                        userVerification.TokenExpirationDate = DateTime.UtcNow.AddMinutes(tokenForgotExpirationMinutes);
                        userVerification.DeletedDate = DateTime.UtcNow.AddDays(recordForgotExpirationMinutes);

                        userVerification.Link = NormalizeUrlToken(EncryptToken(input.Email, token, UserVerificationMode.ForgotPassword), UserVerificationMode.ForgotPassword);

                        var deliverData = new SendVerificationByEmailInput()
                        {
                            Locale = input.Locale,
                            Mode = UserVerificationMode.ForgotPassword,
                            Code = userVerification.VerificationCode,
                            Token = token,
                            Link = userVerification.Link,
                            Email = input.Email,
                            UserName = userInfo.UserName
                        };

                        await _emailService.SendVerificationPasswordReset(deliverData);

                        break;
                    }
                case UserVerificationMode.VerifyCurrentUserEmail:
                    {
                        var token = Guid.NewGuid().ToString("N").Truncate(328);

                        userVerification.Mode = UserVerificationMode.VerifyCurrentUserEmail;

                        //userVerification.Token = token;

                        userVerification.DeletedDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForVerification.ExpireTimeRecord);
                        userVerification.CodeExpirationDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForVerification.ExpireTimeCode);

                        // userVerification.TokenExpirationDate = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes);

                        userVerification.Link = _appConfig.ClientRootAddress;

                        //deliverData = new
                        //{
                        //    locale = input.Locale,
                        //    mode = UserVerificationMode.VerifyCurrentUserEmail,
                        //    code = userVerification.VerificationCode,
                        //    token,
                        //    link = userVerification.Link,
                        //    email = input.Email,
                        //    username = userInfo?.FirstName
                        //};
                        //await _emailService.SendVerificationPasswordReset(deliverData);

                        break;
                    }
                case UserVerificationMode.VerifyCurrentUserEmailByLinkOnly:
                    {
                        userVerification.DeletedDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForVerification.ExpireTimeRecord);
                        userVerification.CodeExpirationDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForVerification.ExpireTimeCode);

                        var token = Guid.NewGuid().ToString("N").Truncate(328);

                        userVerification.Mode = UserVerificationMode.VerifyCurrentUserEmailByLinkOnly;
                        userVerification.Token = token;

                        // userVerification.TokenExpirationDate = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes);
                        //userVerification.Link = NormalizeUrlToken(EncryptToken(input.Email, token, UserVerificationMode.VerifyCurrentUserEmailByLinkOnly), UserVerificationMode.VerifyCurrentUserEmailByLinkOnly);

                        userVerification.Link = _appConfig.ClientRootAddress;

                        //deliverData = new
                        //{
                        //    locale = input.Locale,
                        //    mode = UserVerificationMode.VerifyCurrentUserEmailByLinkOnly,
                        //    code = userVerification.VerificationCode,
                        //    token,
                        //    link = userVerification.Link,
                        //    email = input.Email,
                        //    username = userInfo?.FirstName
                        //};

                        break;
                    }
                case UserVerificationMode.VerificationThisEmail:
                    {
                        userVerification.DeletedDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForVerificationThisEmail.ExpireTimeRecord);
                        userVerification.CodeExpirationDate =
                            DateTime.UtcNow.AddDays(_verificationConfig.Email.TimeForVerificationThisEmail.ExpireTimeCode);

                        userVerification.Mode = UserVerificationMode.VerificationThisEmail;

                        //deliverData = new
                        //{
                        //    locale = input.Locale,
                        //    mode = UserVerificationMode.VerificationThisEmail,
                        //    code = userVerification.VerificationCode,
                        //    email = input.Email,
                        //    username = userInfo?.FirstName
                        //};

                        break;
                    }
            }

            if (userVerification.Id > 0)
            {
                await _userVerificationRepository.UpdateAsync(userVerification);
            }
            else
            {
                await _userVerificationRepository.AddAsync(userVerification);
            }

            return new SendVerificationEmailOutputModel
            {
                Email = input.Email,
                Message = "",
                Sent = true,
                UserId = userId
            };
        }

        public async Task<EmailConfirmVerificationOutput> VerifyUserEmailByCode(EmailVerificationModel input, string mode)
        {
            if (string.IsNullOrEmpty(input.Code))
            {
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.InvalidParams,
                };
            }

            var verificationCode = await _userVerificationRepository.FirstOrDefaultAsync(v => v.Email == input.Email && v.VerificationCode == input.Code);

            if (verificationCode == null)
            {
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.InvalidEmailOrCode,
                };
            }

            if (mode != verificationCode.Mode &&
                mode != UserVerificationMode.VerifyCurrentUserEmail)
            {
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.NotAllowed,
                };
            }

            if (verificationCode.CodeExpirationDate < DateTime.UtcNow)
            {
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.CodeExpired,
                };
            }

            verificationCode.CodeExpirationDate = DateTime.UtcNow;
            verificationCode.Status = UserVerificationStatus.Inactive;
            verificationCode.DeletedDate = DateTime.UtcNow.AddDays(15);

            await _userVerificationRepository.UpdateAsync(verificationCode);

            return new EmailConfirmVerificationOutput
            {
                VerifiedEmail = true,
                Email = verificationCode.Email,
                UserId = verificationCode.UserId
            };
        }

        public async Task<EmailConfirmVerificationOutput> VerifyUserEmailByToken(EmailVerificationModel input)
        {
            if (string.IsNullOrEmpty(input.Token))
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.InvalidParams,
                };

            var verificationToken = await _userVerificationRepository.FirstOrDefaultAsync(v => v.Email == input.Email && v.Token == input.Token);

            if (verificationToken == null)
            {
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.InvalidEmailOrCode,
                };
            }

            if (verificationToken.TokenExpirationDate < DateTime.UtcNow)
            {
                return new EmailConfirmVerificationOutput
                {
                    VerifiedEmail = false,
                    Email = "",
                    Message = CustomResponseMessage.TokenExpired,
                };
            }

            verificationToken.CodeExpirationDate = DateTime.UtcNow;
            verificationToken.Status = UserVerificationStatus.Inactive;
            verificationToken.DeletedDate = DateTime.UtcNow.AddDays(15);

            await _userVerificationRepository.UpdateAsync(verificationToken);

            return new EmailConfirmVerificationOutput
            {
                VerifiedEmail = true,
                Email = verificationToken.Email,
                UserId = verificationToken.UserId
            };
        }

        public async Task<EmailConfirmVerificationOutput> VerifyCurrentUserEmail(VerifyCurrentUserEmailInput input, string mode)
        {
            EmailConfirmVerificationOutput verificationOutput;

            if (string.IsNullOrEmpty(input.Code) && string.IsNullOrEmpty(input.c))
            {
                throw new Exception(CustomResponseMessage.InvalidParams);
            }

            if (!string.IsNullOrEmpty(input.c))
            {
                var verifyTokenInput = new VerifyUserEmailTokenInput(input.c);
                verifyTokenInput.ResolveParameters();

                if (verifyTokenInput.Mode == UserVerificationMode.VerifyCurrentUserEmail || verifyTokenInput.Mode == UserVerificationMode.VerifyCurrentUserEmailByLinkOnly)
                {
                    verificationOutput = await VerifyUserEmailByToken(new EmailVerificationModel
                    {
                        Token = verifyTokenInput.Token,
                        Email = verifyTokenInput.Email
                    });
                }
                else
                {
                    throw new Exception(CustomResponseMessage.NotAllowed);
                }
            }
            else
            {
                verificationOutput = await VerifyUserEmailByCode(new EmailVerificationModel
                {
                    Code = input.Code,
                    Email = input.Email
                }, mode);
            }

            if (!verificationOutput.VerifiedEmail)
            {
                return verificationOutput;
            }

            if (verificationOutput.UserId == null)
                throw new Exception("Current user does not exist");

            // await _unitOfWorkCatalog.Users.SetVerificationEmail(verificationOutput.UserId.Value);
            return verificationOutput;
        }

        public async Task<bool> HasSendVerificationCurrentUserEmail(string email)
        {
            var userVerificationActive = await _userVerificationRepository.FirstOrDefaultAsync(v => v.Email == email);

            return userVerificationActive is { Mode: UserVerificationMode.VerifyCurrentUserEmail };
        }

        public EmailVerificationModel ResolveParameters(string t)
        {
            if (string.IsNullOrEmpty(t))
                throw new Exception("Invalid parameters");

            var parameterDecryptString = Utils.DecryptString(t);

            var parameters = parameterDecryptString.Split("|");

            if (parameters.Length != 2)
            {
                throw new Exception("Invalid parameters");
            }

            return new EmailVerificationModel
            {
                Email = parameters[0],
                Token = parameters[1]
            };
        }

        public string EncryptToken(string email, string tokenCode, string mode)
        {
            var token = email + "|" + tokenCode + "|" + mode;

            return Utils.EncryptString(token);
        }

        public string NormalizeUrlToken(string token, string mode)
        {
            var cToken = HttpUtility.UrlEncode(token);
            return mode switch
            {
                UserVerificationMode.VerifyCurrentUserEmail =>
                    $"{_appConfig.ClientRootAddress}verify-email?c={cToken}",
                UserVerificationMode.VerifyCurrentUserEmailByLinkOnly =>
                    $"{_appConfig.ClientRootAddress}verify-email?c={cToken}",
                UserVerificationMode.ForgotPassword =>
                    $"{_appConfig.ClientRootAddress}verify-pwd-token?c={cToken}",
                _ => throw new Exception("Mode does not support")
            };
        }

        private async Task<UserVerification> PrepareDataUserVerification(string email, string mode, int? userId = null)
        {
            var code = Utils.GetRandomRange(100000, 999999);

            var userVerification = new UserVerification
            {
                Token = string.Empty,
                TokenExpirationDate = null,
                VerificationCode = code.ToString(),
                CreatedDate = DateTime.UtcNow,
                Link = string.Empty,
                UserId = userId,
                Status = UserVerificationStatus.Active,
                Email = email,
                Phone = string.Empty
            };

            // this is case verify user profile email
            if (mode == UserVerificationMode.VerifyCurrentUserEmail)
            {
                if (!userId.HasValue)
                {
                    throw new NullReferenceException("This mode need UserId");
                }

                var userVerificationActives = _userVerificationRepository
                    .Query(v => v.Email == email && v.Mode == UserVerificationMode.VerifyCurrentUserEmail).ToList();

                // filter by UserId
                userVerificationActives = userVerificationActives.Where(x => x.UserId == userId.Value).ToList();

                if (userVerificationActives.Count <= 0)
                    return userVerification;

                if (userVerificationActives.Count == 1)
                {
                    return userVerificationActives[0];
                }

                var takeOneUserVerification = userVerificationActives.First();

                foreach (var userVerificationActive in
                    userVerificationActives.Where(userVerificationActive => userVerificationActive.Id != takeOneUserVerification.Id))
                {
                    userVerificationActive.CodeExpirationDate = DateTime.UtcNow;
                    userVerificationActive.Status = UserVerificationStatus.Inactive;
                    userVerificationActive.DeletedDate = DateTime.UtcNow.AddDays(15);

                    await _userVerificationRepository.UpdateAsync(userVerificationActive);
                }
                return takeOneUserVerification;
            }

            // this is other case verify email - still use once session
            var verifications = _userVerificationRepository
                .Query(v => v.Email == email && v.Mode == UserVerificationMode.VerifyCurrentUserEmail).ToList();

            if (verifications.Count <= 0)
            {
                return userVerification;
            }

            foreach (var verification in verifications.Where(x => x.Mode != UserVerificationMode.VerifyCurrentUserEmail))
            {
                verification.CodeExpirationDate = DateTime.UtcNow;
                verification.Status = UserVerificationStatus.Inactive;
                verification.DeletedDate = DateTime.UtcNow.AddDays(15);

                await _userVerificationRepository.UpdateAsync(verification);
            }

            return userVerification;
        }

        public string NormalizeLocale(string locale)
        {
            return locale switch
            {
                EmailSupportLanguageConst.France =>
                    EmailSupportLanguageConst.France,
                EmailSupportLanguageConst.English =>
                    EmailSupportLanguageConst.English,
                _ => EmailSupportLanguageConst.Vietnamese
            };
        }
    }
}
using Application.Constants;
using Application.DTOs.Authorization;
using Application.DTOs.Configuration;
using Application.DTOs.Verification;
using Application.Interfaces.Authentication;
using Application.Interfaces.Service;
using Application.Utility;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Helpers;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    /// <summary>
    /// Verification
    /// </summary>
    public class VerificationController : BaseNonAuthClientApiController
    {
        private readonly IVerificationService _verificationService;
        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;
        private readonly IUserService _userService;
        private bool _isLoginBlocked;
        private readonly ILogger<VerificationController> _logger;

        private readonly AppConfiguration _appConfiguration;

        public VerificationController(IVerificationService verificationService,
            IJwtAuthenticationManager jWtAuthenticationManager,
            IUserService userService,
            AppConfiguration appConfiguration,
            ILogger<VerificationController> logger)
        {
            _verificationService = verificationService;
            _jWtAuthenticationManager = jWtAuthenticationManager;
            _userService = userService;
            _appConfiguration = appConfiguration;
            _logger = logger;

            _isLoginBlocked = false;
        }

        /// <summary>
        /// Send verification code By phone number
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendVerificationCodeByPhoneNumber")]
        public async Task<AjaxResponse<VerificationOutput>> SendVerificationCodeByPhoneNumber(SendVerificationInput input)
        {
            try
            {
                input.PhoneNumber = Utils.NormalizeString(input.PhoneNumber);
                //  Check block login by phone code
                this.CheckBlockUserLogin("", input.PhoneNumber);

                // check exist when type is login

                return new AjaxResponse<VerificationOutput>(await Task.Run(() => true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, input);
                return new AjaxResponse<VerificationOutput>(VerificationHandleError.HandleErrorForSendVerifyPhone(ex.Message, _isLoginBlocked));
            }
        }

        /// <summary>
        /// ReSend verification code By phone number
        /// </summary>
        /// <returns></returns>
        [HttpPost("ReSendVerificationCodeByPhoneNumber")]
        public async Task<AjaxResponse<VerificationOutput>> ReSendVerificationCodeByPhoneNumber(SendVerificationInput input)
        {
            try
            {
                input.PhoneNumber = Utils.NormalizeString(input.PhoneNumber);
                //  Check block login by phone code
                this.CheckBlockUserLogin("", input.PhoneNumber);

                // check exist when type is login

                return new AjaxResponse<VerificationOutput>(await Task.Run(() => true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, input);
                return new AjaxResponse<VerificationOutput>(VerificationHandleError.HandleErrorForSendVerifyPhone(ex.Message, _isLoginBlocked));
            }
        }

        /// <summary>
        /// "Verify code User Phone Number and login
        /// </summary>
        /// <returns></returns>
        [HttpPost("LoginByPhoneCode")]
        public async Task<AjaxResponse<AuthenticateResultModel>> LoginByPhoneCode(PhoneVerificationInput input)
        {
            try
            {
                input.PhoneNumber = Utils.NormalizeString(input.PhoneNumber);
                //  Check block login by phone code
                this.CheckBlockUserLogin("", input.PhoneNumber);

                // check exist when type is login

                return new AjaxResponse<AuthenticateResultModel>(await Task.Run(() => true));
            }
            catch (Exception ex)
            {
                return new AjaxResponse<AuthenticateResultModel>(new ErrorInfo
                {
                    Code = _isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.LoginFailed,
                    Message = ex.Message
                }, true);
            }
        }

        /// <summary>
        /// "Verify code User Phone Number
        /// </summary>
        /// <returns></returns>
        [HttpPost("VerifyUserPhoneNumber")]
        public async Task<AjaxResponse<bool>> VerifyUserPhoneNumber(PhoneVerificationInput input)
        {
            try
            {
                input.PhoneNumber = Utils.NormalizeString(input.PhoneNumber);
                //  Check block login by phone code
                this.CheckBlockUserLogin("", input.PhoneNumber);

                // check exist when type is login

                return new AjaxResponse<bool>(await Task.Run(() => true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, input);

                var message = ex.Message;
                if (!_isLoginBlocked)
                    message = this.CheckBlockUserLogin("", input.PhoneNumber, false);

                return new AjaxResponse<bool>(VerificationHandleError.HandleErrorForVerifyPhone(message));
            }
        }

        /// <summary>
        /// Send verification code By email
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendVerificationCodeByEmail")]
        public async Task<AjaxResponse<bool>> SendVerificationCodeByEmail(SendVerificationEmailInput input)
        {
            try
            {
                input.Email = Utils.NormalizeString(input.Email);
                this.CheckBlockUserLogin(input.Email, "");

                // check exist when type is login

                var checkEmailExisted = await _userService.CheckEmailExisted(input.Email);
                if (!checkEmailExisted.Existed && input.Type == VerificationType.SignIn)
                {
                    return new AjaxResponse<bool>(new ErrorInfo
                    {
                        Message = CustomResponseMessage.EmailDoesNotExist,
                        Code = _isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.SendVerificationFailed
                    });
                }

                if (checkEmailExisted.Existed && input.Type == VerificationType.SignUp)
                {
                    return new AjaxResponse<bool>(new ErrorInfo
                    {
                        Message = CustomResponseMessage.EmailAlreadyExists,
                        Code = _isLoginBlocked
                            ? CustomResponseCode.LoginBlocked
                            : CustomResponseCode.SendVerificationFailed
                    });
                }

                var modelInput = new SendVerificationEmailModel
                {
                    Locale = input.Locale,
                    Email = input.Email,
                    Mode = input.Type switch
                    {
                        VerificationType.SignIn => UserVerificationMode.VerificationForSignin,
                        VerificationType.SignUp => UserVerificationMode.VerificationForSignUp,
                        _ => UserVerificationMode.VerificationThisEmail
                    }
                };

                //  Count the number of sent email code

                await _verificationService.SaveAndSendVerificationByEmail(modelInput);

                return new AjaxResponse<bool>(result: true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<bool>(new ErrorInfo
                {
                    Message = ex.Message,
                    Code = _isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.SendVerificationFailed
                });
            }
        }

        /// <summary>
        /// ReSend verification code By email
        /// </summary>
        /// <returns></returns>
        [HttpPost("ReSendVerificationCodeByEmail")]
        public async Task<AjaxResponse<bool>> ReSendVerificationCodeByEmail(SendVerificationEmailInput input)
        {
            try
            {
                input.Email = Utils.NormalizeString(input.Email);
                this.CheckBlockUserLogin(input.Email, "");

                // check exist when type is login

                var checkEmailExisted = await _userService.CheckEmailExisted(input.Email);
                if (!checkEmailExisted.Existed && input.Type == VerificationType.SignIn)
                {
                    return new AjaxResponse<bool>(new ErrorInfo
                    {
                        Message = CustomResponseMessage.EmailDoesNotExist,
                        Code = _isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.SendVerificationFailed
                    });
                }

                if (checkEmailExisted.Existed && input.Type == VerificationType.SignUp)
                {
                    return new AjaxResponse<bool>(new ErrorInfo
                    {
                        Message = CustomResponseMessage.EmailAlreadyExists,
                        Code = _isLoginBlocked
                            ? CustomResponseCode.LoginBlocked
                            : CustomResponseCode.SendVerificationFailed
                    });
                }

                var modelInput = new SendVerificationEmailModel
                {
                    Locale = input.Locale,
                    Email = input.Email,
                    Mode = input.Type switch
                    {
                        VerificationType.SignIn => UserVerificationMode.VerificationForSignin,
                        VerificationType.SignUp => UserVerificationMode.VerificationForSignUp,
                        _ => UserVerificationMode.VerificationThisEmail
                    }
                };

                //  Count the number of sent email code

                await _verificationService.SaveAndSendVerificationByEmail(modelInput);

                return new AjaxResponse<bool>(result: true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<bool>(new ErrorInfo
                {
                    Message = ex.Message,
                    Code = _isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.SendVerificationFailed
                });
            }
        }

        /// <summary>
        /// "Verify code User email and login
        /// Enter: 000000 for skip verify
        /// </summary>
        /// <returns></returns>
        [HttpPost("LoginByEmailCode")]
        public async Task<AjaxResponse<AuthenticateResultModel>> LoginByEmailCode(EmailVerificationInput input)
        {
            try
            {
                var data = await Task.Run(() => true);
                return new AjaxResponse<AuthenticateResultModel>(data);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (!_isLoginBlocked) message = this.CheckBlockUserLogin(input.Email, "", false);

                return new AjaxResponse<AuthenticateResultModel>(new ErrorInfo
                {
                    Code = _isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.LoginFailed,
                    Message = _isLoginBlocked ? message : ex.Message
                });
            }
        }

        /// <summary>
        /// verify code By user email
        /// </summary>
        /// <returns></returns>
        [HttpPost("VerifyUserEmailByCode")]
        public async Task<AjaxResponse<EmailConfirmVerificationOutput>> VerifyUserEmailByCode(EmailVerificationInput input)
        {
            try
            {
                input.Email = Utils.NormalizeString(input.Email);
                var response = await _verificationService.VerifyUserEmailByCode(new EmailVerificationModel
                {
                    Email = input.Email,
                    Code = input.Code
                }, input.Type switch
                {
                    VerificationType.SignIn => UserVerificationMode.VerificationForSignin,
                    VerificationType.SignUp => UserVerificationMode.VerificationForSignin,
                    _ => UserVerificationMode.VerificationThisEmail
                });

                return new AjaxResponse<EmailConfirmVerificationOutput>(response);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<EmailConfirmVerificationOutput>(new ErrorInfo
                {
                    Message = ex.Message,
                    Code = CustomResponseCode.SendVerificationFailed
                });
            }
        }

        /// <summary>
        ///  /// verify current user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("VerifyCurrentUserEmail")]
        public async Task<AjaxResponse<EmailConfirmVerificationOutput>> VerifyCurrentUserEmail([FromBody] VerifyCurrentUserEmailInput input)
        {
            try
            {
                input.Email = Utils.NormalizeString(input.Email);
                var response = await _verificationService.VerifyCurrentUserEmail(input, UserVerificationMode.VerifyCurrentUserEmail);

                return new AjaxResponse<EmailConfirmVerificationOutput>(response);
            }
            catch (Exception e)
            {
                return new AjaxResponse<EmailConfirmVerificationOutput>(new ErrorInfo(e.Message));
            }
        }

        private string CheckBlockUserLogin(string email, string phone, bool reThrowEx = true)
        {
            try
            {
                return "";
            }
            catch (Exception ex)
            {
                _isLoginBlocked = true;
                if (reThrowEx) throw;
                return ex.Message;
            }
        }
    }
}
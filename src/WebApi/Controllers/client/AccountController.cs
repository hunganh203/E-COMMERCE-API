using Application.Constants;
using Application.Dtos.Customer;
using Application.DTOs.Authorization;
using Application.DTOs.Authorization.Accounts;
using Application.DTOs.Verification;
using Application.Interfaces.Authentication;
using Application.Interfaces.Service;
using Application.Utility;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    /// <summary>
    /// Handle Account business
    /// </summary>
    public class AccountController : BaseNonAuthClientApiController
    {
        private readonly ICustomerService _customerService;

        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;

        [HttpPost("register")]
        public async Task<AjaxResponse<CustomerDto>> Register(CustomerDto input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input.Email) && !Utils.CheckEmailIsValid(input.Email))
                {
                    throw new Exception(CustomResponseMessage.InvalidEmail);
                }

                var res = await _customerService.Register(input);

                return new AjaxResponse<CustomerDto>(res);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(500, e.Message));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jWtAuthenticationManager"></param>
        /// <param name="customerService"></param>
        public AccountController(
            IJwtAuthenticationManager jWtAuthenticationManager, ICustomerService customerService)
        {
            _jWtAuthenticationManager = jWtAuthenticationManager;
            _customerService = customerService;
        }

        /// <summary>
        /// This is action ForgotPassword
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        [HttpPost("forgot-password")]
        public async Task<AjaxResponse> ForgotPassword(SendPasswordResetCodeInput input)
        {
            try
            {
                var res = await _customerService.ForgotPassword(input);

                return new AjaxResponse(res);
            }
            catch (Exception e)
            {
                return new AjaxResponse(new ErrorInfo(500, e.Message));
            }
        }

        [HttpPost("validate-reset-password-code")]
        public async Task<AjaxResponse> ValidResetPasswordCode(ValidateResetPasswordCodeInput input)
        {
            try
            {
                var output = await _customerService.ValidResetPasswordCode(input);
                return new AjaxResponse(output);
            }
            catch (Exception e)
            {
                return new AjaxResponse(new EmailConfirmVerificationOutput
                {
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// Reset Password by link from email
        /// </summary>
        /// <param name="input">Reset with c is param</param>
        /// <returns></returns>
        ///
        [HttpPost("reset-password")]
        public async Task<AjaxResponse<AuthenticateResultModel>> ResetPassword(ResetPasswordInput input)
        {
            try
            {
                var userEmail = await _customerService.ResetPassword(input);

                return new AjaxResponse<AuthenticateResultModel>(true);
            }
            catch (Exception e)
            {
                return new AjaxResponse<AuthenticateResultModel>(new ErrorInfo
                {
                    Code = CustomResponseCode.LoginFailed,
                    Message = string.IsNullOrEmpty(e.Message) ? CustomResponseMessage.InvalidParams : e.Message,
                });
            }
        }
    }
}
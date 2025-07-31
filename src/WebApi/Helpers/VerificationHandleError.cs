using Application.Constants;
using WebApi.Models.Response;

namespace WebApi.Helpers
{
    public class VerificationHandleError
    {
        public static ErrorInfo HandleErrorForVerifyPhone(string msg)
        {
            var err = new ErrorInfo
            {
                Message = CustomResponseMessage.InvalidPhoneOrCode,
                Code = CustomResponseCode.CodeInvalid
            };

            if (string.IsNullOrEmpty(msg))
            {
                return err;
            }

            if (msg.Contains(CustomResponseMessage.InvalidSessionInfo))
            {
                err.Message = CustomResponseMessage.InvalidSessionInfo;
                err.Code = CustomResponseCode.InvalidSessionInfo;
            }
            else if (msg.Contains(CustomResponseMessage.CodeInvalid))
            {
                err.Message = CustomResponseMessage.CodeInvalid;
                err.Code = CustomResponseCode.CodeInvalid;
            }
            else if (msg.Contains(CustomResponseMessage.SessionExpired))
            {
                err.Message = CustomResponseMessage.CodeExpired;
                err.Code = CustomResponseCode.CodeInvalid;
            }
            else if (msg.Contains(CustomResponseMessage.ServiceUnavailable))
            {
                err.Message = CustomResponseMessage.ServiceUnavailable;
                err.Code = CustomResponseCode.CodeInvalid;
            }
            else if (msg.Contains(CustomResponseMessage.BlockLoginByCode))
            {
                err.Code = CustomResponseCode.LoginBlocked;
                err.Message = msg;
            }

            return err;
        }

        public static ErrorInfo HandleErrorForSendVerifyPhone(string msg, bool isLoginBlocked = false, string timeBlockRemaining = "")
        {
            var err = new ErrorInfo
            {
                Message = CustomResponseMessage.SendVerificationFailed,
                Code = CustomResponseCode.SendVerificationFailed
            };

            if (string.IsNullOrEmpty(msg))
            {
                return err;
            }

            if (msg.Contains(CustomResponseMessage.CaptchaCheckFailed))
            {
                err.Message = CustomResponseMessage.CaptchaCheckFailed;
                err.Code = isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.CaptchaCheckFailed;
            }
            else if (msg.Contains(CustomResponseMessage.InvalidPhoneNumber))
            {
                err.Message = CustomResponseMessage.InvalidPhoneNumber;
                err.Code = isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.InvalidPhoneNumber;
            }
            else if (msg.Contains(CustomResponseMessage.BlockLoginByCode))
            {
                err.Code = CustomResponseCode.LoginBlocked;
                err.Message = msg;
            }
            else
            {
                err.Message = CustomResponseMessage.SendVerificationFailed;
                err.Code = isLoginBlocked ? CustomResponseCode.LoginBlocked : CustomResponseCode.SendVerificationFailed;
            }

            return err;
        }
    }
}
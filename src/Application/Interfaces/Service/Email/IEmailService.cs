using Application.DTOs.Verification;

namespace Application.Interfaces.Service.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        Task SendVerificationPasswordReset(SendVerificationByEmailInput input, string language = "vi");

        /// <summary>
        ///
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="fullName"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        Task ChangePasswordSuccessfullyAsync(string emailAddress, string fullName, string language = "vi");

        Task SendVerificationCodeAsync(SendVerificationByEmailInput input, string language = "vi");

        Task SendVerificationEmailVerify(SendVerificationByEmailInput input, string language = "vi");

        Task SendVerificationEmailVerifyLinkOnly(SendVerificationByEmailInput input, string language = "vi");

        Task SendVerificationCodeForUpdateProfileAsync(SendVerificationByEmailInput input, string language = "vi");
    }
}
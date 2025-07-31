using Application.Constants;
using Application.Constants.EmailTemplate;
using Application.DTOs.Verification;
using Application.Interfaces.Service.Email;
using Application.Localization;

namespace Shared.Services.System
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;

        private readonly IEmailSender _emailSender;

        public EmailService(IEmailTemplateProvider emailTemplateProvider, IEmailSender emailSender)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
        }

        public async Task SendVerificationPasswordReset(SendVerificationByEmailInput input, string language = "vi")
        {
            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.ResetPassword, language);

            emailTemplate = emailTemplate.Replace("{USER}", input.UserName);

            if (!string.IsNullOrEmpty(input.Code) && !string.IsNullOrEmpty(input.Token))
            {
                emailTemplate = emailTemplate.Replace("{CODE}", input.Code);
                emailTemplate = emailTemplate.Replace("{LINK}", input.Link);
            }

            await ReplaceBodyAndSend(input.Email, L.T("EMAIL_REQUEST_CHANGE_PASSWORD_TITLE", language), emailTemplate);
        }

        public async Task SendVerificationEmailVerify(SendVerificationByEmailInput input, string language = "vi")
        {
            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.CurrentEmail, language);

            if (!string.IsNullOrEmpty(input.Code))
            {
                emailTemplate = emailTemplate.Replace("{USER}", input.UserName);
                emailTemplate = emailTemplate.Replace("{CODE}", input.Code);
                emailTemplate = emailTemplate.Replace("{EMAIL}", input.Email);

                //emailTemplate = emailTemplate.Replace("{LINK}", input.Link);
            }

            await ReplaceBodyAndSend(input.Email, L.T("EMAIL_VERIFICATION_TITLE", language), emailTemplate);
        }

        [Obsolete]
        public async Task SendVerificationEmailVerifyLinkOnly(SendVerificationByEmailInput input, string language = "vi")
        {
            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.CurrentEmailOnlyLink, language);

            if (!string.IsNullOrEmpty(input.Code) && !string.IsNullOrEmpty(input.Token))
            {
                emailTemplate = emailTemplate.Replace("{LINK}", input.Link);
                emailTemplate = emailTemplate.Replace("{EMAIL}", input.Email);
            }

            await ReplaceBodyAndSend(input.Email, L.T("EMAIL_VERIFICATION_TITLE", language), emailTemplate);
        }

        public async Task ChangePasswordSuccessfullyAsync(string emailAddress, string username, string language = "vi")
        {
            if (string.IsNullOrEmpty(emailAddress)) return;

            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.ChangePwdSuccess, language);

            emailTemplate = emailTemplate.Replace("{USER}", username);
            emailTemplate = emailTemplate.Replace("{EMAIL}", emailAddress);

            await ReplaceBodyAndSend(emailAddress, L.T("EMAIL_CHANGED_PASSWORD_SUCCESSFULLY", language), emailTemplate);
        }

        public async Task SendVerificationCodeForUpdateProfileAsync(SendVerificationByEmailInput input, string language = "vi")
        {
            if (string.IsNullOrEmpty(input.Email) || !input.Email.Contains("@"))
                return;

            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.VerifyEmailForUpdateInfo, language);

            emailTemplate = emailTemplate.Replace("{USER}", input.UserName);
            emailTemplate = emailTemplate.Replace("{CODE}", input.Code);

            await ReplaceBodyAndSend(input.Email, L.T("EMAIL_REQUEST_CHANGE_IN_PROFILE_TITLE", language), emailTemplate);
        }

        public async Task SendUserInvitationAsync(SendInviteUserEmailInput input, string language = "vi")
        {
            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.UserInvitation, language);
            var title = L.T("EMAIL_INVITE_USER_TITLE", language);
            title = title.Replace("{InviteeName}", input.InviteeName);
            emailTemplate = emailTemplate.Replace("{TITLE}", title);
            emailTemplate = emailTemplate.Replace("{InviteeName}", input.InviteeName);
            emailTemplate = emailTemplate.Replace("{InviteeEmail}", input.InviteeEmail);
            emailTemplate = emailTemplate.Replace("{OrganizationName}", input.OrganizationName);

            if (!string.IsNullOrEmpty(input.Link))
            {
                emailTemplate = emailTemplate.Replace("{LINK}", input.Link);
            }

            await ReplaceBodyAndSend(input.Email, L.T("EMAIL_INVITE_USER_SUBJECT", language), emailTemplate);
        }

        public async Task SendVerificationCodeAsync(SendVerificationByEmailInput input, string language = "vi")
        {
            var emailTemplate = _emailTemplateProvider.GetTemplateByName(EmailTemplateNameConst.LoginEmailCode, language);

            emailTemplate = emailTemplate.Replace("{EMAIL}", input.Email);

            switch (input.Mode)
            {
                case UserVerificationMode.VerificationForSignin:
                    emailTemplate = emailTemplate.Replace("{TITLE}", L.T("EMAIL_LOGIN_CODE_TITLE", language));
                    break;

                default:
                    emailTemplate = emailTemplate.Replace("{TITLE}", L.T("EMAIL_VERIFICATION_TITLE", language));
                    break;
            }

            if (!string.IsNullOrEmpty(input.Code))
            {
                emailTemplate = emailTemplate.Replace("{CODE}", input.Code);
            }

            switch (input.Mode)
            {
                case UserVerificationMode.VerificationForSignin:
                    await ReplaceBodyAndSend(input.Email, L.T("EMAIL_LOGIN_CODE_TITLE", language), emailTemplate);
                    break;

                default:
                    await ReplaceBodyAndSend(input.Email, L.T("EMAIL_VERIFICATION_TITLE", language), emailTemplate);
                    break;
            }
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, string emailTemplate)
        {
            await _emailSender.SendAsync(string.Empty, emailAddress, subject, emailTemplate);
        }
    }
}
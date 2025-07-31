using Application.DTOs.Verification;

namespace Application.Interfaces.CloudService.Google
{
    public interface IGoogleApiIdentityToolkitCloudService
    {
        Task<VerificationOutput> SendVerificationCodeByPhoneNumber(SendVerificationInput input);

        Task<PhoneConfirmVerificationOutput> VerifyUserPhoneNumber(PhoneVerificationInput input);
    }
}
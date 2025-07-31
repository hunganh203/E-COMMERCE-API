using Application.DTOs.Configuration;
using Application.DTOs.Verification;
using Application.Interfaces.CloudService.Google;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Uri = System.Uri;

namespace Cloud.Service.Google
{
    public class GoogleApiIdentityToolkitCloudService : IGoogleApiIdentityToolkitCloudService
    {
        private readonly ILogger<GoogleApiIdentityToolkitCloudService> _logger;

        private readonly CloudServiceConfiguration _cloudServiceConfiguration;

        public GoogleApiIdentityToolkitCloudService(ILogger<GoogleApiIdentityToolkitCloudService> logger, CloudServiceConfiguration cloudServiceConfiguration)
        {
            _logger = logger;
            _cloudServiceConfiguration = cloudServiceConfiguration;
        }

        public async Task<VerificationOutput> SendVerificationCodeByPhoneNumber(SendVerificationInput input)
        {
            var resultContent = "";
            var uri = "";
            try
            {
                uri = _cloudServiceConfiguration.AppFireBase.Uri;
                var apiKey = _cloudServiceConfiguration.AppFireBase.ApiKey;

                using var client = new HttpClient
                {
                    BaseAddress = new Uri(uri)
                };

                var content = new
                {
                    phoneNumber = input.PhoneNumber,
                    recaptchaToken = input.CaptchaToken
                };

                var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                var result = await client.PostAsync($"/identitytoolkit/v3/relyingparty/sendVerificationCode?key={apiKey}", stringContent);

                resultContent = await result.Content.ReadAsStringAsync();

                dynamic response = JObject.Parse(resultContent);

                if (response.error == null)
                    return new VerificationOutput
                    {
                        SessionInfo = response.sessionInfo
                    };

                var errMsg = response.error.message;
                var errMsgVal = $"Can not send verification: {errMsg.Value}";
                throw new Exception(errMsgVal);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Can not send verification code By Phone Number: {input.PhoneNumber}");
                _logger.LogError($"Result from {uri}: {resultContent}");
                if (e.Message.Contains("Can not send verification"))
                {
                    throw new Exception(e.Message);
                }
                throw new Exception($"Can not send verification code By Phone Number: {input.PhoneNumber}");
            }
        }

        public async Task<PhoneConfirmVerificationOutput> VerifyUserPhoneNumber(PhoneVerificationInput input)
        {
            var resultContent = "";
            var uri = "";
            try
            {
                uri = _cloudServiceConfiguration.AppFireBase.Uri;
                var apiKey = _cloudServiceConfiguration.AppFireBase.ApiKey;

                using var client = new HttpClient
                {
                    BaseAddress = new Uri(uri)
                };

                var content = new FormUrlEncodedContent(new[]  {
                    new KeyValuePair<string, string>("code", input.Code),
                    new KeyValuePair<string, string>("sessionInfo", input.SessionInfo)
                });

                var result = await client.PostAsync($"/identitytoolkit/v3/relyingparty/verifyPhoneNumber?key={apiKey}", content);
                resultContent = await result.Content.ReadAsStringAsync();

                dynamic response = JObject.Parse(resultContent);

                if (response.error != null)
                {
                    return new PhoneConfirmVerificationOutput
                    {
                        Phone = response.phoneNumber,
                        VerifiedPhone = false,
                        UpdatedDate = 0,
                        Message = response.error.message
                    };
                }

                var now = DateTime.Now;
                var unixTime = ((DateTimeOffset)now).ToUnixTimeMilliseconds();

                return new PhoneConfirmVerificationOutput
                {
                    Phone = response.phoneNumber,
                    VerifiedPhone = true,
                    UpdatedDate = unixTime
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Can not confirm verification code By Phone Number: {input.PhoneNumber}, Code: {input.Code}");
                _logger.LogError($"Result from {uri}: {resultContent}");
                throw new Exception($"Can not confirm verification code By Phone Number: {input.PhoneNumber}, Code: {input.Code}");
            }
        }
    }
}
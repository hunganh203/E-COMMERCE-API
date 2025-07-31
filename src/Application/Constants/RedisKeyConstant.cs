namespace Application.Constants
{
    public class RedisKeyPrefix
    {
        #region Organization

        public const string OrganizationStatus = "organization_status";
        public const string OrganizationKey = "organization_key";
        public const string OrganizationList = "organization_list";

        #endregion Organization

        #region User And Login

        public const string LoginFailedByEmail = "failed_login_by_email";
        public const string LoginFailedByPhone = "failed_login_by_phone";

        public const string SendVerificationPhoneCode = "send_verification_phone_code";
        public const string SendVerificationEmailCode = "send_verification_email_code";

        public const string ActiveUserList = "active_user_list";

        #endregion User And Login

        #region Security layer

        public const string SuspectIpAddressRequestCount = "suspect_ip_address_request_count";
        public const string ApiKey = "apikey";

        #endregion Security layer

        #region Remote config

        public const string RemoteConfiguration = "remote_configurations";

        #endregion Remote config

        #region App version

        public const string AppVersion = "app_version";

        #endregion App version

        #region App version

        public const string UserTaskDetail = "user_tasks_detail";

        #endregion App version

        #region Cloud service

        public const string ZoomVideoCallSession = "video_call";
        public const string ZoomVideoCallParticipants = "video_call_participants";

        #endregion Cloud service

        #region Refresh token

        public const string UserRefreshToken = "refresh_token";
        public const string UserRefreshTokenList = "refresh_tokens";

        #endregion Refresh token
    }
}
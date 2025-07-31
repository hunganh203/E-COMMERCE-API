namespace Application.Constants
{
    public class CustomResponseCode
    {
        public const int LoginFailed = 4001;
        public const int LoginBlocked = 4002;
        public const int OrganizationInactive = 4003;
        public const int NotFinishSetup = 4004; //  For mobile only
        public const int SendPhoneCodeBlocked = 4005;
        public const int CaptchaCheckFailed = 4006;
        public const int InvalidPhoneNumber = 4007;
        public const int InvalidSessionInfo = 4008;
        public const int SendVerificationFailed = 4009;
        public const int CodeInvalid = 4010;
        public const int WaitingActivationOrganization = 4011;

        public const int SuspiciousRequest = 5000;

        //  show small message that show task was deleted
        public const int TaskDeleted = 6000;

        public const int TaskNotExist = 6403;

        //  Invite code
        public const int InviteTokenInvalid = 6100;

        public const int InviteTokenExpired = 6101;
        public const int InviteTokenCanceled = 6102;
        public const int InviteUserAttached = 6103;
        public const int InviteJoinedByOtherMethods = 6104;

        public const int AttachmentCodeInvalid = 7100;
    }

    public class CustomResponseMessage
    {
        public const string UserDeactivated = "USER_DEACTIVATED";
        public const string InvalidUserOrPassword = "USER_PASSWORD_INVALID";
        public const string NotYetVerifyAfterIntervalTime = "UNVERFIED_AFTER_INTERVAL_TIME";
        public const string InvalidPhoneOrCode = "INVALID_PHONE_OR_CODE";
        public const string InvalidEmailOrCode = "INVALID_EMAIL_OR_CODE";
        public const string InvalidParams = "INVALID_PARAM";
        public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
        public const string FeatureDoesNotSupport = "FEATURE_DOES_NOT_SUPPORT";
        public const string CodeInvalid = "INVALID_CODE";
        public const string CodeExpired = "CODE_EXPIRED";
        public const string TokenExpired = "TOKEN_EXPIRED";
        public const string SessionExpired = "SESSION_EXPIRED";
        public const string PhoneNotVerified = "PHONE_UNVERIFIED";
        public const string EmailNotVerified = "EMAIL_UNVERIFIED";
        public const string NotAllowed = "NOT_ALLOW";
        public const string CaptchaCheckFailed = "CAPTCHA_CHECK_FAILED";
        public const string InvalidPhoneNumber = "INVALID_PHONE_NUMBER";
        public const string InvalidSessionInfo = "INVALID_SESSION_INFO";
        public const string SendVerificationFailed = "SEND_VERIFICATION_FAILED";

        public const string UserDoesNotExist = "USER_DOES_NOT_EXIST";
        public const string EmailDoesNotExist = "EMAIL_NOT_EXISTED";
        public const string PhoneDoesNotExist = "PHONE_NOT_EXISTED";

        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
        public const string PhoneAlreadyExists = "PHONE_ALREADY_EXISTS";

        public const string UserNameAlreadyExists = "USERNAME_ALREADY_EXISTS";

        public const string BlockLoginByCode = "LOGIN_BY_CODE_BLOCKED";
        public const string BlockSendPhoneCode = "SEND_PHONE_CODE_BLOCKED";

        public const string NotYetFinishingSetupOrg = "NOT_YET_FINISHING_SETUP_ORGANIZATION";

        public const string InvalidEmail = "INVALID_EMAIL";
        public const string InvalidUserNameOrPassword = "INVALID_USER_NAME_OR_PASSWORD";

        public const string SuspiciousRequest = "SUSPICIOUS_REQUEST_DETECTED";

        public const string InvalidApiKey = "INVALID_API_KEY";
        public const string JoinTokenDoesNotExist = "JOIN_TOKEN_DOES_NOT_EXIST";
        public const string CanNotJoinVideoCall = "CAN_NOT_JOIN_VIDEO_CALL";
        public const string VideoCallNotFound = "VIDEO_CALL_NOT_FOUND";

        public const string ArticleDoesNotExist = "ARTICLE_DOES_NOT_EXIST";

        public const string RoleDoesNotExist = "ROLE_DOES_NOT_EXIST";
        public const string UserRoleDoesNotExist = "USER_ROLE_DOES_NOT_EXIST";
        public const string UserClaimDoesNotExist = "USER_CLAIM_DOES_NOT_EXIST";

        public const string ProductDoesNotExist = "PRODUCT_DOES_NOT_EXIST";
        public const string CategoryHasBeenUsed = "CATEGORY_HAS_BEEN_USED";
        public const string CategoryRemoveParentFirst = "REMOVE_PARENT_FIRST";
        public const string MenuDoesNotExist = "MENU_DOES_NOT_EXIST";
        public const string OrderDoesNotExist = "ORDER_DOES_NOT_EXIST";
        public const string InsertErr = "INSERT_ERR";

        public const string ProductHasExisted = "PRODUCT_HAS_EXISTED";

        public const string CustomerDoesNotExist = "CUSTOMER_DOES_NOT_EXIST";
        public const string OldPasswordIncorrect = "OLD_PASSWORD_INCORRECT";
    }
}
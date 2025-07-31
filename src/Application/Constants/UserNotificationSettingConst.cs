namespace Application.Constants
{
    public class UserNotificationSettingScopeConst
    {
        public const string All = "ALL";
        public const string Personal = "PERSONAL";
        public const string Group = "GROUP";
    }

    public class UserNotificationSettingFunctionConst
    {
        public const string All = "ALL";
        public const string Task = "TASK";
        public const string Messaging = "MESSAGING";
    }

    public class UserNotificationSettingTimeOffConst
    {
        public const int OneHour = 60;
        public const int FourHour = 240;
        public const int OneDay = 1440;
        public const int Until8Hour = -8;
        public const int Unlimited = -1;
    }
}
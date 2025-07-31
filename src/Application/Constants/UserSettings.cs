namespace Application.Constants
{
    public static class UserSettingKeys
    {
        public const string FilterPublicGroup = "FILTER_PUBLIC_GROUP";
        public const string OnlyUnreadNotification = "ONLY_UNREAD_NOTIFICATION";

        public static List<string> AllSupportKeys =>
            new List<string>
            {
                FilterPublicGroup,
                OnlyUnreadNotification
            };
    }
}
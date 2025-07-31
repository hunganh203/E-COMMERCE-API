namespace Application.Constants
{
    public class Common
    {
        public const int UserTypeAdmin = 1001;

        public const int UserTypeCustomer = 1002;

        public class JwtConfig
        {
            public const string SecretKey = "6d3492e466f2d9be10cbe956cdd7c61d";
            public const int ExpirationInMinutes = 14400;
        }

        public class EmailKeyGuide
        {
            public const string OTP = "@OTP";
            public const string OrderCode = "@OrderCode";
            public const string OrderDate = "@OrderDate";
            public const string Customer = "@Customer";
            public const string Address = "@Address";
            public const string OrderDetail = "@OrderDetail";
            public const string NewPassword = "@NewPassword";
        }

        public class ResponseHeaderExpose
        {
            public const string TokenExpired = "Token-Expired";
            public const string TokenDeprecated = "Token-Deprecated";
        }
    }
}
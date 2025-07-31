namespace Application.Constants
{
    public class ClaimTypeExtends : ClaimTypeSystem
    {
        public const string OrganizationCode = "OrganizationCode";
    }

    /// <summary>
    /// Parser claim type from system: System.Security.Claims.ClaimTypes
    /// </summary>
    public class ClaimTypeSystem
    {
        public const string NameId = "nameid";
        public const string UniqueName = "unique_name";
        public const string Role = "role";
    }

    public class CustomRequestParams
    {
        public const string XOrganizationCode = "x-organization-code";
        public const string XRequestTime = "x-request-time";

        //  For mobile only
        public const string XPlatform = "x-platform";

        public const string Authorization = "Authorization";
        public const string TokenSchema = "Bearer";
    }
}
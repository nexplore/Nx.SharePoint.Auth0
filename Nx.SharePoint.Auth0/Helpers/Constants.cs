namespace Nx.SharePoint.Auth0.Helpers
{
    public static class Constants
    {
        //Placeholder you need to set it - same key in otherproject  
        public const string PassPhrase = "";
        public const string UserInformationList = "UserInformationList";
        public const string UserProfileApplication = "UserProfileApplication";

        public const string Auth0ClaimPrefix = "i:05.t|auth0|";

        public static class PropertyBagMappings
        {
            public const string UserInfoTarget = "UserInfoTarget";
            public const string Auth0ApiUrl = "Auth0ApiUrl";
            public const string Auth0JwtToken = "Auth0JwtToken";
            public const string SyncMappings = "SyncMappings";
            public const string ProxyUrl = "ProxyUrl";
            public const string UserName = "UserName";
            public const string Password = "Password";
            public const string Domain = "Domain";
        }
    }
}

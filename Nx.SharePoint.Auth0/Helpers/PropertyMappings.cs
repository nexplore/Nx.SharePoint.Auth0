using Microsoft.Office.Server.UserProfiles;

namespace Nx.SharePoint.Auth0.Helpers
{
    public class PropertyMappings
    {
        public const string Auth0Key = "email";
        public const string Auth0ClientKey = "Email";
        public const string UserProfileListKey = "Name";
        public const string UserProfileAppKey = "AccountName";

        public static readonly PropertyMapping[] Mappings =
        {
            //new PropertyMapping("email", "Email", "EMail"),
            new PropertyMapping("name", "FullName", "PreferredName", "Title"),
            //new PropertyMapping("given_name", "FirstName", "FirstName"),
            //new PropertyMapping("family_name", "LastName", "LastName"),
            new PropertyMapping("picture", "Picture", PropertyConstants.PictureUrl, "Picture"),
            //new PropertyMapping("user_id", "UserId", "Name"),
            //new PropertyMapping("nickname", "NickName", string.Empty),
        };
    }

    public class PropertyMapping
    {
        public PropertyMapping(string auth0PropertyName, string auth0ClientPropertyName, string userProfilePropertyName, string userInfoListPropertyName)
        {
            Auth0PropertyName = auth0PropertyName;
            Auth0ClientPropertyname = auth0ClientPropertyName;
            UserProfilePropertyName = userProfilePropertyName;
            UserInfoListPropertyName = userInfoListPropertyName;
        }

        public string Auth0PropertyName { get; private set; }
        public string Auth0ClientPropertyname { get; private set; }
        public string UserProfilePropertyName { get; private set; }
        public string UserInfoListPropertyName { get; private set; }
    }
}

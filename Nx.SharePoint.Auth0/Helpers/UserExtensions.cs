using Auth0.Core;

namespace Nx.SharePoint.Auth0.Helpers
{
    public static class UserExtensions
    {
        public static object GetPropertyValue(this User user, string propertyName)
        {
            return user.GetType().GetProperty(propertyName).GetValue(user, null);
        }
    }
}

using System.Linq;

namespace Nx.SharePoint.Auth0.Helpers
{
    public class UserPropertyMapping
    {
        public UserPropertyMapping(string auth0PropertyName)
        {
            Mapping = PropertyMappings.Mappings.Single(m => m.Auth0PropertyName == auth0PropertyName);
        }

        public PropertyMapping Mapping { get; private set; }
    }
}

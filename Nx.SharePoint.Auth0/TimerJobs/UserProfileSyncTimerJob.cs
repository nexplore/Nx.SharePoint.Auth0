using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Auth0.Core;
using Auth0.Core.Collections;
using Auth0.ManagementApi;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Nx.SharePoint.Auth0.Helpers;

namespace Nx.SharePoint.Auth0.TimerJobs
{
    public class UserProfileSyncTimerJob : SPJobDefinition
    {
        public UserProfileSyncTimerJob() : base()
        {
        }

        public UserProfileSyncTimerJob(string jobName, SPWebApplication webapp) :
            base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "Auth0 User Profile Synchronization";
        }

        public override void Execute(Guid targetInstanceId)
        {
            var webApp = Parent as SPWebApplication;

            if (webApp == null)
            {
                return;
            }

            var auth0ApiUrl = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.Auth0ApiUrl)
                ? Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.Auth0ApiUrl])
                : null;

            var auth0JwtToken = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.Auth0JwtToken)
                ? Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.Auth0JwtToken])
                : null;

            var syncMappings = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.SyncMappings)
                ? Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.SyncMappings])
                : null;

            var proxyUrl = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.ProxyUrl)
                ? Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.ProxyUrl])
                : null;

            var upa = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.UserInfoTarget) && Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.UserInfoTarget]) ==
                      Constants.UserProfileApplication;

            if (string.IsNullOrEmpty(auth0ApiUrl) || string.IsNullOrEmpty(auth0JwtToken) || string.IsNullOrEmpty(syncMappings))
            {
                return;
            }
            
            var mappingValues = syncMappings.Split(',');
            var userMappings = new UserPropertyMapping[mappingValues.Length];

            for (var index = 0; index < mappingValues.Length; index++)
            {
                userMappings[index] = new UserPropertyMapping(mappingValues[index]);
            }

            var auth0Mappings = string.Join(",", userMappings.Select(m => m.Mapping.Auth0PropertyName).Concat(new []{"email", "user_id"}));
            
            var totalUsers = new List<User>();
            
            //// Call the Auth0 API - https://docs.auth0.com/api/v2
            Task<IPagedList<User>> allAuthUsers;
            try
            {
                int pageIndex = 0;

                do
                {
                    allAuthUsers = GetAll(auth0ApiUrl, auth0JwtToken, pageIndex, auth0Mappings, proxyUrl, webApp);
                    allAuthUsers.Wait();

                    totalUsers.AddRange(allAuthUsers.Result);

                    pageIndex++;


                } while (allAuthUsers.Result.Paging.Start <=allAuthUsers.Result.Paging.Total);
            }
            catch (AggregateException agrex)
            {
                LogError(agrex);

                foreach (var ex in agrex.InnerExceptions)
                {
                    LogError(ex);
                }

                throw new Exception("Auth0 API call failed. Please revisit ULS (Category contains 'Auth0') for details");
            }
            catch (Exception ex)
            {
                LogError(ex);

                throw;
            }

            try
            {
                if (upa)
                {
                    SPSecurity.RunWithElevatedPrivileges(() => SynchronizeUserProfile(totalUsers.ToArray(), userMappings));
                }
                else
                {
                    SPSecurity.RunWithElevatedPrivileges(() => SynchronizeUsersList(webApp, totalUsers, userMappings));
                }

            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            base.Execute(targetInstanceId);
        }

        private void SynchronizeUserProfile(User[] allAuthUsers, UserPropertyMapping[] userMappings)
        {
            var serviceContext = SPServiceContext.GetContext(SPServiceApplicationProxyGroup.Default, SPSiteSubscriptionIdentifier.Default);
            var userProfileManager = new UserProfileManager(serviceContext);

            for (int index = 0; index < allAuthUsers.Length; index++)
            {
                this.UpdateProgress((index * 100) / allAuthUsers.Length);

                var authUser = allAuthUsers[index];

                var currentUser = GetUserProfile(userProfileManager, authUser);

                if (currentUser != null)
                {
                    currentUser["WorkEmail"].Value = authUser.Email;

                    foreach (var userMapping in userMappings)
                    {
                        currentUser[userMapping.Mapping.UserProfilePropertyName].Value =
                            authUser.GetPropertyValue(userMapping.Mapping.Auth0ClientPropertyname).ToString();
                    }

                    currentUser.Commit();
                }
                else
                {
                    var accountName = Constants.Auth0ClaimPrefix + authUser.Email;

                    // Create new User here.
                    var newUser = userProfileManager.CreateUserProfile(accountName);
                    newUser["WorkEmail"].Value = authUser.Email;

                    foreach (var userMapping in userMappings)
                    {
                        newUser[userMapping.Mapping.UserProfilePropertyName].Value =
                            authUser.GetPropertyValue(userMapping.Mapping.Auth0ClientPropertyname).ToString();
                    }

                    newUser.Commit();
                }
            }
        }

        private static UserProfile GetUserProfile(UserProfileManager userProfileManager, User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                foreach (UserProfile userProfile in userProfileManager)
                {
                    if (userProfile.AccountName.IndexOf(user.Email, StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        return userProfile;
                    }
                }
            }

            return null;
        }

        private void SynchronizeUsersList(SPWebApplication webApp, IEnumerable<User> allAuthUsers, UserPropertyMapping[] userMappings)
        {
            foreach (SPSite oSite in webApp.Sites)
            {
                var rootWeb = oSite.RootWeb;

                rootWeb.AllowUnsafeUpdates = true;

                var userInformationList = rootWeb.SiteUserInfoList;

                for (var index = 0; index < userInformationList.ItemCount; index++)
                {
                    this.UpdateProgress((index * 100) / userInformationList.ItemCount);

                    try
                    {
                        var userId = userInformationList.Items[index].ID;

                        var userItem = userInformationList.GetItemById(userId);

                        var userName = Convert.ToString(userItem[PropertyMappings.UserProfileListKey]).ToLower();

                        var authUser = allAuthUsers.FirstOrDefault(au => UserExists(au, userName));

                        if (authUser != null)
                        {
                            foreach (var userMapping in userMappings)
                            {
                                if (userMapping.Mapping.UserInfoListPropertyName == "Picture")
                                {
                                    var value = new SPFieldUrlValue
                                    {
                                        Description = "Profile Image",
                                        Url = authUser.GetPropertyValue(userMapping.Mapping.Auth0ClientPropertyname).ToString()
                                    };

                                    userItem[userMapping.Mapping.UserInfoListPropertyName] = value;
                                }
                                else
                                {
                                    userItem[userMapping.Mapping.UserInfoListPropertyName] =
                                        authUser.GetPropertyValue(userMapping.Mapping.Auth0ClientPropertyname).ToString();
                                }
                            }

                            userItem.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                    }
                }

                rootWeb.AllowUnsafeUpdates = false;
            }
        }

        /// <summary>Gets the credentials.</summary>
        /// <param name="webApp">The web application.</param>
        /// <returns></returns>
        private NetworkCredential GetCredentials(SPWebApplication webApp)
        {
            var userName = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.UserName)
                ? Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.UserName])
                : null;

            var userPassword = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.Password)
                ? Crypto.Decrypt(Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.Password]), Constants.PassPhrase)
                : null;

            var domain = webApp.Properties.ContainsKey(Constants.PropertyBagMappings.Domain)
                ? Convert.ToString(webApp.Properties[Constants.PropertyBagMappings.Domain])
                : null;

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userPassword) && !string.IsNullOrEmpty(domain))
            {
                return new NetworkCredential(userName, userPassword, domain);
            }

            return CredentialCache.DefaultNetworkCredentials;
        }

        private static bool UserExists(User au, string userName)
        {
            return au.Email != null && userName.IndexOf(au.Email, 0, StringComparison.InvariantCultureIgnoreCase) > 0;
        }

        public async Task<IPagedList<User>> GetAll(string auth0ApiUrl, string auth0JwtToken, int pageIndex, string userMappings = null, string proxyUrl = null, SPWebApplication webApp = null)
        {
            ManagementApiClient client;
            var creds = GetCredentials(webApp);

            if (string.IsNullOrEmpty(proxyUrl))
            {
                client = new ManagementApiClient(auth0JwtToken, new Uri(auth0ApiUrl));
            }
            else
            {
                client = new ManagementApiClient(auth0JwtToken, new Uri(auth0ApiUrl), creds, new Uri(proxyUrl));
            }

            try
            {
                return await client.Users.GetAllAsync(pageIndex, null, true, null, null, userMappings, true, null, "v2");
            }
            catch (Exception ex)
            {
                LoggingService.LogInformation("Proxy URL: " + proxyUrl);
                LoggingService.LogInformation("Username: " + creds.UserName);
                LoggingService.LogInformation("Domain: " + creds.Domain);
#if DEBUG
                LoggingService.LogInformation("Password: " + creds.Password);
#endif

                throw ex;
            }
        }

        private static void LogError(Exception ex)
        {
            LoggingService.LogError(ex.Message + "\r\n" + ex.StackTrace);

            if (ex.InnerException != null)
            {
                LogError(ex.InnerException);
            }
        }
    }
}

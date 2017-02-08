using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using Nx.SharePoint.Auth0.Helpers;

namespace Nx.SharePoint.Auth0.ADMIN.Nx
{
    public partial class Auth0 : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetWebApps();

                SyncMappingsCheckBoxList.DataSource = PropertyMappings.Mappings.Select(m => m.Auth0PropertyName);
                SyncMappingsCheckBoxList.DataBind();

                UpdateContolValues();
            }
        }

        protected void SetWebApps()
        {
            WebApplicationsList.Items.Clear();
           
            foreach (var webApp in SPWebService.ContentService.WebApplications)
            {
                WebApplicationsList.Items.Add(new ListItem(webApp.Name, webApp.Id.ToString()));
            }
        }

        protected void ButtonSave_OnClick(object sender, EventArgs e)
        {
            try
            {
                var id = WebApplicationsList.SelectedValue;
                var webApp = SPWebService.ContentService.WebApplications[new Guid(id)];

                var userTarget = UserProfileApplication.Checked
                    ? Constants.UserProfileApplication
                    : Constants.UserInformationList;

                AddProperty(webApp, Constants.PropertyBagMappings.UserInfoTarget, userTarget);

                AddProperty(webApp, Constants.PropertyBagMappings.Auth0ApiUrl, Auth0ApiUrlTextBox.Text);
                AddProperty(webApp, Constants.PropertyBagMappings.Auth0JwtToken, JwtTokenTextBox.Text);

                var syncMappings = string.Join(",",
                    SyncMappingsCheckBoxList.Items.Cast<ListItem>().Where(i => i.Selected).Select(p => p.Value));

                AddProperty(webApp, Constants.PropertyBagMappings.SyncMappings, syncMappings);

                AddProperty(webApp, Constants.PropertyBagMappings.ProxyUrl, ProxyUrlTextBox.Text);
                AddProperty(webApp, Constants.PropertyBagMappings.UserName, UserNameTextBox.Text);
                
                AddProperty(webApp, Constants.PropertyBagMappings.Password, UserPasswordTextBox.Text, true);

                AddProperty(webApp, Constants.PropertyBagMappings.Domain, DomainTextBox.Text);

                Auth0ApiUrlTextBox.Enabled = false;
                JwtTokenTextBox.Enabled = false;
                SyncMappingsCheckBoxList.Enabled = false;
                ProxyUrlTextBox.Enabled = false;
                UserNameTextBox.Enabled = false;
                UserPasswordTextBox.Enabled = false;
                DomainTextBox.Enabled = false;
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex.Message);
            }
        }

        private void AddProperty(SPWebApplication webApp, string key, string value, bool useCrypto = false)
        {
            if (webApp.Properties.ContainsKey(key))
            {
                webApp.Properties.Remove(key);
                webApp.Update();
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            webApp.Properties.Add(key, useCrypto ? Crypto.Encrypt(value, Constants.PassPhrase) : value);
            webApp.Update();
        }

        private string GetPropertyValue(SPWebApplication webApp, string key)
        {
            if (webApp.Properties.ContainsKey(key))
            {
                return Convert.ToString(webApp.Properties[key]);
            }

            return string.Empty;
        }

        protected void WebApplicationsList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateContolValues();
        }

        private void UpdateContolValues()
        {
            var id = WebApplicationsList.SelectedValue;
            var webApp = SPWebService.ContentService.WebApplications[new Guid(id)];

            var upa = GetPropertyValue(webApp, Constants.PropertyBagMappings.UserInfoTarget) ==
                      Constants.UserProfileApplication;

            UserInformationList.Checked = !upa;
            UserProfileApplication.Checked = upa;

            Auth0ApiUrlTextBox.Text = GetPropertyValue(webApp, Constants.PropertyBagMappings.Auth0ApiUrl);
            JwtTokenTextBox.Text = GetPropertyValue(webApp, Constants.PropertyBagMappings.Auth0JwtToken);
            ProxyUrlTextBox.Text = GetPropertyValue(webApp, Constants.PropertyBagMappings.ProxyUrl);
            UserNameTextBox.Text = GetPropertyValue(webApp, Constants.PropertyBagMappings.UserName);
            DomainTextBox.Text = GetPropertyValue(webApp, Constants.PropertyBagMappings.Domain);

            var syncMappings = GetPropertyValue(webApp, Constants.PropertyBagMappings.SyncMappings).Split(',');

            foreach (ListItem item in SyncMappingsCheckBoxList.Items)
            {
                if (syncMappings.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
        }

        protected void ButtonReset_OnClick(object sender, EventArgs e)
        {
            Auth0ApiUrlTextBox.Enabled = true;
            JwtTokenTextBox.Enabled = true;
            SyncMappingsCheckBoxList.Enabled = true;

            ProxyUrlTextBox.Enabled = true;
            UserNameTextBox.Enabled = true;
            UserPasswordTextBox.Enabled = true;
            DomainTextBox.Enabled = true;
        }
    }
}

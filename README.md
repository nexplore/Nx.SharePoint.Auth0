# Nx.SharePoint.Auth0 (timerjob to sync between auth0 userprofile information and sharepoint userlist)

##Prerequisites:
- SharePoint solution development tools for Visual Studio 2015

## Installation

  1. Open solution and enable "NuGet Package Restore"
  2. Compile solution
  3. Right click on project -> Package (that will generate a .wsp file)
  4. Open a SharePoint Powershell session to install and deploy the solution:

  ~~~ps1
  Add-SPSolution -LiteralPath "<path to .wsp file>"
  Install-SPSolution -Identity nx.sharepoint.auth0.wsp -GACDeployment
  Enable-SPFeature -Identity Nx.SharePoint.Auth0.UserProfileSync -Url "<WebApp>" # Where the sync should run
  ~~~

## Configuration
  1. (Optional) Set the webproxy username + domain + password
	 You can set the password plaintext or set the PassPhrase in the <a href="https://github.com/nexplore/Nx.SharePoint.Auth0/blob/master/Nx.SharePoint.Auth0/Helpers/Constants.cs" target="_blank">Constants.cs line 6 </a> and/or <a href="https://github.com/nexplore/Auth0.ClaimsProvider/blob/master/Auth0.ClaimsProvider/Helper/Crypto.cs" target="_blank">Crypto.cs line 14</a>

  2. Go to Central Admin -> Auth0 Administration Settings        
    1. click on "Auth0 Sync Timer Job "
    2. Set the required configuration parameters (like Sync to UserProfile or User Information List, webapp, Auth0 API Url (https://[Domain]/api/v2), JWT Token(https://auth0.com/docs/api/management/v2 -> read:users), Sync Mappings, Proxy Url, Username (optional), Passwort (optional), Domain (optional))
  
## Uninstall 

~~~ps1
Disable-SPFeature -Identity e61779ec-bb4d-44ae-99e8-01ea027bef16 -url "<WebApp>"
Uninstall-SPFeature -Identity e61779ec-bb4d-44ae-99e8-01ea027bef16

 Uninstall-SPSolution -Identity nx.sharepoint.auth0.wsp
 Remove-SPSolution -Identity nx.sharepoint.auth0.wsp
~~~


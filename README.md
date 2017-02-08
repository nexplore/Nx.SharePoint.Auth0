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

## Uninstall 
~~~ps1
Disable-SPFeature -Identity e61779ec-bb4d-44ae-99e8-01ea027bef16 -url "<WebApp>"
Uninstall-SPFeature -Identity e61779ec-bb4d-44ae-99e8-01ea027bef16 
~~~






## Configuration  
  1. When enable Auth0 for the SharePoint application, make sure that "Client ID" (http://schemas.auth0.com/clientID) and       "Connection" (http://schemas.auth0.com/connection) claims are part of the list of required claims  
  2. Associate Auth0 (SP Trusted Identity Token Issuer) with our Claims Provider: 
  
  ~~~ps1
  Set-SPTrustedIdentityTokenIssuer -identity Auth0 -ClaimProvider "Auth0FederatedUsers"  
  ~~~
  
  3. Go to Central Admin -> Security      
    1. Under General Security section, click on "Configure Auth0 Claims Provider"      
    2. Set the required configuration parameters (like domain, client ID, client secret and identifier user field)
  
  4. Set the Webproxy url
  
    ~~~ps1
    $wa = Get-SPWebapplication https://sharepoint.example.ch
    $wa.Properties["ProxyUrl"] = "http://webproxy.example.ch:9055"
    $wa.Update()
    ~~~

  5. (Optional) Set the webproxy username + domain + password
    1. You can set the password plaintext or set the PassPhrase in the https://github.com/nexplore/Auth0.ClaimsProvider/blob/master/Auth0.ClaimsProvider/Helper/Crypto.cs line 14  
    2. Open PowerShell  
    ~~~ps1
    $wa = Get-SPWebapplication https://sharepoint.example.ch
    $wa.Properties["UserName"] = "Property Value"
    $wa.Properties["Password"] = "Property Value" # plain text or step 3 encrypted 
    $wa.Properties["Domain"] = "Property Value"
    $wa.Update()
    ~~~
    3. <a href="https://gallery.technet.microsoft.com/scriptcenter/PowerShell-Script-410ef9df" target="_blank">$encryptedpassowrd = Encrypt-String "PassPhrase from step 1" "MyStrongPassword"</a>


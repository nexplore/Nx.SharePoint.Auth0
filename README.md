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
  ~~~

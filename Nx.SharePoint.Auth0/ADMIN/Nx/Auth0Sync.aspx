<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Auth0Sync.aspx.cs" Inherits="Nx.SharePoint.Auth0.ADMIN.Nx.Auth0" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        .nxlabel {
            display: block;
            margin-bottom: 3px;
        }

        .nxinput {
            width: 100%;
            margin-bottom: 8px;
        }

        .nxcontent {
            width: 30%;
        }

        .nxbutton {
            float: right;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="nxcontent">
        <div class="nxinput">
            <label class="nxlabel">Sync to UserProfile or User Information List:</label>
            <asp:RadioButton GroupName="UserInfoTarget" ID="UserProfileApplication" runat="server" Text="UserProfile" />
            <asp:RadioButton GroupName="UserInfoTarget" ID="UserInformationList" runat="server" Text="User Information List" />
        </div>

        <label class="nxlabel">Select Web App:</label>
        <asp:DropDownList CssClass="nxinput" ID="WebApplicationsList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="WebApplicationsList_OnSelectedIndexChanged" />
        <label class="nxlabel">Auth0 API Url (https://[Domain]/api/v2): </label>
        <asp:TextBox CssClass="nxinput" ID="Auth0ApiUrlTextBox" runat="server"></asp:TextBox>
        <label class="nxlabel">JWT Token:</label>
        <asp:TextBox CssClass="nxinput" runat="server" TextMode="MultiLine" Rows="5" ID="JwtTokenTextBox"></asp:TextBox>

        <asp:Label CssClass="nxlabel" runat="server">Sync Mappings:</asp:Label>
        <asp:CheckBoxList CssClass="nxinput" ID="SyncMappingsCheckBoxList" runat="server"></asp:CheckBoxList>

        <hr />

        <label class="nxlabel">Proxy Url: </label>
        <asp:TextBox CssClass="nxinput" ID="ProxyUrlTextBox" runat="server"></asp:TextBox>
        <label class="nxlabel">Username (optional): </label>
        <asp:TextBox CssClass="nxinput" ID="UserNameTextBox" runat="server"></asp:TextBox>
        <label class="nxlabel">Passwort (optional): </label>
        <asp:TextBox CssClass="nxinput" TextMode="Password" ID="UserPasswordTextBox" runat="server"></asp:TextBox>
        <label class="nxlabel">Domain (optional): </label>
        <asp:TextBox CssClass="nxinput" ID="DomainTextBox" runat="server"></asp:TextBox>

        <asp:Button Text="Save" ID="ButtonSave" CssClass="nxbutton" runat="server" OnClick="ButtonSave_OnClick" />
        <asp:Button Text="Reset" ID="ButtonReset" CssClass="nxbutton" runat="server" OnClick="ButtonReset_OnClick" />
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Auth0 Sync Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Auth0 Sync Settings
</asp:Content>

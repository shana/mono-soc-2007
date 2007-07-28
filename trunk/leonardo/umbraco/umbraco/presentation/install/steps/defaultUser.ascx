<%@ Control Language="c#" AutoEventWireup="True" Codebehind="defaultUser.ascx.cs" Inherits="umbraco.presentation.install.steps.defaultUser" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<h1>Step 3: Check umbraco security</h1>
<asp:Panel ID="identify" Runat="server" Visible="True">
	<P>umbraco creates a default user with a login ('admin') and password ('default'). It's important that the password is changed to something unique.<br />
		<br />
		This step will check the default user's password and suggest if it needs to be changed.<br />
		<br />
		<asp:Literal id="identifyResult" Runat="server"></asp:Literal></P>
</asp:Panel>
<asp:Panel ID="changeForm" Runat="server" Visible="false">
Enter 
Password: 
<asp:TextBox id="password" Runat="server" Width="150px"></asp:TextBox>
<asp:Button id="changePassword" Runat="server" Text="Change Password" onclick="changePassword_Click"></asp:Button>
</asp:Panel>
<asp:Panel ID="passwordChanged" Runat="server" Visible="False">
The password is changed!
</asp:Panel>

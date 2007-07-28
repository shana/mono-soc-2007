<%@ Control Language="c#" AutoEventWireup="True" Codebehind="detect.ascx.cs" Inherits="umbraco.presentation.install.steps.detect" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<h1>Step 1: System Inspection</h1>
<asp:Panel ID="identify" Runat="server" Visible="True">
<p>Your database has been found and is identified as:<br/>
	<b>
		<asp:Literal ID="version" Runat="server"></asp:Literal></b></p>
<p>
	<asp:Literal ID="v3" Runat="server" Visible="False">
Your current database is up-to-date!
</asp:Literal>
	<asp:Literal ID="other" Runat="server" Visible="False">
Press the 
<B>upgrade</B> button to upgrade your database to umbraco 3.0. <br /><br/>
Don't worry - no content will be deleted and everything will continue working afterwards!
</asp:Literal>
	<asp:Literal ID="none" Runat="server" Visible="False">
Press the 
<B>install</B> button to install the umbraco 3.0 database!
</asp:Literal>
	<asp:Literal ID="error" Runat="server" Visible="False">
Database not found! Please check that the information in the "connection string" of the “web.config” file is correct.<br/>
<br/>
To proceed, please edit the "web.config" file (using Visual Studio or your favourite text editor), scroll to the bottom, add the connection string for your database in the key named "umbracoDbDSN" and save the file. <br/>
<br/>
<br /><br />Click the <B>retry</B> button when 
done.<br /><A href="http://umbraco.org/redir/installWebConfig" target="_blank">
			More information on editing web.config here.</A><br />
</asp:Literal>
</p>
</asp:Panel>
<asp:Button ID="upgrade" Text="Upgrade" Visible="False" Runat="server" CssClass="button" onclick="upgrade_Click"></asp:Button>
<asp:Button ID="install" Text="Install" Visible="False" Runat="server" CssClass="button" onclick="install_Click"></asp:Button>
<asp:Button ID="retry" Text="Retry" Visible="False" Runat="server" CssClass="button"></asp:Button>
<asp:Panel ID="confirms" Runat="server" Visible="False">
	<P>
		<asp:Literal id="installConfirm" Runat="server" Visible="False">The umbraco 3.0 
database has now been copied to your database. Press <B>Next</B> to proceed. 
</asp:Literal>
		<asp:Literal id="upgradeConfirm" Runat="server" Visible="False">Your 
database has been upgraded to the final version 3.0.<br />Press <B>Next</B> to 
proceed. </asp:Literal></P>
</asp:Panel>

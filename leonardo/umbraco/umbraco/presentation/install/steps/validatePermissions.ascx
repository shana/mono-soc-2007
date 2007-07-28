<%@ Control Language="c#" AutoEventWireup="True" Codebehind="validatePermissions.ascx.cs" Inherits="umbraco.presentation.install.steps.validatePermissions" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<h1>Step 2: Validating File Permissions</h1>
<p>umbraco needs write/modify access to certain directories in order to store files like pictures and PDF's. It also stores temporary data (aka: cache) for enhancing the performance of your website.</p>
<p>
	<asp:literal id="perfect" Runat="server" Visible="False"><B>Your permission settings 
			are perfect!</B><br />You are ready to run umbraco and install packages!</asp:literal><asp:literal id="noPackages" Runat="server" Visible="False"><B>Your 
			permission settings are almost perfect!</B><br />You can run umbraco without problems, but you will not be able to install packages which are recommended to take full advantage of umbraco.
</asp:literal><asp:literal id="noFolders" Runat="server" Visible="False"><B>Your permission settings might be an issue!</B><br />You can run umbraco without problems, but you will not be able to create folders or install packages which are recommended to take full advantage of umbraco.</asp:literal><asp:literal id="error" Runat="server" Visible="False"><B>Your 
			permission settings are not ready for umbraco!</B><br />In order to run umbraco, you'll need to update your permission settings.</asp:literal>
</p>
<p>
	<asp:Panel Visible="False" Runat="server" ID="howtoResolve">
		<B>How to Resolve</B><br />
		<asp:Panel id="grant" Visible="True" Runat="server">You need to grant ASP.NET modify permissions to 
the following: 
<UL>
				<asp:Literal id="permSummary" Runat="server"></asp:Literal></UL>More <A href="http://216.26.163.129/web/umbraco/installerdocs/settingfolderpermissions.htm"
				target="_blank">information on setting up permissions for umbraco here</A>.<br /></asp:Panel>
		<asp:Panel id="folderWoes" Visible="False" Runat="server">
<br/>
<b>Resolving folder issue</b><br/>
				Follow <A href="http://groups.google.com/group/microsoft.public.dotnet.framework.aspnet/browse_thread/thread/46d6582afffc96b0/9088396b3323d4ae?lnk=st">this link for more information on problems with ASP.NET and creating 
				folders</A>.<br /></asp:Panel>
	</asp:Panel>
	<br />
<P></P>
<p><a href="#" onClick="javascript:document.getElementById('detailsPane').style.display = 'block';"><b>View 
			Details:</b></a><br />
</p>
<div id="detailsPane" style="DISPLAY: none">
	<p style="FONT-SIZE: 80%">
		- Checking default folder permissions:<br />
		<asp:literal id="permissionResults" Runat="server"></asp:literal><br />
		- Checking optional folder permissions for packages:<br />
		<asp:literal id="packageResults" Runat="server"></asp:literal><br />
		- Cache (the umbraco.config file):
		<asp:literal id="xmlResult" Runat="server"></asp:literal><br />
		<br />
		- Creating folders:
		<asp:literal id="foldersResult" Runat="server"></asp:literal><br />
		<br />
	</p>
</div>

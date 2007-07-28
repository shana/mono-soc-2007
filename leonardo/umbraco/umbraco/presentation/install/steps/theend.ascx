<%@ Control Language="c#" AutoEventWireup="True" Codebehind="theend.ascx.cs" Inherits="umbraco.presentation.install.steps.theend" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
function openDialog(diaTitle, diaDoc, dwidth, dheight, optionalParams)
{
  theDialogWindow = window.open(diaDoc,'dialogpage', "width="+dwidth+"px,height="+dheight+"px"+optionalParams);// window.showModalDialog(diaDoc, "MyDialog", strFeatures);
}

function runStarterKits() {
    openDialog('packager', 'http://packages.umbraco.org/packages/website-packages?callback=<%=Request.ServerVariables["SERVER_NAME"] + umbraco.GlobalSettings.Path + "/dialogs/packager.aspx" %>', 530, 550, ',scrollbars=yes');

}
</script>
<h1>That's it!</h1>
<asp:Panel ID="success" runat="server">
<p>Congratulations - umbraco is now properly configured.</p>
<p>umbraco <%=umbraco.GlobalSettings.CurrentVersion%> have automatically configured and saved all settings and you're ready to go.<br/>
</asp:Panel>
<asp:Panel ID="updateUmbracoSettingsFailed" Visible="false" runat="server">
<p>Congratulations - umbraco is now almost properly configured.</p>
<p>Unfortunately, umbraco couldn't update the /config/umbracoSettings.config due to permission problems (<a href="#" onclick="document.getElementById('errorDetails').style.display = 'block';">details</a>).</p>
<p style="display:none;" id="errorDetails"><asp:Literal ID="errorLiteral" runat="server"></asp:Literal></p>
<p> To finish the installation, you'll need to 
manually edit the <strong>/web.config file</strong> and update the AppSetting key <strong>umbracoConfigurationStatus</strong> in the bottom to the value of <strong>'<%=umbraco.GlobalSettings.CurrentVersion %>'</strong>.</p>
</asp:Panel>	<br />
	It's now important that you delete the /install directory to ensure that no one can alter the configuration of your site!<br/>
	<br />
	
	<h3 style="border-top: 1px solid #ccc; padding-top: 10px;">Getting started...</h3>
<p>Once you're done with the steps above, you can start using umbraco by clicking the button below. <br /><br />
You can also browse and install available <a href="http://umbraco.org/redir/starter-kits">umbraco Starter Kits</a> that can help you getting started. Just click the "Browse Starter Kits" button.<br /><br />
</p>

<p>If you need information on how to use umbraco, visit <a href="http://umbraco.org/redir/welcome-books">the books section on umbraco.org</a>.<br /><br /></p>
<p>		<input type="button" value="Browse Starter Kits" onclick="runStarterKits()"> &nbsp; 
        <input type="button" value="Launch umbraco" onclick="document.location.href = '<%=umbraco.GlobalSettings.Path %>';">
</P>
</p>
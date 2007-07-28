<%@ Page language="c#" Codebehind="packager.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.packager" trace="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - Packager </title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>BODY {
	MARGIN: 2px
}
</style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><A class="guiDialogTiny" href="javascript:window.close()"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></A></div>
			<h3 style="MARGIN-BOTTOM: 5px"><IMG style="FILTER: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/package2.png', sizingMethod='scale')"
					height="16" src="../images/nada.gif" width="16" align="absMiddle"> Import 
				Package</h3>
			<IMG class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif"><br />
			<asp:literal id="FeedBackMessage" Runat="server"></asp:literal><asp:panel id="TheForm" Runat="server" Visible="True"><SPAN class="guiDialogNormal" style="MARGIN-LEFT: 10px"><STRONG>
						<asp:Literal id="SubHeader" Runat="server"></asp:Literal><br />
					</STRONG>
				</SPAN>
				<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyContent" colSpan="2"><INPUT id="tempFile" type="hidden" name="tempFile" runat="server"><INPUT id="processState" type="hidden" name="processState" runat="server">
							<asp:Panel id="PanelUpload" runat="server"><SPAN class="guiDialogTiny">Choose Package from your machine, 
      by clicking the Browse button and locate the package. umbraco packages has 
      an ".umb" extension.<br /></SPAN><br /><INPUT id="file1" type="file" name="file1" runat="server"><br /><br /><INPUT onclick="window.close()" type="button" value="Cancel"> 
      &nbsp; 
<asp:Button id="ButtonLoadPackage" runat="server" Text="Load Package" onclick="Button1_Click"></asp:Button>
<br /><br />
<h3 style="margin-left: 0px;">Browse repository</h3>
<p class="guiDialogNormal">You can also browse the "umbraco package directory" for automated installation:<br />
    <a href="http://packages.umbraco.org/packages?callback=<%=Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + umbraco.GlobalSettings.Path + "/dialogs/packager.aspx" %>">Go to the package repository</a>
     </p>
</asp:Panel>
							<asp:Panel id="PanelAccept" runat="server" Visible="False">
								<P>
									<TABLE cellSpacing="0" cellPadding="4" border="0">
										<TR>
											<TD><B>
													<asp:Label id="Label1" runat="server">Name:</B></asp:Label>&nbsp;</B></TD>
											<TD>
												<asp:Label id="LabelName" runat="server">Label</asp:Label></TD>
										</TR>
										<TR>
											<TD><B>
													<asp:Label id="Label4" runat="server">More info:</B></asp:Label>&nbsp;</B></TD>
											<TD>
												<asp:Label id="LabelMore" runat="server">Label</asp:Label></TD>
										</TR>
										<TR>
											<TD><B>
													<asp:Label id="Label6" runat="server">Author:</B></asp:Label>&nbsp;</B></TD>
											<TD>
												<asp:Label id="LabelAuthor" runat="server">Label</asp:Label></TD>
										</TR>
										<TR>
											<TD vAlign="top"><B>
													<asp:Label id="Label10" runat="server">License:</B></asp:Label>&nbsp;</B></TD>
											<TD>
												<asp:Label id="LabelLicense" runat="server">Label</asp:Label><br />
												<INPUT onmouseup="document.getElementById('ButtonInstall').disabled = this.checked" type="checkbox">
												Accept License</TD>
										</TR>
										<TR>
											<TD><B>
													<asp:Label id="Label2" runat="server">Read Me:</B></asp:Label>&nbsp;</B></TD>
											<TD>
												<asp:Literal id="readme" Runat="server"></asp:Literal></TD>
										</TR>
									</TABLE>
									&nbsp;<br />
									<span style="display:none;" id="installingMessage"><img src="../images/nada.gif" align="absmiddle" style="width: 16px; height: 16px;" id="pleaseWaitIcon" alt="Please wait" /> <em>Installing package, please wait...</em><br /></span>
									<asp:Button id="ButtonInstall" runat="server" Text="Install Package" Enabled="False" onclick="ButtonInstall_Click"></asp:Button></P>
								<P>&nbsp;</P>
							</asp:Panel>
							<asp:Panel id="optionalControl" Runat="server" Visible="false"></asp:Panel>
							<asp:Panel id="succes" Runat="server" Visible="false">
								<B>Package is installed!</B></asp:Panel>
						</TD>
					</TR>
				</TABLE>
			</asp:panel></form>
	</body>
</HTML>

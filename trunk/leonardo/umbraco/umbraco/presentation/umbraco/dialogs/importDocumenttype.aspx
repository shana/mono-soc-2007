<%@ Page language="c#" Codebehind="importDocumenttype.aspx.cs" AutoEventWireup="false" Inherits="umbraco.presentation.umbraco.dialogs.importDocumentType" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco -
			<%=umbraco.ui.Text("importDocumenttype")%>
		</title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>BODY { MARGIN: 2px }
	</style>
			<script type="text/javascript" src="../js/xmlextras.js"></script>
			<script type="text/javascript" src="../js/xmlRequest.js"></script>
			<script language="javascript">
			</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<INPUT id="tempFile" type="hidden" name="tempFile" runat="server">
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/importDocumenttype.png', sizingMethod='scale')">
				<%=umbraco.ui.Text("importDocumentType")%>
			</h3>
			<img class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif"><br />
			<asp:Literal ID="FeedBackMessage" Runat="server" />
			<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
				runat="server">
				<TR>
					<TD class="propertyContent" colSpan="2">
						<asp:Panel ID="Wizard" Runat="server" Visible="True">
							<SPAN class="guiDialogNormal">
								<%=umbraco.ui.Text("importDocumentTypeHelp")%>
							</SPAN>
							<br /><br />
							<INPUT id="documentTypeFile" type="file" runat="server">
							<br /><br />
							<asp:Button id="submit" Runat="server"></asp:Button>
						</asp:Panel>
						<asp:Panel ID="Confirm" Runat="server" Visible="False">
							<STRONG>
								<%=umbraco.ui.Text("name")%>
								:</STRONG>
							<asp:Literal id="dtName" Runat="server"></asp:Literal>
							<br />
							<STRONG>
								<%=umbraco.ui.Text("alias")%>
								:</STRONG>
							<asp:Literal id="dtAlias" Runat="server"></asp:Literal>
							<br />
							<br />
							<asp:Button id="import" Runat="server"></asp:Button>
						</asp:Panel>
						<asp:Panel ID="done" Runat="server" Visible="False">
<asp:Literal id="dtNameConfirm" Runat="server"></asp:Literal> has been imported!
						</asp:Panel>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>

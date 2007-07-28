<%@ Page language="c#" Codebehind="cruds.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.cruds" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco -
			<asp:Literal id="Title" runat="Server"></asp:Literal>
		</title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>BODY { MARGIN: 2px }
	</style>
			<script type="text/javascript" src="../js/xmlextras.js"></script>
			<script type="text/javascript" src="../js/xmlRequest.js"></script>
			<script language="javascript">


function doSubmit() {document.Form1["ok"].click()}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	
			</script>
			<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3 style="margin-bottom: 5px;"><img src="../images/permission.png" align="absMiddle">
				<asp:Literal ID="Header" Runat="server"></asp:Literal></h3>
				<span class="guiDialogNormal" style="margin-left: 15px;"><%=umbraco.ui.Text("permissionsEdit")%> <b><asp:Literal Runat="server" ID="pageName"/></b></span><br/>
			<img class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif"><br />
			<asp:Literal ID="FeedBackMessage" Runat="server" />
			<br />
			<asp:Panel ID="TheForm" Runat="server" Visible="True">
				<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyHeader" colSpan="2">
							<asp:PlaceHolder id="PlaceHolder1" runat="server"></asp:PlaceHolder></TD>
					</TR>
				</TABLE>
				&nbsp; <asp:Button id="Button1" runat="server" Text="" onclick="Button1_Click"></asp:Button> &nbsp; <input type="button" onclick="parent.window.close()" value="<%=umbraco.ui.Text("cancel")%>"/>
				<P></P>
			</asp:Panel>
		</form>
	</body>
</HTML>

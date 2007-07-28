<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="publish.aspx.cs" Inherits="umbraco.presentation.actions.publish" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title><%=umbraco.ui.Text("publish") %> - umbraco CMS</title>
	<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
</head>
	<body marginheight="0" marginwidth="0" topmargin="0" leftmargin="0" onLoad="resizePanel('Panel2',false);"
		onResize="resizePanel('Panel2',false);">
		<form id="Form1" method="post" runat="server">
		
		    <cc1:UmbracoPanel ID="Panel2" runat="server">

			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/publish.gif" align="absMiddle" width="16" height="16"/>
				<asp:Literal ID="header" runat="server"></asp:Literal></h3>
			<div style="border-bottom: 1px solid #ccc; margin: 0 10px;"></div>
			<div style="margin-left: 10px;">
			<asp:Panel ID="confirm" runat="server">
			<p class="guiDialogNormal"><asp:Literal ID="warning" runat="server"></asp:Literal><br /><br />
			<asp:Button ID="deleteButton" runat="server" OnClick="deleteButton_Click" />
			</p>
			</asp:Panel>
			<asp:Panel ID="deleteMessage" runat="server" Visible="false">
			<p class="guiDialogNormal"><asp:Literal ID="deleted" runat="server"></asp:Literal></p>
			</asp:Panel>
			</div>
			</cc1:UmbracoPanel>
			</form>
</body>
</html>

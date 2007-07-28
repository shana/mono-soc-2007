<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="preview.aspx.cs" Inherits="umbraco.presentation.translation.preview" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Translation Previewer</title>
	<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
</head>
	<body marginheight="0" marginwidth="0" topmargin="0" leftmargin="0" onLoad="resizePanel('Panel2',false);"
		onResize="resizePanel('Panel2',false);">
		<form id="Form1" method="post" runat="server">
			<cc1:UmbracoPanel id="Panel2" runat="server" Height="224px" Width="412px" hasMenu="false">

			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/sendToTranslate.png', sizingMethod='scale')">
				Preview page</h3>
			<div style="border-bottom: 1px solid #ccc; margin: 0 10px;"></div>
            <p style="margin: 10px;" class="guiDialogNormal">Click the links below to see the original and translated version.</p>
			<div style="padding: 2px 15px 0px 15px">
			<p class="guiDialogNormal">
			    <a href='<asp:literal ID="newLink" runat="Server"></asp:literal>' target='_blank'><IMG src="../images/sendToTranslate.png" align="absMiddle" border="0"> See the translated page (<asp:Literal ID="newName" runat="server"></asp:Literal>)</a><br /><br />
			    <asp:Panel ID="originalPanel" runat="server" CssClass="guiDialogNormal">
			    <a href='<asp:literal ID="orgLink" runat="Server"></asp:literal>' target='_blank'><IMG src="../images/forward.png" align="absMiddle" border="0"> See the original page (<asp:Literal ID="orgName" runat="server"></asp:Literal>)</a><br /><br />
			    </asp:Panel>
			    <asp:Panel ID="originalPanelError" runat="server" Visible="false" CssClass="guiDialogNormal">
			        <IMG src="../images/forward.png" align="absMiddle" border="0"> Unable to preview original page (the translated version has not been related to the original).<br />
			    </asp:Panel>
			</p>
			</div>
			</cc1:UmbracoPanel>
		</form>
	</body>
</html>

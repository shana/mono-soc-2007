<%@ Page language="c#" Codebehind="viewCacheItem.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.developer.viewCacheItem" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>viewCacheItem</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script>
		function resizePanel(PanelName, hasMenu) {
				
				var clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
				var clientWidth = document.body.clientWidth;

				panelWidth = clientWidth;
			
				contentHeight = clientHeight-68;
				if (hasMenu) contentHeight = contentHeight - 32;
				
				document.getElementById(PanelName).style.width = panelWidth + "px";
				document.getElementById(PanelName+'_content').style.height = contentHeight + "px";
				document.getElementById(PanelName+'_content').style.width = panelWidth + "px";
				
				document.getElementById(PanelName+'_menu').style.width = (panelWidth - 7)+"px"
				scrollwidth = panelWidth - 35;
				document.getElementById(PanelName +"_menu").style.width = scrollwidth + "px";
				document.getElementById(PanelName +"_menu_slh").style.width = scrollwidth + "px";
				}
		</script>
	</HEAD>
	<body onload="resizePanel('Panel1',true);" onResize="resizePanel('Panel1',true);">
		<form id="Form1" method="post" runat="server">
			<cc1:UmbracoPanel id="Panel1" runat="server" Width="612px" Height="375px" hasMenu="false">
			<div class="guiDialogNormal" style="margin: 10px">
				<b>Cache Alias:</b>
				<asp:Label id="LabelCacheAlias" runat="server">Label</asp:Label>
				<br />
				<br />
				<b>Cache Value:</b>
				<asp:Label id="LabelCacheValue" runat="server">Label</asp:Label>
				</div>
			</cc1:UmbracoPanel>
		</form>
	</body>
</HTML>

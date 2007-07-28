<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Page validateRequest="false" language="c#" Codebehind="editNodeType.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.settings.editNodeType" trace="false" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editXslt</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script>
		function resizeTabView(TabPageArr, TabViewName) {
		
		var clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
		var clientWidth = document.body.clientWidth;

		var leftWidth = parseInt(clientWidth*0.28);
		var rightWidth = clientWidth; // leftWidth - 40;
			
		newWidth = rightWidth; //-20;
		newHeight = clientHeight-37;
			
		document.getElementById(TabViewName + "_container").style.width = newWidth + "px";
		document.getElementById(TabViewName + "_container").style.height = newHeight + "px";
		//document.getElementById(TabViewName + "_container").style.border = "1px solid";
			for (i=0;i<TabPageArr.length;i++) {
				scrollwidth = newWidth - 32;
				document.getElementById(TabPageArr[i] +"layer").style.height = (newHeight-100) + "px";
				document.getElementById(TabPageArr[i] +"layer_menu").style.width = scrollwidth + "px";
				document.getElementById(TabPageArr[i] +"layer_menu_slh").style.width = scrollwidth + "px";
			}
		}
		</script>
	</HEAD>
	<body onresize="resizeTabView(TabView1_tabs, 'TabView1')" bgColor="#f2f2e9" onload="resizeTabView(TabView1_tabs, 'TabView1')">
		<form id="contentForm" runat="server">
			<asp:PlaceHolder Runat="server" ID="plc"></asp:PlaceHolder>
		</form>
	</body>
</HTML>

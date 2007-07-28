<%@ Page language="c#" Codebehind="TreeInit.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.TreeInit" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>TreeInit</title>
		<script language="javascript">
var menuType = 'treeEdit';

		<asp:PlaceHolder id="PlaceHolderJavascript" runat="server"></asp:PlaceHolder>
		</script>
		<script type="text/javascript" src="js/umbContext.js"></script>
		<script type="text/javascript" src="js/contextmenu.aspx?test=23"></script>
		<script type="text/javascript" src="js/xtree.js?test=23"></script>
		<script type="text/javascript" src="js/xmlextras.js"></script>
		<script type="text/javascript" src="js/xloadtree.js"></script>
		<script type="text/javascript" src="js/treeGui.js"></script>
		<!-- effect library -->
		<script src="js/prototype.js" type="text/javascript"></script>
		<script src="js/effects.js" type="text/javascript"></script>
		<script src="js/dragdrop.js" type="text/javascript"></script>
		<!-- styles -->
		<link type="text/css" rel="stylesheet" href="css/xtree.css">
			<link type="text/css" rel="stylesheet" href="css/umbContext.css">
				<link type="text/css" rel="stylesheet" href="css/umbracoGui.css">
					<style>
			BODY { BACKGROUND-COLOR: white }
			</style>
	</HEAD>
	<body bgcolor="white" leftmargin="0" marginheight="0" marginwidth="0" rightmargin="0"
		topmargin="0">
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 5px; PADDING-LEFT: 2px; PADDING-BOTTOM: 0px; PADDING-TOP: 1px">
				<asp:PlaceHolder id="PlaceHolderTree" runat="server"></asp:PlaceHolder>
			</div>
		</form>
	</body>
</HTML>

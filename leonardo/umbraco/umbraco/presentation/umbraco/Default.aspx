<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="True" Inherits="umbraco._Default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Default</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript">
    function startUmbraco() {
		
		window.open('umbraco.aspx','u<%=Request.ServerVariables["SERVER_NAME"].Replace(".","").Replace("-","")%>','height=600,width:850,scrollbars=yes,resizable=yes,top=0,left=0,status=yes');
    }
		</script>
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style> body { text-align:center; height:100%; padding: 40px; background-color: white}
	#container { height:200px; border:1px solid #ccc; width:400px; }
		</style>
	</HEAD>
	<body onload="startUmbraco();">
		<form id="Form1" method="post" runat="server">
				<img src="images/umbraco20splash.gif" alt="umbraco" style="width:354px;height:61px;"/><br/><br/>
				<h3>umbraco <%=umbraco.ui.Text("dashboard", "openinnew")%></h3>
				<span class="guiDialogNormal">
				<br/>
				<br/>
				<a href="#" onclick="startUmbraco();"><img src="images/forward.png" align="absmiddle" alt="<%=umbraco.ui.Text("dashboard", "restart")%> umbraco" style="width:16px;height:16px;"/> <%=umbraco.ui.Text("dashboard", "restart")%> umbraco</a> &nbsp; <a href="../"><img src="images/forward.png" align="absmiddle" alt="<%=umbraco.ui.Text("dashboard", "browse")%>" style="width:16px;height:16px;"/> <%=umbraco.ui.Text("dashboard", "browser")%></a></span>
				<br/><br/>
				<span class="guiDialogTiny">(<%=umbraco.ui.Text("dashboard", "nothinghappens")%>)</span>
<br/><br/>
				<span class="guiDialogTiny"><a href="http://umbraco.org"><%=umbraco.ui.Text("dashboard", "visit")%> umbraco.org</a></span>
				
		</form>
	</body>
</HTML>

<%@ Page language="c#" Codebehind="republish.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.republish" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - <%=umbraco.ui.Text("siterepublished")%></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" style="padding: 10px;">
		<form id="Form1" method="post" runat="server">
		<h3><img src="images/publish.gif" alt="Republish" align="absmiddle" style="float:left; margin-top: 3px; margin-right: 5px"/> <%=umbraco.ui.Text("siterepublished")%></h3>
		<br/>
		<a href="#" onClick="javascript:window.close();" style="margin-left: 30px" class="guiDialogNormal"><%=umbraco.ui.Text("closewindow")%></a>
		</form>
	</body>
</HTML>

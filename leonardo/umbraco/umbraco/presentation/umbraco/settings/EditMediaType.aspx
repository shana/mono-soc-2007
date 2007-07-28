<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Page language="c#" Codebehind="EditMediaType.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.settings.EditMediaType" %>
<%@ Register TagPrefix="uc1" TagName="ContentTypeControlNew" Src="../controls/ContentTypeControlNew.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editXslt</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		
	</HEAD>
	<body onresize="resizeTabView(ContentTypeControlNew1_TabView1_tabs, 'ContentTypeControlNew1_TabView1')" bgColor="#f2f2e9" onload="resizeTabView(ContentTypeControlNew1_TabView1_tabs, 'ContentTypeControlNew1_TabView1')">
		<form id="contentForm" runat="server">
			<uc1:ContentTypeControlNew id="ContentTypeControlNew1" runat="server"></uc1:ContentTypeControlNew>
		</form>
	</body>
</HTML>

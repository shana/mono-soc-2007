<%@ Page language="c#" Codebehind="EditNodeTypeNew.aspx.cs" AutoEventWireup="True" trace="false" Inherits="umbraco.settings.EditContentTypeNew" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
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
	<body onresize="resizeTabView(ContentTypeControlNew1_TabView1_tabs, 'ContentTypeControlNew1_TabView1')"
		bgColor="#f2f2e9" onload="resizeTabView(ContentTypeControlNew1_TabView1_tabs, 'ContentTypeControlNew1_TabView1')">
		<form id="contentForm" runat="server">
			<uc1:contenttypecontrolnew id="ContentTypeControlNew1" runat="server"></uc1:contenttypecontrolnew>
			<cc1:Pane id="tmpPane" runat="server">
				<asp:Panel id="pnlTemplate" Runat="server">
					<TABLE>
						<TR>
							<TH vAlign="top">
								Allowed templates</TH>
							<TD>
								<DIV style="BORDER-RIGHT: #ccc 1px solid; BORDER-TOP: #ccc 1px solid; BACKGROUND: #fff; OVERFLOW: auto; BORDER-LEFT: #ccc 1px solid; WIDTH: 300px; BORDER-BOTTOM: #ccc 1px solid; HEIGHT: 150px; scroll: auto">
									<asp:CheckBoxList id="templateList" Runat="server"></asp:CheckBoxList></DIV>
							</TD>
							<DIV></DIV>
						</TR>
						<TR>
							<TH>
								Default template</TH>
							<TD>
								<asp:DropDownList id="ddlTemplates" Runat="server"></asp:DropDownList></TD>
						</TR>
					</TABLE>
				</asp:Panel>
			</cc1:Pane>
		</form>
	</body>
</HTML>

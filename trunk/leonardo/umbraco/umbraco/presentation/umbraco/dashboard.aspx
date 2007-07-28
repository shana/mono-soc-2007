<%@ Page language="c#" Codebehind="dashboard.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.dashboard" trace="false" validateRequest="false"%>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>dashboard</title>
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
			
	</HEAD>
	<body marginheight="0" marginwidth="0" topmargin="0" leftmargin="0" <asp:Literal ID="bodyAttributes" runat="server"></asp:Literal>>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
			<cc1:UmbracoPanel id="Panel2" runat="server" Height="224px" Width="412px" hasMenu="false">
				<div style="padding: 2px 15px 0px 15px">
				<asp:PlaceHolder id="dashBoardContent" Runat="server"></asp:PlaceHolder>
				</div>
			</cc1:UmbracoPanel>
			<cc1:TabView
                id="dashboardTabs" Width="400px" Visible="false" runat="server" />
		</form>
	</body>
</HTML>

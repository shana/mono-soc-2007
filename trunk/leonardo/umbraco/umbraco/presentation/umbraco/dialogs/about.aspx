<%@ Register Namespace="umbraco" TagPrefix="umb" Assembly="umbraco" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Page language="c#" Codebehind="about.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.about" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<HTML style="WIDTH: 100%; HEIGHT: 100%">
	<HEAD>
		<title>umbraco -
			<%=umbraco.ui.Text("about")%>
		</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>
		P { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px }
		</style>
	</HEAD>
	<body bgColor="#f2f2e9" style="FONT-SIZE:11px;MARGIN:0px;WIDTH:100%;PADDING-TOP:20px;FONT-FAMILY:Trebuchet MS, verdana, arial, Lucida Grande;TEXT-ALIGN:center">
		<form id="Form1" method="post" runat="server">
			<cc1:UmbracoPanel style="BACKGROUND:url(../images/loginBackground.gif); TEXT-ALIGN:left" id="Panel1"
				runat="server" Height="347px" Width="351px" Text="">
				<DIV style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 0px; PADDING-TOP: 63px; HEIGHT: 235px">
					<P style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 0px; MARGIN: 0px; COLOR: #999; PADDING-TOP: 5px">umbraco 
						v
						<asp:Literal id="version" Runat="server"></asp:Literal><br />
						<br />
						Copyright © 2001 -
						<asp:Literal id="thisYear" Runat="server"></asp:Literal> umbraco / Niels Hartvig<br />
						Developed by: <A href="http://umbraco.org/redir/niels-hartvig" target="_blank">Niels 
							Hartvig</A> and the <a href="http://umbraco.org/redir/core-team" target="_blank">core team</a><br />
						<br />
						The umbraco framework is licensed under <a href="http://umbraco.org/redir/license" target="_blank">the open source license MIT</a>, the umbraco UI is licensed under <a href="http://umbraco.org/redir/license" target="_blank">the "umbraco license"</a><br />
						<br />
						Visit <a href="http://umbraco.org/redir/from-about" target="_blank">umbraco.org</a> for more information.<br /><br />
						Dedicated to Gry, August and Villum!<br /><br />
						<A href="javascript:window.close()">Close window</A>
					</P>
				</DIV>
			</cc1:UmbracoPanel>
		</form>
	</body>
</HTML>

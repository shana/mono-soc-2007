<%@ Page language="c#" Codebehind="login.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.login" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register Namespace="umbraco" TagPrefix="umb" Assembly="umbraco" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<HTML style="WIDTH: 100%; HEIGHT: 100%">
	<HEAD>
		<title>login</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body bgColor="#f2f2e9" style="FONT-SIZE:11px;WIDTH:100%;FONT-FAMILY:Trebuchet MS, verdana, arial, Lucida Grande;TEXT-ALIGN:center;padding-top: 50px;margin:0px;">
		<form id="Form1" method="post" runat="server">
			<cc1:UmbracoPanel style="TEXT-ALIGN:left; background: url(images/loginBackground.gif) no-repeat" id="Panel1" runat="server" Height="347px" Width="351px"
				Text="Umbraco 2.0.351 RC  login">
				<div style="padding: 63px 5px 0px 5px;height: 210px; _height: 275px">
				<p style="margin: 0px; padding: 5px 5px 0px 0px; color: #999">
				<asp:Literal id="TopText" Runat="server"></asp:Literal></p>
				</p>
				<TABLE cellSpacing="0" cellPadding="0" border="0">
					<TR>
						<TD align="right">
							<b><asp:Literal id="username" Runat="server"></asp:Literal></b>: &nbsp;</TD>
						<TD>
							<asp:TextBox id="lname" style="padding-left: 3px; background: url(images/gradientBackground.png); _background: none; BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid; width: 180px;"
								Runat="server"></asp:TextBox></TD>
					</TR>
					<tr><td colspan="2" style="height: 8px;"></td></tr>
					<TR>
						<TD align="right">
							<b><asp:Literal id="password" Runat="server"></b></asp:Literal>: &nbsp;</TD>
						<TD>
							<asp:TextBox id="passw" style="padding-left: 3px; background: url(images/gradientBackground.png); _background: none; BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid; width: 180px;"
								Runat="server" TextMode="Password"></asp:TextBox></TD>
					</TR>
					<tr><td colspan="2" style="height: 10px;"></td></tr>
					<TR>
						<TD align="right" colSpan="2">
							<asp:Button id="Button1" style="width: 60px; font-weight: bold" Text="" Runat="server" onclick="Button1_Click"></asp:Button></TD>
					</TR>
				</TABLE>
				
				<p style="margin: 0px; padding: 10px 5px 0px 0px; color: #999">
				<asp:Literal id="BottomText" Runat="server"></asp:Literal>
				</p>
				</div>
			</cc1:UmbracoPanel>
		</form>
		<script>
			document.getElementById("lname").focus();
		</script>
	</body>
</HTML>

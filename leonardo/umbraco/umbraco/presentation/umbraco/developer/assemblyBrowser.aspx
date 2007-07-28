<%@ Page language="c#" Codebehind="assemblyBrowser.aspx.cs" AutoEventWireup="True" Inherits="umbraco.developer.assemblyBrowser" %>
<%@ Register TagPrefix="wc1" Namespace="umbraco.controls" Assembly="umbraco" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>assemblyBrowser</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<FORM id="Form1" method="post" runat="server">
			<cc1:UmbracoPanel id="Panel2" runat="server" Height="400px" Width="472px" hasMenu="false">
				<H3 style="MARGIN-LEFT: -2px">
					<asp:Label id="AssemblyName" runat="server"></asp:Label></H3>
				<asp:Panel id="ChooseProperties" runat="server">
					<P class="guiDialogTiny">The following list shows the Public Properties from the 
						Control. By checking the Properties and click the "Save Properties" button at 
						the bottom, umbraco will create the corresponding Macro Elements.</P>
					<asp:CheckBoxList id="MacroProperties" runat="server"></asp:CheckBoxList>
					<P>
						<asp:Button id="Button1" runat="server" Text="Save Properties" onclick="Button1_Click"></asp:Button></P>
				</asp:Panel>
				<asp:Panel id="ConfigProperties" runat="server" Visible="False">
					<SCRIPT type="text/javascript">
			parent.opener.document.location.href = parent.opener.document.location.href;
//			window.close();
					</SCRIPT>
<!--					<wc1:windowCloser id="Windowcloser1" runat="server" seconds="10" secondText=" seconds" text="Window closing in"></wc1:windowCloser>-->
				</asp:Panel>
			</cc1:UmbracoPanel></FORM>
	</body>
</HTML>

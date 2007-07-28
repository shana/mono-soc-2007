<%@ Page language="c#" Codebehind="editLanguage.aspx.cs" AutoEventWireup="True" Inherits="umbraco.settings.editLanguage" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - edit language</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body onLoad="resizePanel('Panel1',true);" onResize="resizePanel('Panel1',true);" bgColor="#f2f2e9">
		<form id="Form2" method="post" runat="server">
			<cc1:UmbracoPanel id="Panel1" runat="server" Width="608px" Height="336px" hasMenu="true" style="TEXT-ALIGN:center">
				<CC2:Pane id="Pane7" style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px; TEXT-ALIGN: left"
					runat="server" Height="44px" Width="528px">
					<TABLE id="Table1" width="100%">
						<TR>
							<TH width="145">
								<%=umbraco.ui.Text("language", "displayName", base.getUser())%>
							</TH>
							<TD>
								<ASP:DROPDOWNLIST id="Cultures" Runat="server"></ASP:DROPDOWNLIST></TD>
						</TR>
					</TABLE>
				</CC2:PANE>
			</cc1:UmbracoPanel>
		</form>
	</body>
</HTML>

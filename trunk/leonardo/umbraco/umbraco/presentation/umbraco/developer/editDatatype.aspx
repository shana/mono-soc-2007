<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Page language="c#" Codebehind="editDatatype.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.developer.editDatatype" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editDatatype</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script>
		function resizePanel(PanelName, hasMenu) {
				var clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
				var clientWidth = document.body.clientWidth;

				panelWidth = clientWidth;
			
				contentHeight = clientHeight-68;
				if (hasMenu) contentHeight = contentHeight - 32;
				
				document.getElementById(PanelName).style.width = panelWidth + "px";
				document.getElementById(PanelName+'_content').style.height = contentHeight + "px";
				document.getElementById(PanelName+'_content').style.width = panelWidth + "px";
				
				document.getElementById(PanelName+'_menu').style.width = (panelWidth - 7)+"px"
				scrollwidth = panelWidth - 35;
				document.getElementById(PanelName +"_menu").style.width = scrollwidth + "px";
				document.getElementById(PanelName +"_menu_slh").style.width = scrollwidth + "px";
				}
		</script>
	</HEAD>
	<body onload="resizePanel('Panel1',true);" onResize="resizePanel('Panel1',true);">
		<form id="Form1" method="post" runat="server">
			<cc1:UmbracoPanel id="Panel1" runat="server" Width="496px" Height="584px">
				<cc1:Pane id="Pane2" runat="server">
					<TABLE>
						<TR>
							<TH>
								<%=umbraco.ui.Text("name") %></TH>
							<TD>
								<asp:TextBox id="txtName" Runat="server"></asp:TextBox></TD>
						</TR>
						<TR>
							<TH>
								Rendercontrol</TH>
							<TD>
								<asp:DropDownList id="ddlRenderControl" Runat="server"></asp:DropDownList></TD>
						</TR>
					</TABLE>
				</cc1:Pane>
				<cc1:Pane id="Pane1" runat="server">
					<H3>Controlpanel:</H3>
					<asp:PlaceHolder id="plcEditorPrevalueControl" runat="server"></asp:PlaceHolder>
				</cc1:Pane>
			</cc1:UmbracoPanel>
		</form>
	</body>
</HTML>

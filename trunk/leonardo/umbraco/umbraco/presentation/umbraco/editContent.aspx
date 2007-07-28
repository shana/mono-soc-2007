<%@ Page language="c#" Codebehind="editContent.aspx.cs" validateRequest="false" AutoEventWireup="True" Inherits="umbraco.cms.presentation.editContent" trace="false" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<HTML>
	<HEAD>
		<script>
		// Save handlers for IDataFields		
		var saveHandlers = new Array()
		
		// A hack to make sure that any javascript can access page id and version
		<asp:Literal id="jsIds" runat="server"></asp:Literal>
		
		// For short-cut keys
		var isDialog = true;
		var functionsFrame = this;
		var disableEnterSubmit = true;
	
		
		function addSaveHandler(handler) {
			saveHandlers[saveHandlers.length] = handler;
		}		
		
		function invokeSaveHandlers() {
			for (var i=0;i<saveHandlers.length;i++) {
				eval(saveHandlers[i]);
			}
		}
		
		function doSubmit() {
			invokeSaveHandlers();
			document.getElementById("TabView1_tab01layer_save").click();
		}
		
		
		</script>
		<script language="javascript" src="js/richtextfunctions.js"></script>
		<!-- effect library -->
		<script src="js/prototype.js" type="text/javascript"></script>
		<script src="js/effects.js" type="text/javascript"></script>
		<script src="js/dragdrop.js" type="text/javascript"></script>
		<script src="js/umbracoCheckKeys.js" type="text/javascript"></script>
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body onresize="resizeTabView(TabView1_tabs, 'TabView1')" onload="resizeTabView(TabView1_tabs, 'TabView1')"
		link="#666" alink="#666" vlink="#999">
		<form id="contentForm" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
			<TABLE height="38" cellSpacing="0" cellPadding="0" width="371" border="0" ms_2d_layout="TRUE">
				<TR vAlign="top">
					<TD height="20"></TD>
					<TD><asp:placeholder id="plc" Runat="server"></asp:placeholder></TD>
				</TR>
			</TABLE>
			<INPUT id="doSave" type="hidden" name="doSave" runat="server"> <INPUT id="doPublish" type="hidden" name="doPublish" runat="server">
			<asp:Panel ID="syncScript" Runat="server">
				<SCRIPT>
			parent.top.syncTree('<asp:Literal id="SyncPath" runat="server"></asp:Literal>');
				</SCRIPT>
			</asp:Panel>
		</form>
	</body>
</HTML>

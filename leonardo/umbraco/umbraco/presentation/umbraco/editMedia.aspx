<%@ Page language="c#" Codebehind="editMedia.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.editMedia" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<HTML>
	<HEAD>
		<script>
		// Save handlers for IDataFields		
		var saveHandlers = new Array()
		
		function addSaveHandler(handler) {
			saveHandlers[saveHandlers.length] = handler;
		}		
		
		function invokeSaveHandlers() {
			for (var i=0;i<saveHandlers.length;i++) {
				eval(saveHandlers[i]);
			}
		}

		</script>
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body onload="resizeTabView(TabView1_tabs, 'TabView1')" onresize="resizeTabView(TabView1_tabs, 'TabView1')"
		bgColor="#f2f2e9" style="PADDING-RIGHT:0px;PADDING-LEFT:0px;PADDING-BOTTOM:0px;MARGIN:0px;PADDING-TOP:0px">
		<form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
			<asp:PlaceHolder id="plc" Runat="server"></asp:PlaceHolder>
			<INPUT id="doSave" type="hidden" name="doSave" runat="server"> <INPUT id="doPublish" type="hidden" name="doPublish" runat="server">
			<asp:Panel ID="syncScript" Runat="server">
				<SCRIPT>
			parent.top.syncTree('<asp:Literal id="SyncPath" runat="server"></asp:Literal>');
				</SCRIPT>
			</asp:Panel>
		</form>
	</body>
</HTML>

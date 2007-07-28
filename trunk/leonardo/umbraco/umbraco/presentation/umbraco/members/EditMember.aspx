<%@ Page language="c#" Codebehind="EditMember.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.members.EditMember" %>
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
	<body ms_positioning="GridLayout" onload="resizeTabView(TabView1_tabs, 'TabView1')" onresize="resizeTabView(TabView1_tabs, 'TabView1')"
		bgColor="#f2f2e9">
		<form runat="server" id="contentForm">
			<INPUT id="doSave" type="hidden" name="doSave" runat="server"> <INPUT id="doPublish" type="hidden" name="doPublish" runat="server">
			<asp:PlaceHolder id="plc" Runat="server"></asp:PlaceHolder>
		</form>
	</body>
</HTML>

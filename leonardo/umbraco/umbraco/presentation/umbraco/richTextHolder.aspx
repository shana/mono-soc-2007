<%@ Page language="c#" Codebehind="richTextHolder.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.richTextHolder" %>
<HTML>
	<HEAD>
		<title>richTextHolder</title>
		<asp:Literal id="LabelDoctype" runat="server">Label</asp:Literal>
		<link type="text/css" rel="stylesheet" href="css/umbracoGui.css">
			<style>
			UNKNOWN { BORDER-RIGHT: #f8f3ec 1px solid; BORDER-TOP: #f8f3ec 1px solid; BORDER-LEFT: #f8f3ec 1px solid; BORDER-BOTTOM: #f8f3ec 1px solid; HEIGHT: 100%; TEXT-ALIGN: left }
			#holder TD { BORDER-RIGHT: #cccccc 1px dotted; BORDER-TOP: #cccccc 1px dotted; BORDER-LEFT: #cccccc 1px dotted; BORDER-BOTTOM: #cccccc 1px dotted }
			BODY { BACKGROUND-IMAGE: url(images/nada.gif); BACKGROUND-COLOR: white }
			</style>
			<script language="javascript">
			var menuType = 'content';
			var functionsFrame = parent.parent.right;
			var tabFrame = functionsFrame;
			var isDialog = false;
			var currentRichTextObject = null;
			var myAlias = '<asp:Literal id="myAlias" runat="server"></asp:Literal>';
			var umbracoPath = '<%=umbraco.GlobalSettings.Path%>';
			
			function doSubmit() {
				functionsFrame.invokeSaveHandlers();
				functionsFrame.document.getElementById("TabView1_tab01layer_save").click();
			}
			
			// Temp
			function updateToolbarStatus() {

				if (document.selection.type == "Text" || document.selection.type == "None") {				
				var oSel = document.selection.createRange();
				if (el != null && el.tagName != 'DIV')
					var el = event.srcElement;
				else
					var el = oSel.parentElement();
					

				var currentTag = el;
				var tagList = "";
				while (currentTag.tagName != 'BODY' && currentTag.tagName != 'DIV' && currentTag.tagName != 'P') {
					tagList = tagList + currentTag.tagName +',';
					currentTag = currentTag.parentElement;
				}
				tagList = ',' + tagList + currentTag.tagName +',';
				tagList = tagList.toLowerCase();
					
				var elementAttributeList = '';
				for (var temp=0; temp<el.attributes.length; temp++) {
					if (el.attributes[temp].value != '' && el.attributes[temp].value != 'null')
						elementAttributeList += el.attributes[temp].name + ': \'' + el.attributes[temp].value + '\'\n'
				}
				
				// reset buttons
				parent.resetIconState(activeTab() + "layer_menu");
				
				// check for bold
				markIcon(tagList, 'b', 'bold');
				markIcon(tagList, 'strong', 'bold');

				// check italic
				markIcon(tagList, 'i', 'italic');
				markIcon(tagList, 'em', 'italic');
				
				// Check list
				markIcon(tagList, 'ul', 'listBullet');
				markIcon(tagList, 'ol', 'listNumeric');
				
				// check link
				markIcon(tagList, 'a', 'linkInsert');
				
				// styles
				updateStyles(tagList);
				
//				tempStatus.innerText = tagList;
				}
				
			}
			
			function updateStyles(tagList) {
				var tagListSplit = tagList.split(",");
				var styleCombo = parent.document.getElementById(activeTab() + "layer_editorStyle");
				styleCombo.selectedIndex = 0;
				for(var x=0;x<tagListSplit.length;x++) {
					for(var i=0;i<styleCombo.options.length;i++) {
						if (styleCombo.options[i].value == tagListSplit[x]) {
							styleCombo.selectedIndex = i;
							break;
						}
					}		
					if (styleCombo.selectedIndex > 0)
						break;
				}
			}

			function markIcon(tagList, tagName, iconId) {
				var activeTabName = activeTab();
				if (tagList.indexOf(','+tagName+',') > -1)
					parent.markIcon(activeTabName +'layer_menu',activeTab() +'layer_'+iconId);
			}
			
			
			function activeTab() {
				tabs = parent.TabView1_tabs;
				var activeTab;
				for (var i=0; i<tabs.length; i++) {	
					if (parent.document.getElementById(tabs[i]).className == "tabOn") {
						return tabs[i];
					}
				}
			}
			
			function ShortCutKeysDown() {
				if (!parent.umbracoActivateKeys(event.ctrlKey, event.shiftKey, event.keyCode))
					return false;
			}
			
			function ShortCutKeysUp() {
				parent.umbracoActivateKeysUp(event.ctrlKey, event.shiftKey, event.keyCode);
			}

			document.attachEvent( "onkeyup", updateToolbarStatus);			
			document.attachEvent( "onmouseup", updateToolbarStatus);	
//			document.attachEvent("onkeydown", ShortCutKeysDown);
//			document.attachEvent( "onkeyup", ShortCutKeysUp);

			
			</script>
			<script type="text/javascript" src="js/poslib.js"></script>
			<script type="text/javascript" src="js/scrollbutton.js"></script>
			<script type="text/javascript" src="js/menu4.js"></script>
			<script type="text/javascript" src="js/umbracoCheckKeys.js"></script>
			<script type="text/javascript" src="js/contentContextMenu.js"></script>
			<script type="text/javascript" src="js/richTextFunctions.js"></script>
			<style><asp:Literal id="customStyles" runAt="server" /></style>
	</HEAD>
	<body id="holderBody" onload="currentRichTextObject = document.getElementById('holder');">
		<div id="tempStatus" style="DISPLAY: none"></div>
		<div contenteditable="true" id="holder" onFocus="parent.setRichTextObject(myAlias)"
			onPaste="handlePaste();" onBeforePaste="handleBeforePaste()">
			<asp:PlaceHolder id="contentHolder" runat="server"></asp:PlaceHolder>
		</div>
		<div id="pasteHolder" style="VISIBILITY: hidden; WIDTH: 1px; HEIGHT: 1px">Her kan 
			der pastes...</div>
	</body>
</HTML>

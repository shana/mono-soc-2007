<%@ Import namespace="umbraco"%>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="contentButtons.ascx.cs" Inherits="umbraco.buttonHolders.contentButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

			<table border=0 cellpadding=0 cellspacing=0>
				<tr>
					<td valign=top>
					<nobr>
<img src="images/editor/Save.GIF" width="22" height="23" alt="Gem (CTRL+S)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'" onClick="doSubmit();">
<img src="images/editor/SaveAndPublish.gif" width="22" height="23" alt="Udgiv" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'" onClick="doSubmitAndPublish();">
<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">
<img src="images/editor/html.GIF" width="22" height="23" alt="Rediger HTML" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'" onClick="viewHTML();">
<img id="buttonRel" src="images/editor/rel.GIF" width="22" height="23" alt="Tilføj relation" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown';" onClick="doRelation();">
<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">
<a href="/<%=Request["nodeID"]%>?umbracoEditor=on&umbracoVersion=" target="_blank"><img src="images/editor/vis.GIF" width="22" height="23" alt="Se siden" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'"></a>
<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">
</nobr>
					</td>
					<td valign=top><img src="images/nada.gif" height="3"><br />
					<select size="1" name="editorStyle" class="editorDropDown" onChange="addStyle();">
						<option value="">Vælg style
						<option value="miniRed">Test</option>
						
			
					</select><br /></td>
					<td>&nbsp;</td>
					<td valign=top>
<nobr>
<img id="showStyles" src="images/editor/showStyles.GIF" width="22" height="23" alt="Vis koder (CTRL+SHIFT+V)" border="0" onMouseover="if (this.className != 'editorIconOn')this.className='editorIconOver'" onMouseout="if (this.className != 'editorIconOn')this.className='editorIcon'" onMouseup="if (!this.disabled && this.className != 'editorIconOn') this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoShowStyles();">
<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">
<img id="buttonBold" src="images/editor/bold.GIF" width="22" height="23" alt="Fed (CTRL+B)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('bold', '')">
<img id="buttonItalic" src="images/editor/italic.GIF" width="22" height="23" alt="Kursiv (CTRL+I)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('italic', '')">
<img id="buttonTextGen" src="images/editor/umbracoTextGen.GIF" width="22" height="23" alt="Indsæt grafisk overskrift" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver';" onMouseDown="this.className='editorIconDown';" onClick="umbracoTextGen();" class="editorIconDisabled" disabled>

<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">

<img id="buttonLeft" src="images/editor/left.GIF" width="22" height="23" alt="Venstre stil afsnit" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('justifyleft', '')">
<img id="buttonCenter" src="images/editor/center.GIF" width="22" height="23" alt="Centrér afsnit" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('justifycenter', '')">
<img id="buttonRight" src="images/editor/right.GIF" width="22" height="23" alt="Højrestil afsnit" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('justifyright', '')">

<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">

<img id="buttonBullet" src="images/editor/bullist.GIF" width="22" height="23" alt="Punktopstilling" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('insertUnorderedList', '')">
<img id="buttonList" src="images/editor/numlist.GIF" width="22" height="23" alt="Nummerorden" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('insertOrderedList', '')">
<img id="buttonDeIndent" src="images/editor/deindent.GIF" width="22" height="23" alt="Indryk afsnit" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('Outdent', '')">
<img id="buttonIndent" src="images/editor/inindent.GIF" width="22" height="23" alt="Fortryd indryk afsnit" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoEditorCommand('Indent', '')">

<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">

<img id="buttonLink" src="images/editor/link.GIF" width="22" height="23" alt="Indsæt link (CTRL+SHIFT+L)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoLink()">
<img id="buttonAnchor" src="images/editor/anchor.gif" width="22" height="23" alt="Indsæt lokalt link" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoAnchor()">
<img src="images/editor/split.gif" width="8" height="21" alt="" border="0">
<img id="buttonImage" src="images/editor/image.GIF" width="22" height="23" alt="Indsæt billede eller multimedie (CTRL+SHIFT+B)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'; umbracoImage();">
<img id="buttonMacro" src="images/editor/insMacro.GIF" width="22" height="23" alt="Indsæt makro (CTRL+SHIFT+M)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'" onClick="umbracoInsertMacro('<%=umbraco.GlobalSettings.Path%>');">
<img id="buttonTable" src="images/editor/instable.GIF" width="22" height="23" alt="Indsæt tabel (CTRL+SHIFT+T)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver';" onMouseDown="this.className='editorIconDown';" onClick="umbracoInsertTable();" class="editorIconDisabled" disabled>
<img id="buttonForm" src="images/editor/form.GIF" width="22" height="23" alt="Indsæt formular felt (CTRL+SHIFT+F)" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="this.className='editorIconOver';" onMouseDown="this.className='editorIconDown';" onClick="umbracoInsertForm();" class="editorIconDisabled" disabled>
</nobr>
</td></tr></table>
<script language=javascript>

	// <buttons>
	var buttonArray = ["showStyles", "buttonBold", "buttonItalic", "buttonLeft", "buttonCenter", "buttonRight",
						"buttonBullet", "buttonList", "buttonIndent", "buttonDeIndent", "buttonLink", "buttonAnchor", 
						"buttonImage", "buttonMacro"]
	var buttonArrayNonRichText = ["buttonRel"]
	// </buttons>

	var tableSelectorOpen = 0;
	var isRichText = 0;
	var macroEditElement;

	// ---------------------------------------------
	// editorKnapper
	// ---------------------------------------------
	var doScroll = 0;
	
	var editorIconsTotalWidth = -680;
	
	function editorBarScroll(direction) {
		doScroll = 1;
		editorBarScrollDo(direction);
	}
	
	function editorBarScrollStop() {	
		doScroll = 0;
	}
	
	function editorBarScrollDo(direction) {
		if (doScroll && parseInt(document.all["buttons"].style.left)+direction < 3 && parseInt(document.all["buttons"].style.left)+direction > editorIconsTotalWidth+parseInt(document.all["buttonHolder"].style.width)-80) {
			document.all["buttons"].style.left = parseInt(document.all["buttons"].style.left) + direction;
			setTimeout("editorBarScrollDo(" + direction + ");", 4);
		}
	}

	function adjustEditorBar(documentName) {
		// juster knapper?
		var clientWidth = documentName.body.clientWidth;
		var rightWidth = clientWidth - 55;

		if (documentName != '')  {
			documentName.all["buttonHolder"].style.width = rightWidth + 'px';
		} else {
			document.all["buttonHolder"].style.width = rightWidth + 'px';
		}
	}
	
</script>
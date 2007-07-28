<%@ Control Language="c#" AutoEventWireup="True" Codebehind="templateButtons.ascx.cs" Inherits="umbraco.buttonHolders.templateButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<NOBR><IMG onmouseup="this.className='editorIconOver'" onmousedown="this.className='editorIconDown'"
			onmouseover="this.className='editorIconOver'" onclick="doSubmit();" onmouseout="this.className='editorIcon'"
			height="23" alt="Gem (CTRL+S)" src="<%=umbraco.GlobalSettings.Path%>/images/editor/Save.GIF" width="22"
			border="0"></NOBR>
<img src="<%=umbraco.GlobalSettings.Path%>/images/editor/split.gif" width="8" height="21" alt="" border="0">
<img id="buttonField" src="<%=umbraco.GlobalSettings.Path%>/images/editor/insField.GIF" width="22" height="23" alt="Indsæt felt" border="0" onMouseover="this.className='editorIconOver'" onMouseout="this.className='editorIcon'" onMouseup="if (!this.disabled) this.className='editorIconOver'" onMouseDown="this.className='editorIconDown'" onClick="umbracoInsertField('/dialogs/umbracoField', '','', 640, 650,'<%=umbraco.GlobalSettings.Path%>');">
			
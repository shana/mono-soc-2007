<%@ Page language="c#" Codebehind="umbracoField.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.umbracoField" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbracoField</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
	<script>

		function doSubmit()
		{
		
			var tagString = '<?' + document.forms[0].tagName.value
			// hent feltnavne
			var field = document.forms[0].field.value;
			if (field != '')
				tagString += ' field="' + field + '"';
			var useIfEmpty = document.forms[0].useIfEmpty.value;
			if (useIfEmpty != '')
				tagString += ' useIfEmpty="' + useIfEmpty + '"';
			var alternativeText = document.forms[0].alternativeText.value;
			if (alternativeText != '')
				tagString += ' textIfEmpty="' + alternativeText + '"';
			
			var insertTextBefore = document.forms[0].insertTextBefore.value;
			if (insertTextBefore != '')
				tagString += ' insertTextBefore="' + insertTextBefore.replace(/\"/gi,"&quot;").replace(/\</gi,"&lt;").replace(/\>/gi,"&gt;") + '"';
			var insertTextAfter = document.forms[0].insertTextAfter.value;
			if (insertTextAfter != '')
				tagString += ' insertTextAfter="' + insertTextAfter.replace(/\"/gi,"&quot;").replace(/\</gi,"&lt;").replace(/\>/gi,"&gt;") + '"';
			
			if (document.forms[0].formatAsDate[1].checked)
				tagString += ' formatAsDateWithTime="true" formatAsDateWithTimeSeparator="' + document.forms[0].formatAsDateWithTimeSeparator.value + '"';
			else if(document.forms[0].formatAsDate[0].checked)
				tagString += ' formatAsDate="true"';

			if (document.forms[0].toCase[1].checked)
				tagString += ' case="' + document.forms[0].toCase[1].value + '"';
			else if (document.forms[0].toCase[2].checked)
				tagString += ' case="' + document.forms[0].toCase[2].value + '"';

			if (document.forms[0].recursive.checked)
				tagString += ' recursive="true"';

				
			if (document.forms[0].urlEncode.checked)
				tagString += ' urlEncode="true"';
			if (document.forms[0].stripParagraph.checked)
				tagString += ' stripParagraph="true"';
			if (document.forms[0].convertLineBreaks.checked)
				tagString += ' convertLineBreaks="true"';
			
		  	window.opener.right.umbracoInsertFieldDo('<%=umbraco.helper.Request("objectId")%>', tagString + '/>', '<%=umbraco.helper.Request("move")%>');
		  	
		  window.close()
		}
		
	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	</script>

	<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 22px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3>Insert umbraco field</h3>
				<input type="hidden" name="tagName" value="UMBRACO_GETITEM">
			<table class="propertyPane" border="0" cellspacing="0" cellpadding="4" width="100%" class="guiDialogBox">
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "chooseField")%></b></td>
					<td valign="top" class="propertyContent" width="75%">
					<asp:ListBox ID="fieldPicker" Rows="1" Runat="server"></asp:ListBox>
					<input type="text" size="25" name="field" class="guiInputTextTiny"></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "alternativeField")%></b></td>
					<td valign="top" class="propertyContent">
					<asp:ListBox ID="altFieldPicker" Rows="1" Runat="server"></asp:ListBox>
					<input type="text" size="25" name="useIfEmpty" class="guiInputTextTiny"><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "usedIfEmpty")%></span></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "alternativeText")%></b></td>
					<td valign="top" class="propertyContent"><textarea rows="3" cols="40" name="alternativeText" class="guiInputTextTiny"></textarea><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "usedIfAllEmpty")%></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "recursive")%></b></td>
					<td valign="top" class="propertyContent"><input type="checkbox" name="recursive" value="true">
						<%=umbraco.ui.Text("yes")%><br />
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "preContent")%></b></td>
					<td valign="top" class="propertyContent"><input type="text" size="40" name="insertTextBefore" class="guiInputTextTiny"><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "insertedBefore")%></span></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "postContent")%></b></td>
					<td valign="top" class="propertyContent"><input type="text" size="40" name="insertTextAfter" class="guiInputTextTiny"><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "insertedAfter")%></span></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "formatAsDate")%></b></td>
					<td valign="top" class="propertyContent"><input type="radio" name="formatAsDate" value="formatAsDate">
						<%=umbraco.ui.Text("templateEditor", "dateOnly")%><br />
						<input type="radio" name="formatAsDate" value="formatAsDateWithTime"> <%=umbraco.ui.Text("templateEditor", "withTime")%>: <input type="text" size="6" name="formatAsDateWithTimeSeparator" class="guiInputTextTiny"></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "casing")%></b></td>
					<td valign="top" class="propertyContent"><input type="radio" name="toCase" value=""> <%=umbraco.ui.Text("templateEditor", "none")%>
						<input type="radio" name="toCase" value="lower"> <%=umbraco.ui.Text("templateEditor", "lowercase")%> <input type="radio" name="toCase" value="upper">
						<%=umbraco.ui.Text("templateEditor", "uppercase")%>
					</td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "urlEncode")%></b></td>
					<td valign="top" class="propertyContent"><input type="checkbox" name="urlEncode" value="true">
						<%=umbraco.ui.Text("yes")%><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "urlEncodeHelp")%></span></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "convertLineBreaks")%></b></td>
					<td valign="top" class="propertyContent"><input type="checkbox" name="convertLineBreaks" value="true">
						<%=umbraco.ui.Text("yes")%><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "convertLineBreaksHelp")%></span></td>
				</tr>
				<tr>
					<td valign="top" class="propertyHeader"><b><%=umbraco.ui.Text("templateEditor", "removeParagraph")%></b></td>
					<td valign="top" class="propertyContent"><input type="checkbox" name="stripParagraph" value="true">
						<%=umbraco.ui.Text("yes")%><br />
						<span class="guiDialogTiny"><%=umbraco.ui.Text("templateEditor", "removeParagraphHelp")%></span></td>
				</tr>
				<tr>
					<td>
						<input type="button" name="gem" value=" <%=umbraco.ui.Text("insert")%> " class="guiInputButton" onClick="doSubmit()">
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>

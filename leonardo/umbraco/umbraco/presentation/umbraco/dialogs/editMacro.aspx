<%@ Page language="c#" validateRequest="false" Codebehind="editMacro.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.editMacro" trace="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco :: <%=umbraco.ui.Text("general", "insert",this.getUser())%> <%=umbraco.ui.Text("general", "macro",this.getUser())%> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link type="text/css" rel="stylesheet" href="../css/umbracoGui.css">
		<script>
			function saveTreepickerValue(appAlias, macroAlias) {
				var treePicker = window.showModalDialog('treePicker.aspx?appAlias=' + appAlias, 'treePicker', 'dialogWidth=350px;dialogHeight=300px;scrollbars=no;center=yes;border=thin;help=no;status=no')
				document.forms[0][macroAlias].value = treePicker;
				document.getElementById("label" + macroAlias).innerHTML = "</b><i>updated with id: " + treePicker + "</i><b><br/>";
			}
			
			var macroAliases = new Array();
			
			function registerAlias(alias) {
				macroAliases[macroAliases.length] = alias;
			}

		function updateMacro() {
			var macroString = '';
		
			for (i=0; i<macroAliases.length; i++) {
				var propertyName = macroAliases[i]
					// Vi opdaterer macroStringen
					if (document.forms[0][macroAliases[i]].type == 'checkbox') {
						if (document.forms[0][macroAliases[i]].checked)
							macroString += propertyName + "=\"1\" ";
						else
							macroString += propertyName + "=\"0\" ";

					} else if (document.forms[0][macroAliases[i]].length) {
						var tempValue = '';
						for (var j=0; j<document.forms[0][macroAliases[i]].length;j++) {
							if (document.forms[0][macroAliases[i]][j].selected)
								tempValue += document.forms[0][macroAliases[i]][j].value + ', ';
						}
						if (tempValue.length > 2)
							tempValue = tempValue.substring(0, tempValue.length-2)
						macroString += propertyName + "=\"" + tempValue + "\" ";
					} else	{
						macroString += propertyName + "=\"" + pseudoHtmlEncode(document.forms[0][macroAliases[i]].value) + "\" ";
					}
			}
			
			if (macroString.length > 1)
				macroString = macroString.substr(0, macroString.length-1);
			
			if (document.forms[0].macroMode.value == 'edit') {
				var idAliasRef = "";
				if (document.forms[0]["umb_macroAlias"].value != '')
					idAliasRef = " macroAlias=\"" + document.forms[0]["umb_macroAlias"].value;
				else
					idAliasRef = " macroID=\"" + document.forms[0]["umb_macroID"].value;
					
				parent.opener.umbracoEditMacroDo("<?UMBRACO_MACRO" + idAliasRef + 
					"\" " + macroString + 
					">");
			} else {
				parent.opener.umbracoInsertMacroDo("<?UMBRACO_MACRO macroAlias=\"" + document.forms[0]["umb_macroAlias"].value +
					"\" " + macroString + 
					">");
			}
			window.close()
		}

		function pseudoHtmlEncode(text) {
			return text.replace(/\"/gi,"&amp;quot;").replace(/\</gi,"&amp;lt;").replace(/\>/gi,"&amp;gt;");
		}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" runat="server">
			<div style="PADDING-RIGHT: 17px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			
		<asp:Panel id="theForm" runat="server">
		<input type="hidden" name="macroMode" value="<%=Request["mode"]%>"/>
		<%if (Request["umb_macroID"] != null || Request["umb_macroAlias"] != null) {%>
			<h3><%=umbraco.ui.Text("general", "edit",this.getUser())%> <asp:Literal id="macroName" runat="server"></asp:Literal></h3>
			<input type="hidden" name="umb_macroID" value="<%=umbraco.helper.Request("umb_macroID")%>"/>
			<input type="hidden" name="umb_macroAlias" value="<%=umbraco.helper.Request("umb_macroAlias")%>"/>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="100%" border="0">
				<asp:PlaceHolder ID="macroProperties" Runat="server" />
			</TABLE>
			
			<%if (Request["umbPageId"] != null) {%>
			<asp:button id="renderMacro" runat="server" text="ok"></asp:button>
			<%} else {%>
			<input type="button" value="<%=umbraco.ui.Text("general", "ok", this.getUser())%>" onclick="updateMacro()"/>
			<%}%>
		<%} else {%>
			<h3><%=umbraco.ui.Text("general", "insert",this.getUser())%> <%=umbraco.ui.Text("general", "macro", this.getUser())%></h3>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="100%" border="0">
				<tr><td>
				<asp:ListBox Rows="1" ID="umb_macroAlias" Runat="server"></asp:ListBox> <input type="submit" value="<%=umbraco.ui.Text("general", "ok", this.getUser())%>"/>
				</td></tr>
			</TABLE>		
		<%}%>
		
		</asp:Panel>
		<div id="renderContent" style="display: none">
		
		<asp:PlaceHolder id="renderHolder" runat="server"></asp:PlaceHolder>
	
		</div>
		</form>
		
	</body>
</HTML>

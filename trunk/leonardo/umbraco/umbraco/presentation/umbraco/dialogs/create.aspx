<%@ Page language="c#" Codebehind="create.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.create" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - <%=umbraco.ui.Text("defaultdialogs", "create", this.getUser())%> &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp; </title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>body {margin: 2px;}</style>
		<script language="javascript">

			// Hack needed for the way the content picker iframe works
			var tempOpener = "";

			function handleOpener() {
				tempOpener = window.opener;
				window.opener = null;
			}
			
			function revertOpener() {
				window.opener = tempOpener;
			}

			function dialogHandler(id) {
				document.getElementById("nodeId").value = id;
				document.getElementById("ok").disabled = false;
				// Get node name by xmlrequest
				if (id > 0) {
				    umbraco.presentation.webservices.CMSNode.GetNodeName('<%=umbraco.BasePages.BasePage.umbracoUserContextID%>', id, updateName);
					}
				else
					document.getElementById("pageName").innerHTML = "<b><%=umbraco.ui.Text(umbraco.helper.Request("app"))%></b>";
			}
			
			function updateName(result) {
				document.getElementById("pageName").innerHTML = "<b>" + result + "</b>";
			}
			

function doSubmit() {document.Form1["ok"].click()}

		function execCreate()
		{
			var nodeType;
			var rename;
			for (var i=0;i<document.forms[0].length;i++) {
				if (document.forms[0][i].name.indexOf('nodeType') > -1)
					nodeType = document.forms[0][i];
				else if (document.forms[0][i].name.indexOf('rename') > -1)
					rename = document.forms[0][i];
			}
			
			if (rename.value != '') {
			  parent.window.returnValue = document.getElementById("path").value + "|" + document.getElementById("nodeId").value + "|" + nodeType.value + '--- ' + rename.value;
			  parent.window.close()
			} else 
				if (nodeType.value == '') 
					alert('<%=umbraco.ui.Text("errors", "missingType", this.getUser())%>');
				else
					alert('<%=umbraco.ui.Text("errors", "missingTitle", this.getUser())%>');
		}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	
	
		</script>
		<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="../webservices/CMSNode.asmx" />
            </Services>
            </asp:ScriptManager>
		<input type="hidden" id="nodeId" name="nodeId" value="<%=umbraco.helper.Request("nodeId")%>"/>
		<input type="hidden" id="path" name="path" value="" runat="server"/>
		
		<div style="float: right; padding: 3px;"><a href="javascript:window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div><h3><img src="../images/new.gif" align="absmiddle"/> <%=umbraco.ui.Text("create", this.getUser())%> <%=umbraco.ui.Text("sections", umbraco.helper.Request("app"), this.getUser())%></h3>
		<img class="gradient" style="width: 100%; height: 1px; margin-top: 7px;" src="../images/nada.gif"/><br/>
		<asp:Literal ID="FeedBackMessage" Runat="server"/>
		<br/>
		<span style="margin-left: 10px;" class="guiDialogNormal"><strong><asp:Literal ID="Header" Runat="server"></asp:Literal>:<br/></strong></span>
		<asp:Panel id="chooseNode" Runat="server">
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server" ID="Table1">
				<TBODY>
					<TR>
						<TD class="propertyContent">
							<iframe src="../TreeInit.aspx?app=<%=umbraco.helper.Request("app")%>&amp;isDialog=true&amp;dialogMode=id&amp;contextMenu=false" style="LEFT: 9px; OVERFLOW: auto; WIDTH: 290px; POSITION: relative; TOP: 0px; HEIGHT: 250px; BACKGROUND-COLOR: white"></iframe><br/>
							<span style="font-weight:lighter"><%=umbraco.ui.Text("create", "createUnder")%>: </span><asp:Label id="pageName" Runat="server"></asp:Label>
							<br/>
							<br/><input type="button" onclick="if (confirm('<%=umbraco.ui.Text("defaultdialogs", "confirmSure", this.getUser())%>')) window.close()" value="<%=umbraco.ui.Text("general", "cancel", this.getUser())%>" style="width: 60px"/> &nbsp; <input type="button" id="ok" value="<%=umbraco.ui.Text("continue")%>" onclick="revertOpener(); document.location.href = 'create.aspx?nodeType=<%=umbraco.helper.Request("nodeType")%>&app=<%=umbraco.helper.Request("app")%>&nodeId=' + document.getElementById('nodeId').value" disabled="true" style="width: 100px"/>
						</td>
					</TR>
				</tbody>
			</table>
		</asp:Panel>		
		<asp:Panel id="chooseName" Runat="server" Visible="False">
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server" ID="Table2">
				<TBODY>
					<TR>
						<TD class="propertyContent">
							<asp:PlaceHolder ID="phCreate" Runat="server"></asp:PlaceHolder>
						</td>
					</TR>
				</tbody>
			</table>
		</asp:Panel>
		<script>
		<%if (umbraco.helper.Request("nodeId") == "")
			Response.Write("handleOpener();");
		%>
		</script>
				
		</FORM>
	</body>
</HTML>

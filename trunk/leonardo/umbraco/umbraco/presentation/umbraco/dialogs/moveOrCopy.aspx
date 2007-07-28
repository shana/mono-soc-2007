<%@ Page language="c#" Codebehind="moveOrCopy.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.moveOrCopy" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - <asp:Literal id="Title" runat="Server"></asp:Literal>&nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp; </title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>body {margin: 2px;}</style>
		<script language="javascript">

			function dialogHandler(id) {
				document.forms[0].copyTo.value = id;
				document.forms[0].ok.disabled = false;
				
				// Get node name by xmlrequest
				if (id > 0)
						umbraco.presentation.webservices.CMSNode.GetNodeName('<%=umbraco.BasePages.BasePage.umbracoUserContextID%>', id, updateName);
				else
					document.getElementById("pageName").innerHTML = "<b><%=umbraco.ui.Text("content")%></b>";
			}
			
			function updateName(result) {
				document.getElementById("pageName").innerHTML = "<b>" + result + "</b>";
			}

function doSubmit() {document.Form1["ok"].click()}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	
		</script>
		<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body>
	
		<form id="Form1" method="post" runat="server" onsubmit="setTimeout('document.getElementById(\'ok\').disabled = true;', 500); return true;">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="../webservices/CMSNode.asmx" />
            </Services>
            </asp:ScriptManager>
		<input type="hidden" id="copyTo" name="copyTo"/>
		<div style="float: right; padding: 3px;"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div><h3><img src="../images/<%=Request["mode"]%>.small.png" align="absmiddle"/> <asp:Literal ID="Header" Runat="server"></asp:Literal></h3>
		<img class="gradient" style="width: 100%; height: 1px; margin-top: 7px;" src="../images/nada.gif"/><br/>
		<asp:Literal ID="FeedBackMessage" Runat="server"/>
		<br/>
		<asp:Panel ID="TheForm" Runat="server" Visible="True">
			<span style="margin-left: 10px;" class="guiDialogNormal"><strong><asp:Literal id="SubHeader" Runat="server"></asp:Literal><br/></strong></span>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server" ID="Table1">
				<TBODY>
					<TR>
						<TD class="propertyHeader" colspan="2">
							<iframe src="../TreeInit.aspx?app=<%=umbraco.helper.Request("app")%>&amp;isDialog=true&amp;dialogMode=id&amp;contextMenu=false" style="LEFT: 9px; OVERFLOW: auto; WIDTH: 290px; POSITION: relative; TOP: 0px; HEIGHT: 250px; BACKGROUND-COLOR: white"></iframe>
							<br/>
							<span style="font-weight:lighter"><asp:Literal ID="moveOrCopyTo" Runat="server"></asp:Literal>: </span><asp:Label id="pageName" Runat="server"></asp:Label>
							<br/>
							<asp:Panel ID="rememberHistory" runat="server" Visible="false">
							<asp:CheckBox Text="Relate copied items to orignal" runat="server" ID="RelateDocuments" Checked="false" />
							</asp:Panel>
							<br/><asp:Button ID="ok" Runat="server" CssClass="guiInputButton" onclick="HandleMoveOrCopy"></asp:Button> &nbsp; <input type="button" onclick="if (confirm('<%=umbraco.ui.Text("defaultdialogs", "confirmSure", this.getUser())%>')) parent.window.close()" value="<%=umbraco.ui.Text("general", "cancel", this.getUser())%>" style="width: 60px"/>
						</td>
					</TR>
				</tbody>
			</table>
			</asp:Panel>
							
		</FORM>
	</body>
</HTML>

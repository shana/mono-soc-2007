<%@ Page language="c#" Codebehind="publish.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.publish" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - <%=umbraco.ui.Text("actions", "publish", this.getUser())%> &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp; </title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>body {margin: 2px;}</style>
		<script type="text/javascript" src="../js/xmlextras.js"></script>
		<script type="text/javascript" src="../js/xmlRequest.js"></script>
		<script type="text/javascript" src="../webservices/ajax.js"></script>
		<script type="text/javascript" src="../webservices/GetJavaScriptProxy.aspx?service=publication.asmx"></script>
		
		<script language="javascript">
		
		var pubTotal = <asp:Literal ID="total" Runat="server"></asp:Literal>;
		xmlHttpDebug = true;
				
		var reqNode;
		function startPublication() {		
			updateTotal();
		}
		
		function showPublication() {
//			document.getElementById("pubCounter").innerHTML = "<b>0 <%=umbraco.ui.Text("of")%> " + pubTotal + "</b>";
			document.getElementById('formDiv').style.display = 'none'; 
			document.getElementById('animDiv').style.display = 'block'; 
			document.getElementById('anim').src = '../images/anims/publishPages.gif';
		}
		
		function updateTotal() {
			setTimeout("showPublication()", 100);
			setTimeout("updatePublication()", 200);
		}
		
		function updatePublication() {
			proxies.publication.GetPublicationStatus.func = updatePublicationDo;
			proxies.publication.GetPublicationStatus('<%=umbraco.helper.Request("id")%>');
		}
		
		
		function updatePublicationDo(retVal) {

			progressBarUpdateLabel('publishStatus', "<b>" + retVal + " <%=umbraco.ui.Text("of")%> " + pubTotal + "</b>");
			
			// progressbar
			retVal = parseInt(retVal);
			if (retVal > 0) {
				var percent = Math.round((retVal/pubTotal)*100);
				progressBarUpdate('publishStatus', percent);
			}
			setTimeout("updatePublication()", 500);
		}
		
		// pubCounter
function doSubmit() {document.Form1["ok"].click()}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	
		</script>
		<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body>
	
		<form id="Form1" method="post" runat="server">
		
		<div style="float: right; padding: 3px;"><a href="javascript:window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
		
		<h3><img src="../images/publish.gif" align="absmiddle"/> <%=umbraco.ui.Text("actions", "publish", this.getUser())%></h3>
		<img class="gradient" style="width: 100%; height: 1px; margin-top: 7px;" src="../images/nada.gif"/><br/>
		<asp:Literal ID="FeedBackMessage" Runat="server"/>
		<br/>
		
		
		<div id="animDiv" style="display: none; width: 100%; text-align: center">
		
		<div style="width: 240px; padding: 5px; text-align: center; background: white; border: 1px solid #666; margin: auto;">
		
		<img src="../images/anims/publishPages.gif" id="anim" width="150" height="42" alt="<%=umbraco.ui.Text("publish", "inProgress", this.getUser())%>"/>
		<br/>
		<script>
		umbPgStep = 1;
		umbPgIgnoreSteps = true;
		</script>
		
		<div align="center">
		<span class="guiDialogTiny"><%=umbraco.ui.Text("publish", "inProgress", this.getUser())%></span>
		<br/><br/>
		<asp:PlaceHolder ID="progressBar" Runat="server"></asp:PlaceHolder>
		</div>
		
		</div>
		
		</div>
<asp:Panel ID="TheForm" Visible="True" Runat="server">
		<div id="formDiv" style="visibility: visible;">
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server" ID="Table1">
				<TBODY>
					<TR>
						<TD class="propertyHeader" colspan="2">
		<asp:CheckBox Runat="server" ID="PublishAll"></asp:CheckBox>
							<br/>
							<br/><asp:Button ID="ok" Runat="server" CssClass="guiInputButton"></asp:Button> &nbsp; <input type="button" onclick="if (confirm('<%=umbraco.ui.Text("defaultdialogs", "confirmSure", this.getUser())%>')) window.close()" value="<%=umbraco.ui.Text("general", "cancel", this.getUser())%>" style="width: 60px"/>
						</TD>
					</TR>
				</tbody>
			</table>
		</div>
</asp:Panel>				
		</FORM>
	</body>
</HTML>

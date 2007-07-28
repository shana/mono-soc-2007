<%@ Page language="c#" Codebehind="protectPage.aspx.cs" AutoEventWireup="True" Inherits="umbraco.presentation.umbraco.dialogs.protectPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<headEAD>
		<title>umbraco -
			<%=umbraco.ui.Text("protect")%>
		</title>
		<link href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>BODY { MARGIN: 2px }
	</style>
			<script language="javascript">

			function updateLoginId() {
				var treePicker = window.showModalDialog('../dialogs/treePicker.aspx?appAlias=content', 'treePicker', 'dialogWidth=350px;dialogHeight=300px;scrollbars=no;center=yes;border=thin;help=no;status=no')			
				if (treePicker != undefined) {
					document.getElementById("loginId").value = treePicker;
					if (treePicker > 0) {
						umbraco.presentation.webservices.CMSNode.GetNodeName('<%=umbraco.BasePages.BasePage.umbracoUserContextID%>', treePicker, updateLoginTitle);
					} else 
						document.getElementById("loginTitle").innerHTML =  "<strong><%=umbraco.ui.Text("content", base.getUser())%></strong>";
				}
			}			
			function updateLoginTitle(result) {
				document.getElementById("loginTitle").innerHTML = "<strong>" + result + "</strong> &nbsp;";
			}

			function updateErrorId() {
				var treePicker = window.showModalDialog('../dialogs/treePicker.aspx?appAlias=content', 'treePicker', 'dialogWidth=350px;dialogHeight=300px;scrollbars=no;center=yes;border=thin;help=no;status=no')			
				if (treePicker != undefined) {
					document.getElementById("errorId").value = treePicker;
					if (treePicker > 0) {
						umbraco.presentation.webservices.CMSNode.GetNodeName('<%=umbraco.BasePages.BasePage.umbracoUserContextID%>', treePicker, updateErrorTitle);
					} else 
						document.getElementById("errorTitle").innerHTML =  "<strong><%=umbraco.ui.Text("content", base.getUser())%></strong>";
				}
			}			
			function updateErrorTitle(result) {
				document.getElementById("errorTitle").innerHTML = "<strong>" + result + "</strong> &nbsp;";
			}


			function toggleSimple() {
				if (document.getElementById("advanced").style.display != "none") {
					document.getElementById("advanced").style.display = "none";
					document.getElementById("simpleForm").style.display = "block";
					document.getElementById("buttonSimple").style.display = "block";
					togglePages();
				} else {
					document.getElementById("advanced").style.display = "block";
					document.getElementById("simpleForm").style.display = "none";
					document.getElementById("buttonSimple").style.display = "none";
					document.getElementById("pagesForm").style.display = "none";
				}
			}
			
			function togglePages() {
				document.getElementById("pagesForm").style.display = "block";
			}

			function toggleAdvanced() {
				if (document.getElementById("simple").style.display != "none") {
					document.getElementById("simple").style.display = "none";
					document.getElementById("advancedForm").style.display = "block";
					document.getElementById("buttonAdvanced").style.display = "block";
					togglePages();
				} else {
					document.getElementById("simple").style.display = "block";
					document.getElementById("advancedForm").style.display = "none";
					document.getElementById("pagesForm").style.display = "none";
					document.getElementById("buttonAdvanced").style.display = "none";
				}
			}
			</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Services>
                    <asp:ServiceReference Path="../webservices/CMSNode.asmx" />
                </Services>
            </asp:ScriptManager>
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/protect.png', sizingMethod='scale')" />
				<%=umbraco.ui.Text("protect")%>
			</h3>
			<img class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif" /><br />
			<asp:Literal ID="FeedBackMessage" Runat="server" />
			<table class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
				runat="server">
				<tr>
					<td class="propertyContent" colSpan="2"><INPUT id="tempFile" type="hidden" name="tempFile" runat="server">
						<asp:Panel ID="Wizard" Runat="server" Visible="True">
							<span class="guiDialogHeader"><%=umbraco.ui.Text("paHowWould")%></span>
							<br />
							<br />
							<div id="simple" style="DISPLAY: block"><a href="javascript:toggleSimple();"><%=umbraco.ui.Text("paSimple")%> <IMG src="../images/forward.png" align="absMiddle" border="0"></a><br />
								<div id="simpleForm" style="DISPLAY: none">
									<table cellPadding="4" border="0">
										<tr>
											<td width="150"><B><%=umbraco.ui.Text("login")%></B></td>
											<td>
												<asp:TextBox id="simpleLogin" Runat="server" Width="150px"></asp:TextBox></td>
										</tr>
										<tr>
											<td><B><%=umbraco.ui.Text("password")%></B></td>
											<td>
												<asp:TextBox id="simplePassword" Runat="server" Width="150px"></asp:TextBox></td>
										</tr>
									</table>
								</div>
								<br />
							</div>
							<div id="advanced" style="DISPLAY: block"><a href="javascript:toggleAdvanced();"><%=umbraco.ui.Text("paAdvanced")%> <IMG src="../images/forward.png" align="absMiddle" border="0"></a><br />
								<div id="advancedForm" style="DISPLAY: none; MARGIN-LEFT: -5px">
									<asp:PlaceHolder id="groupsSelector" Runat="server"></asp:PlaceHolder></div>
							</div>
						</asp:Panel>
						<asp:Panel ID="Pages" Runat="server" Visible="True">
							<div id="pagesForm" style="DISPLAY: none">
								<table cellPadding="4" border="0">
									<tr>
										<TH width="150">
											<%=umbraco.ui.Text("paLoginPage")%>:</TH>
										<td><span id="loginTitle">
												<asp:Literal id="loginTitle" Runat="server"></asp:Literal>
											</span><a href="javascript:updateLoginId()"><%=umbraco.ui.Text("choose")%>...</a><br />
											<span class="guiDialogTiny"><%=umbraco.ui.Text("paLoginPageHelp")%></span>
										</td>
									</tr>
									<tr>
										<TH>
											<%=umbraco.ui.Text("paErrorPage")%>:</TH>
										<td><span id="errorTitle">
												<asp:Literal id="errorTitle" Runat="server"></asp:Literal>
											</span><a href="javascript:updateErrorId()"><%=umbraco.ui.Text("choose")%>...</a><br />
											<span class="guiDialogTiny"><%=umbraco.ui.Text("paErrorPageHelp")%></span>
										</td>
									</tr>
								</table>
								<input id="errorId" type="hidden" runat="server" /><input id="loginId" type="hidden" runat="server" />
							</div>
							<div id="buttonSimple" style="DISPLAY: none; FLOAT: left">
								<asp:Button id="protectSimple" Runat="server" onclick="protectSimple_Click"></asp:Button></div>
							<div id="buttonAdvanced" style="DISPLAY: none; FLOAT: left">
								<asp:Button id="protectAdvanced" Runat="server" onclick="protectAdvanced_Click"></asp:Button></div>
							<asp:Button id="buttonRemoveProtection" Runat="server" Visible="False" onclick="buttonRemoveProtection_Click"></asp:Button>
							<div></div>
						</asp:Panel></td>
				</tr>
			</table>
			<asp:PlaceHolder ID="js" Runat="server"></asp:PlaceHolder>
		</form>
		<script language="javascript">
		<asp:Literal Runat="server" ID="jsShowWindow"></asp:Literal>
		</script>
	</body>
</HTML>

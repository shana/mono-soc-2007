<%@ Page language="c#" Codebehind="rollBack.aspx.cs" AutoEventWireup="True" Inherits="umbraco.presentation.dialogs.rollBack" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco -
			<%=umbraco.ui.Text("defaultdialogs", "rollback", this.getUser())%>
		</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>BODY { MARGIN: 2px }
		</style>
		<script language="javascript">
function doSubmit() {document.Form1["ok"].click()}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	
		</script>
		<script src="../js/umbracoCheckKeys.js" type="text/javascript"></script>
		<style type="text/css">HTML { BACKGROUND: #e6e6e6 }
	BODY { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px }
	HTML { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px }
	HTML .boxhead H2 { HEIGHT: 1% }
	.sidebox { BACKGROUND: url(../images/backgrounds/boxbody-r.png) no-repeat right bottom; FLOAT: left; MARGIN: 0px; WIDTH: 49% }
	.boxhead { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; BACKGROUND: url(../images/backgrounds/boxhead-r.png) no-repeat right top; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px; TEXT-ALIGN: center }
	.boxhead H2 { PADDING-RIGHT: 30px; PADDING-LEFT: 30px; FONT-SIZE: 18px; BACKGROUND: url(../images/backgrounds/boxhead-l.png) no-repeat left top; PADDING-BOTTOM: 5px; MARGIN: 0px; PADDING-TOP: 22px; FONT-FAMILY: Trebuchet MS, Lucida Grande, verdana, arial; TEXT-ALIGN: left }
	.boxbody { PADDING-RIGHT: 30px; PADDING-LEFT: 30px; BACKGROUND: url(../images/backgrounds/boxbody-l.png) no-repeat left bottom; PADDING-BOTTOM: 51px; MARGIN: 0px; PADDING-TOP: 5px }
	.content { PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px; OVERFLOW: auto; WIDTH: 310px; PADDING-TOP: 5px; HEIGHT: 350px }
		</style>
		<script>
			var active = false;
			function change() {
				if (!active) {
//					document.getElementById("previewHeader").style.display = 'none';
					document.getElementById("previewSelector").style.display = 'block';
					document.getElementById("changeLink").style.display = 'none';
					active = false;
				} else {
//					document.getElementById("previewHeader").style.display = 'block';
					document.getElementById("previewSelector").style.display = 'none';
					document.getElementById("changeLink").style.display = 'block';
					active = true;
				}
			}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><A class="guiDialogTiny" href="javascript:window.close()"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></A></div>
			<h3><IMG src="../images/rollback.gif" align="absMiddle">
				<%=umbraco.ui.Text("defaultdialogs", "rollback", this.getUser())%>
			</h3>
			<IMG class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif"><br />
			<asp:literal id="FeedBackMessage" Runat="server"></asp:literal><br />
			<p style="margin: 0; margin-left: 11px; padding: 0;" class="guiDialogNormal">
			<asp:CheckBox id="CheckBoxHtml" runat="server" Text="Encode html" AutoPostBack="True"></asp:CheckBox><br />
			</p>
			<asp:panel id="theForm" Runat="server">
				<DIV class="sidebox">
					<DIV class="boxhead">
						<H2>Current:
							<asp:label id="currentVersionTitle" Runat="server"></asp:label></H2>
					</DIV>
					<DIV class="boxbody">
						<DIV class="content">
							<P class="guiDialogTiny" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px 0px 7px; PADDING-TOP: 0px">
								<asp:literal id="currentVersionDetails" Runat="server"></asp:literal></P>
							<asp:placeholder id="currentVersionContent" Runat="server"></asp:placeholder></DIV>
					</DIV>
				</DIV>
				<DIV class="sidebox">
					<DIV class="boxhead">
						<H2>
							<asp:label id="previewVersionTitle" Runat="server"></asp:label></H2>
					</DIV>
					<DIV class="boxbody">
						<DIV class="content">
							<DIV id="changeLink">
								<P class="guiDialogTiny" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px 0px 7px; PADDING-TOP: 0px">
									<asp:literal id="previewVersionDetails" Runat="server"></asp:literal><A onclick="change();" href="javascript:void(0);">Change...</A></P>
							</DIV>
							<DIV id="previewSelector" style="DISPLAY: none">
								<asp:DropDownList id="allVersions" Runat="server" Width="290px" AutoPostBack="True" CssClass="guiInputTextTiny"></asp:DropDownList></DIV>
							<asp:PlaceHolder id="previewVersionContent" Runat="server"></asp:PlaceHolder></DIV>
					</DIV>
				</DIV>
				<p style="TEXT-ALIGN: center">
					<asp:Button id="doRollback" Runat="server" Width="250px"  Text="Roll Back" Enabled="False" onclick="doRollback_Click"></asp:Button></p>
			</asp:panel></form>
	</body>
</HTML>

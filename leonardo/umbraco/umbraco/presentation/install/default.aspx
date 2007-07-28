<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="True" Inherits="umbraco.presentation.install._default" EnableViewState="False" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco <%=umbraco.GlobalSettings.CurrentVersion%> Configuration Wizard</title>
		<style>
			#configLeft { MIN-HEIGHT: 450px; BACKGROUND: url(../umbraco/images/install/config_bg_left.png) no-repeat; WIDTH: 577px; _HEIGHT: 450px }
			#config { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; BACKGROUND: url(../umbraco/images/install/config_bg_right.png) no-repeat right top; PADDING-BOTTOM: 0px; MARGIN: 0px; WIDTH: 586px; PADDING-TOP: 0px }
			#configBottom { BACKGROUND: url(../umbraco/images/install/config_bottom.png) no-repeat; WIDTH: 586px }
			#content { PADDING-RIGHT: 25px; PADDING-LEFT: 15px; FONT-SIZE: 14px; PADDING-BOTTOM: 15px; COLOR: #000; PADDING-TOP: 60px; FONT-FAMILY: Lucida Sans; TEXT-ALIGN: left }
			H1 { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; FONT-WEIGHT: bold; FONT-SIZE: 22px; PADDING-BOTTOM: 0px; MARGIN: 10px 0px; COLOR: #fff; PADDING-TOP: 0px }
			P { MARGIN: 5px 0px 10px; LINE-HEIGHT: 140% }
			FORM { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px }
			#buttons { TEXT-ALIGN: right }
			.button, #buttons INPUT { FONT-SIZE: 18px; MARGIN: 30px 0px; WIDTH: 100px }
			</style>
	</HEAD>
	<body>
		<div id="config">
			<div id="configLeft">
				<div id="content">
					<hr size="1" color="#ffffff" noshade>
					<form id="Form1" method="post" runat="server">
						<input type="hidden" runat="server" value="welcome" id="step">
						<asp:PlaceHolder ID="PlaceHolderStep" Runat="server"></asp:PlaceHolder>
						<div id="buttons">
							<asp:Button Visible="False" ID="back" Enabled="False" Text="« Back" Runat="server"></asp:Button>
							<asp:Button ID="next" Enabled="true" Text="Next »" Runat="server" onclick="next_Click"></asp:Button>
						</div>
					</form>
				</div>
			</div>
		</div>
		<div id="configBottom"><img src="../umbraco/images/nada.gif" alt="" style="WIDTH: 786px; HEIGHT: 9px"></div>
	</body>
</HTML>

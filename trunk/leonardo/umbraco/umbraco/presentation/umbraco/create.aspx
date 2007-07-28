<%@ Page language="c#" Codebehind="create.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.Create" %>
<%@ Register Namespace="umbraco" TagPrefix="umb" Assembly="umbraco" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title><asp:Literal id="headerTitle" runat="server"></asp:Literal> - umbraco &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; </title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		
		var preExecute;
		
		
		function doSubmit() {document.forms[0].submit();}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;



		</script>
		<script type="text/javascript" src="js/umbracoCheckKeys.js"></script>
		<style>
		.dropdownType {
		    font-size: 200%; 
		    width: 650px;
		}
		</style>
	</HEAD>
	<body onload="this.focus();" style="BACKGROUND: url(images/backgrounds/create.png); MARGIN: 5px">
		<form id="Form1" method="post" runat="server" style="MARGIN: 0px">
			<div style="MARGIN-TOP: 11px; PADDING-LEFT: 15px">
				<span style="FONT-SIZE: 18px; MARGIN-LEFT: 40px; COLOR: #1272ac; FONT-FAMILY: Trebuchet MS,Lucida Grande; TEXT-ALIGN: left">
					<asp:Literal ID="title" Runat="server"></asp:Literal></span><br />
				<div style="MARGIN-TOP: 12px" class="guiDialogNormal">
					<asp:PlaceHolder id="UI" Runat="server"></asp:PlaceHolder></SPAN>
				</div>
		</form>
		<script>
		function setFocusOnText() {
			for (var i=0;i<document.forms[0].length;i++) {
				if (document.forms[0][i].type == 'text') {
					document.forms[0][i].focus();
					break;
				}
			}
		}
		
		setTimeout("setFocusOnText()", 100);
		</script>
		</DIV>
	</body>
</HTML>

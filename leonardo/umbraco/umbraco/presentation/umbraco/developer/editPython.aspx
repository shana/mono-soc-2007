<%@ Page validateRequest="false" language="c#" Codebehind="editPython.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.developer.editPython" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editPython</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script src="../js/editorBarFunctions.js" type="text/javascript"></script>
		<script language="javascript">
		function pythonInsertValue(theValue) {
			insertAtCaret(document.getElementById('pythonSource'), theValue)
		}
		
		function doSubmit() {
			document.contentForm.submit();
		}
		
		function resizeTextArea() {
		var clientHeight = self.innerHeight;
		var clientWidth = self.innerWidth;
		
		if (clientHeight == null) {
			clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
			clientWidth = document.body.clientWidth;
			}
			document.getElementById('pythonSource').style.width = (clientWidth - 60)+"px";
			document.getElementById('pythonSource').style.height = (clientHeight - 210)+"px" ;
		}
		
		function showError() {
			if (document.getElementById('errorHolder').style.display == 'block' || document.getElementById('errorHolder').style.display == '') {
				document.getElementById('errorHolder').style.display = 'none';
				document.getElementById('showErrorLink').innerHTML = '<b>Show error</b> <img src="../images/arrowForward.gif" align="absmiddle" border="0"/><br/><br/>';
			}
			else {
				document.getElementById('errorHolder').style.display = 'block';
				document.getElementById('showErrorLink').innerHTML = '<b>Hide error</b> <img src="../images/arrowDown.gif" align="absmiddle" border="0"/><br/>';
			}
		}
		</script>
	</HEAD>
	<body onresize="resizePanel('UmbracoPanel1',true);resizeTextArea();" onload="resizePanel('UmbracoPanel1',true);resizeTextArea();">
		<form id="contentForm" runat="server">
			<cc1:umbracopanel id="UmbracoPanel1" runat="server" Text="Edit xsl" height="300" width="500">
				<cc2:Pane id="Pane1" runat="server">
					<TABLE cellSpacing="0" cellPadding="4" width="100%" border="0">
						<TR>
							<TH width="30%">
								Filename</TH>
							<TD class="propertyContent">
								<asp:TextBox id="pythonFileName" runat="server" Width="400" cssClass="guiInputText"></asp:TextBox></TD>
						</TR>
						<TR>
							<TH width="30%">
								Skip testing (ignore errors)</TH>
							<TD class="propertyContent">
								<asp:CheckBox id="SkipTesting" Runat="server"></asp:CheckBox></TD>
						</TR>
						<TR>
							<TD colSpan="2">
								<asp:Literal id="closeErrorMessage" Runat="server" Visible="false">
									<A id="showErrorLink" href="javascript:showError()"><b>Hide error</b> <img src="../images/arrowDown.gif" align="absmiddle" border="0"/></A></asp:Literal>
								<asp:Panel id="errorHolder" Runat="server" Visible="False">
									<SPAN style="COLOR: red">
										<asp:Label id="pythonError" Runat="server"></asp:Label></SPAN></asp:Panel><B>Source:</B><br />
								<cc1:CodeArea id="pythonSource" height="400" width="400" Runat="server"></cc1:CodeArea></TH></TD>
						</TR>
					</TABLE>
				</cc2:Pane>
			</cc1:umbracopanel></form>
	</body>
</HTML>
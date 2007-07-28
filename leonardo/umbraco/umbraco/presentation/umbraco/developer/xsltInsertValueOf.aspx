<%@ Page language="c#" Codebehind="xsltInsertValueOf.aspx.cs" AutoEventWireup="True" Inherits="umbraco.developer.xsltInsertValueOf" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>xsltChooseExtension</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link type="text/css" rel="stylesheet" href="../css/umbracoGui.css">
		<script>
function doSubmit() {
	
	var checked = "";
	if (document.getElementById('disableOutputEscaping').checked)
		checked = ' disable-output-escaping="yes"';
		
	result = '<xsl:value-of select="' + document.getElementById('valueOf').value + '"' + checked + '/>';
	
	window.opener.right.umbracoInsertFieldDo('<%=umbraco.helper.Request("objectId")%>', result, '<%=umbraco.helper.Request("move")%>');
	window.close();
}

function getExtensionMethod() {
	window.open("xsltChooseExtension.aspx", "ext", "width=750, height=170, scrollbars=no");
}

function recieveExtensionMethod(theValue) {
	document.getElementById('valueOf').value = theValue;	
}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
		</script>
		<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body onload="this.focus();">
		<form id="Form1" method="post" runat="server">
			<h3>Insert value of</h3>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="100%" border="0">
				<tr>
					<td class="propertyHeader">Value:</td>
					<td class="propertyContent">
						<P>
							<asp:TextBox Runat="server" ID="valueOf" Width="250px"></asp:TextBox>
							<asp:DropDownList id="preValues" runat="server" Width="150px" Font-Size="XX-Small"></asp:DropDownList>
							<input type="button" value="Get Extension" onClick="getExtensionMethod();" style="FONT-SIZE: xx-small"><br />
						</P>
					</td>
				</tr>
				<TR>
					<td class="propertyHeader">Disable output escaping</td>
					<td class="propertyContent"><asp:CheckBox Runat="server" Text="Yes" ID="disableOutputEscaping"></asp:CheckBox></td>
				</TR>
				<tr>
					<td></td>
					<td class="propertyContent">
						<input type="button" value="Insert value" onClick="doSubmit();" style="FONT-SIZE: xx-small">
						<P></P>
					</td>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>

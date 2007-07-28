<%@ Page language="c#" Codebehind="xsltChooseExtension.aspx.cs" AutoEventWireup="True" Inherits="umbraco.developer.xsltChooseExtension" %>
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
function returnResult() {
	result = document.getElementById('assemblies').value + ":" + document.getElementById('selectedMethod').value + "(";
	for (var i=0; i<document.forms[0].length-1;i++) {
		if(document.forms[0][i].name.indexOf('param') > -1)
			result = result + "'" + document.forms[0][i].value + "', "
	}
	if (result.substring(result.length-1, result.length) == " ")
		result = result.substring(0, result.length-2);
	result = result +  ")"
	parent.opener.recieveExtensionMethod(result);
	window.close();
}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h3>Insert extension method</h3>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="100%" border="0">
			<tr><td class="propertyContent">
	<P>
		<asp:DropDownList id="assemblies" runat="server" Width="100px" Font-Size="XX-Small"></asp:DropDownList>
		<asp:DropDownList id="methods" runat="server" Width="300px" Font-Size="XX-Small"></asp:DropDownList><br/>
		<div style="padding-top: 10px;">
		<asp:PlaceHolder id="PlaceHolderParamters" runat="server"></asp:PlaceHolder>
		</div>
		</P>
</td></tr>
			</TABLE>
					</form>
	</body>
</HTML>

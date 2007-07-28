<%@ Page language="c#" Codebehind="treePicker.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.treePicker" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco ::</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>body {margin: 2px;}</style>
		<script language="javascript">
			function dialogHandler(id) {
			<%
			    if (umbraco.helper.Request("useSubModal") != "") {%>
                window.parent.hidePopWin(true, id)
			<%
} else
{
%>
				window.returnValue = id;
				window.close();
<%
}
%>
			}
			
			function closeWindow() {
			<%
			    if (umbraco.helper.Request("useSubModal") != "") {%>
                window.parent.hidePopWin(false)
			<%
} else
{
%>
				window.close();
<%
}
%>
			}
			
		</script>
	</HEAD>
	<body onLoad="this.focus()">
				<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:closeWindow()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>

		<iframe src="../TreeInit.aspx?app=<%=Request.QueryString["appAlias"]%>&amp;isDialog=true&amp;dialogMode=id&amp;contextMenu=false" style="LEFT: 9px; OVERFLOW: auto; WIDTH: 250px; POSITION: relative; TOP: 0px; HEIGHT: 250px; BACKGROUND-COLOR: white"></iframe>
	</body>
</HTML>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sendToTranslation.aspx.cs" Inherits="umbraco.presentation.dialogs.sendToTranslation" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - <%=umbraco.ui.Text("sendToTranslate")%></title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>BODY { MARGIN: 2px }
	</style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 23px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/sendToTranslate.png', sizingMethod='scale')">
				<%=umbraco.ui.Text("sendToTranslate")%></h3>
			<span class="guiDialogNormal" style="margin-left:10px"><asp:Literal ID="nodeName" Runat="server"></asp:Literal></span>
			<img class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif"><br />
			<asp:Literal ID="FeedBackMessage" Runat="server" />
			<asp:Panel ID="TheForm" Runat="server" Visible="True">
				<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
				<TBODY>
					<TR>
						<TD class="propertyHeader" width="30%"><%=umbraco.ui.Text("translator", this.getUser())%></TD>
						<TD class="propertyContent">							
							<asp:DropDownList ID="translator" runat="server"></asp:DropDownList>
						</td>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%"><%=umbraco.ui.Text("language", this.getUser())%></TD>
						<TD class="propertyContent">							
							<asp:DropDownList ID="language" runat="server"></asp:DropDownList><br />
							<asp:Literal ID="defaultLanguage" runat="server"></asp:Literal>
						</td>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%"><%=umbraco.ui.Text("includeSubpages", this.getUser())%></TD>
						<TD class="propertyContent">							
							<asp:CheckBox ID="includeSubpages" runat="server" />
						</td>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%"><%=umbraco.ui.Text("comment", this.getUser())%></TD>
						<TD class="propertyContent">							
							<asp:TextBox TextMode="multiLine" runat="Server" Rows="4" ID="comment"></asp:TextBox>
						</td>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%">&nbsp;</TD>
						<TD class="propertyContent">							
							<asp:Button ID="doTranslation" runat="Server" OnClick="doTranslation_Click" />
						</td>
					</TR>
				</TABLE>
			</asp:Panel>
		</form>
	</body>
</HTML>

<%@ Page language="c#" Codebehind="viewAuditTrail.aspx.cs" AutoEventWireup="True" Inherits="umbraco.presentation.umbraco.dialogs.viewAuditTrail" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - Audit Trail </title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>BODY { MARGIN: 2px }
	</style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 23px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/audit.png', sizingMethod='scale')">
				<%=umbraco.ui.Text("view")%> <%=umbraco.ui.Text("auditTrail")%></h3>
			<span class="guiDialogNormal" style="margin-left:10px"><%=umbraco.ui.Text("atViewingFor")%> <asp:Literal ID="nodeName" Runat="server"></asp:Literal></span>
			<img class="gradient" style="MARGIN-TOP: 7px; WIDTH: 100%; HEIGHT: 1px" src="../images/nada.gif"><br />
			<asp:Literal ID="FeedBackMessage" Runat="server" />
			<asp:Panel ID="TheForm" Runat="server" Visible="True">
				<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyContent" colSpan="2">
							<asp:DataGrid id="auditLog" Runat="server" HeaderStyle-Font-Bold="True" AutoGenerateColumns="False"
								AlternatingItemStyle-BackColor="#EEEEEE" CellPadding="5">
								<Columns>
									<asp:TemplateColumn>
										<HeaderTemplate>
											<b><%=umbraco.ui.Text("action")%></b>
										</HeaderTemplate>
										<ItemTemplate>
											<%# FormatAction(DataBinder.Eval(Container.DataItem, "Action", "{0}")) %>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderTemplate>
											<b><%=umbraco.ui.Text("user")%></b>
										</HeaderTemplate>
										<ItemTemplate>
											<%# DataBinder.Eval(Container.DataItem, "User", "{0}") %>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderTemplate>
											<b><%=umbraco.ui.Text("date")%></b>
										</HeaderTemplate>
										<ItemTemplate>
											<%# DataBinder.Eval(Container.DataItem, "Date", "{0:D} {0:T}") %>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderTemplate>
											<b><%=umbraco.ui.Text("comment")%></b>
										</HeaderTemplate>
										<ItemTemplate>
											<%# DataBinder.Eval(Container.DataItem, "Comment", "{0}") %>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid></TD>
					</TR>
				</TABLE>
			</asp:Panel>
		</form>
	</body>
</HTML>

<%@ Register TagPrefix="uc1" TagName="ContentTypeControlNew" Src="../controls/ContentTypeControlNew.ascx" %>
<%@ Page language="c#" Codebehind="EditMemberType.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.members.EditMemberType" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register Namespace="umbraco" TagPrefix="umb" Assembly="umbraco" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editXslt</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body onresize="resizeTabView(ContentTypeControlNew1_TabView1_tabs, 'ContentTypeControlNew1_TabView1')"
		bgColor="#f2f2e9" onload="resizeTabView(ContentTypeControlNew1_TabView1_tabs, 'ContentTypeControlNew1_TabView1')">
		<form id="contentForm" runat="server">
			<uc1:ContentTypeControlNew id="ContentTypeControlNew1" runat="server"></uc1:ContentTypeControlNew>
			<cc1:Pane id="Pane1andmore" runat="server">
			<asp:DataGrid id="dgEditExtras" runat="server" AutoGenerateColumns="False" Width="100%" HeaderStyle-Font-Bold=True OnItemDataBound="dgEditExtras_itemdatabound">
				<Columns>
					<asp:BoundColumn DataField="id" HeaderText="" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="name" HeaderText="Property name"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Member can edit">
						<ItemTemplate>
							<asp:CheckBox ID="ckbMemberCanEdit" Runat="server"></asp:CheckBox>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Show on profile">
						<ItemTemplate>
							<asp:CheckBox ID="ckbMemberCanView" Runat="server"></asp:CheckBox>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			</cc1:Pane>
		</form>
	</body>
</HTML>

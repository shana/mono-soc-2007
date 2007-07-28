<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContentTypeControlNew.ascx.cs" Inherits="umbraco.controls.ContentTypeControlNew" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<cc1:tabview id="TabView1" Height="392px" Width="552px" runat="server"></cc1:tabview><asp:panel id="pnlGeneral" Runat="server"></asp:panel><asp:panel id="pnlTab" style="TEXT-ALIGN: left" Runat="server">
	<cc2:Pane id="Pane2" runat="server" Width="528px" Height="44px">
		<TABLE>
			<TR>
				<TH>
					<%=umbraco.ui.Text("newtab", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%>
				</TH>
				<TD>
					<asp:TextBox id="txtNewTab" Runat="server"></asp:TextBox></TD>
				<TD>
					<asp:Button id="btnNewTab" Runat="server" Text="New tab" onclick="btnNewTab_Click"></asp:Button></TD>
			</TR>
		</TABLE>
	</cc2:Pane>
	<br />
	<cc2:Pane id="Pane1" runat="server" Width="216" Height="80">
		<asp:DataGrid id="dgTabs" Width="100%" Runat="server" OnItemCommand="dgTabs_ItemCommand" HeaderStyle-Font-Bold="True"
			AutoGenerateColumns="False">
			<Columns>
				<asp:BoundColumn DataField="id" Visible="False"></asp:BoundColumn>
				<asp:TemplateColumn HeaderText="Name">
					<ItemTemplate>
						<asp:TextBox ID="txtTab" Runat="server" Value='<%#DataBinder.Eval(Container.DataItem,"name")%>'>
						</asp:TextBox>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:ButtonColumn Text="Delete" CommandName="Delete"></asp:ButtonColumn>
			</Columns>
		</asp:DataGrid>
		<CENTER>
			<asp:Literal id="lttNoTabs" Runat="server"></asp:Literal></CENTER>
	</cc2:Pane>
</asp:panel><asp:panel id="pnlInfo" Runat="server">
	<cc2:Pane id="Pane3" runat="server" Width="216" Height="80">
		<TABLE width="100%">
			<TR>
				<TH>
					<%=umbraco.ui.Text("name", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%>
				</TH>
				<TD>
					<asp:TextBox id="txtName" Runat="server"></asp:TextBox></TD>
			</TR>
			<TR>
				<TH>
					Alias</TH>
				<TD>
					<asp:TextBox id="txtAlias" Runat="server"></asp:TextBox></TD>
			</TR>
			<TR>
				<TH>
					<%=umbraco.ui.Text("icon", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%>
				</TH>
				<TD>
					<asp:DropDownList id="ddlIcons" Runat="server"></asp:DropDownList></TD>
			</TR>
			<tr>
			<th>Thumbnail</th>
				<td>
					<asp:DropDownList id="ddlThumbnails" Runat="server"></asp:DropDownList></td>
			</tr>
			<th>Description</th>
				<td>
					<asp:TextBox ID="description" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox></td>
			</tr>
		</TABLE>
	</cc2:Pane>
</asp:panel><asp:panel id="pnlStructure" Runat="server">
	<cc2:Pane id="Pane5" runat="server" Width="528px" Height="44px">
		<TABLE>
			<TR>
				<TH>
					<%=umbraco.ui.Text("allowedchildnodetypes", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%>
					<br />
				</TH>
			</TR>
			<TR>
				<TD>
					<asp:CheckBoxList id="lstAllowedContentTypes" Runat="server"
						EnableViewState="True"></asp:CheckBoxList>
					<asp:PlaceHolder id="PlaceHolderAllowedContentTypes" Runat="server"></asp:PlaceHolder></TD>
			</TR>
		</TABLE>
	</cc2:Pane>
</asp:panel><asp:panel id="pnlProperties" Runat="server">
	<cc2:Pane id="Pane4" runat="server" Width="216" Height="80">
		<P align="left">
			<TABLE style="MARGIN-TOP: 10px" cellSpacing="0" cellPadding="5" border="0">
				<TR>
					<TD>
						<asp:PlaceHolder id="PropertyTypeNew" Runat="server"></asp:PlaceHolder>
						<asp:PlaceHolder id="PropertyTypes" Runat="server"></asp:PlaceHolder></TD>
				</TR>
			</TABLE>
		</P>
	</cc2:Pane> <!--
	<cc2:Pane id="Pane7" runat="server" Width="528px" Height="44px"> 
		<TABLE width="100%">
			<TR>
				<Th><%=umbraco.ui.Text("alias", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%></Th>
				<Th><%=umbraco.ui.Text("name", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%></Th>
				<Th><%=umbraco.ui.Text("type", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%></Th>
				<Th><%=umbraco.ui.Text("tab", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%></Th>
				<TD></TD>
			</TR>
			<TR>
				<TD>
					<asp:TextBox id="txtPropertyAlias" Runat="server"></asp:TextBox></TD>
				<TD>
					<asp:TextBox id="txtPropertyName" Runat="server"></asp:TextBox></TD>
				<TD>
					<asp:DropDownList id="ddlPropertyType" Runat="server"></asp:DropDownList></TD>
				<TD>
					<asp:DropDownList id="ddlPropertyTab" Runat="server"></asp:DropDownList></TD>
				<TD>
					<asp:Button id="btnCreateProperty" Runat="server" Text="Create"></asp:Button></TD>
			</TR>
		</TABLE>
</cc2:Pane>
	<cc2:Pane id="Pane9" runat="server" Width="528px" Height="44px">
		<asp:DataList id="dlTabs" style="TEXT-ALIGN: left" Width="100%" Runat="server" OnItemDataBound="dlTab_itemdatabound">
			<ItemTemplate>
				<h3 style="display:inline;width:250px;"><%=umbraco.ui.Text("tab", umbraco.BasePages.UmbracoEnsuredPage.CurrentUser)%>:
					<%# DataBinder.Eval(Container.DataItem, "TabName") %>
				</h3>
				<asp:Button ID="btnTabUp" Text="Up" Runat="server" CommandName="MoveUp" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "tabid") %>'>
				</asp:Button>
				<asp:Button ID="btnTabDown" Text="Down" Runat="server" CommandName="MoveDown" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "tabid") %>'>
				</asp:Button>
				<asp:Button ID="btnTabDelete" Text="Delete" Runat="server" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "tabid") %>'>
				</asp:Button>
				<%# DataBinder.Eval(Container.DataItem, "genericProperties") %>
				<center>
					<cc2:Pane id="Pane8" Height="44px" Width="528px" runat="server">
						<asp:DataGrid id="dgGenericPropertiesOfTab" DataSource='<%# ((System.Data.DataRowView)Container.DataItem).CreateChildView("tabidrelation") %>' runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Bold="True" Width="100%" OnItemCommand="dgGenericPropertiesOfTab_itemcommand" BorderWidth=0 OnItemDataBound="dgTabs_itemdatabound">
							<Columns>
								<asp:BoundColumn DataField="id" Visible="False"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Alias">
									<ItemTemplate>
										<asp:TextBox ID="txtPAlias" Runat="server" Value='<%#DataBinder.Eval(Container.DataItem,"alias")%>'>
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Name">
									<ItemTemplate>
										<asp:TextBox ID="txtPName" Runat="server" Value='<%#DataBinder.Eval(Container.DataItem,"name")%>'>
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Type">
									<ItemTemplate>
										<asp:DropDownList ID="ddlType" Runat="server" DataSource="<%# DataTypeTable %>" DataTextField="name" DataValueField="id">
										</asp:DropDownList>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Tab">
									<ItemTemplate>
										<asp:DropDownList ID="dllTab" Runat="server" DataSource="<%# TabTable %>" DataTextField="name" DataValueField="id">
											<asp:ListItem Value="0">General properties</asp:ListItem>
										</asp:DropDownList>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:ButtonColumn Text="Delete" CommandName="Delete" ButtonType="PushButton"></asp:ButtonColumn>
							</Columns>
						</asp:DataGrid>
					</cc2:Pane>
				</center>
			</ItemTemplate>
		</asp:DataList>
	</cc2:Pane>
-->
</asp:panel>

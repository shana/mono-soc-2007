<%@ Page language="c#" Codebehind="editMacro.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.developer.editMacro" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editMacro</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		function doSubmit() {
			document.contentForm.submit();
		}
		</script>
	</HEAD>
	<body onresize="resizeTabView(TabView1_tabs, 'TabView1')" bgColor="#f2f2e9" onload="resizeTabView(TabView1_tabs, 'TabView1')">
		<form id="contentForm" runat="server">
			<cc2:tabview id="TabView1" runat="server" Width="552px" Height="392px"></cc2:tabview><cc1:pane id="Panel1" runat="server">
				<TABLE class="propertyPane" id="macroPane" cellSpacing="0" cellPadding="4" width="98%"
					border="0" runat="server">
					<TR>
						<TD class="propertyHeader" width="30%">Name</TD>
						<TD class="propertyContent">
							<asp:TextBox id="macroName" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox></TD>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%">Alias</TD>
						<TD class="propertyContent">
							<asp:TextBox id="macroAlias" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox></TD>
					</TR>
				</TABLE>
				<TABLE class="propertyPane" id="Table2" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyHeader" width="30%"><IMG alt="Xslt Icon" src="../images/developer/xsltIcon.png" align="absMiddle">
							Use XSLT file</TD>
						<TD class="propertyContent">
							<asp:TextBox id="macroXslt" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox>
							<asp:DropDownList id="xsltFiles" Runat="server"></asp:DropDownList></TD>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%"><IMG alt="User control Icon" src="../images/developer/userControlIcon.png" align="absMiddle">
							or .NET User Control</TD>
						<TD class="propertyContent">
							<asp:TextBox id="macroUserControl" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox> 
							<asp:DropDownList ID="userControlList" runat="server"></asp:DropDownList>
							<asp:PlaceHolder id="assemblyBrowserUserControl" Runat="server"></asp:PlaceHolder></TD>
					</TR>
					<TR>
						<TD class="propertyHeader" vAlign="top" width="30%"><IMG alt="Custom Control Icon" src="../images/developer/customControlIcon.png" align="absMiddle">
							or .NET Custom Control</TD>
						<TD class="propertyContent">
							<asp:TextBox id="macroAssembly" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox> (Assembly)<br />
							<asp:TextBox id="macroType" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox> (Type)
							<asp:PlaceHolder id="assemblyBrowser" Runat="server"></asp:PlaceHolder></TD>
					</TR>
					<TR>
			            <TD class="propertyHeader" width="30%"><IMG alt="python Icon" src="../images/developer/pythonIcon.png" align="absMiddle">
				            or python file</TD>
			            <TD class="propertyContent">
				            <asp:TextBox id="macroPython" runat="server" Width="230px" cssClass="guiInputText"></asp:TextBox>
				            <asp:DropDownList id="pythonFiles" Runat="server"></asp:DropDownList></TD>
		            </TR>

				</TABLE>
				<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyHeader" width="30%">Use in editor</TD>
						<TD class="propertyContent">
							<asp:CheckBox id="macroEditor" Runat="server" Text="Yes"></asp:CheckBox></TD>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%">Render content in editor</TD>
						<TD class="propertyContent">
							<asp:CheckBox id="macroRenderContent" Runat="server" Text="Yes"></asp:CheckBox></TD>
					</TR>
				</TABLE>
				<TABLE class="propertyPane" id="Table3" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyHeader" width="30%">Cache Period</TD>
						<TD class="propertyContent">
							<asp:TextBox id="cachePeriod" Width="60px" Runat="server" CssClass="guiInputText"></asp:TextBox>Seconds</TD>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%">Cache By Page</TD>
						<TD class="propertyContent">
							<asp:CheckBox id="cacheByPage" Runat="server" Text="Yes"></asp:CheckBox></TD>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%">Cache Personalized</TD>
						<TD class="propertyContent">
							<asp:CheckBox id="cachePersonalized" Runat="server" Text="Yes"></asp:CheckBox></TD>
					</TR>
				</TABLE>

			</cc1:pane><cc1:pane id="Panel2" runat="server">
								<asp:Repeater id="macroProperties" Runat="server">
					<HeaderTemplate>
						<table class="propertyPane" cellSpacing="0" cellPadding="2" width="98%" border="0">
							<tr>
								<td class="propertyHeader"><%=umbraco.ui.Text("show",this.getUser())%></td>
								<td class="propertyHeader"><%=umbraco.ui.Text("general", "alias",this.getUser())%></td>
								<td class="propertyHeader"><%=umbraco.ui.Text("general", "name",this.getUser())%></td>
								<td class="propertyHeader"><%=umbraco.ui.Text("general", "type",this.getUser())%></td>
								<td class="propertyHeader" />
							</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="propertyContent">
								<asp:CheckBox Runat="server" ID="macroPropertyHidden" Checked='<%# macroIsVisible (DataBinder.Eval(Container.DataItem, "macroPropertyHidden"))%>'/>
							</td>
							<td class="propertyContent">
								<input type="hidden" id="macroPropertyID" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "id")%>' NAME="macroPropertyID"/>
								<asp:TextBox Runat="server" ID="macroPropertyAlias" Text='<%#DataBinder.Eval(Container.DataItem, "macroPropertyAlias")%>'/>
							</td>
							<td class="propertyContent">
								<asp:TextBox Runat="server" ID="macroPropertyName" Text='<%#DataBinder.Eval(Container.DataItem, "macroPropertyName")%>'/>
							</td>
							<td class="propertyContent">
								<asp:DropDownList OnPreRender="AddChooseList" Runat="server" ID="macroPropertyType" DataTextFormatString="" DataTextField='macroPropertyTypeAlias' DataValueField="id" DataSource='<%# GetMacroPropertyTypes()%>' SelectedIndex='<%# SetMacroPropertyTypesIndex(CheckNull(DataBinder.Eval(Container.DataItem,"macroPropertyType").ToString()).ToString()) %>'>
								</asp:DropDownList>
							</td>
							<td class="propertyContent">
								<asp:Button OnClick="deleteMacroProperty" ID="delete" Text="Delete" runat="server" CssClass="guiInputButton" />
							</td>
						</tr>
					</ItemTemplate>
					<FooterTemplate>
						<tr>
							<td class="propertyContent">
								<asp:CheckBox Runat="server" ID="macroPropertyHiddenNew" />
							</td>
							<td class="propertyContent">
								<asp:TextBox Runat="server" ID="macroPropertyAliasNew" Text='New Alias' OnTextChanged="macroPropertyCreate" />
							</td>
							<td class="propertyContent">
								<asp:TextBox Runat="server" ID="macroPropertyNameNew" Text='New Name' />
							</td>
							<td class="propertyContent">
								<asp:DropDownList OnPreRender="AddChooseList" Runat="server" ID="macroPropertyTypeNew" DataTextField="macroPropertyTypeAlias" DataValueField="id" DataSource='<%# GetMacroPropertyTypes()%>'>
								</asp:DropDownList>
							</td>
							<td class="propertyContent">
								<asp:Button ID="createNew" Text="Add" runat="server" CssClass="guiInputButton" />
							</td>
						</tr>
						</table>
					</FooterTemplate>
				</asp:Repeater>
			</cc1:pane></form>
	</body>
</HTML>

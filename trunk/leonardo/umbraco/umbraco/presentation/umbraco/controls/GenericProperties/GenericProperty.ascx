<%@ Control Language="c#" AutoEventWireup="True" Codebehind="GenericProperty.ascx.cs" Inherits="umbraco.controls.GenericProperties.GenericProperty" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<li id="<%=this.FullId%>" onMouseDown="activeDragId = this.id;">
	<div class="propertyForm" id="<%=this.FullId%>_form">
		<div style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px">
			<div id="edit<%=this.ClientID%>" ondblclick="expandCollapse('<%=this.ClientID%>');" style="PADDING-RIGHT: 0px; DISPLAY: none; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px"><h3 style="PADDING-RIGHT: 1px; PADDING-LEFT: 1px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 1px">
					<asp:ImageButton ID="DeleteButton" Runat="server"></asp:ImageButton><a href="javascript:expandCollapse('<%=this.ClientID%>');"><img src="<%=umbraco.GlobalSettings.Path%>/images/collapse.png" id="<%=this.ClientID%>_fold" style="FLOAT: right">
						Edit "<asp:Literal ID="Header" Runat="server"></asp:Literal>"</a></h3>
			</div>
			<div id="desc<%=this.ClientID%>" ondblclick="expandCollapse('<%=this.ClientID%>'); document.getElementById('<%=this.ClientID%>_tbName').focus();" style="PADDING-RIGHT: 0px; DISPLAY: block; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px"><h3 style="PADDING-RIGHT: 1px; PADDING-LEFT: 1px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 1px"><asp:ImageButton ID="DeleteButton2" Runat="server"></asp:ImageButton><a href="javascript:expandCollapse('<%=this.ClientID%>');"><img src="<%=umbraco.GlobalSettings.Path%>/images/expand.png" style="FLOAT: right">
						<asp:Literal ID="FullHeader" Runat="server"></asp:Literal></a></h3>
			</div>
			<table id="form" runat="server">
				<tr>
					<th>
						Name</th>
					<td>
						<asp:TextBox id="tbName" runat="server" CssClass="propertyFormInput"></asp:TextBox></td>
				</tr>
				<tr>
					<th>
						Alias</th>
					<td>
						<asp:TextBox id="tbAlias" runat="server" CssClass="propertyFormInput"></asp:TextBox></td>
				</tr>
				<tr>
					<th>
						Type</th>
					<td>
						<asp:DropDownList id="ddlTypes" runat="server" CssClass="propertyFormInput" BorderColor="#ffffff"></asp:DropDownList></td>
				</tr>
				<tr>
					<th>
						Tab</th>
					<td>
						<asp:DropDownList id="ddlTab" runat="server" CssClass="propertyFormInput"></asp:DropDownList></td>
				</tr>
				<tr>
					<th>
						Mandatory</th>
					<td>
						<asp:CheckBox id="checkMandatory" runat="server"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<th>
						Validation</th>
					<td>
						<asp:TextBox id="tbValidation" runat="server" CssClass="propertyFormInput"></asp:TextBox></td>
				</tr>
				<tr>
					<th>
						Description</th>
					<td>
						<asp:TextBox id="tbDescription" runat="server" CssClass="propertyFormInput" TextMode="MultiLine"
							Height="42px"></asp:TextBox></td>
				</tr>
			</table>
		</div>
	</div>
</li>

<%@ Control Language="c#" AutoEventWireup="True" Codebehind="nodeType.ascx.cs" Inherits="umbraco.cms.presentation.create.controls.nodeType" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="MARGIN-TOP: 25px"><%=umbraco.ui.Text("name")%>:<br />
</div>
<asp:TextBox id="rename" Runat="server" Width="280"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator1" ErrorMessage="*" ControlToValidate="rename" runat="server">*</asp:RequiredFieldValidator><br />
<!-- added to support missing postback on enter in IE -->
<asp:TextBox runat="server" style="visibility:hidden;display:none;" ID="Textbox1"/>
<p style="margin-top: 5px; margin-left: -3px;">
<asp:CheckBox ID="createTemplate" Runat="server" Text="Create matching template"></asp:CheckBox>
</p>
<div style="MARGIN-TOP: 28px; TEXT-ALIGN: right">
	<asp:Button id="sbmt" Runat="server" style="MARGIN-TOP: 14px" Width="90" onclick="sbmt_Click"></asp:Button>
<input type="button" value="<%=umbraco.ui.Text("cancel")%>" onClick="if (confirm('<%=umbraco.ui.Text("areyousure")%>')) window.close()" style="margin-top: 14px; width: 90px; margin-left: 6px;"/>
</div>

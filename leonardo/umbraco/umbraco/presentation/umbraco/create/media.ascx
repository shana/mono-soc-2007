<%@ Control Language="c#" AutoEventWireup="True" Codebehind="media.ascx.cs" Inherits="umbraco.cms.presentation.create.controls.media" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%=umbraco.ui.Text("choose")%> <%=umbraco.ui.Text("media")%> <%=umbraco.ui.Text("type")%>:<br />
<asp:ListBox id="nodeType" Runat="server" Width="280" Rows="1" SelectionMode="Single"></asp:ListBox><br />
<div style="MARGIN-TOP: 10px"><%=umbraco.ui.Text("name")%>:<br />
</div>
<asp:TextBox id="rename" Runat="server" Width="280"></asp:TextBox>
<asp:RequiredFieldValidator id="RequiredFieldValidator1" ErrorMessage="*" ControlToValidate="rename" runat="server">*</asp:RequiredFieldValidator><br />
<!-- added to support missing postback on enter in IE -->
<asp:TextBox runat="server" style="visibility:hidden;display:none;" ID="Textbox1"/>
<div style="MARGIN-TOP: 15px; TEXT-ALIGN: right">
	<asp:Button id="sbmt" Runat="server" style="MARGIN-TOP: 14px" Width="90" onclick="sbmt_Click"></asp:Button>
<input type="button" value="<%=umbraco.ui.Text("cancel")%>" onClick="if (confirm('<%=umbraco.ui.Text("areyousure")%>')) window.close()" style="margin-top: 14px; width: 90px; margin-left: 6px;"/>
</div>

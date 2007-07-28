<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="script.ascx.cs" Inherits="umbraco.presentation.umbraco.create.script" %>
Choose a type:<br />
<input type="hidden" name="nodeType" value="-1">
<asp:ListBox id="scriptType" Runat="server" Width="190" Rows="1" SelectionMode="Single">
	<asp:ListItem Selected="True" Value="">Folder</asp:ListItem>
</asp:ListBox>
<br />
<div style="MARGIN-TOP: 10px">Name:<br />
</div>
<asp:TextBox id="rename" Runat="server" Width="280"></asp:TextBox>
<!-- added to support missing postback on enter in IE -->
<asp:TextBox runat="server" style="visibility:hidden;display:none;" ID="Textbox1"/>
<asp:RequiredFieldValidator id="RequiredFieldValidator1" ErrorMessage="*" ControlToValidate="rename" runat="server">*</asp:RequiredFieldValidator><br />

<div style="MARGIN-TOP: 15px; TEXT-ALIGN: right">
<asp:Button id="sbmt" Runat="server" style="MARGIN-TOP: 14px" Width="90"></asp:Button>
<input type="button" value="<%=umbraco.ui.Text("cancel")%>" onClick="if (confirm('<%=umbraco.ui.Text("areyousure")%>')) window.close()" style="margin-top: 14px; width: 90px; margin-left: 6px;"/>
</div>
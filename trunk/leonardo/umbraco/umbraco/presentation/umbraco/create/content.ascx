<%@ Control Language="c#" AutoEventWireup="True" Codebehind="content.ascx.cs" Inherits="umbraco.cms.presentation.create.controls.content" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:literal id="typeJs" runat="server"/>
<div style="MARGIN-TOP: 10px"><%=umbraco.ui.Text("name")%>:<br />
</div>
<asp:TextBox  id="rename" Runat="server" CssClass="bigInput"></asp:TextBox><br /><br />
<asp:RequiredFieldValidator id="RequiredFieldValidator1" ErrorMessage="*" ControlToValidate="rename" runat="server">*</asp:RequiredFieldValidator>
<span style="margin-left: -7px;"><%=umbraco.ui.Text("choose")%> <%=umbraco.ui.Text("documentType")%>:<br /></span>
<asp:ListBox id="nodeType" Runat="server" cssClass="bigInput" Rows="1" SelectionMode="Single"></asp:ListBox><br />
<!-- added to support missing postback on enter in IE -->
<asp:TextBox runat="server" style="visibility:hidden;display:none;" ID="Textbox1"/>
<div id="typeDescription" class="createDescription">
<asp:Literal ID="descr" runat="server"></asp:Literal>
</div>
<br />
<div style="margin-right: 15px; TEXT-ALIGN: right">
	<asp:Button id="sbmt" Runat="server" style="MARGIN-TOP: 14px" Width="90" onclick="sbmt_Click"></asp:Button>
<input type="button" value="<%=umbraco.ui.Text("cancel")%>" onClick="if (confirm('<%=umbraco.ui.Text("areyousure")%>')) window.close()" style="margin-top: 14px; width: 90px; margin-left: 6px;"/>
</div>

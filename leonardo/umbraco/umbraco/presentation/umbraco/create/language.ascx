<%@ Control Language="c#" AutoEventWireup="True" Codebehind="language.ascx.cs" Inherits="umbraco.cms.presentation.create.controls.language" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<input type="hidden" name="nodeType">
<div style="MARGIN-TOP: 25px"><%=umbraco.ui.Text("choose")%> <%=umbraco.ui.Text("language")%>:<br />
</div>
<asp:DropDownList ID="Cultures" runat="server" Width="290px"></asp:DropDownList><br />
<div style="MARGIN-TOP: 48px; TEXT-ALIGN: right">
	<asp:Button id="sbmt" Runat="server" style="MARGIN-TOP: 14px" Width="90" onclick="sbmt_Click"></asp:Button>
<input type="button" value="<%=umbraco.ui.Text("cancel")%>" onClick="if (confirm('<%=umbraco.ui.Text("areyousure")%>')) window.close()" style="margin-top: 14px; width: 90px; margin-left: 6px;"/>
</div>

<%@ Page language="c#" Codebehind="AssignDomain.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.AssignDomain" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - <%=umbraco.ui.Text("defaultdialogs", "assignDomain", this.getUser())%> &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp; </title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>body {margin: 2px;}</style>
		<script language="javascript">
function doSubmit() {document.Form1["ok"].click()}

	var functionsFrame = this;
	var tabFrame = this;
	var isDialog = true;
	var submitOnEnter = true;
	
		</script>
		<script type="text/javascript" src="../js/umbracoCheckKeys.js"></script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
		<input type="hidden" name="domainId" value="<%=umbraco.helper.Request("editDomain")%>"/>
		<div style="float: right; padding: 3px;"><a href="javascript:window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div><h3><img src="../images/domain.gif" align="absmiddle"/> <%=umbraco.ui.Text("defaultdialogs", "assignDomain", this.getUser())%></h3>
		<img class="gradient" style="width: 100%; height: 1px; margin-top: 7px;" src="../images/nada.gif"/><br/>
		<asp:Literal ID="FeedBackMessage" Runat="server"/>
		<br/>
		<span style="margin-left: 10px;" class="guiDialogNormal"><strong><img src="../images/newStar.gif" align="absmiddle"/> <%=umbraco.ui.Text("assignDomain", "addNew", this.getUser())%>:<br/></strong></span>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server">
				<TBODY>
					<TR>
						<TD class="propertyHeader" width="30%"><%=umbraco.ui.Text("assignDomain", "domain", this.getUser())%></TD>
						<TD class="propertyContent">
							<asp:RequiredFieldValidator ControlToValidate="Languages" ErrorMessage="*" ID="DomainValidator" Runat="server" Display="Dynamic"/>
							<asp:TextBox id="DomainName" runat="server" Width="252px"></asp:TextBox>&nbsp;
						</td>
					</TR>
					<TR>
						<TD class="propertyHeader" width="30%"><%=umbraco.ui.Text("general", "language", this.getUser())%></TD>
						<TD class="propertyContent">
						<asp:RequiredFieldValidator ControlToValidate="Languages" ErrorMessage="*" ID="LanguageValidator" Runat="server" Display="Dynamic"/>
							<asp:DropDownList ID="Languages" Runat="server"/> 
							<br/>
							<br/><asp:Button ID="ok" Runat="server" CssClass="guiInputButton" onclick="SaveDomain"></asp:Button> &nbsp; <input type="button" onclick="if (confirm('<%=umbraco.ui.Text("defaultdialogs", "confirmSure", this.getUser())%>')) window.close()" value="<%=umbraco.ui.Text("general", "cancel", this.getUser())%>" style="width: 60px"/>
						</td>
					</TR>
				</tbody>
			</table>
							
<br/>
		<span style="margin-left: 10px;" class="guiDialogNormal"><strong>...<%=umbraco.ui.Text("assignDomain", "orEdit", this.getUser())%>:<br/></strong></span>
			<TABLE class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server" ID="Table1">
				<TBODY>
					<TR>
						<TD class="propertyHeader" colspan="2">
						<asp:Literal ID="allDomains" Runat="server"/>
						</TD>
					</TR>
				</tbody>
			</table>
				
		</FORM>
	</body>
</HTML>

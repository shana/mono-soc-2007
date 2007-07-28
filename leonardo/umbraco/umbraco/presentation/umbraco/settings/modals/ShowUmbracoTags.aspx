<%@ Page language="c#" Codebehind="ShowUmbracoTags.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.settings.modal.ShowUmbracoTags" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ShowUmbracoTags</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../../css/umbracoGui.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:parent.window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
		<h3 style="margin-left: -1px">Quick Guide to umbraco template tags</h3>
		<hr size="1" noshade="true">
			<table>
				<tr>
					<td>
						<h3 style="margin-left: -1px;">Insert field syntax (GetItem)</h3>
						Insert field sample:
					</td>
				</tr>
				<tr>
					<td>
						<textarea style="WIDTH:450px;HEIGHT:35px">&lt;?UMBRACO_GETITEM field="pageName"/&gt;</textarea>
					</td>
				</tr>
				<tr>
					<td>
						<a href="http://umbraco.org/redir/documentation/reference/umbracogetitem.aspx" target="_blank"><img src="../../images/forward.png" align="absmiddle"> Full reference for GetItem tag</a>
					</td>
				</tr>
				<tr>
					<td>
						<h3 style="margin-left: -1px;">Insert macro syntax</h3>
						Insert macro sample
					</td>
				</tr>
				<tr>
					<td>
						<textarea style="WIDTH:450px;HEIGHT:35px">&lt;?UMBRACO_MACRO macroID="5"/&gt;</textarea>
					</td>
				</tr>
				<tr>
					<td>
						<a href="http://umbraco.org/redir/documentation/reference/umbracomacro.aspx"><img src="../../images/forward.png" align="absmiddle"> Full reference for Macro tag</a>
					</td>
				</tr>
				<tr>
					<td>
						<h3 style="margin-left: -1px;">Load child template syntax</h3>
						Insert "load child template" sample
					</td>
				</tr>
				<tr>
					<td>
						<textarea style="WIDTH:450px;HEIGHT:35px">&lt;?UMBRACO_TEMPLATE_LOAD_CHILD/&gt;</textarea>
					</td>
				</tr>
				<tr>
					<td>
						<a href="http://umbraco.org/redir/documentation/reference/umbracotemplateloadchild.aspx"><img src="../../images/forward.png" align="absmiddle"> Full reference for TemplateLoadChild tag</a>
					</td>
				</tr>
				<tr>
					<td>
						<h3 style="margin-left: -1px;">ASP.NET form</h3>
						Insert ASP.NET formular sample
					</td>
				</tr>
				<tr>
					<td>
						<textarea style="WIDTH:450px;HEIGHT:35px">&lt;?ASPNET_FORM&gt;&lt;/?ASPNET_FORM&gt;</textarea>
					</td>
				</tr>
				<tr>
					<td>
						<a href="http://umbraco.org/redir/documentation/reference/aspnetform.aspx"><img src="../../images/forward.png" align="absmiddle"> Full reference for AspNetForm</a>
					</td>
				</tr>
				<tr>
					<td>
						<h3 style="margin-left: -1px;">ASP.NET head</h3>
						Insert ASP.NET head sample
					</td>
				</tr>
				<tr>
					<td>
						<textarea style="WIDTH:450px;HEIGHT:35px">&lt;?ASPNET_HEAD&gt;&lt;/?ASPNET_HEAD&gt;</textarea>
					</td>
				</tr>
				<tr>
					<td>
					</td>
				</tr>
				<tr>
					<td>
						<a href="http://umbraco.org/redir/documentation/reference/aspnethead.aspx"><img src="../../images/forward.png" align="absmiddle"> Full reference for AspNetHead</a>
					</td>
				</tr>
				<tr>
					<td>
						<h3 style="margin-left: -1px;">MetaBlogApi / Content Channels</h3>
						Insert the following two elements to the head element to gain optimal support for using the MetaBlog Apis
						with 3rd party clients and to enable autodiscovery for Windows Live Writer
					</td>
				</tr>
				<tr>
					<td>
						<textarea style="WIDTH:450px;HEIGHT:35px">    &lt;link rel="EditURI" type="application/rsd+xml" 
        href="http://umbraco/umbraco/channels/rsd.aspx" /&gt;
    &lt;link rel="wlwmanifest" type="application/wlwmanifest+xml" 
        href="http://umbraco/umbraco/channels/wlwmanifest.aspx" /&gt;</textarea>
					</td>
				</tr>
				<tr>
					<td>
					</td>
				</tr>
				<tr>
					<td>
						<a href="http://umbraco.org/redir/documentation/reference/contentChannels.aspx"><img src="../../images/forward.png" align="absmiddle"> Full reference for AspNetHead</a>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>

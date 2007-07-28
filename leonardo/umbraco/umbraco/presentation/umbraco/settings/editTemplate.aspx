<%@ Page language="c#" Codebehind="editTemplate.aspx.cs" validateRequest="false" AutoEventWireup="True" Inherits="umbraco.cms.presentation.settings.editTemplate" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>editMacro</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script type="text/javascript" src="../js/xbf.js"></script>
		<script src="../js/prototype.js" type="text/javascript"></script>
    	<script src="../js/scriptaculous/scriptaculous.js" type="text/javascript"></script>
		
		<script language="javascript" type="text/javascript">
        function doSubmit() {
        
        umbraco.presentation.webservices.codeEditorSave.SaveTemplate($F('NameTxt'), $F('AliasTxt'), editorSource.getCode(), '<%= Request.QueryString["templateID"] %>', $F('MasterTemplate'), submitSucces, submitFailure);
        }
        
        function submitSucces(t)
        {
            if(t != 'true')
            {
                top.umbSpeechBubble('error', 'Saving template', '');
            }
            else
            {
                top.umbSpeechBubble('save', 'Template saved', '')
            }
            
        }
        function submitFailure(t)
        {
            alert('FAILURE' + t.responseText);
        }
        
            	
    	
		function resizeTextArea() {
		var clientHeight = self.innerHeight;
		var clientWidth = self.innerWidth;
		
		if (clientHeight == null) {
			clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
			clientWidth = document.body.clientWidth;
			}
			if(document.getElementById('editorSource') != null)
			{
			document.getElementById('editorSource').style.width = (clientWidth - 60)+"px";
			document.getElementById('editorSource').style.height = (clientHeight - 180)+"px" ;
			}
			else
			{
			    document.getElementById('editorSource_cp').style.width = (clientWidth - 90)+"px";
			    document.getElementById('editorSource_cp').style.height = (clientHeight - 180)+"px" ;
			}
		}
		</script>
	</HEAD>
	<body onLoad="resizePanel('Panel1',true);resizeTextArea()" onResize="resizePanel('Panel1',true);resizeTextArea();"
		bgColor="#f2f2e9">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="../webservices/codeEditorSave.asmx" />
            </Services>
            </asp:ScriptManager>
			<cc1:UmbracoPanel id="Panel1" runat="server" Width="608px" Height="336px" hasMenu="true" style="TEXT-ALIGN:center">
				<CC2:Pane id="Pane7" style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px; TEXT-ALIGN: left"
					runat="server" Height="44px" Width="528px">
					<TABLE id="Table1" width="100%">
						<TR>
							<TH width="45">
								<%=umbraco.ui.Text("name", base.getUser())%>
							</TH>
							<TD>
								<ASP:TEXTBOX id="NameTxt" Runat="server"></ASP:TEXTBOX></TD>
						</TR>
						<TR>
							<TH width="45">
								<%=umbraco.ui.Text("alias", base.getUser())%>
							</TH>
							<TD>
								<ASP:TEXTBOX id="AliasTxt" Runat="server"></ASP:TEXTBOX></TD>
						</TR>
						<TR>
							<TH>
								<%=umbraco.ui.Text("mastertemplate", base.getUser())%>
							</TH>
							<TD>
								<ASP:DROPDOWNLIST id="MasterTemplate" Runat="server"></ASP:DROPDOWNLIST></TD>
						</TR>
						<TR>
							<TH colSpan="2">
								<%=umbraco.ui.Text("template", base.getUser())%>
								:<BR>
								  <asp:TextBox ID="editorSource" runat="server" CssClass="codepress html" TextMode="MultiLine" Height="178px" Width="216px" />
						    </TH></TR>
					</TABLE>
				</CC2:Pane>
			</cc1:UmbracoPanel>
		</form>
		<asp:Literal ID="editorJs" runat="server"></asp:Literal>
	</body>
</HTML>

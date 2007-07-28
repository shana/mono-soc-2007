<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editScript.aspx.cs" Inherits="umbraco.cms.presentation.settings.scripts.editScript" ValidateRequest="False" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>scripteditor</title>
		<LINK href="../../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script src="../../js/prototype.js" type="text/javascript"></script>
    	<script src="../../js/scriptaculous/scriptaculous.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">

        function doSubmit() {
        
         umbraco.presentation.webservices.codeEditorSave.SaveScript($F('NameTxt'), Content.getCode(), submitSucces, submitFailure);
        }
        
        function submitSucces(t)
        {
            if(t != 'true')
            {
                top.umbSpeechBubble('error', 'Saving the file failed', t);
            }
            else
            {
                top.umbSpeechBubble('save', 'Script saved', '')
            }
            
        }
        function submitFailure(t)
        {
            alert('FAILURE' + t);
        }
        
            	
    	
		function resizeTextArea() {
		var clientHeight = self.innerHeight;
		var clientWidth = self.innerWidth;
		
		if (clientHeight == null) {
			clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
			clientWidth = document.body.clientWidth;
			}
			if(document.getElementById('Content') != null)
			{
			document.getElementById('Content').style.width = (clientWidth - 60)+"px";
			document.getElementById('Content').style.height = (clientHeight - 130)+"px" ;
			}
			else
			{
			    document.getElementById('Content_cp').style.width = (clientWidth - 90)+"px";
			    document.getElementById('Content_cp').style.height = (clientHeight - 130)+"px" ;
			}
		}
		</script>
	</HEAD>
	<body onLoad="resizePanel('Panel1',true);resizeTextArea()" onResize="resizePanel('Panel1',true);resizeTextArea();"
		bgColor="#f2f2e9">
		
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="../../webservices/codeEditorSave.asmx" />
            </Services>
            </asp:ScriptManager>
			<cc1:UmbracoPanel id="Panel1" runat="server" Width="608px" Height="336px" hasMenu="true" style="TEXT-ALIGN:center">
				<cc2:Pane id="Pane7" style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px; TEXT-ALIGN: left"
					runat="server" Height="44px" Width="528px">
					<TABLE id="Table1" width="100%">
						<TR>
							<TH width="45">
								<%=umbraco.ui.Text("name", base.getUser())%>
							</TH>
							<TD>
								<asp:TextBox id="NameTxt" Runat="server"></asp:TextBox>
								<asp:Literal id="lttPath" Runat="server"></asp:Literal></TD>
						</TR>
						<TR>
							<TH colSpan="2">
								<%=umbraco.ui.Text("contents", base.getUser())%>
								:<BR>
								<asp:TextBox ID="Content" TextMode="MultiLine" runat="server" Height="178px" Width="216px"></asp:TextBox>
								</TH></TR>
					</TABLE>
		      </cc2:Pane>
			</cc1:UmbracoPanel>
		</form>
		
		<asp:Literal ID="editorJs" runat="server"></asp:Literal>
	</body>
</HTML>
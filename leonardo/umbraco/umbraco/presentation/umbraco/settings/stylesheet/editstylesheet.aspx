<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>

<%@ Page Language="c#" Codebehind="editstylesheet.aspx.cs" AutoEventWireup="True"
    Inherits="umbraco.cms.presentation.settings.stylesheet.editstylesheet" ValidateRequest="False" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>editstylesheet</title>
    <script src="../../js/prototype.js" type="text/javascript"></script>

    <script src="../../js/scriptaculous/scriptaculous.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
		
	    
        function doSubmit() {
            umbraco.presentation.webservices.codeEditorSave.SaveCss($F('NameTxt'), editorSource.getCode(), '<%= Request.QueryString["id"] %>', submitSucces, submitFailure);
  }
        
        function submitSucces(t)
        {
            if(t != 'true')
            {
                top.umbSpeechBubble('error', 'Saving the stylesheet failed', t);
            }
            else
            {
                top.umbSpeechBubble('save', 'Stylesheet saved', '')
            }
            
        }
        function submitFailure(t)
        {
            top.speechBubble(speechBubbleIcon.error, "Error saving stylesheet", t);
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

</head>
<body onload="resizePanel('Panel1',true);resizeTextArea()" onresize="resizePanel('Panel1',true);resizeTextArea();" class="editStyleSheet">
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="../../webservices/codeEditorSave.asmx" />
            </Services>
        </asp:ScriptManager>
        <cc1:UmbracoPanel ID="Panel1" runat="server" CssClass="panel" hasMenu="true">
            <cc2:Pane ID="Pane7" CssClass="pane" runat="server">
                <fieldset>
                <label for="NameTxt"><%=umbraco.ui.Text("name", base.getUser())%></label>
                <asp:TextBox ID="NameTxt" runat="server"></asp:TextBox>
                <span class="path"><asp:Literal ID="lttPath" runat="server"></asp:Literal></span>
                
                <label><%=umbraco.ui.Text("styles", base.getUser())%>:</label><br/>
                <asp:TextBox CssClass="codepress css" TextMode="MultiLine" Height="178px" Width="216px" ID="editorSource" runat="server"></asp:TextBox>
                </fieldset>
            </cc2:Pane>
        </cc1:UmbracoPanel>
    </form>

		<asp:Literal ID="editorJs" runat="server"></asp:Literal>

</body>
</html>

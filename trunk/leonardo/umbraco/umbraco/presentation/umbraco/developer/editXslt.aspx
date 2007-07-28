<%@ Page validateRequest="false" language="c#" Codebehind="editXslt.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.developer.editXslt" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="cc2" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>editXslt</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script src="../js/xbf.js" type="text/javascript"></script>
		<script src="../js/prototype.js" type="text/javascript"></script>
    	<script src="../js/scriptaculous/scriptaculous.js" type="text/javascript"></script>
		
		<style>
		#errorDiv{
		    color: red;
		    border: 1px solid red;
		    padding: 5px;
		    font-size: 11px;
		}
		
		#errorDiv a{float: right; color: #000;}
		</style>
		
		<script language="javascript" type="text/javascript">
		
	    function closeErrorDiv()
	    {
	        document.getElementById("errorDiv").style.display = 'none';
	    }
	    
        function doSubmit() {
        closeErrorDiv();
        umbraco.presentation.webservices.codeEditorSave.SaveXslt($F('xsltFileName'), editorSource.getCode(), document.getElementById("SkipTesting").checked, submitSucces, submitFailure);
        }
        
        function submitSucces(t)
        {
            if(t != 'true')
            {
                top.umbSpeechBubble('error', 'Saving Xslt file failed', '');
                document.getElementById("errorDiv").innerHTML = '<a href="#" onclick=\'closeErrorDiv()\'>Hide Errors</a><strong>Error occured</strong><br/>' + t;
                //$F('errorDiv').update('test');
                //$F('errorDiv').update(t.responseText);
                Effect.BlindDown('errorDiv');
            }
            else
            {
                top.umbSpeechBubble('save', 'Xslt file saved', '')
                //alert("else" + t.responseText);
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
			if(document.getElementById('editorSource') != null)
			{
			document.getElementById('editorSource').style.width = (clientWidth - 60)+"px";
			document.getElementById('editorSource').style.height = (clientHeight - 210)+"px" ;
			}
			else
			{
			    document.getElementById('editorSource_cp').style.width = (clientWidth - 90)+"px";
			    document.getElementById('editorSource_cp').style.height = (clientHeight - 220)+"px" ;
			}
		}
		</script>
	</head>
	<body onresize="resizePanel('UmbracoPanel1',true);resizeTextArea();" onload="resizePanel('UmbracoPanel1',true);resizeTextArea();">
		<form id="contentForm" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="../webservices/codeEditorSave.asmx" />
            </Services>
            </asp:ScriptManager>
			<cc1:umbracopanel id="UmbracoPanel1" runat="server" Text="Edit xsl" height="300" width="500">
				<cc2:Pane id="Pane1" runat="server">
					<table cellSpacing="0" cellPadding="4" width="100%" border="0">
						<tr>
							<th width="30%">
								Filename</th>
							<td class="propertyContent">
								<asp:TextBox id="xsltFileName" runat="server" Width="400" cssClass="guiInputText"></asp:TextBox></td>
						</tr>
						<tr>
							<th width="30%">
								Skip testing (ignore errors)</th>
							<td class="propertyContent">
								<asp:CheckBox id="SkipTesting" Runat="server"></asp:CheckBox></td>
						</tr>
					</table>
					<table cellSpacing="0" cellPadding="4" width="100%" border="0">
					    <tr>
							<td>
								<asp:Literal id="closeErrorMessage" Runat="server" Visible="false">
									<A id="showErrorLink" href="javascript:showError()"><b>Hide error</b> <img src="../images/arrowDown.gif" align="absmiddle" border="0"/></A></asp:Literal>
								
								<asp:Panel id="errorHolder" Runat="server" Visible="False">
									<SPAN style="COLOR: red">
									<asp:Label id="xsltError" Runat="server"></asp:Label></SPAN></asp:Panel>
								<div id="errorDiv" style="display: none;">hest</div>
									
							    <b>Source:</b><br />
								<asp:TextBox TextMode="MultiLine" Height="400" Width="100%" runat="server" CssClass="codepress xslt" ID="editorSource" />
							
					
								</td>
						</tr>
					</table>
				</cc2:Pane>
			</cc1:umbracopanel></form>
		<asp:Literal ID="editorJs" runat="server"></asp:Literal>
	</body>
</html>

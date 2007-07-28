<%@ Control Language="c#" AutoEventWireup="True" Codebehind="upgradeIndex.ascx.cs" Inherits="umbraco.presentation.install.steps.upgradeIndex" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript" src="../umbraco/webservices/ajax.js"></script>
<script type="text/javascript" src="../umbraco/webservices/GetJavaScriptProxy.aspx?service=progressStatus.asmx"></script>
<script>
	var total = <asp:Literal id="total" runat="server"/>;
	
		function showStatus() {
			document.getElementById('formDiv').style.display = 'none'; 
			document.getElementById('animDiv').style.display = 'block'; 
			document.getElementById('anim').src = '../umbraco/images/anims/publishPages.gif';
		}
		
		function doAjax() {
			setTimeout("showStatus()", 100);
			setTimeout("updateStatus()", 200);
		}
		
		function updateStatus() {
			proxies.progressStatus.GetStatus.func = updateStatusDo;
			proxies.progressStatus.GetStatus('cmsXmlDone');
		}
		
		
		function updateStatusDo(retVal) {

			progressBarUpdateLabel('_ctl0_upgradeStatus', "<b>" + retVal + " <%=umbraco.ui.Text("of")%> " + total + "</b>");
			
			// progressbar
			retVal = parseInt(retVal);
			if (retVal > 0) {
				var percent = Math.round((retVal/total)*100);
				progressBarUpdate('_ctl0_upgradeStatus', percent);
			}
			setTimeout("updateStatus()", 500);
		}
</script>
<h1>Step 1a: Upgrade indexes</h1>
<p>umbraco 2.1 uses a new enhanced publishing routine with great improvements in 
	both speed and stability. With 2.1 you can be confident that you'll wave 
	goodbye to any publishing nightmares.<br />
	<br />
	<asp:Panel ID="form" Runat="server"></p>
<DIV id="animDiv" style="DISPLAY: none; WIDTH: 100%; TEXT-ALIGN: center"><SPAN style="BORDER-RIGHT: #666 1px solid; PADDING-RIGHT: 5px; BORDER-TOP: #666 1px solid; PADDING-LEFT: 5px; BACKGROUND: white; PADDING-BOTTOM: 5px; BORDER-LEFT: #666 1px solid; WIDTH: 240px; PADDING-TOP: 5px; BORDER-BOTTOM: #666 1px solid; TEXT-ALIGN: center"><IMG id="anim" height="42" alt="Re-Indexing in progress" src="../umbraco/images/anims/publishPages.gif"
			width="150"><br />
		<SCRIPT>
		umbPgStep = 1;
		umbPgIgnoreSteps = true;
		</SCRIPT>
		<SPAN class="guiDialogTiny" style="TEXT-ALIGN: center">Re-indexing in progress</SPAN>
		<br />
		<br />
		<asp:PlaceHolder id="progressBar" Runat="server"></asp:PlaceHolder></SPAN></DIV>
<DIV id="formDiv">Press "Upgrade Index" to finish the upgrade.<br/><br/>
	<P>
		<asp:Button id="Button1" Text="Upgrade Index" runat="server" onclick="Button1_Click"></asp:Button></P>
</DIV>
</asp:Panel>
<asp:Panel ID="result" Runat="server" Visible="False">
Upgrading index 
done!<br />
</asp:Panel>

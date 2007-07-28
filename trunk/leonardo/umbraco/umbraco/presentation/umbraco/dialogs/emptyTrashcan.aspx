<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="emptyTrashcan.aspx.cs" Inherits="umbraco.presentation.dialogs.emptyTrashcan" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
		<title>umbraco - <%=umbraco.ui.Text("actions", "emptyTrashcan", this.getUser())%> &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp;  &nbsp; </title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>body {margin: 2px;}</style>
		<script>
		
		    var emptyTotal = '<%= umbraco.cms.businesslogic.RecycleBin.Count().ToString()%>';
		    
		    function emptyRecycleBin() {
    			$get('formDiv').style.display = 'none'; 
	    		$get('animDiv').style.display = 'block'; 
		    	$get('anim').src = '../images/anims/emptyTrashcan.gif';
		    	
		    	// call the empty trashcan webservice
		    	umbraco.presentation.webservices.trashcan.EmptyTrashcan();

                // wait one second to start the status update
                setTimeout('updateStatus();', 1000);
		    }
		    
		    function updateStatus() {
		        umbraco.presentation.webservices.trashcan.GetTrashStatus(updateStatusLabel, failure);
		    }
		    
		    function failure(retVal) {
		        alert('error: ' + retVal);
		    }
		    
		    function updateStatusLabel(retVal) {
                $get('statusLabel').innerHTML =  "<strong>" + retVal + " <%=umbraco.ui.Text("remaining")%></strong>";            

                if (retVal != '' && retVal != '0') {
                    setTimeout('updateStatus();', 500);
                } else {
                    $get('anim').style.display = 'none';
                    $get('statusLabel').innerHTML =  "<br /><br /><h3><%=umbraco.ui.Text("defaultdialogs", "recycleBinIsEmpty")%></h3>";
                }
                
		    }
		</script>
</head>
<body>
    <form runat="server" id="form1">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    <Services>
        <asp:ServiceReference Path="../webservices/trashcan.asmx" />
    </Services>
    </asp:ScriptManager>
    
		<div style="float: right; padding: 3px;"><a href="javascript:window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
		
		<h3><img src="../images/tree/bin_empty.png" align="absmiddle"/> <%=umbraco.ui.Text("actions", "emptyTrashcan", this.getUser())%></h3>
		<img class="gradient" style="width: 100%; height: 1px; margin-top: 7px;" src="../images/nada.gif"/><br/>
		<asp:Literal ID="FeedBackMessage" Runat="server"/>
		<br/>
		
		
		<div id="animDiv" style="display: none; width: 100%; text-align: center">
		
		<div style="width: 240px; padding: 5px; text-align: center; background: white; border: 1px solid #666; margin: auto;">
		
		<img src="../images/anims/emptyTrashcan.gif" id="anim" width="150" height="42" alt="<%=umbraco.ui.Text("actions", "emptyTrashcan", this.getUser())%>"/>
		<br/>
		
		<div align="center">
		<span class="guiDialogTiny" id="statusLabel"><%=umbraco.ui.Text("deleting", this.getUser())%></span>
		<br/><br/>
		</div>
		
		</div>
		
		</div>
<asp:Panel ID="TheForm" Visible="True" Runat="server">
		<div id="formDiv" style="visibility: visible;">
			<table class="propertyPane" cellSpacing="0" cellPadding="4" width="98%"
				border="0" runat="server" ID="Table1">
				<tbody>
					<tr>
						<td class="propertyHeader" colspan="2">
		<input type="checkbox" id="confirmDelete" onclick="$get('ok').disabled = !this.checked;" /> <label for="confirmDelete"><%=umbraco.ui.Text("defaultdialogs", "confirmEmptyTrashcan", umbraco.cms.businesslogic.RecycleBin.Count().ToString(), this.getUser())%></label>
							<br />
							<br /><input type="button" ID="ok" value="<%=umbraco.ui.Text("actions", "emptyTrashcan", this.getUser()) %>" class="guiInputButton" onclick="if ($get('confirmDelete').checked) {emptyRecycleBin();}" disabled="true" />  &nbsp; <input type="button" onclick="window.close()" value="<%=umbraco.ui.Text("general", "cancel", this.getUser())%>" style="width: 60px"/>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
</asp:Panel>				
		</form>
	</body>
</html>

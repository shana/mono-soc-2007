<%@ Page language="c#" Codebehind="uploadImage.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.uploadImage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco ::
			<%=umbraco.ui.Text("defaultdialogs", "upload", this.getUser())%>
		</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>BODY { MARGIN: 2px }
		</style>
		<script language="javascript">
		
			function validateImage() {
				// Disable save button
				document.getElementById("Button1").disabled = true;
				var imageTypes = ",jpeg,jpg,gif,bmp,png,tiff,tif,";
				
				var imageName = document.getElementById("uploadFile").value;
				if (imageName.length > 0) {
					var extension = imageName.substring(imageName.lastIndexOf(".")+1, imageName.length);
					if (imageTypes.indexOf(','+extension+',') > 0) {
						document.getElementById("Button1").disabled = false;
						if (document.getElementById("TextBoxTitle").value == "")
							document.getElementById("TextBoxTitle").value = imageName.substring(imageName.lastIndexOf("\\")+1, imageName.length);
					}
				}
			}
			
			function chooseId() {
				var treePicker = window.showModalDialog('<%=umbraco.GlobalSettings.Path%>/dialogs/treePicker.aspx?appAlias=media', 'treePicker', 'dialogWidth=350px;dialogHeight=300px;scrollbars=no;center=yes;border=thin;help=no;status=no')			
				if (treePicker != undefined) {
					document.getElementById("folderid").value = treePicker;
					if (treePicker > 0) {
						umbraco.presentation.webservices.CMSNode.GetNodeName('<%=umbraco.BasePages.BasePage.umbracoUserContextID%>', treePicker, updateContentTitle);
					}				
				}
			}			
			function updateContentTitle(result) {
				document.getElementById("mediaTitle").innerHTML = "<strong>" + result + "</strong>";
			}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services><asp:ServiceReference Path="../webservices/CMSNode.asmx" /></Services>
            </asp:ScriptManager>
			<asp:Panel id="Panel1" runat="server">
				<P>
					<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0">
						<TR>
							<TD class="propertyHeader" width="30%">
								<asp:Literal id="LiteralTitle" runat="server"></asp:Literal>:
							</TD>
							<TD class="propertyContent">
								<asp:TextBox id="TextBoxTitle" runat="server"></asp:TextBox>
								<P></P>
							</TD>
						</TR>
						<TR>
							<TD class="propertyHeader" width="30%">
								<asp:Literal id="LiteralUpload" runat="server"></asp:Literal>:
							</TD>
							<TD class="propertyContent">
								<asp:PlaceHolder id="PlaceHolder1" runat="server"></asp:PlaceHolder>
								<P></P>
							</TD>
						</TR>
						<TR>
							<TD class="propertyHeader" width="30%">
								<P class="guiDialogNormal">Save in folder:
								</P>
							</TD>
							<TD class="propertyContent"><SPAN id="mediaTitle">
									<asp:Literal id="FolderName" Runat="server"></asp:Literal></SPAN>&nbsp; <A href="javascript:chooseId()">
									<%=umbraco.ui.Text("choose")%>
									...</A><INPUT id="folderid" type="hidden" name="folderid" runat="server">
								<P></P>
								<P>
									<asp:Button id="Button1" runat="server" Enabled="False" Text="Button" onclick="Button1_Click"></asp:Button></P>
						</TR>
					</TABLE>
			</asp:Panel>
			<asp:Panel id="Panel2" runat="server" Visible="False">
				<div style="padding:10px;">
				<b><asp:literal ID="thumbnailtext" Runat="server"></asp:literal></b>
				<br /><br />
				<IMG id="uploadedimage" src="" runat="server">
				</div>
			</asp:Panel>
		</form>
		</P>
	</body>
</HTML>

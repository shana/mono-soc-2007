<%@ Page language="c#" Codebehind="PackagerCreate.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.PackagerCreate" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>umbraco - Create Package</title>
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<style>BODY { MARGIN: 2px }
		</style>
<!-- effect library -->
 		<script src="../js/prototype.js" type="text/javascript"></script>
		<script src="../js/effects2.js" type="text/javascript"></script>
		<script src="../js/dragdrop.js" type="text/javascript"></script>
		
		<script>
		function addFile() {
			new Effect2.BlindUp("fileAddContainer");
			setTimeout("new Effect2.BlindUp('fileAddContainer');", 2000);
						
		}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px; FLOAT: right; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><a href="javascript:window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div>
			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/package2.png', sizingMethod='scale')">
				Create Package</h3>
			<img class="gradient" style="MARGIN-TOP: 7px; MARGIN-BOTTOM: 5px; WIDTH: 100%; HEIGHT: 1px"
				src="../images/nada.gif"><br />
			<asp:Panel ID="TheForm" Runat="server" Visible="true">
				<SPAN class="guiDialogNormal" style="MARGIN-LEFT: 10px"><B>1. Package Details</B> -&gt; 
					2. Macros -&gt; 3. Files -&gt; 4. Commands -&gt; 5. Finish
					<br />
				</SPAN>
				<TABLE class="propertyPane" id="Table1" cellSpacing="0" cellPadding="4" width="98%" border="0"
					runat="server">
					<TR>
						<TD class="propertyContent" colSpan="2">
							<asp:Panel id="Panel1_Details" Runat="server" Visible="False">
								<TABLE class="propertyPane" id="Table2" cellSpacing="0" cellPadding="4" width="98%" border="0"
									runat="server">
									<TR>
										<TD class="propertyHeader" width="100">Package Title
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="TextBox_Title" Runat="server" Width="350px"></asp:TextBox></TD>
									</TR>
									<TR>
										<TD class="propertyHeader">Package Version
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="TextBox_Version_Major" Runat="server" Width="30px"></asp:TextBox>.
											<asp:TextBox id="TextBox_Version_Minor" Runat="server" Width="30px"></asp:TextBox>.
											<asp:TextBox id="TextBox_Version_Patch" Runat="server" Width="30px"></asp:TextBox></TD>
									</TR>
									<TR>
										<TD class="propertyHeader">Package Url
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="Textbox_Package_Url" Runat="server" Width="350px"></asp:TextBox></TD>
									</TR>
								</TABLE>
								<br />
								<TABLE class="propertyPane" id="Table3" cellSpacing="0" cellPadding="4" width="98%" border="0"
									runat="server">
									<TR>
										<TD class="propertyHeader" width="100">License
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="Textbox_License" Runat="server" Width="350px"></asp:TextBox></TD>
									</TR>
									<TR>
										<TD class="propertyHeader">License Url
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="Textbox_License_Url" Runat="server" Width="350px"></asp:TextBox></TD>
									</TR>
								</TABLE>
								<br />
								<TABLE class="propertyPane" id="Table4" cellSpacing="0" cellPadding="4" width="98%" border="0"
									runat="server">
									<TR>
										<TD class="propertyHeader" width="100">Author
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="Textbox_Author" Runat="server" Width="350px"></asp:TextBox></TD>
									</TR>
									<TR>
										<TD class="propertyHeader">Author Url
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="Textbox_Author_Url" Runat="server" Width="350px"></asp:TextBox></TD>
									</TR>
								</TABLE>
								<br />
								<TABLE class="propertyPane" id="Table5" cellSpacing="0" cellPadding="4" width="98%" border="0"
									runat="server">
									<TR>
										<TD class="propertyHeader" width="100">Requirements
										</TD>
										<TD class="propertyContent">
											<asp:DropDownList id="DropDownList_Requirements" Runat="server" Width="350px">
												<asp:ListItem Value="2.1.0">umbraco 2.1.0</asp:ListItem>
											</asp:DropDownList></TD>
									</TR>
									<TR>
										<TD class="propertyHeader" width="100">Remarks
										</TD>
										<TD class="propertyContent">
											<asp:TextBox id="Textbox1" Runat="server" Width="350px" TextMode="MultiLine" Height="150px"></asp:TextBox></TD>
									</TR>
								</TABLE>
							</asp:Panel>
							<asp:Panel id="Panel_Macros" Runat="server" Visible="false">
								<TABLE class="propertyPane" id="Table6" cellSpacing="0" cellPadding="4" width="98%" border="0"
									runat="server">
									<TR>
										<TD class="propertyHeader" width="100">Choose macros
										</TD>
										<TD class="propertyContent">
											<DIV style="BORDER-RIGHT: #ccc 1px solid; BORDER-TOP: #ccc 1px solid; BACKGROUND: #fff; OVERFLOW: auto; BORDER-LEFT: #ccc 1px solid; WIDTH: 300px; BORDER-BOTTOM: #ccc 1px solid; HEIGHT: 150px; scroll: auto">
												<asp:CheckBoxList id="macroList" Runat="server"></asp:CheckBoxList></DIV>
										</TD>
									</TR>
								</TABLE>
								<br />
							</asp:Panel>

							<asp:Panel id="Panel_Files" Runat="server" Visible="true">
								<div id="fileAddContainer" style="width: 450px; height: 80px">
								<b>File: &nbsp; &nbsp;</b><input type="file" id="fileFile" style="width: 250px;"><br/>
								<b>Destination: &nbsp; &nbsp;</b><input type="text" id="fileDest" style="width: 250px;"><br/>
											<input type="button" value=" Add " style="width: 100px" onclick="addFile()">
											</div>
								<br />
								<h3 style="margin-bottom: 5px;">Added files</h3>
								<span id="noFilesAdded" style="color: #999; margin-top: 5px; margin-left: 10px;">No files added yet</span>
								<br /><br />
							</asp:Panel>
							<asp:Button id="Button_Cancel" Runat="server" Text="Cancel" Width="100px"></asp:Button>
							<asp:Button id="Button_Previous" Runat="server" Text="<< Previous" Enabled="False" Width="100px"></asp:Button>
							<asp:Button id="Button_Next" Runat="server" Text="Next >>" Width="100px"></asp:Button></TD>
					</TR>
				</TABLE>
			</asp:Panel>
		</form>
	</body>
</HTML>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="umbraco.presentation.translation._default" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Translation Tasks</title>
	<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
</head>
	<body marginheight="0" marginwidth="0" topmargin="0" leftmargin="0" onLoad="resizePanel('Panel2',false);"
		onResize="resizePanel('Panel2',false);">
		<form id="Form1" method="post" runat="server">
		
		    <cc1:UmbracoPanel ID="Panel2" runat="server">

			<h3 style="MARGIN-BOTTOM: 5px"><img src="../images/nada.gif" align="absMiddle" width="16" height="16" style="FILTER:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/sendToTranslate.png', sizingMethod='scale')">
				Your Translation Tasks</h3>
			<div style="border-bottom: 1px solid #ccc; margin: 0 10px;"></div>
			<asp:Panel ID="uploadStatus" runat="server" Visible="false">
				<div style="padding: 0px 15px 0px 15px; margin: 10px; border: 1px solid #ddd">
                    <p class="guiDialogTiny"><a href="./">Task List</a> » Upload Status</p>
                    <div class="feedbackCreate">Translation file upload successfully!</div>
                    <p class="guiDialogNormal">
                    Id: <asp:Literal ID="translationFileId" runat="Server"></asp:Literal><br />
                    Page: <asp:Literal ID="translationFilePage" runat="Server"></asp:Literal><br />
                    <br />
                    <asp:Literal ID="translationFilePreviewLink" runat="server"></asp:Literal><img src="../images/forward.png" align="absMiddle" border="0"/> Go to preview page<asp:Literal ID="translationFilePreviewLinkEnd" runat="server"></asp:Literal>.<br />
                    </p>
                </div>
			</asp:Panel>
			<asp:Panel ID="panelZipFile" runat="server" Visible="false">
				<div style="padding: 0px 15px 0px 15px; margin: 10px; border: 1px solid #ddd">
                    <p class="guiDialogTiny"><a href="./">Task List</a> » Upload Status</p>
                    <p class="guiDialogNormal">
                    <strong>You've uploaded a Zip File!</strong><br />
                    Are you sure you want to update <asp:Literal ID="zipContents" runat="server"></asp:Literal> items?<br />
                    <asp:Button ID="zipYes" Text=" Yes " runat="server" OnClick="zipYes_Click" /> &nbsp;  <input type="button" value=" No " onclick="document.location.href='./';" />
                    </p>
                </div>
			</asp:Panel>
			<asp:Panel ID="panelZipSuccess" runat="server" Visible="false">
				<div style="padding: 0px 15px 0px 15px; margin: 10px; border: 1px solid #ddd">
                    <p class="guiDialogTiny"><a href="./">Task List</a> » Upload Status</p>
                    <div class="feedbackCreate">Zip translation file imported successfully!</div>
                    <p class="guiDialogNormal">
                    <strong>Imported translations:</strong><br />
                    <ul class="guiDialogNormal">
                    <asp:Literal ID="zipStatus" runat="server"></asp:Literal>
                    </ul>
                    </p>
                </div>
			</asp:Panel>
		
			<asp:Panel ID="taskView" runat="server">
                <p style="margin: 10px;" class="guiDialogNormal">The list below shows pages assigned to you. To see a detailed view including comments, click on "Details" or just the page name. You can also download the page as XML directly by clicking the "Download Xml" link. To close a translation task, please go to the Details view and click the "Close" button.</p>
				<div style="padding: 2px 15px 0px 15px">
				<asp:PlaceHolder id="tasks" Runat="server">

				    <p class="guiDialogNormal" id="uploadHolder" style="margin: -10px; padding: 10px;"><a href="#" onclick="if (document.getElementById('xmlUpload').style.display == 'block') {document.getElementById('xmlUpload').style.display = 'none';document.getElementById('uploadHolder').style.border = 'none';} else {document.getElementById('xmlUpload').style.display = 'block';document.getElementById('uploadHolder').style.border = '1px solid #ddd';}"><img src="../images/uploadTranslation.png" border="0" align="absmiddle"/> Upload translation file</a>
				    <span id="xmlUpload" style="display: none;">
				        <br />
				        <input type="file" runat="server" id="translationFile" /> &nbsp; <asp:CheckBox ID="closeTask" runat="server" Text="Close translation task" /><br /><br />
				        <asp:Button ID="uploadFile" runat="server" Text="Upload file" OnClick="uploadFile_Click" />				         
				    </span>
				    </p>

				    <p class="guiDialogNormal"><a href="xml.aspx?task=all"><img src="../images/umbraco/settingXML.gif" border="0" align="absmiddle"/> Download all translation tasks as xml</a></p>

				    
                    <asp:GridView GridLines=None ID="taskList" runat="server" AlternatingRowStyle-BackColor="#eeeeee" CellPadding="5" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Page"><ItemTemplate><img src="../images/sendToTranslate.png" align="absmiddle" /> <a href="?view=details&id=<%#Eval("Id") %>"><%#Eval("NodeName") %></a></ItemTemplate></asp:TemplateField>
                        <asp:BoundField DataField="ReferingUser" HeaderText="Assigned by"/>
                        <asp:BoundField DataField="Date" HeaderText="Date"/>
                        <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="?view=details&id={0}" Text="Details" />
                        <asp:HyperLinkField DataNavigateUrlFields="NodeId" DataNavigateUrlFormatString="xml.aspx?id={0}" Text="Download Xml" />
                    </Columns>
                    </asp:GridView>
            <p class="guiDialogNormal"><a href="translationTasks.dtd"><img src="../images/umbraco/settingXML.gif" border="0" align="absmiddle"/> Download xml dtd</a></p>
                    
                </asp:PlaceHolder>
            </asp:Panel>
            <asp:Panel ID="details" runat="server" Visible="false">
				<div style="padding: 0px 15px 0px 15px">
                    <p class="guiDialogTiny"><a href="./">Task List</a> » <asp:Literal ID="currentPage" runat="server"></asp:Literal></p>
                    <asp:DetailsView CellPadding="0" CellSpacing="0" ID="translationDetails" runat="server" AutoGenerateRows="false" BorderStyle="none" DefaultMode="ReadOnly" GridLines="none">
                    <Fields>
                    <asp:TemplateField><ItemTemplate>
                        <h3 style="margin:0">Translate '<%#Eval("NodeName") %>'</h3>
                        <p>
                        <strong>Translation Details:</strong> Assigned to you by <%#Eval("ReferingUser") %> at <%#Eval("Date") %>.<br />
                        <strong>Total Words: </strong> <%#Eval("Words") %><br />
                        <strong>Comments: </strong> <%#Eval("Comments") %><br />
                        <br />
                        </p>
                        <h3 style="margin:0">Tasks:</h3>
                        <p>
                        <a href="xml.aspx?Id=<%#Eval("NodeID") %>"><img src="../images/umbraco/settingXML.gif" border="0" align="absmiddle"/> Download Page Xml</a> &nbsp; | &nbsp; <a href="?id=<%#Eval("Id") %>&view=close"><img src="../images/close.png" border="0" align="absmiddle"/> Close this translation Task</a>
                        </p>
                        </ItemTemplate>
                        
                    </asp:TemplateField>
                    </Fields>
                    </asp:DetailsView>
                    <br />

                    <h3 style="margin:0">Fields:</h3>
                    <asp:DetailsView ID="pageDetails" runat="server" AutoGenerateRows="true" BorderStyle="none" CellPadding="5">
                    <AlternatingRowStyle BackColor="#eeeeee" />
                    </asp:DetailsView>
                </div>
            </asp:Panel>
            </cc1:UmbracoPanel>
		</form>
	</body>
</html>

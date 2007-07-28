<%@ Register Namespace="umbraco" TagPrefix="umb" Assembly="umbraco" %>
<%@ Page language="c#" Codebehind="EditUser.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.user.EditUser" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>EditUser</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/umbracoGui.css" type="text/css" rel="stylesheet">
		<script>
				function resizePanel(PanelName, hasMenu) {
					var clientHeight =(document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
					var clientWidth = document.body.clientWidth;

					panelWidth = clientWidth;
				
					contentHeight = clientHeight-72;
					if (hasMenu) contentHeight = contentHeight - 32;
				
					document.getElementById(PanelName).style.width = panelWidth + "px";
					document.getElementById(PanelName+'_content').style.height = contentHeight + "px";
					document.getElementById(PanelName+'_content').style.width = panelWidth + "px";
					
					document.getElementById(PanelName+'_menu').style.width = (panelWidth - 7)+"px"
					scrollwidth = panelWidth - 35;
					document.getElementById(PanelName +"_menu").style.width = scrollwidth + "px";
					document.getElementById(PanelName +"_menu_slh").style.width = scrollwidth + "px";
				}
		</script>
		<script type="text/javascript" src="../js/xmlextras.js"></script>
		<script type="text/javascript" src="../js/xmlRequest.js"></script>
		<script language="javascript">
			
			var updateMethod = "";
			var contentOrMediaId = "";
			var windowChooser;
			var treePickerId = -1;
			var prefix;
			
			function dialogHandler(id) {
				windowChooser.close();
				treePickerId = id;
				if (treePickerId != undefined) {
				    
					$get(contentOrMediaId).value = treePickerId;
				
					if (treePickerId > 0) {
				    umbraco.presentation.webservices.CMSNode.GetNodeName('<%=umbraco.BasePages.BasePage.umbracoUserContextID%>', treePickerId, testMethod);
					} else {
						if (contentOrMediaId == "startNode" || contentOrMediaId == "cstartNode")
							$get(contentOrMediaId + "Title").innerHTML =  "<strong><%=umbraco.ui.Text("content", base.getUser())%></strong>";
						else
							$get(contentOrMediaId + "Title").innerHTML =  "<strong><%=umbraco.ui.Text("media", base.getUser())%></strong>";
						}
				}
			}

function testMethod(result) {
    $get(prefix+"startNodeTitle").innerHTML = "<strong>" + result + "</strong>";
}
			
			function updateContentId(usePrefix) {
			    prefix = usePrefix;
				contentOrMediaId = usePrefix+"startNode";
				windowChooser = window.open('../dialogs/treePicker.aspx?appAlias=content&amp;isDialog=true&amp;dialogMode=id&amp;contextMenu=false', 'treePicker', 'width=350px,height=300px,scrollbars=no,center=yes,border=thin,help=no,status=no')			
			}			

			function updateMediaId(usePrefix) {
			    prefix = usePrefix;
				contentOrMediaId = usePrefix + "StartNode";
				windowChooser = window.open('<%=umbraco.GlobalSettings.Path%>/dialogs/treePicker.aspx?appAlias=media', 'treePicker', 'width=350px,height=300px,scrollbars=no,center=yes,border=thin,help=no,status=no')			
			}			

		</script>
	</HEAD>
	<body onLoad="resizeTabView(UserTabs_tabs, 'UserTabs');" onResize="resizeTabView(UserTabs_tabs, 'UserTabs');">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
             <asp:ServiceReference Path="../webservices/CMSNode.asmx" />
            </Services>
            </asp:ScriptManager>
		
		<cc1:TabView
                id="UserTabs" Width="400px" Visible="true" runat="server" />
		</form>
	</body>
</HTML>

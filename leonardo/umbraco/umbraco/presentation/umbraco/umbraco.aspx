<%@ Page trace="false" language="c#" Codebehind="umbraco.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation._umbraco" clienttarget="uplevel" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="uc1" TagName="quickEdit" Src="dashboard/quickEdit.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML style="WIDTH: 100%">
	<HEAD>
		<title>umbraco</title>
		<script type="text/javascript" src="js/language.aspx"></script>
		<script type="text/javascript" src="js/guiFunctions.js"></script>
		<script type="text/javascript" src="js/xmlextras.js"></script>
		<script type="text/javascript" src="js/xmlRequest.js"></script>
		<script type="text/javascript" src="js/umbracoDefault.js"></script>
		<LINK href="css/xtree.css" type="text/css" rel="stylesheet">
		<link href="css/umbContext.css" type="text/css" rel="stylesheet" />
			<LINK href="css/umbracoGui.css" type="text/css" rel="stylesheet">
			<style>
				#rightTD {
					padding-left:8px;		
				}
			</style>
			<script>
			this.name = 'umbracoMain';
			</script>
	</HEAD>
	<body onresize="resizePage();" bgColor="#fff" leftMargin="0" topMargin="0" onload="resizePage()"
		marginwidth="0" marginheight="0" style="PADDING-RIGHT: 0px;PADDING-LEFT: 0px;PADDING-BOTTOM: 0px;MARGIN: 0px;PADDING-TOP: 0px;HEIGHT: 100%; background: white">
		<form id="Form1" method="post" runat="server" style="margin: 0px; padding: 0px">
			<div style="PADDING-RIGHT:0px; PADDING-LEFT:10px; PADDING-BOTTOM:0px; PADDING-TOP:0px">
				<div id="speechbubble" style="Z-INDEX: 10; FILTER: Alpha(Opacity=0); LEFT: 80px; BACKGROUND-IMAGE: url(images/speechbubble.gif); VISIBILITY: hidden; WIDTH: 231px; POSITION: absolute; TOP: 50px; HEIGHT: 84px">
					<div id="speechIcon" style="LEFT: 10px; POSITION: absolute; TOP: 6px"><img src="images/speechbubble_info.gif" alt="Info" width="16" height="16" id="speechIconSrc"></div>
					<div id="speechClose" style="LEFT: 208px; POSITION: absolute; TOP: 6px"><a href="javascript:umbSpeechBubbleHide(100)"><img src="images/speechBubble_close.gif" width="18" height="18" border="0" alt="Close"
								onMouseOver="this.src = 'images/speechBubble_close_over.gif';" onMouseOut="this.src='images/speechBubble_close.gif';"></a></div>
					<div id="speechHeader" class="guiInputText" style="FONT-WEIGHT: bold; LEFT: 30px; POSITION: absolute; TOP: 1px">Data 
						gemt!</div>
					<div id="speechMessage" class="guiInputText" style="LEFT: 8px; WIDTH: 210px; POSITION: absolute; TOP: 19px">Her 
						kan man skrive noget tekst!</div>
				</div>
				<div id="speechbubbleShadow" style="Z-INDEX: 2; FILTER: Alpha(Opacity=0); LEFT: 80px; BACKGROUND-IMAGE: url(images/speechbubble_shadow.gif); VISIBILITY: hidden; WIDTH: 235px; POSITION: absolute; TOP: 50px; HEIGHT: 88px">
				</div>
				<script>
			<asp:PlaceHolder ID="bubbleText" Runat="server"/>
				</script>
				<div class="topBar" id="topBar">
				<div style="float: left"><button id="buttonCreate" onClick="createWizard(); return false;" class="topBarButton" style="margin-left: 10px; margin-top: -1px; height: 26px; padding: 1px; width: 100px;"><img src="images/new.gif" style="width: 16px; height: 16px; margin-right: 5px;" align="absmiddle"/> <%=umbraco.ui.Text("general", "create")%>...</button> &nbsp; </div>
				<asp:Panel ID="FindDocuments" Runat="server">
				<div style="float: left"; margin-left: 20px;"><uc1:quickEdit id="QuickEdit1" runat="server"></uc1:quickEdit></div>
				</asp:Panel>
				<div style="float: right; margin-right: 5px;">
				<button onClick="window.open('dialogs/about.aspx', 'about', 'width=450,height=400,scrollbars=auto'); return false;" class="topBarButton" style="margin-left: 10px; height: 26px; margin-top: -1px; padding: 1px; width: 70px;"><img src="images/umbraco.gif" style="width: 16px; height: 18px; margin-right: 5px;" align="absmiddle"/> <%=umbraco.ui.Text("general", "about")%></button> &nbsp; 
				<button onClick="window.open('http://umbraco.org/redir/help/<%=this.getUser().Language%>/<%=this.getUser().UserType.Name%>/' + currentApp + fetchUrl(right.document.location.href), 'help','width=750,height=500,scrollbars=auto,resizable=1;'); return false;" class="topBarButton" style="height: 26px; margin-top: -1px; padding: 1px; width: 70px;"><img src="images/help.gif" style="width: 14px; height: 14px; margin-right: 3px;" align="absmiddle"/> <%=umbraco.ui.Text("general", "help")%></button> &nbsp;  
				<button onClick="if (confirm('<%=umbraco.ui.Text("areyousure")%>')) {document.location.href='logout.aspx'; return false;} else {return false}" class="topBarButton" style="height: 26px; margin-top: -1px; padding: 3px; width: 170px;"><img src="images/logout_small.gif" style="width: 13px; height: 13px; margin-right: 5px;" align="absmiddle"/> <%=umbraco.ui.Text("general", "logout")%>: <%=this.getUser().Name%></button>
				</div>
				</div>
				<table cellSpacing="0" cellPadding="0" border="0">
					<tr>
						<td vAlign="top">
							<cc1:UmbracoPanel id="treeWindow" runat="server" Height="380px" Width="280px" hasMenu="false">
								<IFRAME class="umbracoGuiWindow" id="tree" name="tree" src="TreeInit.aspx" frameBorder="0"
									width="95%" height="98%"></IFRAME>
							</cc1:UmbracoPanel>
							<IMG height="8" alt="" src="images/nada.gif"><br />
							<cc1:UmbracoPanel id="PlaceHolderAppIcons" Text="Sektioner" runat="server" Height="130px" Width="267px"
								hasMenu="false">
								<asp:Panel id="plcIcons" style="PADDING-RIGHT: 0px; PADDING-LEFT: 5px; PADDING-BOTTOM: 0px; PADDING-TOP: 5px"
									runat="server"></asp:Panel>
							</cc1:UmbracoPanel>
						</td>
						<td id="rightTD" vAlign="top">
							<!-- umbraco dashboard -->
							<%if (umbraco.helper.Request("rightAction") == "")%>
							<iframe name="right" id="right" marginWidth="0" marginHeight="0" src="dashboard.aspx" frameBorder="0"
								width="695" scrolling="no" style="HEIGHT: 520px"></iframe>
<%if (umbraco.helper.Request("rightAction") != "")
      Response.Write("							<iframe name=\"right\" id=\"Iframe1\" marginWidth=\"0\" marginHeight=\"0\" src=\"" + umbraco.helper.Request("rightAction") + ".aspx?id=" + umbraco.helper.Request("id")+ "\" frameBorder=\"0\"	width=\"695\" scrolling=\"no\" style=\"HEIGHT: 520px\"></iframe>");
      %>


						</td>
					</tr>
				</table>
				<iframe src="keepalive.aspx" style="width:1px;height:1px;"></iframe>
		</form>
		</DIV>
		<script type="text/javascript" language="javascript">
		<%if (umbraco.helper.Request("app") != "")
		    Response.Write("tree.document.location.href = 'treeInit.aspx?app=" + umbraco.helper.Request("app") + "'");
		    %>
		</script>
	</body>
</HTML>
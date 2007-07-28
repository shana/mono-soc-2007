<%@ Page language="c#" Codebehind="sort.aspx.cs" AutoEventWireup="True" Inherits="umbraco.cms.presentation.sort" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%=umbraco.ui.Text("sort")%></title>
        <script type="text/javascript" src="js/extjs/adapter/yui/yui-utilities.js"></script>
        <script type="text/javascript" src="js/extjs/adapter/yui/ext-yui-adapter.js"></script>
        <script type="text/javascript" src="js/extjs/ext-all.js"></script>
        <link rel="stylesheet" type="text/css" href="css/umbracoGui.css">
        <link rel="stylesheet" type="text/css" href="js/extjs/resources/css/ext-all.css">
        <link rel="stylesheet" type="text/css" href="js/extjs/resources/css/ytheme-gray.css">
        <style>
            body {margin: 10px;}
            
            #loading {
                z-index: 1;
                background-color:#CF4342;
                color:#fff;
                top:0px;
                right:0px;
                position:fixed;
                padding: 5px;
            }
            
            #loading img {
               vertical-align:middle;
               margin:2px;
            }
        </style>

        <script type="text/javascript">	
		var ds;

        Ext.onReady(function() {
            
            // load data from webservices
            umbraco.presentation.webservices.nodeSorter.GetNodes(<%=umbraco.helper.Request("ID")%>, '<%=umbraco.helper.Request("app")%>', initData);

        });

        function initData(dataSet) {
        ds = new Ext.data.Store({
		    proxy: new Ext.data.MemoryProxy(dataSet),
    		
            reader: new Ext.data.JsonReader({
                root: 'SortNodes',
                id: 'Id'
            }, [
                {name: 'Id', mapping: 'Id'},
                {name: 'Name', mapping: 'Name'},
                {name: 'Sort Order', mapping: 'SortOrder'},
                {name: 'Create Date', mapping: 'CreateDate'},
                {name: 'Type', mapping: '__type'},
                {name: 'Total Nodes', mapping: 'TotalNodes'}
            ]),        
            
            // turn on remote sorting
            remoteSort: false
            });
             
	    var colModel = new Ext.grid.ColumnModel([
		    {header: "Name", width: 240, sortable: true, dataIndex: 'Name'},
		    {header: "Sort Order", width: 120, sortable: true, dataIndex: 'Sort Order'},
		    {header: "Create Date", width: 120, sortable: true, renderer: Ext.util.Format.dateRenderer('m/d/Y'), dataIndex: 'Create Date'}
	    ]);
    	
		    var grid = new Ext.grid.Grid('grid-example', {ds: ds, cm: colModel, autoSizeColumns: true,
    autoSizeHeaders: false,
    loadMask : {msg: 'Loading Data...', msgCls: 'some-css-class'},
    trackMouseOver: true,
    enableColumnHide: false,
    enableColumnMove: false,
    minColumnWidth: 120,
    autoExpandColumn: 1,
    enableColumnResize: false,
    enableCtxMenu: false,
    enableDragDrop: true});

    var ddrow = new Ext.dd.DropTarget(grid.container, {
    ddGroup : 'GridDD',
    copy:false,
    notifyDrop : function(dd, e, data){
    var sm=grid.getSelectionModel();
    var rows=sm.getSelections();

    var cindex=dd.getDragData(e).rowIndex;
    if (cindex > -1) {
    for(i = 0; i < rows.length; i++) {
    rowData=ds.getById(rows[i].id);
    if(!this.copy) ds.remove(ds.getById(rows[i].id));
    ds.insert(cindex,rowData);
    };
    }
    grid.getSelectionModel().clearSelections();
    }
    });


    ds.load(dataSet);
    grid.render();
    	
    }
		function fire() {
		
		    var records = ds.getRange(0, ds.getTotalCount());
		    var sortOrder = "";
		    for(var i=0;i<records.length;i++) {
		        sortOrder += records[i].get('Id') + ",";
		    }
		    
		    // Update sortOrder on server
		    document.getElementById("sortingDone").style.display = 'none';	    
		    document.getElementById("loading").style.display = 'block';	    
		    umbraco.presentation.webservices.nodeSorter.UpdateSortOrder(<%=umbraco.helper.Request("ID")%>, sortOrder, showConfirm);
		    
		}
		
		function showConfirm() {
		    document.getElementById("loading").style.display = 'none';	    
		    document.getElementById("sortingDone").style.display = 'block';	
		    parent.opener.top.reloadCurrentNode();
		}
		</script>
	</head>
	<body>

        <div id="loading" style="display:none"><img id="loadingAnim" src="js/extjs/resources/images/default/grid/loading.gif" alt="loading" /> <span id="msgLoading"><%=umbraco.ui.Text("updating...") %></span></div>

		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
            <asp:ServiceReference Path="webservices/nodeSorter.asmx" /></Services>
            </asp:ScriptManager>
		<div style="float: right; padding: 3px;"><a href="javascript:window.close()" class="guiDialogTiny"><%=umbraco.ui.Text("general", "closewindow", this.getUser())%></a></div><h3><img src="images/sort.png" align="absmiddle"/> <%=umbraco.ui.Text("sort")%></h3>
		<img class="gradient" style="width: 100%; height: 1px; margin-top: 7px;" src="images/nada.gif"/><br/>
		<asp:Literal ID="FeedBackMessage" Runat="server"/>
		<br/>
            
				<div id="sortingDone" style="display:none; height: 21px;" class="feedbackCreate"><asp:Literal runat="server" ID="sortDone"></asp:Literal></div>
				<div id="form" style="display: block; margin: 10px">
				<p class="guiDialogTiny" style="margin: 10px 0;"><asp:Literal runat="server" ID="help"></asp:Literal></p>
				<p style="text-align: right; margin-bottom: 5px;"><input type="button" onClick="fire()" value='<%=umbraco.ui.Text("save") %>' /> &nbsp; <input type="button" onClick="window.close()" value='<%=umbraco.ui.Text("cancel") %>' /><br /></p>
<div id="grid_panel" style="width: 550px">
<div id="grid_table"></div>
<div id="GridDD"></div>
<div id="grid-example" style="width: 550px; border:2px solid #999;visibility:hidden;"></div>
</div>
				<p style="text-align: right; margin-bottom: 5px;"><input type="button" onClick="fire()" value='<%=umbraco.ui.Text("save") %>' /> &nbsp; <input type="button" onClick="window.close()" value='<%=umbraco.ui.Text("cancel") %>' /><br /></p>
				<br style="clear:both" />
				</div>
		</form>

	</body>
</HTML>

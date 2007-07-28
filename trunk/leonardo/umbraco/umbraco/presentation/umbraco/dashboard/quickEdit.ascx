<%@ Control Language="c#" AutoEventWireup="True" Codebehind="quickEdit.ascx.cs" Inherits="dashboardUtilities.quickEdit" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<style type="text/css">
	.comboBoxList {
		padding: 0px;
		border: 1px solid #999;
		background-color: #f7f7ff;
		overflow: visible;
	}
	.comboBoxItem {
		margin: 0px;
		padding: 2px 5px;
		background-color: inherit;
		cursor: default;
		font-size: small;
		overflow: hidden;
	}
	.comboBoxSelectedItem {
		background-color: #ddf;
	}
</style>
<script type="text/javascript" src="js/xmlextras.js"></script>
<script type="text/javascript" src="js/xmlRequest.js"></script>
<script type="text/javascript" >
var minLengthBeforeQuery = 1;
var comboBox = null;
var searchIds = new Array();
var searchTexts = new Array()

function ComboBox_choose(realval)
{
	if (currentApp.toLowerCase() != "content" && currentApp != "") {
		tree.document.location.href = 'TreeInit.aspx';
		currentApp = 'content';
		document.getElementById("buttonCreate").disabled = false;		
		setTimeout("right.document.location.href= 'editContent.aspx?id=" + searchIds[realval] + "'", 500);
	}
	else
		right.document.location.href= 'editContent.aspx?id=' + searchIds[realval];
}


</script>
<script type="text/javascript"  src="js/utilities.js"></script>
<script type="text/javascript"  src="js/combobox.js"></script>
<script type="text/javascript">
// this is my store of options.  Note that options CAN contain HTML
// markup in them.  The HTML will be automatically stripped out when
// the value is inserted into the text field, but will be left in
// place in the dropdown.
//
// In most cases, a simple array is not a very useful way of providing
// options; dynamically creating them on the fly in your callback
// method is a must more useful technique, especially when combined
// with JS Remoting.
names = "".split(",");

function init() {
	// initialize the combobox widget, and pass in the callback function.
	var cb = new ComboBox("f_name", new Array());
}


function umbracoPopulateSearch(combo) {
	comboBox = combo;
	if (loading) {
		window.clearTimeout(loading);
	}
	
	if (combo.value != "*" && combo.value != "* ") {
		doc = "umbracoStartXmlRequest('dashboard/search.aspx?q=" + encodeURI(combo.value) + "&rnd=" + returnRandom() + "', '', 'showDocuments()')";
		loading = window.setTimeout(doc, 250);
	}
}

function showDocuments() {

	_searchIds = "";
	_searchTexts = "";
	
	if (umbracoXmlRequestResultTxt().length > 0) {
		var resultsText = umbracoXmlRequestResultTxt();
		if (resultsText != "0") {
			var results= resultsText.split("\n");
			for (var i=0;i<results.length;i++) {
				var temp = results[i].split("|||");
				_searchTexts += temp[0] + "|||";
				_searchIds += temp[1] + "|||";
			}
		}
	}
	searchIds = _searchIds.split("|||");
	if (_searchTexts.substring(0, _searchTexts.length-3) != "")
		comboBox.comboBox.setItems(_searchTexts.substring(0, _searchTexts.length-3).split("|||"));
	else
		comboBox.comboBox.setItems(new Array());
	
}

</script>
		<div style="margin-top: 2px;">
<div class="guiDialogTiny" style="color: black; float: left; padding-left: 8px; padding-top: 3px;"><img src="images/findDOcument.gif" style="float: left;"/> <%=umbraco.ui.Text("findDocument")%>: &nbsp;</div>
<div style="float: left;" id="quickEditHolder"></div>
<input id="f_name" accesskey="f" class="guiDialogTinyMark" style="border: 1px solid #999; height: 20px; padding: 2px; width: 200px" autocomplete="off" />
</div>
<script type="text/javascript">
setTimeout('init();',500);
</script>
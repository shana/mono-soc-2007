var loading = null;
var modalActive = false;

var rightWindowTab = false;
var rightButtons = false;

var nodeID = -1;
var nodeKey = '';
var nodeType = '';
var nodeName = '';
var topNodeID = -1;
var currentApp = "";
var nodeDeleteEffectElement = null;
var deleteNode;

/* REFRESH NODE CODE */

	var expandTries = 0;
	
	var refresh = true;
	var refreshParent = true;
	var dontDelete = false;
	
	function deleteEffect() {
		if (deleteNode.parentNode.childNodes.length == 1)
			deleteNode.parentNode.childNodes = [];

		// Add delete puff effectEffects
		deleteNode.collapse();
		new tree.Effect2.DropOut(deleteNode.id);
		nodeDeleteEffectElement = deleteNode;
		
		// Update recyclebin
		if ((currentApp == "" || currentApp == "content") && deleteNode.parentNode.nodeID != -20)
		    updateRecycleBin();

		setTimeout('nodeDeleteEffectElement.remove()', 1000);
	}

	function refreshTree(refParent, dontDel) {

		expandTries = 0;
		refreshParent = refParent;
		dontDelete = dontDel;
		
		if (refreshParent) {
			
			if (node != null && refresh) {
				if (node.parentNode != null) {
					if (dontDelete)
						node = node.parentNode;
					else {
						if (node.parentNode.childNodes.length == 1)
							node.parentNode.childNodes = [];

						// Add delete puff effectEffects
						node.collapse();
						new tree.Effect2.DropOut(node.id);
						nodeDeleteEffectElement = node;
						
						// Update recyclebin
						if (node.parentNode.nodeID != -20)
						    updateRecycleBin();

						setTimeout('nodeDeleteEffectElement.remove()', 1000);
					}
				}
			}
		} 
		
		if (!refreshParent || (refreshParent && dontDelete)) {
			if (node != null && refresh) {
				if (node.parentNode != null) {
					if (node.src != "" && node.src != null) {
						node.src = node.src + '&rnd=' + returnRandom();
						node.reload();
						setTimeout("expandNode()", 200);
					} else if (node.srcRoot != "" && node.srcRoot != null) {
						node.src = node.srcRoot;
						node.reload();
						node.expand();
						setTimeout("expandNode()", 200);
					}	
				} else {
					tree.document.location.href = tree.document.location.href;
					}
			} else if (!refresh)
				tree.document.location.href = tree.document.location.href;
		}
	}
		
		
	function expandNode() {
		if (node.childNodes.length == 0 && expandTries < 10) {
			expandTries++;
			setTimeout("expandNode()", 200);
		} else {
			node.expand();
			expandTries = 0;
		}
	}

/* REFRESH NODE CODE */

function clearLoading() {
	window.clearTimeout(loading);	
}

function fetchUrl(url) {
	url = url.split("\/");
	if (url.length > 0)
		tmp = url[url.length-1];
	else
		tmp = url;
		
	if (tmp.indexOf("?") > 0)
		tmp = tmp.substring(0, tmp.indexOf("?"));
	return tmp;
}

function expandNode() {
	if (parent.node.childNodes.length == 0 && expandTries < 10) {
		expandTries++;
		setTimeout("expandNode()", 200);
	} else {
		parent.node.expand();
		expandTries = 0;
	}
}

function reloadParentNode() {
	var reloadNode = false;
	
	if (node != null)
		if (node.parentNode)
			if (node.parentNode.parentNode)
				reloadNode = true;

	if (reloadNode) {
		node.parentNode.src = node.parentNode.src + "&rnd2=" + returnRandom();
		node.parentNode.reload();
	} else
		tree.document.location.href = tree.document.location.href;
}

function reloadCurrentNode() {
	var reloadNode = false;

	if (node != null)
		if (node.parentNode)
		    reloadNode = true;
    
	if (reloadNode) {
		node.src = node.src + "&rnd2=" + returnRandom();
		node.reload();
		node.expand();
	} else
		tree.document.location.href = tree.document.location.href;
	    
}


function returnRandom() {
	day = new Date()
	z = day.getTime()
	y = (z - (parseInt(z/1000,10) * 1000))/10
	return y
}

function disableModal() {
	modalActive = false;
}

var theDialogWindow = null;

function openDialog(diaTitle, diaDoc, dwidth, dheight, optionalParams)
{
  modalActive = true;
    
  if (theDialogWindow != null && !theDialogWindow.closed) {
	theDialogWindow.close();
	}
  theDialogWindow = window.open(diaDoc,'dialogpage', "width="+dwidth+"px,height="+dheight+"px"+optionalParams);// window.showModalDialog(diaDoc, "MyDialog", strFeatures);
}

function createNew() {
	if (nodeType != '') {
	    if (currentApp == "content" || currentApp == "")
	    		openDialog("Opret", "create.aspx?nodeID="+nodeID+"&nodeType="+nodeType+"&nodeName="+nodeName+'&rnd='+returnRandom(), 600, 425);
        else
		    openDialog("Opret", "create.aspx?nodeID="+nodeID+"&nodeType="+nodeType+"&nodeName="+nodeName+'&rnd='+returnRandom(), 320, 225);
	
	}
}

function importDocumentType() {
	if (nodeType != '') {
		openDialog("Import", "dialogs/importDocumentType.aspx?rnd="+returnRandom(), 420, 265);
	
	}
}

function exportDocumentType() {
	if (nodeType != '') {
		openDialog("Emport", "dialogs/exportDocumentType.aspx?nodeID=" + nodeID + "&rnd="+returnRandom(), 320, 205);
	
	}
}

function refreshNode() {
	if (nodeKey != '') {
		if (tree.webFXTreeHandler.all[nodeKey]) {
			tree.webFXTreeHandler.all[nodeKey].src = 
				tree.webFXTreeHandler.all[nodeKey].src + '&rnd='+ Math.random()*10;

			if (tree.webFXTreeHandler.all[nodeKey].parentNode) {
				// Hvis punktet er lukket, skal det åbnes
				if (tree.webFXTreeHandler.all[nodeKey].childNodes.length > 0) {
					if (!tree.webFXTreeHandler.all[nodeKey].open)
						tree.webFXTreeHandler.all[nodeKey].expand();
					tree.webFXTreeHandler.all[nodeKey].reload();
					treeEdited = true;
				}
			} else {
				tree.document.location.href = tree.document.location.href;
			}
		}
	}
}

function accessThis() {
	if (nodeID != '-1' && nodeType != '') {
	
		task.document.dataForm.nodeID.value = nodeID;
		task.document.dataForm.nodeType.value = nodeType;

		newName = window.open("dialogs/publicAccess.aspx?nodeID="+nodeID+'&rnd='+returnRandom(), "access", 'width=530,height=550,scrollbars=auto');
	
	}
}


function assignDomain() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = window.open("dialogs/assignDomain.aspx?id="+nodeID, "assignDomain", 'width=500,height=450,scrollbars=yes');
	
	}
}

function publish() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = window.open("dialogs/publish.aspx?id="+nodeID, "publish", 'width=500,height=250,scrollbars=auto');
	
	}
}

function emptyTrashcan() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = window.open("dialogs/emptyTrashcan.aspx", "emptyTrashcan", 'width=500,height=250,scrollbars=auto');
	
	}
}

function sortThis() {
	if (nodeID != '0' && nodeType != '') {
		// task.document.dataForm.nodeID.value = nodeID;
		// task.document.dataForm.nodeType.value = nodeType;
		openDialog("Sort", "sort.aspx?id="+nodeID + '&app=' + currentApp + '&rnd='+returnRandom(), 600, 450,',scrollbars=yes');
		/*
		// læg variable i form
		 if (newName != 'undefined' && newName != null) {
			task.document.dataForm.task.value = "sort";
			task.document.dataForm.parameterName.value = newName;
			// submit form
			task.document.dataForm.submit();
		}
		*/
		
	}
}

function protectThis() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = openDialog("protect", "dialogs/modalHolder.aspx?url=protectPage.aspx&params=app=" + currentApp + "|mode=cut|amp;nodeId="+nodeID + '&rnd='+returnRandom(), 535, 470);
	
	}
}

function moveThis() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = openDialog("move", "dialogs/modalHolder.aspx?url=moveOrCopy.aspx&params=app=" + currentApp + "|mode=cut|amp;id="+nodeID + '&rnd='+returnRandom(), 500, 450);
	
	}
}

function rightsThis() {
	if (nodeID != '-1' && nodeType != '') {		
		newName = openDialog("rights", "dialogs/modalHolder.aspx?url=cruds.aspx&params=id="+nodeID + '&rnd='+returnRandom(), 700, 340);
	
	}
}

function changeDashboard(path) {
    right.location.href = path;
}

function notifyThis() {
	if (nodeID != '-1' && nodeType != '') {		
		newName = openDialog("notifications", "dialogs/modalHolder.aspx?url=notifications.aspx&params=id="+nodeID + '&rnd='+returnRandom(), 680, 340);
	
	}
}

function createWizard() {
	if (currentApp == 'media' || currentApp == 'content' || currentApp == '') {
		if (currentApp == '') currentApp = 'content';
		openDialog("create", "dialogs/create.aspx?nodeType=" + currentApp + "&app=" + currentApp + "&rnd="+returnRandom(), 600, 470);

	} else
		alert('Not supported - please create by right clicking the parentnode and choose new...');
}

function copyThis() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = openDialog("move", "dialogs/modalHolder.aspx?url=moveOrCopy.aspx&params=app=" + currentApp + "|mode=copy|id="+nodeID + '&rnd='+returnRandom(), 500, 450);
	
	}
}

function translateThis() {
	if (nodeID != '-1' && nodeType != '') {
		
		newName = openDialog("translate", "dialogs/modalHolder.aspx?url=sendToTranslation.aspx&params=app=" + currentApp + "|id="+nodeID + '&rnd='+returnRandom(), 500, 450);
	
	}
}

function deleteThis() {
	var tempID = nodeID;
	var tempNodeType = nodeType;
	var tempNodeName = nodeName;	
	if (confirm(parent.uiKeys['defaultdialogs_confirmdelete'] + ' "' + nodeName + '"?\n\n')) {
		deleteNode = node;
		nodeID = tempID;
		nodeType = tempNodeType;
		nodeName = tempNodeName;
		umbracoStartXmlRequest('webservices/aspx_ajax_calls/delete.aspx?nodeId=' + tempID + '&nodeType=' + tempNodeType + '&nodeName=' + nodeName, '', 'refreshDelete()');
	}
}

function refreshDelete() {
	deleteEffect();	
}

// Just used for users, which aren't deleted but only disabled
function disableThis() {
	if (confirm(parent.uiKeys['defaultdialogs_confirmdisable'] + ' "' + nodeName + '"?\n\n')) {
		umbracoStartXmlRequest('webservices/aspx_ajax_calls/disableUser.aspx?id=' + nodeID, '', 'refreshDisabled()');
	}
}

function refreshDisabled() {
    tree.document.location.href = tree.document.location.href + "&refresh=true";
}

function republish() {
	if (confirm(parent.uiKeys['defaultdialogs_confirmSure'] + '\n\n'))
		openDialog('republish', 'republish.aspx?rnd='+returnRandom(),300,170);
}

function viewAuditTrail() {
	parent.openDialog('auditTrail', 'dialogs/viewAuditTrail.aspx?nodeID='+nodeID+'&rnd='+returnRandom(), 650, 460, ',scrollbars=yes');
}

function rollback() {
    openDialog('', 'dialogs/rollback.aspx?nodeID='+nodeID+'&rnd='+returnRandom(), 780, 580, 'scrollbars=yes,center=yes,border=thin,help=no,status=no');
}

function importPackage() {
	parent.openDialog('packager', 'dialogs/packager.aspx?rnd='+parent.returnRandom(), 530, 550, ',scrollbars=yes');
}


function closeUmbraco() {
	if (confirm(parent.uiKeys['defaultdialogs_confirmlogout'] + '\n\n'))
		document.location.href = 'logout.aspx';
}

function shiftApp(whichApp, appName) {
	currentApp = whichApp.toLowerCase();
	
	if (currentApp != 'media' && currentApp != 'content')
		document.getElementById("buttonCreate").disabled = true;
	else
		document.getElementById("buttonCreate").disabled = false;
	
	// Vi fjerner tab-vindue property
	rightWindowTab = false;
	
	// Vi skifter indhold i vinduer
	right.location.href = 'dashboard.aspx?app='+whichApp;
	tree.location.href = 'TreeInit.aspx?app='+whichApp;
	top.document.getElementById("treeWindowLabel").innerHTML = appName;
	top.document.title = appName + " - Umbraco CMS";

}

function openDashboard(whichApp) {
	right.location.href = 'dashboard.aspx?app='+whichApp;	
}

var path = null;
var step = 0;
var currentNode = null
var callers = "";

function syncTree(orgPath) {
	if (loading) 
		clearLoading();
		

	callers = "";
	path = orgPath.split(",");
	
	if (path.length == 2)
		tree.location.href = tree.location.href;
	else {
		currentNode = tree.tree;
		step = 1;
		treeFindChild("init");
	}

}

function findNodeById(id) {
    return findNodeByIdDo(tree.tree, id);
}

function findNodeByIdDo(node, id) {
    var childNodes = node.childNodes;
    
    if (childNodes != null)
    for(var i=0;i<childNodes.length;i++) {
        if (childNodes[i].nodeID == id)
            return childNodes[i];
        else if (childNodes[i].childNodes != null) {
           var tempNode = findNodeByIdDo(childNodes[i], id);
           if (tempNode != "")
            return tempNode;
        }
    }
    
    return "";
}

var recycleBin;
function updateRecycleBin() {
    recycleBin = findNodeById(-20);
	new tree.Effect2.Highlight(recycleBin.id);

    if (recycleBin.src == "") {
        recycleBin.src = "tree.aspx?isRecycleBin=true&isDialog=&dialogMode=&app=content&id=" + recycleBin.nodeID + "&treeType=" + currentApp.toLowerCase();
        tree.document.getElementById(recycleBin.id + '-plus').src = recycleBin.plusIcon;
        recycleBin.folder = true;
        recycleBin.forceLoad(recycleBin.src, recycleBin);
    } else {
        recycleBin.src+= "&rnd=" + + Math.random()*10;
        recycleBin.reload();
    }
                    

   
}

function treeFindChild() {
	findedNode = null;
	doLoad = false;
	firstLoad = false;
	
	if (!currentNode.loading) {
		for (var i=0;i<currentNode.childNodes.length;i++)
			if (currentNode.childNodes[i].nodeID == path[step])
				findedNode = currentNode.childNodes[i];
				
		if (findedNode) {
			
			// expand
			if (step < path.length-1) {
			
				// refresh node if it's the current
				if (step == path.length-2) {
					if (findedNode.src == "") {
						if (currentApp == "")
							currentApp = "Content";
						findedNode.src = "tree.aspx?isDialog=&dialogMode=&app=" + currentApp + "&id=" + findedNode.nodeID + "&treeType=" + currentApp.toLowerCase();
						doLoad = false;
						setTimeout("findedNode.expand();", 300);
						setTimeout("findedNode.expand();", 800);
						return "";
					} else {
						findedNode.src = findedNode.src + '&rnd='+returnRandom();
						findedNode.reload();
					}
				}
			
				if (!findedNode.open) {
					findedNode.expand();
				}
				step++;
				currentNode = findedNode;
				findedNode = null;
				doLoad = true;
			} else {
				findedNode.select();
			}
		}
		else
			doLoad = true;
	} else
		doLoad = true;
			
	if (doLoad) {
		if (loading)
			clearLoading();
		
		if (firstLoad)
			loading = window.setTimeout("treeFindChild()", 800);
		else
			loading = window.setTimeout("treeFindChild()", 200);
		
	}
}


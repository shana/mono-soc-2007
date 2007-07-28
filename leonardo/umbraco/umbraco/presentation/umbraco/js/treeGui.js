webFXTreeConfig.rootIcon		= "images/umbraco/folder.gif";
webFXTreeConfig.openRootIcon	= "images/umbraco/folder_o.gif";
webFXTreeConfig.folderIcon		= "images/xp/folder.png";
webFXTreeConfig.openFolderIcon	= "images/xp/openfolder.png";
webFXTreeConfig.fileIcon		= "images/tree/wait.gif";
webFXTreeConfig.lMinusIcon		= "images/xp/Lminus.png";
webFXTreeConfig.lPlusIcon		= "images/xp/Lplus.png";
webFXTreeConfig.tMinusIcon		= "images/xp/Tminus.png";
webFXTreeConfig.tPlusIcon		= "images/xp/Tplus.png";
webFXTreeConfig.iIcon			= "images/xp/I.png";
webFXTreeConfig.lIcon			= "images/xp/L.png";
webFXTreeConfig.tIcon			= "images/xp/T.png";

function umbracoXtreeUpdateNode(nodeID, nodeClass, nodeTitle) {

	for (var i=0;i<parent.tree.webFXTreeHandler.idCounter;i++) {
		
		if (parent.tree.webFXTreeHandler.all["webfx-tree-object-"+i]) {
			if (parent.tree.webFXTreeHandler.all["webfx-tree-object-"+i].nodeID == nodeID) {	
			
				if (webFXTreeHandler.all["webfx-tree-object-"+i].parentNode.parentNode) {
					parent.tree.webFXTreeHandler.all["webfx-tree-object-"+i].parentNode.src = 
						parent.tree.webFXTreeHandler.all["webfx-tree-object-"+i].parentNode.src + '&rnd='+ Math.random()*10;
					webFXTreeHandler.all["webfx-tree-object-"+i].parentNode.reload();
					
					setTimeout("umbracoTreeDoFocus(" + nodeID + ")", 200);
				}
				else
					document.location.href = document.location.href;
					
			}
		}
	}
}

function umbracoTreeDoFocus(nodeID) {

	var done = 0;
	
	if (webFXTreeHandler) {
		for (var i=0;i<webFXTreeHandler.idCounter;i++) {
			if (webFXTreeHandler.all["webfx-tree-object-"+i]) {
				if (webFXTreeHandler.all["webfx-tree-object-"+i].nodeID == nodeID) {
					webFXTreeHandler.all["webfx-tree-object-"+i].select();
					done = 1;
					webFXTreeHandler.all["webfx-tree-object-"+i].blur();
				}
			}	
		}
		
	} 

	if (done == 0) {
		setTimeout("umbracoTreeDoFocus("+nodeID+")", 200);		
	}
}
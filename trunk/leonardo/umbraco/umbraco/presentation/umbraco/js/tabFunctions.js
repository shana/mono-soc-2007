var oldTab = 1;

function initializeHasChanged() {
	hasChanged = false;
}

function hasChanged() {
	hasChanged = true;
}

function tabFocus(tab, theUrl) {
	tabChangeFocus(tab)
	content.location.href = theUrl;
}

function tabChangeFocus(tab) {
	tabChangeClass(oldTab, 'guiLine');
	tabChangeClass(tab, 'guiLineSelected');
	oldTab = tab;
}

function tabChangeClass(tab, theClass) {
	if (tab == 1) {
		if (theClass == 'guiLine') {
			document.all["firstTabImg"].src = "/v1/images/nada.gif";
		} else {
			document.all["firstTabImg"].src = "/v1/images/faner/lineStart.gif";
		}
	}

	document.all["tabLineStart"+tab].className = theClass;
	document.all["tabLine"+tab].className = theClass;
	document.all["tabLineEnd"+tab].className = theClass;
}


// --------------------------------------------------------------------------------------------------
// NH 29-12-03
// Added javascript functionality to the tabs, so they can be changed through ctrl+tab
// --------------------------------------------------------------------------------------------------
function tabSwitch(pos) {
	if (totalTabs > 1) {
		if (pos > 0) {
			if (oldTab+pos > totalTabs)
				tabFocus(1, tabs[1]);
			else
				tabFocus(oldTab+pos, tabs[oldTab+pos]);
		} else {
			if (oldTab+pos < 1)
				tabFocus(totalTabs, tabs[totalTabs]);
			else
				tabFocus(oldTab+pos, tabs[oldTab+pos]);
		}
	}
}

// --------------------------------------------------------------------------------------------------
// NH 29-12-03
// Javascript modification ends here
// --------------------------------------------------------------------------------------------------

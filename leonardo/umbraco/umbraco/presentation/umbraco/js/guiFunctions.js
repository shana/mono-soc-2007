	// ---------------------------------------------
	// guiFunctions
	// ---------------------------------------------

	function umbSpeechBubble(icon, header, message) {
		var speechBubble = document.getElementById("speechbubble");
		var speechBubbleShadow = document.getElementById("speechbubbleShadow");
		document.getElementById("speechHeader").innerHTML = header;
		document.getElementById("speechMessage").innerHTML = message;
		document.getElementById("speechIconSrc").src = 'images/speechBubble/' + icon + '.gif';
		
		      speechBubble.style.left = document.body.clientWidth - 244;
		speechBubbleShadow.style.left = document.body.clientWidth - 244;
		      speechBubble.style.top = document.body.clientHeight - 94;
		speechBubbleShadow.style.top = document.body.clientHeight - 94;
		      speechBubble.style.visibility = 'visible';
		speechBubbleShadow.style.visibility = 'visible';
		umbSpeechBubbleShow(0);
	}
	
	function umbSpeechBubbleShow(opacity) {
		document.getElementById("speechbubble").style.filter = 'Alpha(Opacity=' + opacity + ')';
		document.getElementById("speechbubbleShadow").style.filter = 'Alpha(Opacity=' + parseInt(opacity/2) + ')';		
		opacity = parseInt(opacity)+10;
		if (opacity < 101) 
			setTimeout("umbSpeechBubbleShow(" + opacity + ");", 50);
		else {
			setTimeout("umbSpeechBubbleHide(100);", 5000);
		}
	}
	
	function umbSpeechBubbleHide(opacity) {
		document.getElementById("speechbubble").style.filter = 'Alpha(Opacity=' + opacity + ')';
		document.getElementById("speechbubbleShadow").style.filter = 'Alpha(Opacity=' + parseInt(opacity/2) + ')';

		opacity = parseInt(opacity)-10;
		if (opacity > 1)
			setTimeout("umbSpeechBubbleHide(" + opacity + ");", 50);
		else {
			document.getElementById("speechbubble").style.visibility = 'hidden';
			document.getElementById("speechbubbleShadow").style.visibility = 'hidden'
		}
	}

	function resizePage() {
	   	var clientHeight = self.innerHeight; 
		if (clientHeight == null) {
			clientHeight = (document.compatMode=="CSS1Compat")?document.documentElement.clientHeight : document.body.clientHeight;
		}
		
		var clientHeight = clientHeight-50;
		var clientWidth = document.body.clientWidth;
		var leftWidth = parseInt(clientWidth*0.28);
		var rightWidth = clientWidth - leftWidth - 20 // parseInt(clientWidth*0.65);
		
		// vi justerer venstre-siden med træ og app-dock (som pr. default skal fylde 25% af skærmen width)		
		resizeGuiWindow("treeWindow", leftWidth, parseInt(clientHeight)-165)
		resizeGuiWindow("PlaceHolderAppIcons", leftWidth, 130)
		
		document.getElementById('right').style.width = (rightWidth-8) +"px";
		document.getElementById('right').style.height = (clientHeight-25) + "px";
	}
	
	function resizeGuiWindow(windowName, newWidth, newHeight, window) {
		resizePanelTo(windowName, false, newWidth, newHeight);
		}

	function resizeGuiWindowWithTabs(windowName, newWidth, newHeight) {


		right.document.all[windowName+"ContainerTable"].width = newWidth+22
		right.document.all[windowName+"ContainerTableSpacer"].width = newWidth
		right.document.all[windowName+"Bottom"].width = newWidth+12
		right.document.all[windowName+"BottomSpacer"].width = newWidth
		right.document.all[windowName].style.width = newWidth


		// Der skal forskellig størrelse på højden afhængig af om vinduet har en label i bunden
		if (right.document.all[windowName+'BottomLabel']) {
			right.document.all[windowName+"ContainerTable"].height = newHeight-13;
			right.document.all[windowName].style.height = newHeight-13;
		} else {
			right.document.all[windowName+"ContainerTable"].height = newHeight+3;
			right.document.all[windowName].style.height = newHeight+3;
		}
	}
	

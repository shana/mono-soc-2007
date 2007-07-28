function umbracoUpdateDateForm(fieldName) {
	var newDate = '';
	var theForm = document.forms[0]
	var monthName=["Jan","Feb","Mar","Apr","May","Jun",
             "Jul","Aug","Sep","Oct","Nov","Dec"];

	
	// vi løber igennem de forskellige items og ser om der er noget i dem
	if (theForm[fieldName+'-d'].selectedIndex > 0 
			&& theForm[fieldName+'-m'].selectedIndex > 0
				&& theForm[fieldName+'-y'].selectedIndex > 0)
			newDate = 	theForm[fieldName+'-d'][theForm[fieldName+'-d'].selectedIndex].value + ' ' +
						monthName[theForm[fieldName+'-m'][theForm[fieldName+'-m'].selectedIndex].value-1] + ' ' + 
						theForm[fieldName+'-y'][theForm[fieldName+'-y'].selectedIndex].value
						
	// vi skal lige se om der også er sat klokkeslet
	if (theForm[fieldName+'-th']) {
		if (theForm[fieldName+'-th'].selectedIndex > 0 
				&& theForm[fieldName+'-tm'].selectedIndex > 0)
			newDate += ' ' + theForm[fieldName+'-th'][theForm[fieldName+'-th'].selectedIndex].value + ':' +
						theForm[fieldName+'-tm'][theForm[fieldName+'-tm'].selectedIndex].value
	}	
	
	theForm[fieldName].value = newDate;
}

function umbracoNewWindow(side, bredde, hoejde) {
	window.open(side, 'nytVindue', 'width=' + bredde + ',height=' + hoejde + ',scrollbars=auto');
}

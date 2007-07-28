		var activeDragId = "";
		function expandCollapse(theId) {
			theForm = document.getElementById(theId + "_form");
			if (theForm.style.display == 'none') {
				theForm.style.display = 'block';
				document.getElementById("edit" + theId).style.display = 'block';
				document.getElementById("desc" + theId).style.display = 'none';
			}
			else {
				theForm.style.display = 'none';
				document.getElementById("edit" + theId).style.display = 'none';
				document.getElementById("desc" + theId).style.display = 'block';
			}
				
		}

	// ---------------------------------------------
	// editorKnapper
	// ---------------------------------------------
	var doScroll = 0;
	
	var editorIconsTotalWidth = -680;
	
	function editorBarScroll(direction) {
		doScroll = 1;
		editorBarScrollDo(direction);
	}
	
	function editorBarScrollStop() {	
		doScroll = 0;
	}
	
	function editorBarScrollDo(direction) {
		if (doScroll && parseInt(document.all["buttons"].style.left)+direction < 3 && parseInt(document.all["buttons"].style.left)+direction > editorIconsTotalWidth+parseInt(document.all["buttonHolder"].style.width)-80) {
			document.all["buttons"].style.left = parseInt(document.all["buttons"].style.left) + direction;
			setTimeout("editorBarScrollDo(" + direction + ");", 4);
		}
	}
	

	// ---------------------------------------------
	// dato-funktioner
	// ---------------------------------------------

function showDatePicker(dateType)
{
        var theBody = document.body;
        datePickerDiv = document.all("datePickerDiv" + dateType);

        if (theBody!=null && datePickerDiv==null)
        {
            theBody.insertAdjacentHTML("BeforeEnd", "<DIV ID=\"datePickerDiv" + dateType + "\" STYLE=\"position:absolute;display:none;\"><TABLE BORDER=0 CELLPADDING=1 CELLSPACING=0><TR><TD BGCOLOR=#000000><IFRAME NAME=\"datePickerFrame\" SRC=\"\" WIDTH=140 HEIGHT=140 FRAMEBORDER=0 MARGINLEFT=0 MARGINTOP=0 MARGINWIDTH=0 MARGINHEIGHT=0 SCROLLING=NO></IFRAME></TD></TR></TABLE></DIV>");
            datePickerDiv=document.all("datePickerDiv" + dateType);
        }
			
    datePickerDiv.style.left=10;
    datePickerDiv.style.top=theBody.offsetTop + 22;

    if(dateType == 0)
    {
        if(document.all.visibleDate.value == "")
            datePickerDiv.all.datePickerFrame.src = "/v1/datepicker.asp?datetype=0&curdate=&showdate="; //"datepicker.asp";
        else
            datePickerDiv.all.datePickerFrame.src = "/v1/datepicker.asp?datetype=0&curdate=" + document.all.visibleDate.value + "&showdate=" + document.all.visibleDate.value; //"datepicker.asp";
    } else {if (dateType == 1)
    {
        if(document.all.expireDate.value == "")
            datePickerDiv.all.datePickerFrame.src = "/v1/datepicker.asp?datetype="+dateType+"&curdate=&showdate="; //"datepicker.asp";
        else
            datePickerDiv.all.datePickerFrame.src = "/v1/datepicker.asp?datetype="+dateType+"&curdate=" + document.all.expireDate.value + "&showdate=" + document.all.expireDate.value; //"datepicker.asp";
    } else {
		datePickerDiv.all.datePickerFrame.src = "/v1/datepicker.asp?datetype="+dateType+"&curdate=&showdate="; //"datepicker.asp";
	}}

    datePickerDiv.style.display="block";
}

function hideDatePicker(dateType, d, m, y)
{
//	alert("document.all[" + dateType + "].value");
        datePickerDiv = document.all("datePickerDiv" + dateType);

        if (datePickerDiv != null)
        {
            datePickerDiv.style.display = "none";

            if(d != null && m != null && y != null)
            {
                if(d != 0 && m!= 0 && y != 0)
                {
                    if(dateType == 0)
                        document.all[dateType].value = d + " " + m + " " + y;
                    else if (dateType == 1)
                        document.all[dateType].value = d + " " + m + " " + y;
					else
                        document.all[dateType].value = d + " " + m + " " + y;
                }
            } else
            {
                if(dateType == 0)
                    document.all[dateType].value = "";
                else
                    document.all[dateType].value = "";
            }
        }
}	

function AllowTabCharacter() {
if (event != null) {
if (event.srcElement) {
if (event.srcElement.value) {
if (event.keyCode == 9) {
// tab character               
if (document.selection != null) {
document.selection.createRange().text = '\t';
event.returnValue = false;
}               
else 
{                  
event.srcElement.value += '\t';                  
return false;               
}            
}          
}      
}   
}}
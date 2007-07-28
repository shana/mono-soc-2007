
// A simple solution to access textarea code when the codepress editor is disabled
var textareaEditorMode = "true";

textareaEditor = function(obj) {
    
	var self = document.createElement('textarea');
	self.id = obj.id;
	self.style.width = obj.style.width;
	self.style.height = obj.style.height;
	obj.style.visibility = 'hidden';
	self.value = obj.value;	
	
	// caret support
	self.className = 'guiInputCode';
	
    if (navigator.userAgent.match('MSIE')) {
	    self.onclick = function() {storeCaret(this);}
	    self.onselect = function() {storeCaret(this);}
	    self.onkeyup = function() {storeCaret(this);}
	    self.onkeydown = function() { AllowTabCharacter();}
	}
	
	self.getCode = function() {
		return this.value;
	}
	
	self.insertCode = function(code) {
	    insertAtCaret(self, code)
	}
	
	return self;
}


textareaEditor.run = function() {
	t = document.getElementsByTagName('textarea');
	for(var i=0,n=t.length;i<n;i++) {
		if(t[i].className.match('codepress')) {
			id = t[i].id;
			eval(id+' = new textareaEditor(t[i])');
			t[i].id = id+'_stub';
			t[i].parentNode.insertBefore(eval(id), t[i]);
		} 
	}
}

if(window.attachEvent) window.attachEvent('onload',textareaEditor.run);
else window.addEventListener('DOMContentLoaded',textareaEditor.run,false);

// Ctrl + S support
    var ctrlDown = false;
        var shiftDown = false;
        var keycode = 0		

        function shortcutCheckKeysDown(e) {
            	
    	        ctrlDown = e.ctrlKey;
	            shiftDown = e.shiftKey;
	            keycode = e.keyCode;
        	    if (ctrlDown && keycode == 83) {
	                 doSubmit();
                    if (window.addEventListener) {
                        e.preventDefault();
                    } else
	                    return false;
	            }
	         }	

        function shortcutCheckKeysUp(e) {
	            ctrlDown = e.ctrlKey;
	            shiftDown = e.shiftKey;
        }
        
        function shortcutCheckKeysPressFirefox(e) {
        	    if (ctrlDown && keycode == 83)
        	        e.preventDefault();
        }
        
        if (window.addEventListener) {
            document.addEventListener('keyup', shortcutCheckKeysUp, false);
            document.addEventListener('keydown', shortcutCheckKeysDown, false);
            document.addEventListener('keypress', shortcutCheckKeysPressFirefox, false);
        } else {
		    document.attachEvent( "onkeyup", shortcutCheckKeysUp);
            document.attachEvent("onkeydown", shortcutCheckKeysDown);            
        }


// Old caret methods
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
 }
}	

//////////////////////////////////////
// CARET funktioner
//////////////////////////////////////
var tempCaretEl;
  
	function storeCaret (editEl) {
		if (editEl.createTextRange)
			editEl.currRange = document.selection.createRange().duplicate();
			
	}
	
	function setCaretToEnd (el) {
		if (el.createTextRange) {
			var v = el.value;
			var r = el.createTextRange();
			r.moveStart('character', v.length);
			r.select();
		}
	}
	
	function insertAtEnd (el, txt) {
		txt = caretTextUnencode(txt);
		el.value += txt;
		setCaretToEnd (el);
	}
	
	function insertAtCaret (el, txt) {
		txt = caretTextUnencode(txt);
		if (el.currRange) {
			el.currRange.text =
				el.currRange.text.charAt(el.currRange.text.length - 1) != ' ' ? txt :
			txt + ' ';
			el.currRange.select();

		}
		else
			insertAtEnd(el, txt);
	}
	
	function insertAtCaretAndMove (el, txt, move) {
		txt = caretTextUnencode(txt);
		if (el.currRange) {
			el.currRange.text =
				el.currRange.text.charAt(el.currRange.text.length - 1) != ' ' ? txt :
			txt + ' ';
			el.currRange.moveStart('character', move);
			el.currRange.moveEnd('character',move);
			el.currRange.select();
		}
		else
			insertAtEnd(el, txt);
	}	
	
	function caretTextUnencode(txt) {
		return txt;//.replace(/\&quot;/gi,"\"")
	}
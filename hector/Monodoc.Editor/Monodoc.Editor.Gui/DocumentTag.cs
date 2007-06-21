//
// DocumentEditor.cs: TextView based class that represent the editor for Monodoc documentation..
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentTag : TextTag {
	private bool is_attribute;
	private bool is_element;
	private bool has_text;
	
	public DocumentTag (string tagName) : base (tagName)
	{
		Initialize ();
	}
	
	public void Initialize () 
	{
		is_attribute = is_element = has_text = false;
	}
	
	public bool IsAttribute {
		get {
			return is_attribute;
		}
		
		set {
			is_attribute = value;
		}
	}
	
	public bool IsElement {
		get {
			return is_element;
		}
		
		set {
			is_element = value;
		}
	}
	
	public bool HasText {
		get {
			return has_text;
		}
		
		set {
			has_text = value;
		}
	}
}
}
//
// DocumentTab.cs: ScrolledWindow based class that represents a tab in notebook that contains a document.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentTab : Gtk.ScrolledWindow {
	private DocumentEditor editor;
	private Label title_label;
	string title;
	
	public DocumentTab () : base ()
	{
		InitializeProperties ();
		
		editor = new DocumentEditor ();
		this.Add (editor);
		
		title = "Untitled";
		title_label = new Label (title);
	}
	
	private void InitializeProperties ()
	{
		this.CanFocus = true;
		this.VscrollbarPolicy = PolicyType.Automatic;
		this.HscrollbarPolicy = PolicyType.Automatic;
		this.ShadowType = ((ShadowType) 1);
	}
	
	public Label TitleLabel {
		get {
			return title_label;
		}
	}
	
	public string Title {
		get {
			return title;
		}
		
		set {
			title = title_label.Text = value;
		}
	}
	
	public TextBuffer Buffer {
		get {
			return editor.Buffer;
		}
		
		set {
			editor.Buffer = value;
		}
	}
}
}
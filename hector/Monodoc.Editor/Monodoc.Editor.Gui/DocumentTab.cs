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
	private Notebook doc_tabs;
	private DocumentEditor editor;
	private HBox tab_label;
	private Label title_label;
	
	public DocumentTab (Notebook docTabs) : base ()
	{
		doc_tabs = docTabs;
		InitializeProperties ();
		
		editor = new DocumentEditor ();
		this.Add (editor);
		
		tab_label = new HBox (false, 2);
		title_label = new Label ("Untitled");
		
		// Close tab button
		Button tabClose = new Button ();
		Image img = new Image (Stock.Close, IconSize.SmallToolbar);
		tabClose.Add (img);
		tabClose.Relief = ReliefStyle.None;
		tabClose.SetSizeRequest (23, 23);
		tabClose.Clicked += new EventHandler (OnTabClose);
		
		tab_label.PackStart (title_label, true, true, 0);
		tab_label.PackStart (tabClose, false, false, 2);
		
		tab_label.ShowAll ();
	}
	
	private void InitializeProperties ()
	{
		this.CanFocus = true;
		this.VscrollbarPolicy = PolicyType.Automatic;
		this.HscrollbarPolicy = PolicyType.Automatic;
		this.ShadowType = ((ShadowType) 1);
	}
	
	public HBox TabLabel {
		get {
			return tab_label;
		}
	}
	
	public string Title {
		get {
			return title_label.Text;
		}
		
		set {
			title_label.Text = value;
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
	
	private void OnTabClose (object sender, EventArgs args)
	{
		doc_tabs.RemovePage (doc_tabs.PageNum (this));
		doc_tabs.ShowTabs = (doc_tabs.NPages > 1);
	}
}
}
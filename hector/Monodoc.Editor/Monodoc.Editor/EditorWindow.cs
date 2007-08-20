//
// EditorWindow.cs: Main window of the app.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;
using System.IO;
using Monodoc.Editor.Gui;
using Monodoc.Editor.Utils;

namespace Monodoc.Editor {
public partial class EditorWindow : Gtk.Window {
	private DocumentTab current_tab;
	private DocumentUndoManager current_manager;
	private Notebook nb_tabs;
	private uint id = 1;
	
	public EditorWindow () : base (Gtk.WindowType.Toplevel)
	{
		this.Build ();
		
		GLib.LogFunc logfunc = new GLib.LogFunc (GLib.Log.PrintTraceLogFunction);
		GLib.Log.SetLogHandler ("Gtk", GLib.LogLevelFlags.All, logfunc);
		
		nb_tabs = new Notebook ();
		nb_tabs.CanFocus = true;
		nb_tabs.Scrollable = true;
		
		edit_container.Add (nb_tabs);
		AddTab ();
		
		status_bar.Push (id, "Welcome");
	}
	
	private void AddTab ()
	{
		current_tab = new DocumentTab (nb_tabs);
		current_manager = current_tab.Buffer.Undoer;
		current_manager.UndoChanged += UndoChanged;
		nb_tabs.AppendPage (current_tab, current_tab.TabLabel);
		nb_tabs.ShowTabs = (nb_tabs.NPages > 1);
		nb_tabs.ShowAll ();
		nb_tabs.CurrentPage = nb_tabs.PageNum (current_tab);
		nb_tabs.SwitchPage += new SwitchPageHandler (OnChangeTab);
	}
	
	private void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	private void OnQuitActivated(object sender, System.EventArgs e)
	{
		Application.Quit ();
	}

	private void OnOpenActivated(object sender, System.EventArgs e)
	{
		string filename = String.Empty;
		OpenDocDialog dialog = new OpenDocDialog ();
		if (dialog.Run () == (int) ResponseType.Ok)
			filename = dialog.Document;
			
		dialog.Destroy ();
		
		if (filename != String.Empty) {
			try {
				MonoDocument doc = new MonoDocument (filename);
				
				if (!current_tab.Title.Equals ("Untitled"))
					AddTab ();
				
				current_tab.Title = doc.Filename;
				current_tab.Buffer.Load (doc);
				SaveAs.Sensitive = Save.Sensitive = true;
			} catch (ArgumentException emsg) {
				// TODO: Add message dialog about error.
				Console.WriteLine (emsg.Message);
			}
		}
	}

	private void OnSaveAsActivated (object sender, System.EventArgs e)
	{
		string filename = String.Empty;
		SaveDocDialog dialog = new SaveDocDialog (current_tab.Title);
		if (dialog.Run () == (int) ResponseType.Ok)
			filename = dialog.Document;
		
		dialog.Destroy ();
		
		try {
			if (filename != String.Empty)
				using (FileStream fileStream = new FileStream (filename, FileMode.CreateNew)) {
					using (StreamWriter streamWriter = new StreamWriter (fileStream)) {
						streamWriter.Write (DocumentBufferArchiver.Serialize (current_tab.Buffer));
					}
				}
		} catch (IOException emsg) {
			//TODO: Add message dialog about error.
			Console.WriteLine (emsg.Message);
		}
	}

	private void OnSaveActivated (object sender, System.EventArgs e)
	{
		Console.WriteLine ("Serialize: \n{0}", DocumentBufferArchiver.Serialize (current_tab.Buffer));
	}
	
	private void OnChangeTab (object sender, SwitchPageArgs args)
	{
		current_tab = (DocumentTab) nb_tabs.GetNthPage((int) args.PageNum);
		current_manager = current_tab.Buffer.Undoer;
		
		Undo.Sensitive = current_manager.CanUndo;
		Redo.Sensitive = current_manager.CanRedo;
	}

	private void OnCloseFileActivated (object sender, System.EventArgs e)
	{
		nb_tabs.RemovePage (nb_tabs.PageNum (current_tab));
		nb_tabs.ShowTabs = (nb_tabs.NPages > 1);
	}

	private void OnAboutActivated (object sender, System.EventArgs e)
	{
		AboutEditorDialog dialog = new AboutEditorDialog ();
		
		dialog.Run ();
		
		dialog.Destroy ();
	}

	private void OnCopyActivated (object sender, System.EventArgs e)
	{
		Clipboard cb = Clipboard.Get (Gdk.Selection.Clipboard);
		current_tab.Buffer.CopyClipboard (cb);
	}
	
	private void OnUndoActivated (object sender, System.EventArgs e)
	{
		if (current_manager.CanUndo) {
			Console.WriteLine ("Running Undo");
			current_manager.Undo ();
		}
	}
	
	private void OnRedoActivated (object sender, System.EventArgs e)
	{
		if (current_manager.CanRedo) {
			Console.WriteLine ("Running Redo");
			current_manager.Redo ();
		}
	}
	
	private void UndoChanged (object sender, System.EventArgs e)
	{
		Undo.Sensitive = current_manager.CanUndo;
		Redo.Sensitive = current_manager.CanRedo;
	}
}
}
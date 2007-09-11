using System;

using Gtk;

namespace Mono.Debugger.Frontend
{
	public abstract class DebuggerPad: Gtk.ScrolledWindow
	{
		DebuggerService debuggerService;
		Gtk.TreeStore gtkStore;
		Gtk.TreeView gtkTree;
		
		protected DebuggerService DebuggerService {
			get { return debuggerService; }
		}
		
		protected TreeStore GtkStore {
			get { return gtkStore; }
		}
		
		protected TreeView GtkTree {
			get { return gtkTree; }
		}
		
		public DebuggerPad(DebuggerService debuggerService, Type[] columnTypes)
		{
			this.debuggerService = debuggerService;
			this.gtkStore = new Gtk.TreeStore(columnTypes);
			this.gtkTree = new Gtk.TreeView(gtkStore);
			
			this.ShadowType = ShadowType.In;
			gtkTree.RulesHint = true;
			gtkTree.HeadersVisible = true;
			
			Add(gtkTree);
			ShowAll();
			
			// Regularly update the GUI
			GLib.Timeout.Add(50, ReceiveUpdatesTimer);
		}
		
		protected TreeViewColumn AddImageColumn(string columnHeader, int columnIndex)
		{
			TreeViewColumn column = new TreeViewColumn();
			CellRenderer iconRenderer = new CellRendererPixbuf();
			column.Title = columnHeader;
			column.PackStart(iconRenderer, false);
			column.AddAttribute(iconRenderer, "pixbuf", columnIndex);
			column.Resizable = true;
			column.Alignment = 0.0f;
			gtkTree.AppendColumn(column);
			
			return column;
		}
		
		protected TreeViewColumn AddImageTextColumn(string columnHeader, int imageColumnIndex, int textColumnIndex)
		{
			TreeViewColumn column = new TreeViewColumn();
			CellRenderer textRenderer = new CellRendererText();
			CellRenderer iconRenderer = new CellRendererPixbuf();
			column.Title = columnHeader;
			column.PackStart(iconRenderer, false);
			column.PackStart(textRenderer, true);
			column.AddAttribute(iconRenderer, "pixbuf", imageColumnIndex);
			column.AddAttribute(textRenderer, "text", textColumnIndex);
			column.Resizable = true;
			column.Alignment = 0.0f;
			gtkTree.AppendColumn(column);
			
			return column;
		}
		
		protected TreeViewColumn AddTextColumn(string columnHeader, int columnIndex)
		{
			TreeViewColumn column = new TreeViewColumn();
			CellRenderer textRenderer = new CellRendererText();
			column.Title = columnHeader;
			column.PackStart(textRenderer, true);
			column.AddAttribute(textRenderer, "text", columnIndex);
			column.Resizable = true;
			column.Alignment = 0.0f;
			gtkTree.AppendColumn(column);
			
			return column;
		}
		
		bool ReceiveUpdatesTimer()
		{
			ReceiveUpdates();
			return true;
		}
		
		public abstract void ReceiveUpdates();
	}
}

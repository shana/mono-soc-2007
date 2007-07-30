using System;

using Gtk;

namespace Mono.Debugger.Frontend
{
	public static class GtkTreeStoreUpdater
	{
		/// <summary>
		/// Use a list of modifications to update Gtk.TreeStore
		/// </summary>
		public static void Update(RemoteTreeStore remoteStore, Gtk.TreeStore gtkStore)
		{
			RemoteTreeModification[] modifications = remoteStore.GetModifications();
			if (modifications.Length > 0) {
				Console.WriteLine(String.Format("Received {0} modifications from {1}", modifications.Length, remoteStore));
			}
			foreach(RemoteTreeModification mod in modifications) {
				Console.WriteLine(mod);
				if (mod is RemoteTreeModification.InsertNode) {
					RemoteTreeModification.InsertNode insertMod = (RemoteTreeModification.InsertNode)mod;
					if (insertMod.ParentNodePath.Indices.Length == 0) {
						// Insert to the top level
						gtkStore.InsertNode(insertMod.NodeIndex);
					} else {
						TreeIter it;
						gtkStore.GetIter(out it, new TreePath(insertMod.ParentNodePath.Indices));
						gtkStore.InsertNode(it, insertMod.NodeIndex);
					}
				}
				if (mod is RemoteTreeModification.RemoveNode) {
					RemoteTreeModification.RemoveNode removeMod = (RemoteTreeModification.RemoveNode)mod;
					TreeIter it;
					gtkStore.GetIter(out it, new TreePath(removeMod.NodePath.Indices));
					gtkStore.Remove(ref it);
				}
				if (mod is RemoteTreeModification.UpdateNode) {
					RemoteTreeModification.UpdateNode updateMod = (RemoteTreeModification.UpdateNode)mod;
					// Igonre the root node
					if (updateMod.NodePath.IsRoot) {
						continue;
					}
					TreeIter it;
					gtkStore.GetIter(out it, new TreePath(updateMod.NodePath.Indices));
					object value = updateMod.Value;
					// If it is image, dereference it
					if (value is PixmapRef) {
						value = ((PixmapRef)value).GetPixbuf();
					}
					gtkStore.SetValue(it, updateMod.ColumnIndex, value);
				}
			}
		}
	}
}

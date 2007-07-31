//
// Authors:
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Gtk;
using System;
using System.Threading;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Designer
{
	public partial class TriggersEditorWidget : Gtk.Bin
	{
		public event EventHandler ContentChanged;
		
		private ISchemaProvider schemaProvider;
		
		private ListStore store;
		private ListStore storeTypes;
		private ListStore storeEvents;
		
		private const int colNameIndex = 0;
		private const int colTypeIndex = 1;
		private const int colEventIndex = 2;
		private const int colPositionIndex = 3;
		private const int colActiveIndex = 4;
		private const int colCommentIndex = 5;
		private const int colObjIndex = 6;
		
		public TriggersEditorWidget (ISchemaProvider schemaProvider)
		{
			if (schemaProvider == null)
				throw new ArgumentNullException ("schemaProvider");
			
			this.schemaProvider = schemaProvider;
			
			this.Build();
			
			store = new ListStore (typeof (string), typeof (string), typeof (string), typeof (string), typeof (bool), typeof (string), typeof (object));
			storeTypes = new ListStore (typeof (string));
			storeEvents = new ListStore (typeof (string));
			listTriggers.Model = store;
			
			foreach (string name in Enum.GetNames (typeof (TriggerType)))
			         storeTypes.AppendValues (name);
			foreach (string name in Enum.GetNames (typeof (TriggerEvent)))
			         storeEvents.AppendValues (name);
			
			TreeViewColumn colName = new TreeViewColumn ();
			TreeViewColumn colType = new TreeViewColumn ();
			TreeViewColumn colEvent = new TreeViewColumn ();
			TreeViewColumn colPosition = new TreeViewColumn ();
			TreeViewColumn colActive = new TreeViewColumn ();
			TreeViewColumn colComment = new TreeViewColumn ();
			
			colName.Title = GettextCatalog.GetString ("Name");
			colType.Title = GettextCatalog.GetString ("Type");
			colEvent.Title = GettextCatalog.GetString ("Event");
			colPosition.Title = GettextCatalog.GetString ("Position");
			colActive.Title = GettextCatalog.GetString ("Active");
			colComment.Title = GettextCatalog.GetString ("Comment");
			
			colType.MinWidth = 120;
			colEvent.MinWidth = 120;
			
			CellRendererText nameRenderer = new CellRendererText ();
			CellRendererCombo typeRenderer = new CellRendererCombo ();
			CellRendererCombo eventRenderer = new CellRendererCombo ();
			CellRendererText positionRenderer = new CellRendererText ();
			CellRendererToggle activeRenderer = new CellRendererToggle ();
			CellRendererText commentRenderer = new CellRendererText ();
			
			nameRenderer.Editable = true;
			nameRenderer.Edited += new EditedHandler (NameEdited);
			
			typeRenderer.Model = storeTypes;
			typeRenderer.TextColumn = 0;
			typeRenderer.Editable = true;
			typeRenderer.Edited += new EditedHandler (TypeEdited);
			
			eventRenderer.Model = storeEvents;
			eventRenderer.TextColumn = 0;
			eventRenderer.Editable = true;
			eventRenderer.Edited += new EditedHandler (EventEdited);
			
			positionRenderer.Editable = true;
			positionRenderer.Edited += new EditedHandler (PositionEdited);
			
			activeRenderer.Activatable = true;
			activeRenderer.Toggled += new ToggledHandler (ActiveToggled);
			
			commentRenderer.Editable = true;
			commentRenderer.Edited += new EditedHandler (CommentEdited);

			colName.PackStart (nameRenderer, true);
			colType.PackStart (typeRenderer, true);
			colEvent.PackStart (eventRenderer, true);
			colPosition.PackStart (positionRenderer, true);
			colActive.PackStart (activeRenderer, true);
			colComment.PackStart (commentRenderer, true);

			colName.AddAttribute (nameRenderer, "text", colNameIndex);
			colType.AddAttribute (typeRenderer, "text", colTypeIndex);
			colEvent.AddAttribute (eventRenderer, "text", colEventIndex);
			colPosition.AddAttribute (positionRenderer, "text", colPositionIndex);
			colActive.AddAttribute (activeRenderer, "active", colActiveIndex);
			colComment.AddAttribute (commentRenderer, "text", colCommentIndex);
			
			listTriggers.AppendColumn (colName);
			listTriggers.AppendColumn (colType);
			listTriggers.AppendColumn (colEvent);
			listTriggers.AppendColumn (colPosition);
			listTriggers.AppendColumn (colActive);
			listTriggers.AppendColumn (colComment);
			
			ShowAll ();
		}

		protected virtual void RemoveClicked (object sender, System.EventArgs e)
		{
		}

		protected virtual void AddClicked (object sender, System.EventArgs e)
		{
		}
		
		private void NameEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
				if (!string.IsNullOrEmpty (args.NewText)) {
					store.SetValue (iter, colNameIndex, args.NewText);
				} else {
					string oldText = store.GetValue (iter, colNameIndex) as string;
					(sender as CellRendererText).Text = oldText;
				}
			}
		}
		
		private void TypeEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
				foreach (string name in Enum.GetNames (typeof (TriggerType))) {
					if (args.NewText == name) {
						store.SetValue (iter, colTypeIndex, args.NewText);
						return;
					}
				}
				string oldText = store.GetValue (iter, colTypeIndex) as string;
				(sender as CellRendererText).Text = oldText;
			}
		}
		
		private void EventEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
				foreach (string name in Enum.GetNames (typeof (TriggerEvent))) {
					if (args.NewText == name) {
						store.SetValue (iter, colEventIndex, args.NewText);
						return;
					}
				}
				string oldText = store.GetValue (iter, colEventIndex) as string;
				(sender as CellRendererText).Text = oldText;
			}
		}
		
		private void PositionEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
				int len;
				if (!string.IsNullOrEmpty (args.NewText) && int.TryParse (args.NewText, out len)) {
					store.SetValue (iter, colPositionIndex, args.NewText);
				} else {
					string oldText = store.GetValue (iter, colPositionIndex) as string;
					(sender as CellRendererText).Text = oldText;
				}
			}
		}
		
		private void ActiveToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) store.GetValue (iter, colActiveIndex);
	 			store.SetValue (iter, colActiveIndex, !val);
	 		}
		}
		
		private void CommentEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path))
				store.SetValue (iter, colCommentIndex, args.NewText);
		}
		
		public virtual bool Validate ()
		{
			return false;
		}
		
		protected virtual void EmitContentChanged ()
		{
			if (ContentChanged != null)
				ContentChanged (this, EventArgs.Empty);
		}
	}
}

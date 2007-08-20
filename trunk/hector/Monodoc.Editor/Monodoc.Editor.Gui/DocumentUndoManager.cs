//
// DocumentUndoManager.cs: Class that implements a manager for each buffer that handles
// the undo and redo of contents in the buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;
using System.Collections;

namespace Monodoc.Editor.Gui {
public class ChopBuffer : TextBuffer {
	public ChopBuffer (TextTagTable table) : base  (table)
	{
	}
	
	public TextRange AddChop (TextIter startIter, TextIter endIter)
	{
		int chop_start, chop_end;
		TextIter current_end = EndIter;
		
		chop_start = EndIter.Offset;
		InsertRange (ref current_end, startIter, endIter);
		chop_end = EndIter.Offset;
		
		return new TextRange (GetIterAtOffset (chop_start), GetIterAtOffset (chop_end));
	}
}

public class DocumentUndoManager {
	private uint frozen_cnt;
	private bool try_merge;
	private DocumentBuffer buffer;
	private ChopBuffer chop_buffer;
	
	private Stack undo_stack;
	private Stack redo_stack;
	
	public DocumentUndoManager (DocumentBuffer buffer)
	{
		frozen_cnt = 0;
		try_merge = false;
		undo_stack = new Stack ();
		redo_stack = new Stack ();
		
		this.buffer  = buffer;
		chop_buffer = new ChopBuffer (buffer.TagTable);
		
		buffer.InsertText += OnInsertText;
		buffer.DeleteRange += OnDeleteRange;
	}
	
	public bool CanUndo {
		get {
			return undo_stack.Count > 0;
		}
	}
	
	public bool CanRedo {
		get {
			return redo_stack.Count > 0;
		}
	}
	
	public void Undo ()
	{
		UndoRedo (undo_stack, redo_stack, true);
	}
	
	public void Redo ()
	{
		UndoRedo (redo_stack, undo_stack, false);
	}
	
	public event EventHandler UndoChanged;
	
	public void FreezeUndo ()
	{
		++frozen_cnt;
	}
	
	public void ThrawUndo ()
	{
		--frozen_cnt;
	}
	
	private void UndoRedo (Stack popFrom, Stack pushTo, bool isUndo)
	{
		if (popFrom.Count > 0) {
			EditAction action = (EditAction) popFrom.Pop ();
			
			FreezeUndo ();
			if (isUndo)
				action.Undo (buffer);
			else
				action.Redo (buffer);
			ThrawUndo ();
			
			pushTo.Push (action);
			
			// Lock merges until a new undoable event comes in
			try_merge = false;
			
			if (popFrom.Count == 0 || pushTo.Count == 1)
				if (UndoChanged != null)
					UndoChanged (this, new EventArgs ());
		}
	}
	
	private void ClearActionStack (Stack stack)
	{
		foreach (EditAction action in stack)
			action.Destroy ();
		
		stack.Clear ();
	}
	
	public void ClearUndoHistory ()
	{
		ClearActionStack (undo_stack);
		ClearActionStack (redo_stack);
		
		if (UndoChanged != null)
			UndoChanged (this, new EventArgs ());
	}
	
	public void AddUndoAction (EditAction action)
	{
		if (try_merge && undo_stack.Count > 0) {
			EditAction top = (EditAction) undo_stack.Peek ();
			
			if (top.CanMerge (action)) {
				// Merging object should handle freeing
				// action's resources if needed.
				top.Merge (action);
				return;
			}
		}
		
		undo_stack.Push (action);
		
		// Clear the redo stack
		ClearActionStack (redo_stack);
		
		// Try to merge new incoming actions
		try_merge = true;
		
		// Have undoable actions now
		if (undo_stack.Count == 1) {
			if (UndoChanged != null) {
				UndoChanged (this, new EventArgs ());
			}
		}
	}
	
	// Action-creating event handlers...
	
//	[GLib.ConnectBefore]
	private void OnInsertText (object sender, InsertTextArgs args)
	{
		if (frozen_cnt == 0) {
			InsertAction action = new InsertAction (args.Pos, args.Text, args.Text.Length, chop_buffer);
			AddUndoAction (action);
		}
	}
	
	[GLib.ConnectBefore]
	private void OnDeleteRange (object sender, DeleteRangeArgs args)
	{
		if (frozen_cnt == 0) {
			EraseAction action = new EraseAction (args.Start, args.End, chop_buffer);
			AddUndoAction (action);
		}
	}
}
}

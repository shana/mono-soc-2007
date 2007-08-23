//
// DocumentActions.cs: Contains several implementations of interface EditAction that
// defines a contract for any document action.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using Gtk;
using System;
using System.Collections;

namespace Monodoc.Editor.Gui {
public abstract class SplitterAction : EditAction {
	protected TextRange chop;
	
	public TextRange Chop {
		get {
			return chop;
		}
	}
	
	public abstract void Undo (TextBuffer buffer);
	public abstract void Redo (TextBuffer buffer);
	public abstract void Merge (EditAction action);
	public abstract bool CanMerge (EditAction action);
	public abstract void Destroy ();
}

public class InsertAction : SplitterAction {
	private int index;
	private bool is_paste;
	
	public InsertAction (TextIter start, string text, int length, ChopBuffer chop_buf)
	{
		#if DEBUG
		Console.WriteLine ("DEBUG: InsertAction: {0}", text);
		Console.WriteLine ("DEBUG: Insert Offset: {0} Char {1}", start.Offset, start.Char);
		#endif
		
		this.index = start.Offset - length;
		this.is_paste = length > 1;
		
		TextIter indexIter = start.Buffer.GetIterAtOffset (index);
		#if DEBUG
		Console.WriteLine ("DEBUG: Start Offset: {0} Char: {1}", indexIter.Offset, indexIter.Char);
		#endif
		this.chop = chop_buf.AddChop (indexIter, start);
		#if DEBUG
		Console.WriteLine ("DEBUG: Chop Text: {0}", chop.Text);
		#endif
	}
	
	public override void Undo (TextBuffer buffer)
	{
		#if DEBUG
		Console.WriteLine ("DEBUG: Chop Text: {0} Length: {1}", chop.Text, chop.Length);
		#endif
		
		TextIter startIter = buffer.GetIterAtOffset (index);
		TextIter endIter = buffer.GetIterAtOffset (index + chop.Length);
		buffer.Delete (ref startIter, ref endIter);
		buffer.MoveMark (buffer.InsertMark, buffer.GetIterAtOffset (index));
		buffer.MoveMark (buffer.SelectionBound, buffer.GetIterAtOffset (index));
	}
	
	public override void Redo (TextBuffer buffer)
	{
		TextIter insertIter = buffer.GetIterAtOffset (index);
		buffer.InsertRange (ref insertIter, chop.Start, chop.End);
		buffer.MoveMark (buffer.SelectionBound, buffer.GetIterAtOffset (index));
		buffer.MoveMark (buffer.InsertMark, buffer.GetIterAtOffset (index + chop.Length));
	}
	
	public override void Merge (EditAction action)
	{
		InsertAction insert = (InsertAction) action;
		chop.End = insert.Chop.End;
		
		insert.chop.Destroy ();
	}
	
	public override bool CanMerge (EditAction action)
	{
		InsertAction insert = action as InsertAction;
		if (insert == null)
			return false;
		
		// Don't group text pastes
		if (is_paste || insert.is_paste)
			return false;
		
		// Must meet each other
		if (insert.index != index + chop.Length)
			return false;
		
		// Don't group more than one line (inclusive)
		if (chop.Text [0] == '\n')
			return false;
		
		// Don't group more than one word (exclusive)
		if (insert.chop.Text [0] == ' ' || insert.chop.Text [0] == '\t')
			return false;
		
		return true;
	}
	
	public override void Destroy ()
	{
		chop.Erase ();
		chop.Destroy ();
	}
}

public class EraseAction : SplitterAction {
	private int start;
	private int end;
	private bool is_forward;
	private bool is_cut;
	private bool whole_region;
	
	public EraseAction (TextIter startIter, TextIter endIter, ChopBuffer chop_buf)
	{
		#if DEBUG
		Console.WriteLine ("DEBUG: EraseAction: {0}", startIter.GetText (endIter));
		Console.WriteLine ("DEBUG: Start Offset: {0} Char: {1}", startIter.Offset, startIter.Char);
		Console.WriteLine ("DEBUG: End Offset: {0} Char: {1}", endIter.Offset, endIter.Char);
		#endif
		
		this.start = startIter.Offset;
		this.end = endIter.Offset;
		this.is_cut = end - start > 1;
		
		TextIter previousIter = startIter.Buffer.GetIterAtOffset (start - 1);
		bool startsRegion = previousIter.Char.Equals ("[");
		bool endsRegion = endIter.Char.Equals ("]");
		this.whole_region = startsRegion && endsRegion;
		
		TextIter insert = startIter.Buffer.GetIterAtMark (startIter.Buffer.InsertMark);
		this.is_forward = insert.Offset <= start;
		
		this.chop = chop_buf.AddChop (startIter, endIter);
	}
	
	public override void Undo (TextBuffer buffer)
	{
		TextIter startIter, endIter;
		
		#if DEBUG
		Console.WriteLine ("DEBUG: Whole Region {0}", whole_region);
		#endif
		
		startIter = buffer.GetIterAtOffset (start);
		buffer.InsertRange (ref startIter, chop.Start, chop.End);
		
		if (whole_region) {
			endIter = buffer.GetIterAtOffset (startIter.Offset + 56);
			buffer.Delete (ref startIter, ref endIter);
		}
		
		buffer.MoveMark (buffer.InsertMark, buffer.GetIterAtOffset (is_forward ? start : end));
		buffer.MoveMark (buffer.SelectionBound, buffer.GetIterAtOffset (is_forward ? end : start));
	}
	
	public override void Redo (TextBuffer buffer)
	{
		TextIter startIter = buffer.GetIterAtOffset (start);
		TextIter endIter = buffer.GetIterAtOffset (end);
		
		buffer.Delete (ref startIter, ref endIter);
		
		buffer.MoveMark (buffer.InsertMark, buffer.GetIterAtOffset (start));
		buffer.MoveMark (buffer.SelectionBound, buffer.GetIterAtOffset (start));
	}
	
	public override void Merge (EditAction action)
	{
		EraseAction erase = (EraseAction) action;
		if (start == erase.start) {
			end += erase.end - erase.start;
			chop.End = erase.chop.End;
			
			// Delete the marks, leave the text
			erase.chop.Destroy ();
		} else {
			start = erase.start;
			
			TextIter chopStart = chop.Start;
			chop.Buffer.InsertRange (ref chopStart, erase.chop.Start, erase.chop.End);
			
			// Delete the marks and text
			erase.Destroy ();
		}
	}
	
	public override bool CanMerge (EditAction action)
	{
		EraseAction erase = action as EraseAction;
		if (erase == null)
			return false;
		
		// Don't group separate text cuts
		if (is_cut || erase.is_cut)
			return false;
		
		// Must meet eachother
		if (start != (is_forward ? erase.start : erase.end))
			return false;
		
		// Don't group deletes with backspaces
		if (is_forward != erase.is_forward)
			return false;
		
		// Group if something other than text was deleted
		// (e.g. an email image)
		if (chop.Text.Length == 0 || erase.chop.Text.Length == 0)
			return true;
		
		// Don't group more than one line (inclusive)
		if (chop.Text[0] == '\n')
			return false;
		
		// Don't group more than one word (exclusive)
		if (erase.chop.Text[0] == ' ' || erase.chop.Text[0] == '\t')
			return false;
		
		return true;
	}
	
	public override void Destroy ()
	{
		chop.Erase ();
		chop.Destroy ();
	}

}
}

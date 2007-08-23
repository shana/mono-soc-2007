//
// DocumentBuffer.cs: TextBuffer based class that represent the buffer of the Monodoc documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;
using Monodoc.Editor.Utils;

namespace Monodoc.Editor.Gui {
public class DocumentBuffer : TextBuffer {
	private bool document_loaded;
	private DocumentUndoManager undo_manager;
	
	public DocumentBuffer () : base (new DocumentTagTable ())
	{
		undo_manager = new DocumentUndoManager (this);
		InsertText += OnInsertText;
		DeleteRange += OnDeleteRange;
		TagRemoved += OnRemoved;
		document_loaded = false;
	}
	
	public DocumentUndoManager Undoer {
		get {
			return undo_manager;
		}
	}
	
	public void Load (MonoDocument document)
	{
		undo_manager.FreezeUndo ();
		DocumentBufferArchiver.Deserialize (this, document.Xml);
		document_loaded = true;
		undo_manager.ThrawUndo ();
	}
	
	private void OnInsertText (object sender, InsertTextArgs args)
	{
		int offset = args.Pos.Offset - args.Length;
		string text = args.Text;
		
		TextIter previousIter = GetIterAtOffset (offset - 1); // Previous is the iter before the insert offset.
		TextIter startIter = GetIterAtOffset (offset);
		TextIter endIter = GetIterAtOffset (offset + text.Length);
		// Only handle special inserting cases when we have a fully loaded document.
		if (document_loaded) {
			#if DEBUG
			Console.WriteLine ("DEBUG: Inserting: {0}", text);
			Console.WriteLine ("DEBUG: Start Offset: {0} Char: {1}", startIter.Offset, startIter.Char);
			Console.WriteLine ("DEBUG: End Offset: {0} Char: {1}", endIter.Offset, endIter.Char);
			#endif
			
			TextTag lastEnd = DocumentUtils.GetLastTag (endIter);
			TextTag lastPrevious =  DocumentUtils.GetLastTag (previousIter);
			
			if (endIter.BeginsTag (lastEnd) && lastEnd.Editable) {
				#if DEBUG
				Console.WriteLine ("DEBUG: Inserting text at start of editable region.");
				Console.WriteLine ("DEBUG: Tag Name: {0} Char: {1}", lastEnd.Name, endIter.Char);
				#endif
				
				ApplyTag (lastEnd, startIter, endIter);
			} else if (DocumentUtils.TagEndsHere (lastPrevious, previousIter, startIter) && lastPrevious.Editable) {
				#if DEBUG
				Console.WriteLine ("DEBUG: Inserting text at end of editable region.");
				Console.WriteLine ("DEBUG: Tag Name: {0} Char: {1}", lastPrevious.Name, previousIter.Char);
				#endif

				ApplyTag (lastPrevious, startIter, endIter);
			}
		}
	}
	
	private void OnDeleteRange (object sender, DeleteRangeArgs args)
	{
		TextIter startIter = GetIterAtOffset (args.Start.Offset -1);
		TextIter endIter = GetIterAtOffset (args.Start.Offset);
		#if DEBUG
		Console.WriteLine ("DEBUG: Deleting range");
		Console.WriteLine ("DEBUG: Start Offset: {0} Char: {1}", startIter.Offset, startIter.Char);
		Console.WriteLine ("DEBUG: End Offset: {0} Char: {1}", endIter.Offset, endIter.Char);
		#endif
		
		bool startsRegion = startIter.Char.Equals ("[");
		bool endsRegion = endIter.Char.Equals ("]");
		
		if (startsRegion && endsRegion) {
			#if DEBUG
			Console.WriteLine ("DEBUG: Deleting whole editing region.");
			#endif
			
			TextTag last = DocumentUtils.GetAssociatedTextTag (this, DocumentUtils.GetLastTag (startIter));
			InsertWithTags (ref endIter, "Documentation for this section has not yet been entered.", last);
		}
	}
	
	private void OnRemoved (object o, TagRemovedArgs args)
	{
		Console.WriteLine ("DEBUG: Deleting Tag {0}", args.Tag.Name);
	}
}
}
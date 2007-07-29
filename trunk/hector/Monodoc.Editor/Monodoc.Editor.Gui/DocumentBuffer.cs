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
	public bool document_loaded;

	public DocumentBuffer (TextTagTable tags) : base (tags)
	{
	}
	
	public DocumentBuffer () : base (new DocumentTagTable ())
	{
		TagRemoved += OnRemoved;
		document_loaded = false;
	}

	public void Load (MonoDocument document)
	{
		DocumentBufferArchiver.Deserialize (this, document.Xml);
		document_loaded = true;
	}

	protected override void OnInsertText (TextIter pos, string text)
	{
		int offset = pos.Offset;

		// Call base method to insert text, we only handle special cases in this override.
		base.OnInsertText (pos, text);

		TextIter previousIter = GetIterAtOffset (offset - 1); // Previous is the iter before the insert offset.
		TextIter startIter = GetIterAtOffset (offset);
		TextIter endIter = GetIterAtOffset (offset + text.Length);

		// Only handle special inserting cases when we have a fully loaded document.
		if (document_loaded) {
			#if DEBUG
			Console.WriteLine ("DEBUG: Inserting: {0}", text);
			Console.WriteLine ("DEBUG: Start Offset: {0} Char {1}", startIter.Offset, startIter.Char);
			Console.WriteLine ("DEBUG: End Offset: {0} Char {1}", endIter.Offset, endIter.Char);
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
				Console.WriteLine ("DEBUG: Tag Name: {0} Char: {1}", last_last.Name, previousIter.Char);
				#endif

				ApplyTag (lastPrevious, startIter, endIter);
			}
		}
	}

	protected override void OnDeleteRange (TextIter startIter, TextIter endIter)
	{
		Console.WriteLine ("DEBUG: Deleting range");
		if (startIter.Offset == endIter.Offset)
			Console.WriteLine ("DEBUG: Zero length range.");
		Console.WriteLine ("DEBUG: Start Offset: {0} Char: {1}", startIter.Offset, startIter.Char);
		Console.WriteLine ("DEBUG: End Offset: {0} Char: {1}", endIter.Offset, endIter.Char);

		int startOffset = startIter.Offset;
		int endOffset = endIter.Offset;

		TextTag last = DocumentUtils.GetLastTag (startIter);
		bool startsRegion = startIter.BeginsTag (last);
		bool endsRegion = !endIter.HasTag (last);
		base.OnDeleteRange (startIter, endIter);
		
		if (startsRegion && endsRegion) {
			Console.WriteLine ("Deleting whole editing region");
			TextIter insertIter = GetIterAtOffset (startOffset);
			InsertWithTags (ref insertIter, "Insert documentation", last);
		}
	}

	private void OnRemoved (object o, TagRemovedArgs args)
	{
		Console.WriteLine ("DEBUG: Deleting Tag {0}", args.Tag.Name);
	}
}
}
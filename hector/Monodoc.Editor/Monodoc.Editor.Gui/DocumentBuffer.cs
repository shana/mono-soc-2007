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
		DeleteRange += OnDelete;
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
		Console.WriteLine ("DEBUG: Inserting Text: {0} at Offset: {1} with Char: {2}", text, pos.Offset, pos.Char);
		base.OnInsertText (pos, text);

		TextIter previousIter = GetIterAtOffset (offset - 1);
		TextIter startIter = GetIterAtOffset (offset);
		TextIter endIter = GetIterAtOffset (offset + text.Length);

		if (document_loaded) {
			Console.WriteLine ("DEBUG: Inserting: {0}", text);
			Console.WriteLine ("DEBUG: Start Offset: {0} Char {1}", startIter.Offset, startIter.Char);
			Console.WriteLine ("DEBUG: End Offset: {0} Char {1}", endIter.Offset, endIter.Char);
			
			TextTag last = endIter.Tags [endIter.Tags.Length - 1];
			TextTag last_last =  previousIter.Tags [previousIter.Tags.Length - 1];

			if (endIter.BeginsTag (last) && last.Editable) {
				Console.WriteLine ("DEBUG: Inserting text at start of editable region.");
				Console.WriteLine ("DEBUG: Tag Name: {0} Char: {1}", last.Name, endIter.Char);

				ApplyTag (last, startIter, endIter);
			} else if (previousIter.HasTag (last_last) && !startIter.HasTag (last_last) && last_last.Editable) {
				Console.WriteLine ("DEBUG: Inserting text at end of editable region.");
				Console.WriteLine ("DEBUG: Tag Name: {0} Char: {1}", last_last.Name, previousIter.Char);

				ApplyTag (last_last, startIter, endIter);
			}
		}
	}

	private void OnRemoved (object o, TagRemovedArgs args)
	{
		Console.WriteLine ("DEBUG: Deleting Tag {0}", args.Tag.Name);
	}

	private void OnDelete (object o, DeleteRangeArgs args)
	{
		Console.WriteLine ("DEBUG: Deleting range");
		if (args.Start.Offset == args.End.Offset)
			Console.WriteLine ("DEBUG: Zero length range.");
		Console.WriteLine ("DEBUG: Start Offset: {0} Char: {1}", args.Start.Offset, args.Start.Char);
		Console.WriteLine ("DEBUG: End Offset: {0} Char: {1}", args.End.Offset, args.End.Char);
	}
}
}
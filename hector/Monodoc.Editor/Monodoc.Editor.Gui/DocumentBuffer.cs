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
		InsertText += OnAddedText;
		TagRemoved += OnRemoved;
		DeleteRange += OnDelete;
		document_loaded = false;
	}

	public void Load (MonoDocument document)
	{
		DocumentBufferArchiver.Deserialize (this, document.Xml);
		document_loaded = true;
	}

	private void OnAddedText (object o, InsertTextArgs args)
	{
		if (document_loaded) {
			Console.WriteLine ("DEBUG: Inserting: {0}, at Offset {1}", args.Text, args.Pos.Offset);
			Console.WriteLine ("DEBUG: Offset: {0} Char {1}", args.Pos.Offset, args.Pos.Char);

			TextIter insertIter = args.Pos;
			int last_index = insertIter.Tags.Length - 1;
			TextTag last = insertIter.Tags [last_index];

			TextIter fooIter = GetIterAtOffset (insertIter.Offset - args.Text.Length - 1);
			TextTag last_last = fooIter.Tags [fooIter.Tags.Length - 1];

			TextIter nextIter = fooIter;
			nextIter.ForwardChar ();

			if (insertIter.BeginsTag (last) && last.Editable) {
				Console.WriteLine ("DEBUG: Inserting text from start of editable region");
				Console.WriteLine ("DEBUG: Tag Name: {0} HasTag: {1} Char: {2}" , last_last.Name, fooIter.HasTag (last_last), fooIter.Char);  

				int offset = insertIter.Offset - args.Text.Length;
				TextIter newIter = GetIterAtOffset (offset);
				ApplyTag (last, newIter, insertIter);
			} else if (fooIter.HasTag (last_last) && !nextIter.HasTag (last_last) && last_last.Editable) {
				Console.WriteLine ("DEBUG: Inserting text from end of editable region");
				Console.WriteLine ("DEBUG: Tag Name: {0} HasTag: {1} Char: {2}" , last_last.Name, fooIter.HasTag (last_last), fooIter.Char);  

				int offset = fooIter.Offset + args.Text.Length + 1;
				TextIter newIter = GetIterAtOffset (offset);
				ApplyTag (last_last, fooIter, newIter);
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
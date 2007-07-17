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
		this.InsertText += OnAddedText;
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
		}
	}
}
}
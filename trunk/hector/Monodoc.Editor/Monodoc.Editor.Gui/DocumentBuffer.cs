//
// DocumentBuffer.cs: TextBuffer based class that represent the buffer of the Monodoc documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentBuffer : TextBuffer {
	public DocumentBuffer (TextTagTable tags) : base (tags)
	{
	}
	
	public DocumentBuffer () : base (new DocumentTagTable ())
	{
	}
}
}
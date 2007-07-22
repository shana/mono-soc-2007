//
// EditAction.cs: Interface that establish a contract for any action that can be undo in the editor.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
	public interface EditAction 
	{
		void Undo (TextBuffer buffer);
		void Redo (TextBuffer  buffer);
		void Destroy ();
	}
}

//
// DocumentUndo.cs:
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using System.Collections;

namespace Monodoc.Editor.Gui {
public class DocumentUndo {
	
	public DocumentUndo ()
	{
	}
}

public class UndoManager {
	DocumentBuffer buffer;
	Stack undo_stack;
	Stack redo_stack;
	
	public UndoManager (DocumentBuffer buffer)
	{
		this.buffer  = buffer;
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
}
}

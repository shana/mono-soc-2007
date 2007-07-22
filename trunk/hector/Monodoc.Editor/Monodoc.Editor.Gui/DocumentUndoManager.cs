//
// DocumentUndoManager.cs: Class that implements a manager for each buffer that handles
// the undo and redo of contents in the buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using System.Collections;

namespace Monodoc.Editor.Gui {
public class DocumentUndoManager {
	DocumentBuffer buffer;
	Stack undo_stack;
	Stack redo_stack;
	
	public DocumentUndoManager (DocumentBuffer buffer)
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

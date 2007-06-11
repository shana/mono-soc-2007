//
// Driver.cs: Main class of app.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor {
class MainClass {
	public static void Main (string[] args)
	{
		Application.Init ();
		EditorWindow editor = new EditorWindow ();
		editor.Show ();
		Application.Run ();
	}
}
}
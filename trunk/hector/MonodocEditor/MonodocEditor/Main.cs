// /home/hector/Projects/mono-soc-2007/hector/MonodocEditor/MonodocEditor/Main.cs created with MonoDevelop
// User: hector at 7:38 AM 5/31/2007
//
//
//
// project created on 5/31/2007 at 7:38 AM
using System;
using Gtk;

namespace MonodocEditor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
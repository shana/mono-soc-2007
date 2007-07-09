//
// AboutEditorDialog.cs: About dialog for the application.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;

namespace Monodoc.Editor {
public class AboutEditorDialog : Gtk.AboutDialog {
	
	public AboutEditorDialog ()
	{
		this.Name = "Monodoc Editor";
		this.Version = "0.1-alpha";
		this.Logo = Gdk.Pixbuf.LoadFromResource ("monodoc.png");
		this.Authors = new string [1] { "Hector E. Gomez Morales <hectoregm@gmail.com>" };
		this.Copyright = "Copyright © 2007 Hector E. Gomez Morales";
	}
}
}

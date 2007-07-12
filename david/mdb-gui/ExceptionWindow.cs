using System;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class ExceptionWindow
	{
		[Widget] protected Window exceptionWindow;
		[Widget] protected Image image;
		
		public ExceptionWindow()
		{
			Glade.XML gxml = new Glade.XML("gui.glade", "exceptionWindow", null);
			gxml.Autoconnect(this);
			
			image.Pixbuf = Pixmaps.Error;
		}
		
		public static void Show()
		{
			ExceptionWindow window = new ExceptionWindow();
			window.exceptionWindow.ShowAll();
		}
	}
}

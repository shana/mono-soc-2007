using Gtk;
using System;

namespace MonoTorrent.Interface
{
	
	
	public class AboutDialog : Gtk.AboutDialog
	{
		
		public AboutDialog() : base()
		{
			Name = "MonoTorrent";
			Authors = new String[]{"Alan McGovern (Library)",
						"Jared Hendry (Interface)"};
			
		}
	}
}

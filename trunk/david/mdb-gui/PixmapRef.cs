using System;
using System.Collections;

namespace Mono.Debugger.Frontend
{
	/// <summary>
	/// Reference to a pixmap based on its name.  It is serializable.
	/// </summary>
	[Serializable]
	public class PixmapRef
	{
		const string resourcePrefix = "Mono.Debugger.Frontend.pixmaps.";
		static Hashtable loadedPixmaps = new Hashtable();
		
		string name;
		
		public string Name {
			get { return name; }
		}
		
		public PixmapRef(string name)
		{
			this.name = name;
		}
		
		public Gdk.Pixbuf GetPixbuf()
		{
			if (loadedPixmaps.Contains(name)) {
				return (Gdk.Pixbuf)loadedPixmaps[name];
			} else {
				Gdk.Pixbuf pixbuf = Gdk.Pixbuf.LoadFromResource(resourcePrefix + name);
				loadedPixmaps[name] = pixbuf;
				return pixbuf;
			}
		}
		
		public Gtk.Image GetImage()
		{
			return new Gtk.Image(GetPixbuf());
		}
		
		public override string ToString()
		{
			return string.Format("[PixmapRef Name={0}]", this.name);
		}
	}
}

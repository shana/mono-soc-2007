using System;
using Cairo;
using Gtk;
using Ribbons;

namespace Sample
{
	public class MainWindow : Window
	{
		protected bool composeAvailable = false;
		
		protected Button button0;
		protected RibbonGroup group;
		
		public MainWindow() : base (WindowType.Toplevel)
		{
			Title = "Ribbons Sample";
			AppPaintable = true;
			
			button0 = new Button();
			button0.Label = "Test";
			group = new RibbonGroup ();
			//Add (button0);
			Add (group);
			
			ScreenChanged += Window_OnScreenChanged;
			Window_OnScreenChanged(this, null);
			ExposeEvent += Window_OnExpose;
			DeleteEvent += Window_OnDelete;
			
			this.Resize (200, 200);
			this.ShowAll ();
		}
		
		[GLib.ConnectBefore]
		private void Window_OnExpose(object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create(GdkWindow);
			
			cr.Rectangle(evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			if(composeAvailable)
				cr.SetSourceRGBA(0, 1, 0, 0.5);
			else
				cr.SetSourceRGB(0, 1, 0);
			cr.Fill();
			
			args.RetVal = false;
		}
		
		private void Window_OnScreenChanged(object Send, ScreenChangedArgs args)
		{
			Gdk.Colormap cm = Screen.RgbaColormap;
			composeAvailable = cm != null;	// FIX: Do not seem to detect compose support in all cases 
			
			if(!composeAvailable) cm = Screen.RgbColormap;
			Colormap = cm;
			
			Console.WriteLine("Compose Support: " + composeAvailable);
		}
		
		private void Window_OnDelete(object send, DeleteEventArgs args)
		{
			Application.Quit();
			args.RetVal = true;
		}
	}
}

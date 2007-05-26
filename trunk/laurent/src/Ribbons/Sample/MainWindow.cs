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
		protected Ribbon ribbon;
		protected RibbonGroup group0, group1;
		
		public MainWindow() : base (WindowType.Toplevel)
		{
			VBox master = new VBox ();
			
			Title = "Ribbons Sample";
			AppPaintable = true;
			
			button0 = new Button ();
			button0.Label = "Hello World";
			
			group0 = new RibbonGroup ();
			group0.Label = "Summer of Code";
			group0.Child = button0;
			
			group1 = new RibbonGroup ();
			group1.Label = "I will be back";
			
			HBox page0 = new HBox (false, 2);
			page0.PackStart (group0, false, false, 0);
			page0.PackStart (group1, false, false, 0);
			
			HBox page1 = new HBox (false, 2);
			
			HBox page2 = new HBox (false, 2);
			
			Label pageLabel0 = new Label ("Page 1");
			Label pageLabel1 = new Label ("Page 2");
			Label pageLabel2 = new Label ("Page 3");
			
			ribbon = new Ribbon ();
			ribbon.AppendPage (page0, pageLabel0);
			ribbon.AppendPage (page1, pageLabel1);
			ribbon.AppendPage (page2, pageLabel2);
			
			TextView txt = new TextView ();
			
			master.PackStart (ribbon, false, false, 0);
			master.PackStart (txt, true, true, 0);
			
			Add (master);
			
			ScreenChanged += Window_OnScreenChanged;
			Window_OnScreenChanged (this, null);
			ExposeEvent += Window_OnExpose;
			DeleteEvent += Window_OnDelete;
			
			this.Resize (200, 200);
			this.ShowAll ();
		}
		
		[GLib.ConnectBefore]
		private void Window_OnExpose(object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create (GdkWindow);
			
			if(composeAvailable)
				cr.SetSourceRGBA (1, 1, 1, 0.3);
			else
				cr.SetSourceRGB (0, 0, 0);
			
			cr.Operator = Operator.Source;
			cr.Paint ();
			
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
			Application.Quit ();
			args.RetVal = true;
		}
	}
}

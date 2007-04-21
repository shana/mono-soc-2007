using System;
using Gtk;
using Ribbons;

namespace Sample
{
	public class MainWindow : Window
	{
		protected Button button0;
		protected RibbonButton button1;
		
		public MainWindow() : base (WindowType.Toplevel)
		{
			this.Title = "Ribbons Sample";
			
			button0 = new Button();
			button0.Label = "Test";
			button1 = new RibbonButton ();
			//Add (button0);
			Add (button1);
			
			this.Resize (200, 200);
			this.ShowAll ();
		}
		
		protected override bool OnDeleteEvent (Gdk.Event evnt)
		{
			base.OnDeleteEvent (evnt);
			Application.Quit ();
			return true;
		}
	}
}

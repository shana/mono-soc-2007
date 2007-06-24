using System;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class MdbGui
	{
		[Glade.Widget] TextView sourceView;
		
		public static void Main(string[] args)
		{
			new MdbGui();
		}
		
		public MdbGui() 
		{
			Application.Init();
			
			Glade.XML gxml = new Glade.XML("gui.glade", "mainWindow", null);
			gxml.Autoconnect(this);
			sourceView.Buffer.Text = "No source file";
			
			Application.Run();
		}
		
		public void OnMainWindow_delete_event(object o, DeleteEventArgs e) 
		{
			Application.Quit();
			e.RetVal = true;
		}
		
		public void OnToolbuttonRun_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("Button Run clicked");
		}
		
		public void OnToolbuttonStop_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("Button Stop clicked");
		}
		
		public void OnToolbuttonStepIn_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("Button StepIn clicked");
		}
		
		public void OnToolbuttonStepOver_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("Button StepOver clicked");
		}
		
		public void OnToolbuttonStepOut_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("Button StepOut clicked");
		}
	}
}

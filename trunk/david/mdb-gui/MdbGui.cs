using System;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class MdbGui
	{
		[Widget] protected TextView sourceView;
		[Widget] protected Entry consoleIn;
		
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
		
		protected void OnMainWindow_delete_event(object o, DeleteEventArgs e) 
		{
			Application.Quit();
			e.RetVal = true;
		}
		
		protected void OnToolbuttonRun_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("OnToolbuttonRun_clicked");
		}
		
		protected void OnToolbuttonStop_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("OnToolbuttonStop_clicked");
		}
		
		protected void OnToolbuttonStepIn_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("OnToolbuttonStepIn_clicked");
		}
		
		public void OnToolbuttonStepOver_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("OnToolbuttonStepOver_clicked");
		}
		
		protected void OnToolbuttonStepOut_clicked(object o, EventArgs e) 
		{
			Console.WriteLine("OnToolbuttonStepOut_clicked");
		}
		
		protected void OnConsoleIn_activate(object o, EventArgs e) 
		{
			Console.WriteLine("OnConsoleIn_activate");
			Console.WriteLine("Text: " + consoleIn.Text);
			consoleIn.Text = String.Empty;
		}
	}
}

using System;
using System.IO;
using System.Runtime.InteropServices;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class MdbGui
	{
		[DllImport("monodebuggerserver")]
		static extern int mono_debugger_server_static_init ();

		[DllImport("monodebuggerserver")]
		static extern int mono_debugger_server_get_pending_sigint ();
		
		
		Interpreter interpreter;
		DebuggerEngine engine;
		LineParser parser;
		
		[Widget] protected TextView sourceView;
		[Widget] protected Entry consoleIn;
		[Widget] protected TextView consoleOut;
		[Widget] protected Viewport viewportLocalVariables;
		[Widget] protected Viewport viewportCallstack;
		[Widget] protected Viewport viewportThreads;
		StringWriter consoleOutWriter = new StringWriter();
		
		public static void Main(string[] args)
		{
			new MdbGui(args);
		}
		
		public void DebuggerInit(string[] args)
		{
			mono_debugger_server_static_init ();
			
			bool is_interactive = true;
			
			DebuggerConfiguration config = new DebuggerConfiguration ();
			config.LoadConfiguration ();

			DebuggerOptions options = DebuggerOptions.ParseCommandLine (args);
			if (options.HasDebugFlags)
				Report.Initialize (options.DebugOutput, options.DebugFlags);
			else
				Report.Initialize ();
			
			interpreter = new Interpreter (is_interactive, config, options);
			engine = interpreter.DebuggerEngine;
			parser = new LineParser (engine);
		}
		
		public MdbGui(string[] args) 
		{
			Application.Init();
			
			// Redirect output to the TextView
			Console.SetOut(consoleOutWriter);
			Console.SetError(consoleOutWriter);
			GLib.Timeout.Add(100, UpdateConsoleOut);
			
			DebuggerInit(args);
			
			Glade.XML gxml = new Glade.XML("gui.glade", "mainWindow", null);
			gxml.Autoconnect(this);
			sourceView.Buffer.Text = "No source file";
			
			consoleIn.GrabFocus();
			
			Application.Run();
		}
		
		/// <summary> Add any new output to the TextView </summary>
		bool UpdateConsoleOut()
		{
			if (consoleOutWriter.ToString().Length > 0) {
				consoleOut.Buffer.Text += consoleOutWriter.ToString();
				consoleOutWriter = new StringWriter();
				Console.SetOut(consoleOutWriter);
				Console.SetError(consoleOutWriter);
				// Scroll the window to the end
				TextMark endMark = consoleOut.Buffer.CreateMark(null, consoleOut.Buffer.EndIter, false);
				consoleOut.ScrollToMark(endMark, 0, false, 0, 0);
			}
			return true;
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
		
		/// <summary> Execute entered command </summary>
		protected void OnConsoleIn_activate(object o, EventArgs e) 
		{
			parser.Append (consoleIn.Text);
			if (parser.IsComplete ()){
				parser.Execute ();
				parser.Reset ();
			}
			
			consoleIn.Text = String.Empty;
		}
	}
}

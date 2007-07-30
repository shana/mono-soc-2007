using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
		
		[Widget] protected ToolButton toolbuttonRun;
		[Widget] protected ToolButton toolbuttonStop;
		[Widget] protected ToolButton toolbuttonContinue;
		[Widget] protected ToolButton toolbuttonStepIn;
		[Widget] protected ToolButton toolbuttonStepOver;
		[Widget] protected ToolButton toolbuttonStepOut;
		[Widget] protected ToolButton toolbuttonBreakpoint;
		
		[Widget] protected Viewport viewportSourceView;
		[Widget] protected Viewport viewportLocalVariables;
		[Widget] protected Viewport viewportCallstack;
		[Widget] protected Viewport viewportThreads;
		[Widget] protected Viewport viewportBreakpoints;
		
		[Widget] protected Entry consoleIn;
		[Widget] protected TextView consoleOut;
		
		MemoryStream reporterOutput;
		int reporterOutputReadLength = 0;
		
		SourceView sourceView;
		LocalsPad localsPad;
		CallstackPad callstackPad;
		ThreadPad threadPad;
		BreakpointsPad breakpointsPad;
		
		public Interpreter Interpreter {
			get { return interpreter; }
		}
		
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
			
			// Redirect the Reporter output stream   HACK: Using reflection
			reporterOutput = new MemoryStream();
			FieldInfo writerField = typeof(ReportWriter).GetField("writer", BindingFlags.NonPublic | BindingFlags.Instance);
			StreamWriter writer = new StreamWriter(reporterOutput);
			writer.AutoFlush = true;
			writerField.SetValue(Report.ReportWriter, writer);
			// Redirect the console
			//Console.SetOut(writer);
			//Console.SetError(writer);
			
			interpreter = new GuiInterpreter(this, is_interactive, config, options);
			engine = interpreter.DebuggerEngine;
			parser = new LineParser (engine);
			
			if (interpreter.Options.StartTarget) {
				interpreter.Start ();
			}
		}
		
		public void GuiInit()
		{
			// Redirected output to TextView
			GLib.Timeout.Add(100, UpdateConsoleOut);
			
			// Load XML file
			Glade.XML gxml = new Glade.XML("gui.glade", "mainWindow", null);
			gxml.Autoconnect(this);
			
			// Load icons
			toolbuttonRun.IconWidget = Pixmaps.DebugStart.GetImage();
			toolbuttonRun.IconWidget.Show();
			toolbuttonStop.IconWidget = Pixmaps.DebugStopProcess.GetImage();
			toolbuttonStop.IconWidget.Show();
			toolbuttonContinue.IconWidget = Pixmaps.DebugContinue.GetImage();
			toolbuttonContinue.IconWidget.Show();
			toolbuttonStepIn.IconWidget = Pixmaps.DebugStepInto.GetImage();
			toolbuttonStepIn.IconWidget.Show();
			toolbuttonStepOver.IconWidget = Pixmaps.DebugStepOver.GetImage();
			toolbuttonStepOver.IconWidget.Show();
			toolbuttonStepOut.IconWidget = Pixmaps.DebugStepOut.GetImage();
			toolbuttonStepOut.IconWidget.Show();
			toolbuttonBreakpoint.IconWidget = Pixmaps.Breakpoint.GetImage();
			toolbuttonBreakpoint.IconWidget.Show();
			
			// Source code view
			sourceView = new SourceView(this);
			ScrolledWindow scrolledWindow = new ScrolledWindow();
			scrolledWindow.Add(sourceView);
			viewportSourceView.Add(scrolledWindow);
			scrolledWindow.Show();
			sourceView.Show();
			
			// Load pads
			callstackPad = new CallstackPad(this);
			viewportCallstack.Add(callstackPad);
			threadPad = new ThreadPad(this);
			viewportThreads.Add(threadPad);
			localsPad = new LocalsPad(this);
			viewportLocalVariables.Add(localsPad);
			breakpointsPad = new BreakpointsPad(this);
			viewportBreakpoints.Add(breakpointsPad);
			
			consoleIn.GrabFocus();
		}
		
		public MdbGui(string[] args) 
		{
			Application.Init();
			
			DebuggerInit(args);
			GuiInit();
			UpdateGUI();
			
			Application.Run();
		}
		
		/// <summary> Add any new output to the TextView </summary>
		bool UpdateConsoleOut()
		{
			int unreadLength = (int)reporterOutput.Position - reporterOutputReadLength;
			if (unreadLength > 0) {
				// Read the new data from stream
				byte[] newBytes = new byte[unreadLength];
				Array.Copy(
					reporterOutput.GetBuffer(), reporterOutputReadLength, // Source
					newBytes, 0, // Destination
					unreadLength // Length
				);
				reporterOutputReadLength += unreadLength;
				
				// Append new text
				consoleOut.Buffer.Text += new UTF8Encoding().GetString(newBytes);
				
				// Scroll the window to the end
				TextMark endMark = consoleOut.Buffer.CreateMark(null, consoleOut.Buffer.EndIter, false);
				consoleOut.ScrollToMark(endMark, 0, false, 0, 0);
			}
			return true;
		}
		
		protected void OnMainWindow_delete_event(object o, DeleteEventArgs e) 
		{
			if (interpreter.HasCurrentProcess) {
				new KillCommand().Execute(engine);
			}
			Application.Quit();
			e.RetVal = true;
		}
		
		public void OnToolbuttonRun_clicked(object o, EventArgs e) 
		{
			if (interpreter.HasTarget) {
				Console.WriteLine("Error - alredy running");
			} else {
				new RunCommand().Execute(engine);
				UpdateGUI();
			}
		}
		
		public void OnToolbuttonStop_clicked(object o, EventArgs e) 
		{
			if (interpreter.HasCurrentProcess) {
				new KillCommand().Execute(engine);
				UpdateGUI();
			} else {
				Console.WriteLine("Error - nothing to stop");
			}
		}
		
		public void OnToolbuttonContinue_clicked(object o, EventArgs e) 
		{
			if (interpreter.HasCurrentThread) {
				new ContinueCommand().Execute(engine);
				UpdateGUI();
			} else {
				Console.WriteLine("Error - no current thread");
			}
		}
		
		public void OnToolbuttonStepIn_clicked(object o, EventArgs e) 
		{
			if (interpreter.HasCurrentThread) {
				new StepCommand().Execute(engine);
				UpdateGUI();
			} else {
				Console.WriteLine("Error - no current thread");
			}
		}
		
		public void OnToolbuttonStepOver_clicked(object o, EventArgs e) 
		{
			if (interpreter.HasCurrentThread) {
				new NextCommand().Execute(engine);
				UpdateGUI();
			} else {
				Console.WriteLine("Error - no current thread");
			}
		}
		
		public void OnToolbuttonStepOut_clicked(object o, EventArgs e) 
		{
			if (interpreter.HasCurrentThread) {
				new FinishCommand().Execute(engine);
				UpdateGUI();
			} else {
				Console.WriteLine("Error - no current thread");
			}
		}
		
		public void OnToolbuttonBreakpoint_clicked(object o, EventArgs e) 
		{
			sourceView.ToggleBreakpoint();
		}
		
		/// <summary> Execute entered command </summary>
		protected void OnConsoleIn_activate(object o, EventArgs e) 
		{
			if (consoleIn.Text == "g") {
				UpdateGUI();
			} else {
				parser.Append (consoleIn.Text);
				if (parser.IsComplete ()){
					parser.Execute ();
					parser.Reset ();
					UpdateGUI();
				}
			}
			
			consoleIn.Text = String.Empty;
		}
		
		public void UpdateGUI()
		{
			// Update pads - roughly the fastest ones first
			threadPad.UpdateDisplay();
			callstackPad.UpdateDisplay();
			localsPad.UpdateDisplay();
			breakpointsPad.UpdateDisplay();
			
			// Update the source view
			sourceView.UpdateDisplay();
		}
		
		
	}
	
	public class GuiInterpreter: Interpreter
	{
		MdbGui mdbGui;
		
		public GuiInterpreter(MdbGui mdbGui, bool is_interactive, DebuggerConfiguration config, DebuggerOptions options)
			:base(is_interactive, config, options)
		{
			this.mdbGui = mdbGui;
		}
		
		protected override void OnTargetEvent(Thread thread, TargetEventArgs args)
		{
			base.OnTargetEvent(thread, args);
			if (args.Type == TargetEventType.Exception || args.Type == TargetEventType.UnhandledException) {
				new ExceptionWindow(mdbGui, thread, args).Show();
			}
		}
	}
}

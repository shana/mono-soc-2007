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
		DebuggerService debuggerService;
		
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
		
		SourceView sourceView;
		LocalsPad localsPad;
		CallstackPad callstackPad;
		ThreadPad threadPad;
		BreakpointsPad breakpointsPad;
		
		public DebuggerService DebuggerService {
			get { return debuggerService; }
		}
		
		public static void Main(string[] args)
		{
			new MdbGui(args);
		}
		
		public void GuiInit()
		{
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
			
			// Regularly update the GUI
			ReceiveGUIUpdates();
			GLib.Timeout.Add(50, ReceiveGUIUpdates);
		}
		
		public MdbGui(string[] args) 
		{
			Application.Init();
			
			debuggerService = new DebuggerService(args);
			GuiInit();
			
			Application.Run();
		}
		
		protected void OnMainWindow_delete_event(object o, DeleteEventArgs e) 
		{
			debuggerService.Terminate();
			Application.Quit();
			e.RetVal = true;
		}
		
		public void OnToolbuttonRun_clicked(object o, EventArgs e) 
		{
			debuggerService.Run();
		}
		
		public void OnToolbuttonStop_clicked(object o, EventArgs e) 
		{
			debuggerService.Stop();
		}
		
		public void OnToolbuttonContinue_clicked(object o, EventArgs e) 
		{
			debuggerService.Continue();
		}
		
		public void OnToolbuttonStepIn_clicked(object o, EventArgs e) 
		{
			debuggerService.StepIn();
		}
		
		public void OnToolbuttonStepOver_clicked(object o, EventArgs e) 
		{
			debuggerService.StepOver();
		}
		
		public void OnToolbuttonStepOut_clicked(object o, EventArgs e) 
		{
			debuggerService.StepOut();
		}
		
		public void OnToolbuttonBreakpoint_clicked(object o, EventArgs e) 
		{
			sourceView.ToggleBreakpoint();
		}
		
		/// <summary> Execute entered command </summary>
		protected void OnConsoleIn_activate(object o, EventArgs e) 
		{
			debuggerService.ExecuteConsoleCommand(consoleIn.Text);
			consoleIn.Text = String.Empty;
		}
		
		bool ReceiveGUIUpdates()
		{
			UpdateConsoleOut();
			
			// Update pads
			breakpointsPad.ReceiveUpdates();
			callstackPad.ReceiveUpdates();
			localsPad.ReceiveUpdates();
			threadPad.ReceiveUpdates();
			
			// Update the source view
			sourceView.UpdateDisplay(breakpointsPad.GetBreakpoints());
			
			return true;
		}
		
		/// <summary> Add any new output to the TextView </summary>
		void UpdateConsoleOut()
		{
			string newOutput = debuggerService.GetNewConsoleOutput();
			if (newOutput.Length > 0) {
				// Append new text
				consoleOut.Buffer.Text += newOutput;
				
				// Scroll the window to the end
				TextMark endMark = consoleOut.Buffer.CreateMark(null, consoleOut.Buffer.EndIter, false);
				consoleOut.ScrollToMark(endMark, 0, false, 0, 0);
			}
		}
	}
	
	public class GuiInterpreter: Interpreter
	{
		DebuggerService debuggerService;
		
		public GuiInterpreter(DebuggerService debuggerService, bool is_interactive, DebuggerConfiguration config, DebuggerOptions options)
			:base(is_interactive, config, options)
		{
			this.debuggerService = debuggerService;
		}
		
		protected override void OnTargetEvent(Thread thread, TargetEventArgs args)
		{
			base.OnTargetEvent(thread, args);
			if (args.Type == TargetEventType.Exception || args.Type == TargetEventType.UnhandledException) {
				new ExceptionWindow(debuggerService, thread, args).Show();
			}
		}
	}
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;

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
		[Widget] protected Viewport viewportConsole;
		[Widget] protected Viewport viewportLocalVariables;
		[Widget] protected Viewport viewportCallstack;
		[Widget] protected Viewport viewportThreads;
		[Widget] protected Viewport viewportBreakpoints;
		
		SourceView sourceView;
		MdbConsolePad mdbConsolePad;
		LocalsPad localsPad;
		CallstackPad callstackPad;
		ThreadPad threadPad;
		BreakpointsPad breakpointsPad;
		
		public DebuggerService DebuggerService {
			get { return debuggerService; }
		}
		
		public static void Main(string[] args)
		{
			if (args.Length >= 2 && args[0] == "--listen") {
				int port = int.Parse(args[1]);
				string[] newArgs = new string[args.Length - 2];
				Array.Copy(
					args, 2,
					newArgs, 0,
					newArgs.Length
				);
				string uri = DebuggerService.StartServer(port, newArgs);
				Console.WriteLine("Press Enter to exit");
				Console.ReadLine();
			} else if (args.Length >= 2 && args[0] == "--connect") {
				DebuggerService debuggerService = DebuggerService.Connect(args[1]);
				new MdbGui(debuggerService);
			} else {
				DebuggerService debuggerService = DebuggerService.StartInRemoteProcess(args);
				new MdbGui(debuggerService);
			}
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
			mdbConsolePad = new MdbConsolePad(debuggerService);
			viewportConsole.Add(mdbConsolePad);
			callstackPad = new CallstackPad(debuggerService);
			viewportCallstack.Add(callstackPad);
			threadPad = new ThreadPad(debuggerService);
			viewportThreads.Add(threadPad);
			localsPad = new LocalsPad(debuggerService);
			viewportLocalVariables.Add(localsPad);
			breakpointsPad = new BreakpointsPad(debuggerService);
			viewportBreakpoints.Add(breakpointsPad);
			
			mdbConsolePad.GrabFocus();
			
			// Regularly update the GUI
			GLib.Timeout.Add(50, ReceiveGUIUpdates);
		}
		
		public MdbGui(DebuggerService debuggerService) 
		{
			this.debuggerService = debuggerService;
			
			Application.Init();
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
		
		bool ReceiveGUIUpdates()
		{
			// Update the source view
			sourceView.UpdateDisplay(breakpointsPad.GetBreakpoints());
			
			return true;
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

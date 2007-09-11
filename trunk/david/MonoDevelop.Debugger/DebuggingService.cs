using System;
using System.Collections;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;

using Mono.Debugger.Frontend;

namespace MonoDevelop.Debugger
{
	/// Implementation of IDebuggingService
	public class DebuggingService: AbstractService, IDebuggingService
	{
		static DebuggerService remoteDebugger;

		public static DebuggerService RemoteDebugger {
			get {
				if (remoteDebugger == null) {
					remoteDebugger = DebuggerService.StartInRemoteProcess(new string[0]);
				}
				return remoteDebugger;
			}
		}

		public event EventHandler PausedEvent;
		public event EventHandler ResumedEvent;
		public event EventHandler StartedEvent;
		public event EventHandler StoppedEvent;
		
		public event BreakpointEventHandler BreakpointAdded;
		public event BreakpointEventHandler BreakpointRemoved;
		public event BreakpointEventHandler BreakpointChanged;
		public event EventHandler ExecutionLocationChanged;

		public bool IsRunning {
			get {
				return remoteDebugger != null && remoteDebugger.IsRunning;
			}
		}

		public bool IsDebugging {
			get {
				return remoteDebugger != null && remoteDebugger.IsDebugging;
			}
		}
			
		public bool AddBreakpoint (string filename, int linenum)
		{
			System.Console.WriteLine("<DebuggerService> AddBreakpoint");
			return true;
		}

		public void RemoveBreakpoint (string filename, int linenum)
		{
			System.Console.WriteLine("<DebuggerService> RemoveBreakpoint");
		}

		public bool ToggleBreakpoint (string filename, int linenum)
		{
			System.Console.WriteLine("<DebuggerService> ToggleBreakpoint");
			return true;
		}

		public void Pause ()
		{
			System.Console.WriteLine("<DebuggerService> Pause");
		}
		
		public void Resume ()
		{
			System.Console.WriteLine("<DebuggerService> Resume");
			if (remoteDebugger != null) {
				remoteDebugger.Continue();
			}
		}
		
		public void Run (IConsole console, string[] args)
		{
			System.Console.WriteLine("<DebuggerService> Run {0}", String.Join(" ", args));
			if (remoteDebugger != null) {
				remoteDebugger.ExecuteConsoleCommand("file " + String.Join(" ", args));
				remoteDebugger.Run();
			}
		}
		
		public void Stop ()
		{
			System.Console.WriteLine("<DebuggerService> Stop");
			if (remoteDebugger != null) {
				remoteDebugger.Stop();
			}
		}
		

		public void StepInto ()
		{
			System.Console.WriteLine("<DebuggerService> StepInto");
			if (remoteDebugger != null) {
				remoteDebugger.StepIn();
			}
		}
		
		public void StepOver ()
		{
			System.Console.WriteLine("<DebuggerService> StepOver");
			if (remoteDebugger != null) {
				remoteDebugger.StepOver();
			}
		}
		
		public void StepOut ()
		{
			System.Console.WriteLine("<DebuggerService> StepOut");
			if (remoteDebugger != null) {
				remoteDebugger.StepOut();
			}
		}
		

		public string[] Backtrace {
			get {
				System.Console.WriteLine("<DebuggerService> Backtrace");
				return null;
			}
		}
		

		public string CurrentFilename {
			get {
				System.Console.WriteLine("<DebuggerService> CurrentFilename");
				return remoteDebugger.GetCurrentFilename();
			}
		}
		
		public int CurrentLineNumber {
			get {
				System.Console.WriteLine("<DebuggerService> CurrentLineNumber");
				return remoteDebugger.GetCurrentLine();
			}
		}
		

		public string LookupValue (string expr)
		{
			System.Console.WriteLine("<DebuggerService> LookupValue");
			return String.Format("Lookup: <{0}>", expr);
		}
		
		public IBreakpoint[] Breakpoints {
			get {
				System.Console.WriteLine("<DebuggerService> Breakpoints");
				return null;
			}
		}
		
		public IBreakpoint[] GetBreakpointsAtFile (string sourceFile)
		{
			System.Console.WriteLine("<DebuggerService> GetBreakpointsAtFile");
			return null;
		}
		
		public void ClearAllBreakpoints ()
		{
			System.Console.WriteLine("<DebuggerService> ClearAllBreakpoints");
		}
		
		public IExecutionHandlerFactory GetExecutionHandlerFactory ()
		{
			System.Console.WriteLine("<DebuggerService> GetExecutionHandlerFactory");
			return new DebugExecutionHandlerFactory (this);
		}
	}
}

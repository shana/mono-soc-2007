using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Mono.Debugger.Frontend
{
	public class DebuggerService: MarshalByRefObject
	{
		[DllImport("monodebuggerserver")]
		static extern int mono_debugger_server_static_init ();

		[DllImport("monodebuggerserver")]
		static extern int mono_debugger_server_get_pending_sigint ();
		
		
		Interpreter interpreter;
		DebuggerEngine engine;
		LineParser parser;
		
		BreakpointsStore breakpointsStore;
		CallstackStore callstackStore;
		LocalsStore localsStore;
		ThreadsStore threadsStore;
		
		MemoryStream reporterOutput;
		int reporterOutputReadLength = 0;
		
		
		public BreakpointsStore BreakpointsStore {
			get { return breakpointsStore; }
		}
		
		public CallstackStore CallstackStore {
			get { return callstackStore; }
		}
		
		public LocalsStore LocalsStore {
			get { return localsStore; }
		}
		
		public ThreadsStore ThreadsStore {
			get { return threadsStore; }
		}
		
		public DebuggerService(string[] args)
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
			
			this.breakpointsStore = new BreakpointsStore(this, interpreter);
			this.callstackStore = new CallstackStore(this, interpreter);
			this.localsStore = new LocalsStore(this, interpreter);
			this.threadsStore = new ThreadsStore(this, interpreter);
			
			if (interpreter.Options.StartTarget) {
				interpreter.Start ();
			}
			
			NotifyStateChange();
		}
		
		public string GetNewConsoleOutput()
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
				
				return new UTF8Encoding().GetString(newBytes);
			} else {
				return String.Empty;
			}
		}
		
		public void ExecuteConsoleCommand(string command)
		{
			parser.Append(command);
			if (parser.IsComplete()){
				parser.Execute();
				parser.Reset();
				NotifyStateChange();
			}
		}
		
		void ExecuteCommand(Command command)
		{
			try {
				command.Execute(engine);
			} catch(ThreadAbortException) {
			} catch(ScriptingException ex) {
				interpreter.Error(ex);
			} catch(TargetException ex) {
				interpreter.Error(ex);
			} catch(Exception ex) {
				interpreter.Error("Caught exception while executing command {0}: {1}", engine, ex);
			}
		}
		
		public void Terminate() 
		{
			if (interpreter.HasCurrentProcess) {
				Stop();
			}
		}
		
		public void Run() 
		{
			if (!interpreter.HasCurrentProcess) {
				RunCommand runCommand = new RunCommand();
				ExecuteCommand(runCommand);
				NotifyStateChange();
			} else {
				interpreter.Error("The process is alredy running");
			}
		}
		
		public void Stop() 
		{
			KillCommand killCommand = new KillCommand();
			ExecuteCommand(killCommand);
			NotifyStateChange();
		}
		
		public void Continue() 
		{
			ContinueCommand continueCommand = new ContinueCommand();
			continueCommand.InBackground = true;
			ExecuteCommand(continueCommand);
			NotifyStateChange();
		}
		
		public void StepIn() 
		{
			StepCommand stepCommand = new StepCommand();
			stepCommand.InBackground = true;
			ExecuteCommand(stepCommand);
			NotifyStateChange();
		}
		
		public void StepOver() 
		{
			NextCommand nextCommand = new NextCommand();
			nextCommand.InBackground = true;
			ExecuteCommand(nextCommand);
			NotifyStateChange();
		}
		
		public void StepOut() 
		{
			FinishCommand finishCommand = new FinishCommand();
			finishCommand.InBackground = true;
			ExecuteCommand(finishCommand);
			NotifyStateChange();
		}
		
		public string GetCurrentFilename()
		{
			// Try to get the filename for current location
			try {
				return interpreter.CurrentThread.GetBacktrace().CurrentFrame.SourceAddress.Location.FileName;
			} catch {
				return null;
			}
		}
		
		public int GetCurrentLine()
		{
			try {
				return interpreter.CurrentThread.GetBacktrace().CurrentFrame.SourceAddress.Location.Line;
			} catch {
				return -1;
			}
		}
		
		public string[] ReadFile(string filename)
		{
			SourceBuffer buffer = interpreter.ReadFile(interpreter.CurrentThread.GetBacktrace().CurrentFrame.SourceAddress.Location.FileName);
			return buffer.Contents;
		}
		
		/// <summary> Toggle breakpoint at given location </summary>
		public void ToggleBreakpoint(string filename, int line)
		{
			// Try to find a breakpoint at current location
			foreach (Event breakpoint in interpreter.Session.Events) {
				if (breakpoint is SourceBreakpoint) {
					SourceLocation location = ((SourceBreakpoint)breakpoint).Location;
					if (location != null &&
					    location.FileName == filename &&
					    location.Line == line) {
						
						interpreter.Session.DeleteEvent(breakpoint);
						breakpointsStore.UpdateTree();
						return;
					}
				}
			}
			
			// Add breakpoint at current location
			if (interpreter.HasTarget && interpreter.HasCurrentThread) {
				try {
					SourceLocation newLocation;
					ExpressionParser.ParseLocation(interpreter.CurrentThread, line.ToString(), out newLocation);
					Event newBreakpoint = interpreter.Session.InsertBreakpoint(ThreadGroup.Global, newLocation);
					newBreakpoint.Activate(interpreter.CurrentThread);
				} catch {
				}
				breakpointsStore.UpdateTree();
				return;
			}
		}
		
		public void NotifyStateChange()
		{
			threadsStore.UpdateTree();
			callstackStore.UpdateTree();
			localsStore.UpdateTree();
			breakpointsStore.UpdateTree();
		}
	}
}

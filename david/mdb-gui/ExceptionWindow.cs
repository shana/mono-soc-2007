using System;
using System.Text;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class ExceptionWindow
	{
		DebuggerService debuggerService;
		
		[Widget] protected Window exceptionWindow;
		[Widget] protected Image image;
		[Widget] protected Label labelExceptionCaught;
		[Widget] protected TextView textviewCallstack;
		[Widget] protected Button buttonBreak;
		[Widget] protected Button buttonContinue;
		[Widget] protected Button buttonTerminate;
		
		public ExceptionWindow(DebuggerService debuggerService, Thread thread, TargetEventArgs args)
		{
			this.debuggerService = debuggerService;
			
			Glade.XML gxml = new Glade.XML("gui.glade", "exceptionWindow", null);
			gxml.Autoconnect(this);
			
			image.Pixbuf = Pixmaps.Exception.GetPixbuf();
			
			if (args.Type == TargetEventType.UnhandledException) {
				buttonContinue.Visible = false;
			}
			
			labelExceptionCaught.Text = (args.Type == TargetEventType.Exception ? "Exception" : "Unandled exception") + " has been caugth:";
			
			StringBuilder sb = new StringBuilder();
			StackFrame[] callstack;
			try {
				callstack = thread.GetBacktrace().Frames;
			} catch {
				return;
			}
			
			foreach(StackFrame frame in callstack) {
				sb.Append(frame.ToString() + Environment.NewLine);
			}
			
			textviewCallstack.Buffer.Text = sb.ToString();
		}
		
		public void Show()
		{
			exceptionWindow.Show();
		}
		
		protected void OnButtonBreak_clicked(object o, EventArgs e)
		{
			exceptionWindow.Destroy();
		}
		
		protected void OnButtonContinue_clicked(object o, EventArgs e)
		{
			exceptionWindow.Destroy();
			debuggerService.Continue();
		}
		
		protected void OnButtonTerminate_clicked(object o, EventArgs e)
		{
			exceptionWindow.Destroy();
			debuggerService.Stop();
		}
	}
}

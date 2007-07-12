using System;
using System.Text;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class ExceptionWindow
	{
		MdbGui mdbGui;
		
		[Widget] protected Window exceptionWindow;
		[Widget] protected Image image;
		[Widget] protected Entry entryException;
		[Widget] protected TextView textviewCallstack;
		[Widget] protected Button buttonBreak;
		[Widget] protected Button buttonContinue;
		[Widget] protected Button buttonTerminate;
		
		public ExceptionWindow(MdbGui mdbGui, Thread thread, TargetEventArgs args)
		{
			this.mdbGui = mdbGui;
			
			Glade.XML gxml = new Glade.XML("gui.glade", "exceptionWindow", null);
			gxml.Autoconnect(this);
			
			image.Pixbuf = Pixmaps.Exception;
			
			if (args.Type == TargetEventType.UnhandledException) {
				buttonContinue.Visible = false;
			}
			
			entryException.Text = args.Type == TargetEventType.Exception ? "Exception" : "Unandled exception" + " has been caugth";
			
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
			mdbGui.OnToolbuttonContinue_clicked(o, e);
		}
		
		protected void OnButtonTerminate_clicked(object o, EventArgs e)
		{
			exceptionWindow.Destroy();
			mdbGui.OnToolbuttonStop_clicked(o, e);
		}
	}
}

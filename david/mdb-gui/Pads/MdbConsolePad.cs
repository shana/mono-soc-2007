using GLib;
using Gtk;
using GtkSharp;
using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend
{
	public class MdbConsolePad: VBox
	{
		DebuggerService debuggerService;
		
		ScrolledWindow scrolledWindow;
		TextView consoleOut;
		Entry consoleIn;
		
		public MdbConsolePad(DebuggerService debuggerService)
		{
			this.debuggerService = debuggerService;
			
			scrolledWindow = new ScrolledWindow();
			consoleOut = new TextView();
			consoleIn = new Entry();
			
			consoleIn.Activated += OnConsoleIn_activate;
			scrolledWindow.Add(consoleOut);
			
			this.Homogeneous = false;
			this.PackStart(scrolledWindow, true, true, 0);
			this.PackStart(consoleIn, false, false, 0);
			
			this.ShowAll();
			
			// Regularly update the GUI
			GLib.Timeout.Add(50, ReceiveUpdates);
		}
		
		/// <summary> Execute entered command </summary>
		protected void OnConsoleIn_activate(object o, EventArgs e) 
		{
			debuggerService.ExecuteConsoleCommand(consoleIn.Text);
			consoleIn.Text = String.Empty;
		}
		
		public bool ReceiveUpdates()
		{
			string newOutput = debuggerService.GetNewConsoleOutput();
			if (newOutput.Length > 0) {
				// Append new text
				consoleOut.Buffer.Text += newOutput;
				
				// Scroll the window to the end
				TextMark endMark = consoleOut.Buffer.CreateMark(null, consoleOut.Buffer.EndIter, false);
				consoleOut.ScrollToMark(endMark, 0, false, 0, 0);
			}
			
			return true;
		}
	}
}

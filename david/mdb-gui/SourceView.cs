using System;

using Glade;
using Gtk;
using GtkSharp;

namespace Mono.Debugger.Frontend
{
	public class SourceView: TextView
	{
		MdbGui mdbGui;
		Interpreter interpreter;
		
		string currentlyLoadedSourceFile;
		
		public SourceView(MdbGui mdbGui)
		{
			this.mdbGui = mdbGui;
			this.interpreter = mdbGui.Interpreter;
			
			// Create tags
			TextTag currentLineTag = new TextTag("currentLine");
			currentLineTag.Background = "yellow";
			this.Buffer.TagTable.Add(currentLineTag);
			
			TextTag breakpointTag = new TextTag("breakpoint");
			breakpointTag.Background = "red";
			this.Buffer.TagTable.Add(breakpointTag);
			
			TextTag disabledBreakpointTag = new TextTag("disabledBreakpoint");
			disabledBreakpointTag.Background = "gray";
			this.Buffer.TagTable.Add(disabledBreakpointTag);
			
			
			UpdateDisplay();
		}
		
		public void UpdateDisplay()
		{
			// Try to get the filename for current location
			StackFrame currentFrame = null;
			string filename = null;
			try {
				currentFrame = interpreter.CurrentThread.GetBacktrace().CurrentFrame;
				filename = currentFrame.SourceAddress.Location.FileName;
			} catch {
			}
			if (filename == null) {
				this.Buffer.Text = "No source code";
				currentlyLoadedSourceFile = null;
				return;
			}
			
			// Load the source file if neccessary
			if (currentlyLoadedSourceFile != filename) {
				SourceBuffer buffer = interpreter.ReadFile(currentFrame.SourceAddress.Location.FileName);
				string[] sourceCode = buffer.Contents;
				this.Buffer.Text = string.Join("\n", sourceCode);
				currentlyLoadedSourceFile = filename;
			}
			
			// Remove all current tags
			TextIter bufferBegin, bufferEnd;
			this.Buffer.GetBounds(out bufferBegin, out bufferEnd);
			this.Buffer.RemoveAllTags(bufferBegin, bufferEnd);
			
			// Add tag to show current line
			int currentLine = currentFrame.SourceAddress.Location.Line;
			TextIter currentLineIter = AddSourceViewTag("currentLine", currentLine);
			
			// Add tags for breakpoints
			foreach (Event handle in interpreter.Session.Events) {
				if (handle is SourceBreakpoint) {
					SourceLocation location = ((SourceBreakpoint)handle).Location;
					// If it is current line, do not retag it
					if (location != null && location.Line != currentLine) {
						AddSourceViewTag(handle.IsEnabled && handle.IsActivated ? "breakpoint" : "disabledBreakpoint", location.Line);
					}
				}
			}
			
			// Scroll to current line
			TextMark mark = this.Buffer.CreateMark(null, currentLineIter, false);
			this.ScrollToMark(mark, 0, false, 0, 0);
		}
		
		TextIter AddSourceViewTag(string tag, int line)
		{
			TextIter begin = this.Buffer.GetIterAtLine(line - 1);
			TextIter end   = this.Buffer.GetIterAtLine(line);
			this.Buffer.ApplyTag(tag, begin, end);
			return begin;
		}
		
		public void ToggleBreakpoint()
		{
			// Toggle breakpoint at current location
			if (currentlyLoadedSourceFile != null) {
				int line = this.Buffer.GetIterAtMark(this.Buffer.InsertMark).Line + 1;
				
				// Try to find a breakpoint at current location
				foreach (Event breakpoint in interpreter.Session.Events) {
					if (breakpoint is SourceBreakpoint) {
						SourceLocation location = ((SourceBreakpoint)breakpoint).Location;
						if (location != null &&
						    location.FileName == currentlyLoadedSourceFile &&
						    location.Line == line) {
							
							interpreter.Session.DeleteEvent(breakpoint);
							mdbGui.UpdateGUI();
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
				}
				mdbGui.UpdateGUI();
			} else {
				Console.WriteLine("Error - no source file loaded");
			}
		}
	}
}

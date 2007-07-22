//
// DataProvider.cs
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects.Gui.Completion;

using CBinding.Parser;

namespace CBinding
{
	public class ParameterDataProvider : IParameterDataProvider
	{
		private TextEditor editor;
		private List<Function> functions = new List<Function> ();
		
		public ParameterDataProvider (TextEditor editor, ProjectInformation info, string functionName)
		{
			this.editor = editor;
			
			foreach (Function f in info.Functions) {
				if (f.Name == functionName) {
					functions.Add (f);
				}
			}
		}
		
		// Returns the number of methods
		public int OverloadCount {
			get { return functions.Count; }
		}
		
		// Returns the index of the parameter where the cursor is currently positioned.
		// -1 means the cursor is outside the method parameter list
		// 0 means no parameter entered
		// > 0 is the index of the parameter (1-based)
		public int GetCurrentParameterIndex (ICodeCompletionContext ctx)
		{
			int cursor = editor.CursorPosition;
			int i = ctx.TriggerOffset;
			
			if (i > cursor)
				return -1;
			else if (i == cursor)
				return 0;
			
			int parameterIndex = 1;
			
			while (i++ < cursor) {
				char ch = editor.GetCharAt (i);
				if (ch == ',')
					parameterIndex++;
				else if (ch == ')')
					return -1;
			}
			
			return parameterIndex;
		}
		
		// Returns the markup to use to represent the specified method overload
		// in the parameter information window.
		public string GetMethodMarkup (int overload, string[] parameterMarkup)
		{
			Function function = functions[overload];
			string paramTxt = string.Join (", ", parameterMarkup);
			
			return function.FullName + " (" + paramTxt + ")";
		}
		
		// Returns the text to use to represent the specified parameter
		public string GetParameterMarkup (int overload, int paramIndex)
		{
			Function function = functions[overload];
			
			return function.Parameters[paramIndex];
		}
		
		// Returns the number of parameters of the specified method
		public int GetParameterCount (int overload)
		{
			return functions[overload].Parameters.Length;
		}
	}
}

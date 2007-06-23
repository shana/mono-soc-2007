//
// CTagsTextEditorExtension.cs
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
using System.Collections;

using Mono.Addins;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Gui.Completion;

namespace CTagsCompletion
{
	public class CTagsTextEditorExtension : TextEditorExtension
	{
		public override ICompletionDataProvider HandleCodeCompletion (
		    ICodeCompletionContext completionContext, char completionChar)
		{
			CTagsProject project = IdeApp.ProjectOperations.CurrentSelectedProject as CTagsProject;
			
			if (project == null) return null;
			
			TextEditor editor = IdeApp.Workbench.ActiveDocument.TextEditor;
			
			int line, column;
			editor.GetLineColumnFromPosition (editor.CursorPosition, out line, out column);
			string currentLine = editor.GetLineText (line);
			
			if (!ShouldComplete (currentLine)) return null;
			
			if (project.Tags.Count == 0)
				project.LoadTags ();
			
			CTagsCompletionDataProvider provider = new CTagsCompletionDataProvider ();
			
			project.AddTagsToProvider (provider, currentLine);
			
			return provider;
		}
		
		private bool ShouldComplete (string line)
		{
			if (line == null || line.Length < 2) return false;
			
			int len = line.Length;
			
			if (line[len - 1] == '.' && !line.Trim ().StartsWith ("#"))
				return true;
			if (line[len - 1] == '>' && line[len - 2] == '-')
				return true;
			if (line[len - 1] == ':' && line[len - 2] == ':')
				return true;
			
			return false;
		}
	}
	
	public class CTagsCompletionDataProvider : ICompletionDataProvider
	{
		private string defaultCompletionString;
		private ArrayList completionData = new ArrayList ();
		
		public ICompletionData[] GenerateCompletionData (ICompletionWidget widget, char charTyped)
		{
			return (ICompletionData[])completionData.ToArray (typeof(ICompletionData));
		}
		
		public void AddCompletionData (ICompletionData data)
		{
			completionData.Add (data);
		}
		
		public string DefaultCompletionString {
			get { return defaultCompletionString; }
		}
		public virtual void Dispose ()
		{
		}
	}
	
	public class CTagsCompletionData : ICompletionDataWithMarkup
	{
		private string image;
		private string text;
		private string description;
		private string completion_string;
		private string description_pango;
		
		public CTagsCompletionData (Tag tag, string image)
		{
			this.image = image;
			this.text = tag.Name;
			this.completion_string = tag.Name;
			this.description = string.Empty;
			this.description_pango = string.Empty;
		}
		
		public string Image {
			get { return image; }
		}
		
		public string[] Text {
			get { return new string[] { text }; }
		}
		
		public string Description {
			get { return description; }
		}

		public string CompletionString {
			get { return completion_string; }
		}
		
		public string DescriptionPango {
			get { return description_pango; }
		}		
	}
}

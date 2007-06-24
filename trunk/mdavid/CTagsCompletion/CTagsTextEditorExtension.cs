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
using System.IO;
using System.Collections.Generic;

using Mono.Addins;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Gui.Completion;

namespace CTagsCompletion
{
	public class CTagsTextEditorExtension : TextEditorExtension
	{
		private List<Tag> tags = new List<Tag> ();
		string CurrentLanguagesTagsLoaded = string.Empty;
		
		public override ICompletionDataProvider HandleCodeCompletion (
		    ICodeCompletionContext completionContext, char completionChar)
		{
			TextEditor editor = IdeApp.Workbench.ActiveDocument.TextEditor;
			
			int line, column;
			editor.GetLineColumnFromPosition (editor.CursorPosition, out line, out column);
			string currentLine = editor.GetLineText (line);
			
			string currentDocsExtension = Path.GetExtension (IdeApp.Workbench.ActiveDocument.FileName);
			string currentLanguage = GetLanguageFromExtension (currentDocsExtension);
			
			if (tags.Count == 0 || !CurrentLanguagesTagsLoaded.Equals (currentLanguage)) {
				LoadTags ();
				CurrentLanguagesTagsLoaded = currentLanguage;
			}
			
			CTagsCompletionDataProvider provider = new CTagsCompletionDataProvider ();
			
			// TODO: check specializations
			
			foreach (Tag tag in tags) {
				provider.AddCompletionData (new CTagsCompletionData (tag, "md-class"));
			}
			
			return provider;
		}
		
		public virtual void LoadTags ()
		{
			string extension = Path.GetExtension (IdeApp.Workbench.ActiveDocument.FileName);
			string language = GetLanguageFromExtension (extension);
			
			if (language == null) return;
			
			Project project = IdeApp.ProjectOperations.CurrentSelectedProject;
			
			if (project == null) return;
			
			string tagsFile = Path.Combine (Path.Combine (
			    project.BaseDirectory, ".tags"), string.Format ("{0}TAGS", language));
			
			if (tagsFile == null || tagsFile.Length == 0) return;
			
			StreamReader reader = new StreamReader (tagsFile);
			
			string entry;
			
			while ((entry = reader.ReadLine()) != null) {
				if (entry.StartsWith ("!_")) continue;
				
				tags.Add (Tag.CreateTag (entry));
			}
			
			reader.Close ();
		}
		
		private string GetLanguageFromExtension (string extension)
		{			
			for (int i = 0; i < CTagsProjectServiceExtension.SupportedExtensions.GetLength (0); i++) {
				if (extension.ToUpper ().Equals (CTagsProjectServiceExtension.SupportedExtensions[i,1])) {
					return CTagsProjectServiceExtension.SupportedExtensions[i,0];
				}
			}
			
			return null;
		}
	}
}

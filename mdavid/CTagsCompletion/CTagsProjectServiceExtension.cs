//
// CTagsProjectSrviceExtension.cs
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
using System.Text;
using System.IO;
using System.Collections.Generic;

using Mono.Addins;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;

namespace CTagsCompletion
{
	public class CTagsProjectServiceExtension : ProjectServiceExtension
	{
		// Add here new supported languages and their associated extensions (in all caps)
		public static string[,] SupportedExtensions = 
		{
			// lang - ext
			{ "CPP", ".C" },
			{ "CPP", ".H" },
			{ "CPP", ".CPP" },
			{ "CPP", ".CXX" },
			{ "CPP", ".HPP" }
		};
		
		private Dictionary<string, List<string>> filesPerLanguage = new Dictionary<string, List<string>> ();
		
		// This seems to be the best place to place this code
		public override void SetNeedsBuilding (CombineEntry entry, bool val)
		{
			base.SetNeedsBuilding (entry, val);
			
			filesPerLanguage.Clear ();
			
			Project project = entry as Project;
			
			if (project == null) return;
			
			foreach (ProjectFile file in project.ProjectFiles) {
				if (file.Subtype != Subtype.Code) continue;
				
				for (int i = 0; i < SupportedExtensions.GetLength (0); i++) {
					if (Path.GetExtension (file.Name).ToUpper ().Equals (SupportedExtensions[i,1])) {
						string language = SupportedExtensions[i,0];
						
						if (!filesPerLanguage.ContainsKey (language))
							filesPerLanguage.Add (language, new List<string> ());
						
						filesPerLanguage[language].Add (file.Name);
						break;
					}
				}
			}
			
			StringBuilder builder = new StringBuilder ();
			string tagsDirectory = Path.Combine (entry.BaseDirectory, ".tags");
			string arguments = string.Empty;
			
			object[] nodes = AddinManager.GetExtensionObjects ("/CTagsCompletion/Specialization");
			
			if (!Directory.Exists (tagsDirectory))
				Directory.CreateDirectory (tagsDirectory);
			
			foreach (KeyValuePair<string, List<string>> kvp in filesPerLanguage) {
				foreach (ITagsSpecialization node in nodes) {
					if (node.Language.Equals (kvp.Key))
						arguments = node.CTagsArguments;
				}
				
				builder.AppendFormat ("-f {0}TAGS {1}", kvp.Key, arguments);
				
				foreach (string file in kvp.Value) {
					builder.Append (" " + file);
				}
				
				try {
					ProcessWrapper p = Runtime.ProcessService.StartProcess (
					    "ctags", builder.ToString (), tagsDirectory, null);
					p.WaitForExit ();
				} catch (Exception ex) {
					throw new Exception ("Could not create tags file", ex);
				}
				
				builder.Remove (0, builder.Length);
			}
		}
		
		/// <summary>
		/// recursevly looks for all the files in the specified diretory
		/// NOT USED
		/// </summary>
//		private List<string> ScanDirectory (string directory)
//		{			    
//			List<string> files = new List<string> ();
//			
//			try {
//				foreach (string dir in Directory.GetDirectories (directory)) {
//					files.AddRange (ScanDirectory (dir));
//				}
//				
//				files.AddRange (Directory.GetFiles (directory));
//			} catch (UnauthorizedAccessException ex) {
//				Console.Error.WriteLine (ex.Message);
//			}
//			
//			return files;
//		}
	}
}

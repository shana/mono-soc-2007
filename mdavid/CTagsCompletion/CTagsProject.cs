//
// CTagsProject.cs
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
using System.Text;
using System.Collections.Generic;

using Mono.Addins;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;

namespace CTagsCompletion
{
	public abstract class CTagsProject : Project
	{
		private List<Tag> tags = new List<Tag> ();
		protected bool wantTagsCompletion = false;
		
		public virtual void WriteTags ()
		{
			string dir = Path.Combine (BaseDirectory, ".tags");
			StringBuilder files = new StringBuilder ();
			
			if (!Directory.Exists (dir)) {
				Directory.CreateDirectory (dir);
			}
			
			foreach (ProjectFile f in ProjectFiles) {
				if (f.Subtype == Subtype.Code) {
					files.Append (f.Name + " ");
				}
			}
			
			ProcessWrapper p = new ProcessWrapper ();
			
			try {
				p = Runtime.ProcessService.StartProcess (
				    "ctags", "--language-force=C++ --C++-kinds=+lpx --fields=fksaiKSz " + files.ToString (), dir, null);
				p.WaitForExit ();
			} catch (Exception ex) {
				throw new Exception ("Could not create tags file", ex);
			} finally {
				p.Close ();
			}
		}
		
		public virtual void LoadTags ()
		{
			string tagsFile = TagsFile ();
			
			if (tagsFile == null || tagsFile.Length == 0) return;
			
			StreamReader reader = new StreamReader (tagsFile);
			
			string entry;
			
			while ((entry = reader.ReadLine()) != null) {
				if (entry.StartsWith ("!_")) continue;
				
				tags.Add (Tag.CreateTag (entry));
			}
			
			reader.Close ();
		}
		
		public virtual void AddTagsToProvider (CTagsCompletionDataProvider provider,
		                                       string line)
		{
		}
		
		private string TagsFile ()
		{
			string dir = Path.Combine (BaseDirectory, ".tags");
			string tagsFile = Path.Combine (dir, "tags");
			
			return tagsFile;
		}
		
		public bool WantTagsCompletion {
			get { return wantTagsCompletion; }
		}
		
		public List<Tag> Tags {
			get { return tags; }
		}
	}
}

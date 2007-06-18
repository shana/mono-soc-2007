//
// CTags.cs
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

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;

namespace CTagsCompletion
{
	public class CTags
	{
		public static void WriteTags (Project project)
		{
			string dir = project.BaseDirectory + ".tags";
			StringBuilder files = new StringBuilder ();
			
			if (!Directory.Exists (dir)) {
				Directory.CreateDirectory (dir);
			}
			
			foreach (ProjectFile f in project.ProjectFiles) {
				if (f.Subtype == Subtype.Code) {
					files.Append (f.Name + " ");
				}
			}
			
			ProcessWrapper p = Runtime.ProcessService.StartProcess (
			    "ctags", files.ToString (), dir, null);
			p.WaitForExit ();
			
			if (p.ExitCode != 0)
				throw new Exception ("Could not create tags file");
		}
	}
}

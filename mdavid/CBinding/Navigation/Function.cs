//
// Function.cs
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

using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;

namespace CBinding.Navigation
{
	public class Function
	{
		private Project project;
		private Namespace parentNamespace;
//		private Class parentClass;
		private string name;
		private string file;
		private string pattern;
		private AccessModifier access = AccessModifier.Public;
		
		public Function (Tag tag, Project project)
		{
			this.name = tag.Name;
			this.file = tag.File;
			this.pattern = tag.Pattern;
			this.project = project;
			
			string n;
//			string klass;
			// We need the prototype tag because the implementation tag
			// marks the belonging namespace as a class
			Tag prototypeTag = TagDatabaseManager.Instance.FindTag (name, TagKind.Prototype, project);
			
			if (prototypeTag == null)				
				return;
			
			if ((n = prototypeTag.GetValue ("namespace")) != null) {
				int index = n.LastIndexOf (':');
				
				if (index > 0)
					n = n.Substring (index + 1);
				
				try {
					Tag parentTag = TagDatabaseManager.Instance.FindTag (
					    n, TagKind.Namespace, project);
					
					if (parentTag != null)
						parentNamespace = new Namespace (parentTag, project);
					
				} catch (IOException ex) {
					IdeApp.Services.MessageService.ShowError (ex);
				}
			}
			
			// TODO: Get containing class
		}
		
		public Project Project {
			get { return project; }
		}
		
		public Namespace Namespace {
			get { return parentNamespace; }
		}
		
		public string Name {
			get { return name; }
		}
		
		// TODO: FullName property
		
		public string File {
			get { return file; }
		}
		
		public string Pattern {
			get { return pattern; }
		}
		
		public AccessModifier Access {
			get { return access; }
		}
		
		public override bool Equals (object o)
		{
			Function other = o as Function;
			
			if (other != null &&
			    other.Name.Equals (name) && // Should cjeck for full name
			    other.Project.Equals (project))
				return true;
			
			return false;
		}
		
		public override int GetHashCode ()
		{
			return (name + file + access + project.Name + pattern).GetHashCode ();
		}
	}
}

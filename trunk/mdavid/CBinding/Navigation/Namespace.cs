//
// Namespace.cs
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
using MonoDevelop.Ide.Gui;

namespace CBinding.Navigation
{
	public class Namespace
	{
		private Project project;
		private Namespace parentNamespace;
		private string name;
		private string file;
		private string pattern;
		
		public Namespace (Tag tag, Project project)
		{
			this.name = tag.Name;
			this.file = tag.File;
			this.pattern = tag.Pattern;
			this.project = project;
			
			string parent;
			
			if ((parent = tag.GetValue ("namespace")) != null) {
				int index = parent.LastIndexOf (':');
				
				if (index > 0)
					parent = parent.Substring (index + 1);
				
				try {
					Tag parentTag = TagDatabaseManager.Instance.FindTag (
					    parent, TagKind.Namespace, project);
					
					if (parentTag != null)
						parentNamespace = new Namespace (parentTag, this.project);
					
				} catch (IOException ex) {
					IdeApp.Services.MessageService.ShowError (ex);
				}
			}
		}
		
		public Namespace ParentNamespace {
			get { return parentNamespace; }
		}

		public string Name {
			get { return name; }
		}
		
		public string FullName {
			get {
				if (parentNamespace != null)
					return parentNamespace.FullName + "." + name;
				return name;
			}
		}

		public string File {
			get { return file; }
		}

		public string Pattern {
			get { return pattern; }
		}
		
		public Project Project {
			get { return project; }
		}
		
		public override bool Equals (object o)
		{
			Namespace other = o as Namespace;
			
			if (other != null && other.Name.Equals (name) && other.Project.Equals (project))
				return true;
			
			return false;
		}
		
		public override int GetHashCode ()
		{
			return (name + file + pattern + project.Name).GetHashCode ();
		}
	}
}

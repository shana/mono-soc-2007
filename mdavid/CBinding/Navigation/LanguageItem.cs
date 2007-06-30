//
// LanguageItem.cs
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
	public abstract class LanguageItem
	{
		private Project project;
		private string name;
		private string file;
		private string pattern;
		private AccessModifier access = AccessModifier.Public;
		private LanguageItem parent;
		
		public LanguageItem (Tag tag, Project project)
		{
			this.project = project;
			this.name = tag.Name;
			this.file = tag.File;
			this.pattern = tag.Pattern;
		}
		
		/// <summary>
		/// Attempts to get the namespace encompasing the function
		/// returns true on success and false if it does not have one.
		/// NOTE: if it's a method then even if the class it belongs to
		/// has a namespace the method will not have a namespace since
		/// it should be placed under the class node and not the namespace node
		/// </summary>
		protected bool GetNamespace (Tag tag)
		{
			string n;
			
			if ((n = tag.GetValue ("namespace")) != null) {
				int index = n.LastIndexOf (':');
				
				if (index > 0)
					n = n.Substring (index + 1);
				
				try {
					Tag namespaceTag = TagDatabaseManager.Instance.FindTag (
					    n, TagKind.Namespace, project);
					
					if (namespaceTag != null)
						parent = new Namespace (namespaceTag, project);
					
				} catch (IOException ex) {
					IdeApp.Services.MessageService.ShowError (ex);
				}
				
				return true;
			}
			
			return false;
		}
		
		protected bool GetClass (Tag tag)
		{
			string c;
			
			if ((c = tag.GetValue ("class")) != null) {
				access = (AccessModifier)Enum.Parse (typeof(AccessModifier), tag.GetValue ("access"), true);
				
				int index = c.LastIndexOf (':');
				
				if (index > 0)
					c = c.Substring (index + 1);
				
				try {
					Tag classTag = TagDatabaseManager.Instance.FindTag (
					    c, TagKind.Class, project);
					
					if (classTag != null)
						parent = new Class (classTag, project);
					
				} catch (IOException ex) {
					IdeApp.Services.MessageService.ShowError (ex);
				}
				
				return true;
			}
			
			return false;
		}
		
		protected bool GetStructure (Tag tag)
		{
			string s;
			
			if ((s = tag.GetValue ("struct")) != null) {
				access = (AccessModifier)Enum.Parse (typeof(AccessModifier), tag.GetValue ("access"), true);
				
				int index = s.LastIndexOf (':');
				
				if (index > 0)
					s = s.Substring (index + 1);
				
				try {
					Tag classTag = TagDatabaseManager.Instance.FindTag (
					    s, TagKind.Structure, project);
					
					if (classTag != null)
						parent = new Structure (classTag, project);
					
				} catch (IOException ex) {
					IdeApp.Services.MessageService.ShowError (ex);
				}
				
				return true;
			}
			
			return false;
		}
		
		public Project Project {
			get { return project; }
		}
		
		public LanguageItem Parent {
			get { return parent; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public string FullName {
			get {
				if (Parent != null)
					return Parent.FullName + "::" + Name;
				return Name;
			}
		}
		
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
			LanguageItem other = o as LanguageItem;
			
			if (other != null &&
			    other.FullName.Equals (FullName) &&
			    other.Project.Equals (project))
				return true;
			
			return false;
		}
		
		public override int GetHashCode ()
		{
			return (name + file + pattern + project.Name).GetHashCode ();
		}
	}
}

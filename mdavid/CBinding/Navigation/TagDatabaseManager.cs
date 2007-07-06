//
// TagDatabaseManager.cs
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
using MonoDevelop.Ide.Gui;

namespace CBinding.Navigation
{
	/// <summary>
	/// Singleton class to manage tag databases
	/// </summary>
	public class TagDatabaseManager
	{
		private static TagDatabaseManager instance;
		
		private TagDatabaseManager()
		{
		}
		
		public static TagDatabaseManager Instance
		{
			get {
				if (instance == null)
					instance = new TagDatabaseManager ();
				
				return instance;
			}
		}
		
		public void WriteTags (Project project)
		{
			string tagsDir = Path.Combine (project.BaseDirectory, ".tags");
			
			if (!Directory.Exists (tagsDir))
				Directory.CreateDirectory (tagsDir);
			
			string ctags_options = "--C++-kinds=+p+u --fields=+a-f --language-force=C++ --excmd=pattern";
			
			StringBuilder args = new StringBuilder (ctags_options);
			
			foreach (ProjectFile file in project.ProjectFiles) 
				args.AppendFormat (" {0}", file.Name);
			
			try {
				ProcessWrapper p = Runtime.ProcessService.StartProcess ("ctags", args.ToString (), tagsDir, null);
				p.WaitForExit ();
			} catch (Exception ex) {
				throw new IOException ("Could not create tags database (You must have exuberant ctags installed).", ex);
			}
		}
		
		private Tag ParseTag (string tagEntry)
		{
			int i1, i2;
			string file;
			string pattern;
			string name;
			string tagField;
			TagKind kind;
			AccessModifier access = AccessModifier.Public;
			string _class = null;
			string _namespace = null;
			string _struct = null;
			string _union = null;
			string _enum = null;
			char delimiter;
			
			name = tagEntry.Substring (0, tagEntry.IndexOf ('\t'));
			
			i1 = tagEntry.IndexOf ('\t') + 1;
			i2 = tagEntry.IndexOf ('\t', i1);
			
			file = tagEntry.Substring (i1, i2 - i1);
			
			delimiter = tagEntry[i2 + 1];
			
			i1 = i2 + 2;
			i2 = tagEntry.IndexOf (delimiter, i1) - 1;
			
			pattern = tagEntry.Substring (i1 + 1, i2 - i1 - 1);
			
			tagField = tagEntry.Substring (i2 + 5);
			
			// parse tag field
			kind = (TagKind)tagField[0];
			
			string[] fields = tagField.Split ('\t');
			int index;
			
			foreach (string field in fields) {
				index = field.IndexOf (':');
				
				if (index > 0) {
					string key = field.Substring (0, index);
					string val = field.Substring (index + 1);
					switch (key) {
					case "access":
						access = (AccessModifier)System.Enum.Parse (typeof(AccessModifier), val, true);
						break;
					case "class":
						_class = val;
						break;
					case "namespace":
						_namespace = val;
						break;
					case "struct":
						_struct = val;
						break;
					case "union":
						_union = val;
						break;
					case "enum":
						_enum = val;
						break;
					}
				}
			}
			
			return new Tag (name, file, pattern, kind, access, _class, _namespace, _struct, _union, _enum);
		}
		
		public void FillProjectNavigationInformation (Project project)
		{
			string tagsDir = Path.Combine (project.BaseDirectory, ".tags");
			string tagsFile = Path.Combine (tagsDir, "tags");
			
			if (!File.Exists (tagsFile))
				throw new IOException ("Tags file does not exist for project: " + project.Name);
			
			string tagEntry;
			
			ProjectNavigationInformation info = ProjectNavigationInformationManager.Instance.Get (project);
			
			info.Clear ();
			
			using (StreamReader reader = new StreamReader (tagsFile)) {
				while ((tagEntry = reader.ReadLine ()) != null) {
					if (tagEntry.StartsWith ("!_")) continue;
					
					Tag tag = ParseTag (tagEntry);
					
					switch (tag.Kind)
					{
					case TagKind.Class:
						info.Classes.Add (new Class (tag, project));
						break;
					case TagKind.Enumeration:
						info.Enumerations.Add (new Enumeration (tag, project));
						break;
					case TagKind.Enumerator:
						info.Enumerators.Add (new Enumerator (tag, project));
						break;
					case TagKind.ExternalVariable:
						break;
					case TagKind.Function:
						info.Functions.Add (new Function (tag, project));
						break;
					case TagKind.Local:
						break;
					case TagKind.Macro:
						info.Macros.Add (new Macro (tag, project));
						break;
					case TagKind.Member:
						info.Members.Add (new Member (tag, project));
						break;
					case TagKind.Namespace:
						info.Namespaces.Add (new Namespace (tag, project));
						break;
					case TagKind.Prototype:
						break;
					case TagKind.Structure:
						info.Structures.Add (new Structure (tag, project));
						break;
					case TagKind.Typedef:
						info.Typedefs.Add (new Typedef (tag, project));
						break;
					case TagKind.Union:
						info.Unions.Add (new Union (tag, project));
						break;
					case TagKind.Variable:
						info.Variables.Add (new Variable (tag, project));
						break;
					default:
						break;
					}
				}
			}
		}
		
		public Tag FindTag (string name, TagKind kind, Project project)
		{
			string tagsDir = Path.Combine (project.BaseDirectory, ".tags");
			string tagsFile = Path.Combine (tagsDir, "tags");
			
			if (!File.Exists (tagsFile))
				throw new IOException ("Tags file does not exist for project: " + project.Name);
			
			Tag tag;
			string tagEntry;
			
			using (StreamReader reader = new StreamReader (tagsFile)) {
				while ((tagEntry = reader.ReadLine ()) != null) {
					if (tagEntry.StartsWith ("!_")) continue;
					
					if (tagEntry.Substring (0, tagEntry.IndexOf ('\t')).Equals (name)) {
						tag = ParseTag (tagEntry);
						
						if (tag.Kind == kind)
							return tag;
					}
				}
				
				return null;
			}
		}
	}
}

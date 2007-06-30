//
// ProjectNodeBuilderExtension.cs
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

using Mono.Addins;

using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads;

using CBinding;

namespace CBinding.Navigation
{
	public class ProjectNodeBuilderExtension : NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(CProject).IsAssignableFrom (dataType);
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			CProject p = dataObject as CProject;
			
			if (p == null) return;
			
			try {
				TagDatabaseManager.Instance.WriteTags (p);
				TagDatabaseManager.Instance.FillProjectNavigationInformation (p);
			} catch (IOException ex) {
				IdeApp.Services.MessageService.ShowError (ex);
				return;
			}
			
			bool nestedNamespaces = builder.Options["NestedNamespaces"];
			
			ProjectNavigationInformation info = ProjectNavigationInformationManager.Instance.Get (p);
			
			// Namespaces
			foreach (Namespace n in info.Namespaces) {
				if (nestedNamespaces) {
					if (n.Parent == null) {
						builder.AddChild (n);
					}
				} else {
					builder.AddChild (n);
				}
			}
			
			// Classes
			
			// Structures
			
			// Globals
			builder.AddChild (Globals.Instance);
			
			// Macros
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
}

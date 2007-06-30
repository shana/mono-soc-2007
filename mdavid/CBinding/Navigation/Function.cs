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
	public class Function : LanguageItem
	{	
		public Function (Tag tag, Project project) : base (tag, project)
		{
			// We need the prototype tag because the implementation tag
			// marks the belonging namespace as a if it were a class
			// and it does not have the access field.
			Tag prototypeTag = TagDatabaseManager.Instance.FindTag (Name, TagKind.Prototype, Project);
			
			if (prototypeTag == null) {
				// It does not have a prototype tag which means it is inline
				// and when it is inline it does have all the info we need
				
				if (GetNamespace (tag)) return;
				if (GetClass (tag)) return;
				if (GetStructure (tag)) return;
				
				return;
			}
			
			if (GetNamespace (prototypeTag)) return;
			if (GetClass (prototypeTag)) return;
			if (GetStructure (prototypeTag)) return;
		}
	}
}

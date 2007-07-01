//
// Tag.cs
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

namespace CBinding.Navigation
{
	public enum TagKind {
		Class = 'c', // Done
		Macro = 'd',
		Enumerator = 'e',
		Function = 'f', // Done
		Enumeration = 'g',
		Local = 'l',
		Member = 'm',
		Namespace = 'n', // Done
		Prototype = 'p',
		Structure = 's', // Done
		Typedef = 't',
		Union = 'u',
		Variable = 'v',
		ExternalVariable = 'x',
		Unknown = ' '
	}
	
	public enum AccessModifier {
		Private,
		Protected,
		Public
	}
	
	public class Tag
	{
		private string name;
		private string file;
		private string pattern;
		private TagKind kind;
		private AccessModifier access;
		private string _class;
		private string _namespace;
		private string _struct;
		private string _enum;
		
		public Tag (string name,
		            string file,
		            string pattern,
		            TagKind kind,
		            AccessModifier access,
		            string _class,
		            string _namespace,
		            string _struct,
		            string _enum)
		{
			this.name = name;
			this.file = file;
			this.pattern = pattern;	
			this.kind = kind;
			this.access = access;
			this._class = _class;
			this._namespace = _namespace;
			this._struct = _struct;
			this._enum = _enum;;
		}
		
		public string Name {
			get { return name; }
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
		
		public string Class {
			get { return _class; }
		}
		
		public string Namespace {
			get { return _namespace; }
		}
		
		public string Structure {
			get { return _struct; }
		}
		
		public string Enum {
			get { return _enum; }
		}
		
		public TagKind Kind {
			get { return kind; }
		}
	}
}

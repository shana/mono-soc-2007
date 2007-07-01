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
		Structure = 's',
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
		private string tagField;
		
		public Tag (string name, string file, string pattern, string tagField)
		{
			this.name = name;
			this.file = file;
			this.pattern = pattern;
			this.tagField = tagField;
		}
		
		// FIXEM: Its currently not working with defines
		public static Tag CreateTag (string tagEntry)
		{
			int i1, i2;
			string file;
			string pattern;
			string name;
			string tagField;
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
			
			return new Tag (name, file, pattern, tagField);
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
		
		public string TagField {
			get { return tagField; }
		}
		
		public string GetValue (string key)
		{
			if (tagField == null) return null;
			
			string[] fields = tagField.Split ('\t');
			
			foreach (string field in fields) {
				int index = field.IndexOf (':');
				
				if (index > 0 && field.Substring (0, index).Equals (key))
					return field.Substring (index + 1);
			}

			return null;
		}
		
		public TagKind Kind {
			get {
				if (tagField == null) return TagKind.Unknown;
				return (TagKind)tagField[0];
			}
		}
	}
}
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
using System.Collections.Generic;

namespace CTagsCompletion
{
	public class Tag
	{
		private string name;
		private string file;
		private string pattern;
		private List<string[]> tagField;
		
		public Tag (string name, string file, string pattern, List<string[]> tagField)
		{
			this.name = name;
			this.file = file;
			this.pattern = pattern;
			this.tagField = tagField;
		}
		
		public static Tag CreateTag (string tagEntry)
		{
			int i1, i2;
			string file;
			string pattern;
			string name;
			List<string[]> tagField = new List<string[]> ();
			char delimiter;
			
			name = tagEntry.Substring (0, tagEntry.IndexOf ('\t'));
			
			i1 = tagEntry.IndexOf ('\t') + 1;
			i2 = tagEntry.IndexOf ('\t', i1) - 1;
			
			file = tagEntry.Substring (i1, i2 - i1);
			
			delimiter = tagEntry[i2 + 2];
			
			i1 = i2 + 3;
			i2 = tagEntry.IndexOf (delimiter, i1) - 1;
			
			pattern = tagEntry.Substring (i1 + 1, i2 - i1 - 1);
			
			string substrTagField = tagEntry.Substring (tagEntry.IndexOf (";\"\t") + 3);
			
			foreach (string field in substrTagField.Split ('\t'))
				tagField.Add (field.Split (':'));
			
			return new Tag (name, file, pattern, tagField);
		}
		
		public override string ToString ()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			
			builder.Append (string.Format ("{0}\t{1}\t/^{2}$/;\"", name, file, pattern));
			
			foreach (string[] keyval in tagField) {
				builder.Append ("\t");
				for (int i = 0; i < keyval.Length; i++) {
					builder.Append (keyval[i]);
					
					if (i != keyval.Length - 1)
						builder.Append (":");
				}
			}
			
			return builder.ToString ();
		}
		
		public string File {
			get { return file; }
		}

		public virtual string Pattern {
			get { return pattern; }
		}
		
		public virtual List<string[]> TagField {
			get { return tagField; }
		}

		public virtual string Name {
			get { return name; }
		}
	}
}

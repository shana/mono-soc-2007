//
// Package.cs: A pkg-config package
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

using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public class Package
	{
		[ItemProperty ("name")]
		private string name;
		
		public Package (string name)
		{
			this.name = name;
		}
		
		public Package ()
		{
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public override bool Equals (object o)
		{
			Package other = o as Package;
			
			if (other == null) return false;
			
			return other.Name.Equals (name);
		}
		
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		
//		DataCollection ICustomDataItem.Serialize (ITypeSerializer handler)
//		{
//			DataCollection data = handler.Serialize (this);
//			
//			data.Add (new DataValue ("name", name));
//
//			return data;
//		}
//		
//		void ICustomDataItem.Deserialize (ITypeSerializer handler, DataCollection data)
//		{
//			DataValue name_value = data.Extract ("name") as DataValue;
//			handler.Deserialize (this, data);
//			
//			if (name_value != null) {
//				name = name_value.Value;
//			}
//		}
	}
}

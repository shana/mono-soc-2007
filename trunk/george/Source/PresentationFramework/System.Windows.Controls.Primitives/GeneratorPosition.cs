//
// GeneratorPosition.cs
//
// Author:
//   George Giolfan (georgegiolfan@yahoo.com)
//
// Copyright (C) 2007 George Giolfan
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
#if Implementation
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	public struct GeneratorPosition
	{
		#region Private Fields
		int index;
		int offset;
		#endregion

		#region Public Constructors
		public GeneratorPosition (int index, int offset)
		{
			this.index = index;
			this.offset = offset;
		}
		#endregion

		#region Public Properties
		public int Index {
			get { return index; }
			set { index = value; }
		}

		public int Offset {
			get { return offset; }
			set { offset = value; }
		}
		#endregion

		#region Public Methods
		public override bool Equals (object o)
		{
			if (!(o is GeneratorPosition))
				return false;
			return this == (GeneratorPosition)o;
		}

		public static bool Equals (object objA, object objB)
		{
			if (!(objA is GeneratorPosition) || !(objB is GeneratorPosition))
				return false;
			return (GeneratorPosition)objA == (GeneratorPosition)objB;
		}

		public override string ToString ()
		{
			//WDTDH
			return base.ToString ();
		}
		#endregion

		#region Public Operators
		public static bool operator == (GeneratorPosition gp1, GeneratorPosition gp2)
		{
			return gp1.index == gp2.index && gp1.offset == gp2.offset;
		}

		public static bool operator != (GeneratorPosition gp1, GeneratorPosition gp2)
		{
			return !(gp1 == gp2);
		}
		#endregion
	}
}
//
// Authors:
//	Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
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

namespaceMonoDevelop.Database.Sql
{
	public class Range
	{
		private int min;
		private int max;

		public Range (int min, int max)
		{
			if (min > max) {
				int tmp = min;
				min = max;
				max = tmp;
			}
			
			this.min = min;
			this.max = max;
		}
		
		public Range (int constRange)
			: this (constRange, constRange)
		{
		}
		
		public int Min {
			get { return min; }
		}
		
		
		public int Max {
			get { return max; }
		}

		public bool IsInRange (int value)
		{
			return value >= min && value <= max;
		}
		
		public bool IsFixed {
			get { return min == max; }
		}
	}
}

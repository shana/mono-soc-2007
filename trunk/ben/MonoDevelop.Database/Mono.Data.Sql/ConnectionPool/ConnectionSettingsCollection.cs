//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2005 Christian Hergert
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	[Serializable]
	public class ConnectionSettingsCollection : CollectionBase, IEnumerable<ConnectionSettings>
	{
		public event EventHandler Changed;
		
		public ConnectionSettingsCollection ()
			: base ()
		{
		}

		public ConnectionSettings this[int index] {
			get { return List[index] as ConnectionSettings; }
			set { List[index] = value; }
		}

		public int Add (ConnectionSettings item)
		{
			int retval = List.Add (item);
			OnChanged (EventArgs.Empty);
			return retval;
		}

		public int IndexOf (ConnectionSettings item)
		{
			return List.IndexOf (item);
		}

		public void Insert (int index, ConnectionSettings item)
		{
			List.Insert (index, item);
			OnChanged (EventArgs.Empty);
		}

		public void Remove (ConnectionSettings item)
		{
			List.Remove (item);
			OnChanged (EventArgs.Empty);
		}

		public bool Contains (ConnectionSettings item)
		{
			return List.Contains (item);
		}
		
		IEnumerator<ConnectionSettings> IEnumerable<ConnectionSettings>.GetEnumerator ()
		{
			foreach (ConnectionSettings cs in this)
				yield return cs;
		}
		
		protected virtual void OnChanged (EventArgs e)
		{
			if (Changed != null )
				Changed (this, e);
		}
	}
}

﻿//
// Authors:
//   Ben Motmans  <ben.motmans@gmail.com>
//
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

using Gtk;
using System;
using System.Collections.Generic;
usingMonoDevelop.Database.Sql;

namespace MonoDevelop.Database.Components
{
	public partial class SelectColumnDialog : Gtk.Dialog
	{
		private SelectColumnWidget selecter;
		
		public SelectColumnDialog(bool showCheckBoxes)
		{
			this.Build();
			
			selecter = new SelectColumnWidget (showCheckBoxes);
			//FIXME: vbox.PackStart (selecter, true, true, 0);
		}
		
		public ColumnSchema SelectedColumn {
			get { return selecter.SelectedColumn; }
		}
		
		public IEnumerable<ColumnSchema> CheckedColumns {
			get { return selecter.CheckedColumns; }
		}
		
		public void Append (IEnumerable<ColumnSchema> columns)
		{
			selecter.Append (columns);
		}

		protected virtual void CancelClicked (object sender, System.EventArgs e)
		{
			Respond (ResponseType.Cancel);
			Destroy ();
		}

		protected virtual void OkClicked (object sender, System.EventArgs e)
		{
			Respond (ResponseType.Ok);
			Destroy ();
		}
	}
}
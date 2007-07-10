//
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
using System.Data;
using Mono.Addins;
using MonoDevelop.Core;

namespace MonoDevelop.Database.Components
{
	public partial class DataGrid : Gtk.Bin
	{
		private bool limitPageSize;
		private int pageSize = 50;
		private int currentRecord = 0;
		private int numRecords = 0;
		private DataTable dataSource;
		
		private ListStore store;
		private DataGridColumn[] columns;
		private int columnCount = 0;
		
		public DataGrid ()
		{
			this.Build ();
			
			grid.HeadersVisible = true;
			grid.EnableGridLines = TreeViewGridLines.Both;
			grid.Reorderable = false;
			grid.RulesHint = true;
		}
		
		public bool ShowNavigator {
			get { return hbox.Visible; }
			set { hbox.Visible = value; }
		}
		
		public bool EnableNavigator {
			get { return hbox.Sensitive; }
			set { hbox.Sensitive = value; }
		}
		
		public int PageSize {
			get { return pageSize; }
			set { pageSize = value; }
		}
		
		public bool LimitPageSize {
			get { return limitPageSize; }
			set {
				limitPageSize = value;
				ShowNavigator = LimitPageSize;
			}
		}
		
		public int CurrentRecord {
			get { return currentRecord; }
		}
		
		public int RecordCount {
			get { return numRecords; }
		}
		
		public void DataBind ()
		{
			if (dataSource == null) {
				Clear ();
				return;
			}
			
			int index = 0;
			Type[] storeTypes = new Type[dataSource.Columns.Count];
			columnCount = dataSource.Columns.Count;
			foreach (DataColumn col in dataSource.Columns) {
				DataGridColumn dgCol = new DataGridColumn (col, index);
				grid.AppendColumn (dgCol);
				storeTypes[index] = dgCol.DataType;
					
				index++;
			}
			store = new ListStore (storeTypes);
			
			FillGrid (0, pageSize);
		}
		
		public DataTable DataSource {
			get { return dataSource; }
			set {
				dataSource = value;
				if (value != null)
					numRecords = dataSource.Rows.Count;
				else
					numRecords = 0;
			}
		}
		
		public void Clear ()
		{
			currentRecord = 0;
			numRecords = 0;
			columnCount = 0;

			if (store != null) {
				store.Clear ();
				store = null;
			}
			
			if (columns != null) {
				for (int i=0; i<columns.Length; i++) {
					if (columns[i] != null)
						grid.RemoveColumn (columns[i]);
					columns[i] = null;
				}
				columns = null;
			}
		}

		private void NavigateToRecord (int record)
		{
			if (record < 0 || record > numRecords)
				throw new ArgumentException ("Invalid record index.");
			
			if (!limitPageSize) return;
			
			int count = numRecords - record;
			if (count > pageSize) count = pageSize;
			
			FillGrid (record, count);
		}

		private void FillGrid (int start, int count)
		{
			grid.Model = null;
			
			for (int i=start; i<(start+count); i++) {
				DataRow row = dataSource.Rows[i];
				
				TreeIter iter = store.Append ();
				for (int j=0;j<columnCount; j++)
					store.SetValue (iter, j, row[j]);
			}
			
			grid.Model = store;
		}

		protected virtual void ButtonFirstClicked (object sender, System.EventArgs e)
		{
			currentRecord = 0;
			NavigateToRecord (currentRecord);
		}

		protected virtual void ButtonPreviousClicked (object sender, System.EventArgs e)
		{
			currentRecord -= pageSize;
			if (currentRecord < 0) currentRecord = 0;
			NavigateToRecord (currentRecord);
		}

		protected virtual void ButtonNextClicked (object sender, System.EventArgs e)
		{
			currentRecord += pageSize;
		}

		protected virtual void ButtonLastClicked (object sender, System.EventArgs e)
		{
			int modulus = numRecords % pageSize;
			currentRecord = numRecords - modulus;
			NavigateToRecord (currentRecord);
		}

		protected virtual void EntryCurrentActivated (object sender, System.EventArgs e)
		{
			int entered = 0;
			if (int.TryParse (entryCurrent.Text, out entered)) {
				if (entered >= 0 && entered < numRecords) {
					currentRecord = entered;
					NavigateToRecord (currentRecord);
					return;
				}
			}
			
			entryCurrent.Text = currentRecord.ToString ();
		}
	}
}

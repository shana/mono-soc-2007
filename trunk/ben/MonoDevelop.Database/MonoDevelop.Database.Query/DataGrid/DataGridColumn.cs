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
using System.Collections.Generic;
using Mono.Addins;
using MonoDevelop.Core;

namespace MonoDevelop.Database.Query
{
	public class DataGridColumn : TreeViewColumn
	{
		private DataGridColumnType columnType;
		private BaseCellRenderer baseCell;
		private Type dataType;
		private int index;
		
		public DataGridColumn (DataColumn col, int index)
		{
			this.index = index;
			dataType = col.DataType;
			columnType = GetDataGridColumnType (dataType);
			baseCell = GetBaseCellRenderer (columnType);
			
			Title = col.ColumnName.Replace ("_", "__"); //underscores are normally used for underlining, so needs escape char
			Clickable = columnType != DataGridColumnType.Binary;

			this.PackStart (baseCell, true);
			this.SetCellDataFunc (baseCell, new CellLayoutDataFunc (DataFunc));
		}
		
		public Type DataType {
			get { return dataType; }
		}
		
		public DataGridColumnType ColumnType {
			get { return columnType; }
		}
	
		private void DataFunc (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			object obj = model.GetValue (iter, index);
			BaseCellRenderer baseCell = (BaseCellRenderer)cell;
			baseCell.Value = obj;
		}
		
		private static DataGridColumnType GetDataGridColumnType (Type type)
		{
			if (type == null)
				return DataGridColumnType.String;
			
			if (type.IsArray) {
				//this only occurs with byte[] (blobs)
				return DataGridColumnType.Binary;
			}
			
			switch (type.FullName) {
				case "System.Boolean":
					return DataGridColumnType.Boolean;
				case "System.Int16":
				case "System.Int32":
				case "System.Int64":
				case "System.UInt16":
				case "System.UInt32":
				case "System.UInt64":
					return DataGridColumnType.Integer;
				case "System.Float":
				case "System.Double":
					return DataGridColumnType.Double;
				case "System.Byte":
				case "System.String":
				default:
					return DataGridColumnType.String;
			}
		}
			
		private static BaseCellRenderer GetBaseCellRenderer (DataGridColumnType columnType)
		{
			switch (columnType) {
				case DataGridColumnType.Binary:
					return new BinaryCellRenderer ();
				default:
					return new StringCellRenderer ();
			}
		}
	}
}
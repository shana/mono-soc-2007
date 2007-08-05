//
// Authors:
//	Ben Motmans  <ben.motmans@gmail.com>
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

using System;

namespace MonoDevelop.Database.Sql
{
	[Flags]
	public enum TableMetaData
	{
		Create,
		Alter,
		Drop,
		Rename,
		
		Name,
		TableSpaceName,
		Owner,
		Comment,
		Definition,
		Schema,
		IsSystem,
		Columns,
		PrimaryKeyConstraint,
		ForeignKeyConstraint,
		CheckConstraint,
		UniqueConstraint,
		Triggers,
		
		CanAppendColumn,
		CanInsertColumn,
		CanRemoveColumn
	}

	[AttributeUsage (AttributeTargets.Class)]
	public sealed class TableMetaDataAttribute : Attribute
	{
		private TableMetaData meta;
		
		public TableMetaDataAttribute (TableMetaData meta)
		{
			this.meta = meta;
		}
		
		public TableMetaData TableMetaData {
			get { return meta; }
		}
	}
}
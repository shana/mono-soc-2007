//
// Authors:
//	Christian Hergert  <chris@mosaix.net>
//	Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (C) 2005 Mosaix Communications, Inc.
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

namespace Mono.Data.Sql
{
	public class ColumnSchema : AbstractSchema
	{
		protected string dataType = String.Empty;
		protected string defaultValue = String.Empty;
		protected bool notNull = false;
		protected int length = 0;
		protected int precision = 0;
		protected int scale = 0;
		protected int columnID = 0;
		
		public ColumnSchema (ISchemaProvider schemaProvider)
			: base (schemaProvider)
		{
		}
		
		public DataTypeSchema DataType {
			get {
				return SchemaProvider.GetDataType (dataType);
			}
		}
		
		public string DataTypeName {
			get {
				return dataType;
			}
			set {
				if (dataType != value) {
					dataType = value;
					OnChanged ();
				}
			}
		}
		
		public virtual string Default {
			get {
				return defaultValue;
			}
			set {
				if (defaultValue != value) {
					defaultValue = value;
					OnChanged();
				}
			}
		}
		
		public virtual bool NotNull {
			get {
				return notNull;
			}
			set {
				if (notNull != value) {
					notNull = value;
					OnChanged();
				}
			}
		}
		
		public virtual int Length {
			get {
				return length;
			}
			set {
				if (length != value) {
					length = value;
					OnChanged();
				}
			}
		}
		
		public virtual int Precision {
			get {
				return precision;
			}
			set {
				if (precision != value) {
					precision = value;
					OnChanged();
				}
			}
		}
		
		public virtual int Scale {
			get {
				return scale;
			}
			set {
				if (scale != value) {
					scale = value;
					OnChanged();
				}
			}
		}
		
		public virtual int ColumnID {
			get {
				return columnID;
			}
			set {
				if (columnID != value) {
					columnID = value;
					OnChanged();
				}
			}
		}
	}
}

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
	public static class MetaDataService
	{
		public static bool IsApplied (object obj, Type attributeType)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			if (attributeType == null)
				throw new ArgumentNullException ("attributeType");
			
			return obj.GetType ().GetCustomAttributes (attributeType, true).Length > 0;
		}
		
		public static bool IsApplied (object obj, params Type[] attributeTypes)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			if (attributeTypes == null)
				throw new ArgumentNullException ("attributeTypes");
			
			foreach (Type type in attributeTypes)
				if (obj.GetType ().GetCustomAttributes (type, true).Length > 0)
					return true;
			return false;
		}
		
		public static ConnectionSettingsMetaDataAttribute GetConnectionSettingsMetaData (object obj)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (ConnectionSettingsMetaDataAttribute), true);
			if (attributes.Length > 0)
				return attributes[0] as ConnectionSettingsMetaDataAttribute;
			return null;
		}
		
		public static bool IsIndexMetaDataSupported (object obj, IndexMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (IndexMetaDataAttribute), true);
			foreach (IndexMetaDataAttribute attrib in attributes)
				if ((attrib.IndexMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsTriggerMetaDataSupported (object obj, TriggerMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (TriggerMetaDataAttribute), true);
			foreach (TriggerMetaDataAttribute attrib in attributes)
				if ((attrib.TriggerMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsCheckConstraintMetaDataSupported (object obj, CheckConstraintMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (CheckConstraintMetaDataAttribute), true);
			foreach (CheckConstraintMetaDataAttribute attrib in attributes)
				if ((attrib.CheckConstraintMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsTableColumnMetaDataSupported (object obj, ColumnMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (TableColumnMetaDataAttribute), true);
			foreach (TableColumnMetaDataAttribute attrib in attributes)
				if ((attrib.ColumnMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsViewColumnMetaDataSupported (object obj, ColumnMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (ViewColumnMetaDataAttribute), true);
			foreach (ViewColumnMetaDataAttribute attrib in attributes)
				if ((attrib.ColumnMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsForeignKeyConstraintMetaDataSupported (object obj, ForeignKeyConstraintMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (ForeignKeyConstraintMetaDataAttribute), true);
			foreach (ForeignKeyConstraintMetaDataAttribute attrib in attributes)
				if ((attrib.ForeignKeyConstraintMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsParameterMetaDataSupported (object obj, ParameterMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (ParameterMetaDataAttribute), true);
			foreach (ParameterMetaDataAttribute attrib in attributes)
				if ((attrib.ParameterMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsPrimaryKeyConstraintMetaDataSupported (object obj, PrimaryKeyConstraintMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (PrimaryKeyConstraintMetaDataAttribute), true);
			foreach (PrimaryKeyConstraintMetaDataAttribute attrib in attributes)
				if ((attrib.PrimaryKeyConstraintMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsProcedureMetaDataSupported (object obj, ProcedureMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (ProcedureMetaDataAttribute), true);
			foreach (ProcedureMetaDataAttribute attrib in attributes)
				if ((attrib.ProcedureMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsTableMetaDataSupported (object obj, TableMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (TableMetaDataAttribute), true);
			foreach (TableMetaDataAttribute attrib in attributes)
				if ((attrib.TableMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsUniqueConstraintMetaDataSupported (object obj, UniqueConstraintMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (UniqueConstraintMetaDataAttribute), true);
			foreach (UniqueConstraintMetaDataAttribute attrib in attributes)
				if ((attrib.UniqueConstraintMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsUserMetaDataSupported (object obj, UserMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (UserMetaDataAttribute), true);
			foreach (UserMetaDataAttribute attrib in attributes)
				if ((attrib.UserMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsViewMetaDataSupported (object obj, ViewMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (ViewMetaDataAttribute), true);
			foreach (ViewMetaDataAttribute attrib in attributes)
				if ((attrib.ViewMetaData & meta) == meta)
					return true;
			return false;
		}
		
		public static bool IsDatabaseMetaDataSupported (object obj, DatabaseMetaData meta)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			
			object[] attributes = obj.GetType ().GetCustomAttributes (typeof (DatabaseMetaDataAttribute), true);
			foreach (DatabaseMetaDataAttribute attrib in attributes)
				if ((attrib.DatabaseMetaData & meta) == meta)
					return true;
			return false;
		}
	}
}

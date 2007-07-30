using System;
using System.Collections;
using System.Text;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class EnumVariable: AbstractVariable
	{
		string name;
		StackFrame stackFrame;
		TargetEnumObject obj;
		
		string value;
		
		public override PixmapRef Image {
			get { return Pixmaps.PublicEnum; }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return value; }
		}
		
		public override string Type {
			get {
				return obj.TypeName;
			}
		}
		
		public EnumVariable(string name, StackFrame stackFrame, TargetEnumObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
			
			this.value = GetValue();
		}
		
		string GetValue()
		{
			// Most code taken from the ObjectFormatter
			
			TargetFundamentalObject fobj = (TargetFundamentalObject)obj.Value;
			object value = fobj.GetObject(stackFrame.Thread);
			
			SortedList members = new SortedList ();
			foreach (TargetFieldInfo field in obj.Type.Members) {
				members.Add(field.Name, field.ConstValue);
			}
			
			StringBuilder sb = new StringBuilder();
			
			if (!obj.Type.IsFlagsEnum) {
				foreach (DictionaryEntry entry in members) {
					if (entry.Value.Equals(value)) {
						return (string) entry.Key;
					}
				}
			} else if (value is ulong) {
				bool first = true;
				ulong the_value = (ulong) value;
				foreach (DictionaryEntry entry in members) {
					ulong fvalue = System.Convert.ToUInt64 (entry.Value);
					if ((the_value & fvalue) != fvalue)
						continue;
					if (!first) {
						sb.Append(" | ");
					}
					first = false;
					sb.Append((string) entry.Key);
				}
				if (!first)
					return sb.ToString();
			} else {
				bool first = true;
				long the_value = System.Convert.ToInt64 (value);
				foreach (DictionaryEntry entry in members) {
					long fvalue = System.Convert.ToInt64 (entry.Value);
					if ((the_value & fvalue) != fvalue)
						continue;
					if (!first) {
						sb.Append(" | ");
					}
					first = false;
					sb.Append((string) entry.Key);
				}
				if (!first)
					return sb.ToString();
			}
			
			return value.ToString();
		}
	}
}

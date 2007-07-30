using System;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class ErrorVariable: AbstractVariable
	{
		string name;
		string error;
		
		public override PixmapRef Image {
			get { return Pixmaps.Error; }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return String.Format("<error: {0}>", error); }
		}
		
		public ErrorVariable(string name, string error)
		{
			this.name = name;
			this.error = error;
		}
	}
}

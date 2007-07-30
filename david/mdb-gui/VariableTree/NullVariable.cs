using System;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class NullVariable: AbstractVariable
	{
		string name;
		
		public override PixmapRef Image {
			get { return Pixmaps.PublicClass; }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return "null"; }
		}
		
		public NullVariable(string name)
		{
			this.name = name;
		}
	}
}

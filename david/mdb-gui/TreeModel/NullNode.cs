using System;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class NullNode: AbstractNode
	{
		string name;
		
		public override Gdk.Pixbuf Image {
			get { return Pixmaps.PublicClass; }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return "null"; }
		}
		
		public NullNode(string name)
		{
			this.name = name;
		}
	}
}

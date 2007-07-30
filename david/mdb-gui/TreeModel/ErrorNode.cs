using System;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class ErrorNode: AbstractNode
	{
		string name;
		string error;
		
		public override Gdk.Pixbuf Image {
			get { return Pixmaps.Error.GetPixbuf(); }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return String.Format("<error: {0}>", error); }
		}
		
		public ErrorNode(string name, string error)
		{
			this.name = name;
			this.error = error;
		}
	}
}

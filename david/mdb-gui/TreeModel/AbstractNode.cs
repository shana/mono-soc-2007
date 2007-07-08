using System;

namespace Mono.Debugger.Frontend.TreeModel
{
	public abstract class AbstractNode
	{
		public virtual Gdk.Pixbuf Image {
			get { return null; }
		}
		
		public virtual string Name {
			get { return string.Empty; }
		}
		
		public virtual string Value {
			get { return string.Empty; }
		}
		
		public virtual string Type {
			get { return string.Empty; }
		}
		
		public virtual bool HasChildNodes {
			get { return false; }
		}
		
		public virtual AbstractNode[] ChildNodes {
			get { return new AbstractNode[0]; }
		}
	}
}

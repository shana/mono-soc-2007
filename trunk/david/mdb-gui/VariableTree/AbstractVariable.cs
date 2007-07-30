using System;

namespace Mono.Debugger.Frontend.TreeModel
{
	public abstract class AbstractVariable
	{
		public virtual PixmapRef Image {
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
		
		public virtual AbstractVariable[] ChildNodes {
			get { return new AbstractVariable[0]; }
		}
	}
}

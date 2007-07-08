#if Implementation
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public struct GeneratorPosition {
		#region Private Fields
		int index;
		int offset;
		#endregion

		#region Public Constructors
		public GeneratorPosition(int index, int offset) {
			this.index = index;
			this.offset = offset;
		}
		#endregion

		#region Public Properties
		public int Index {
			get { return index; }
			set { index = value; }
		}

		public int Offset {
			get { return offset; }
			set { offset = value; }
		}
		#endregion

		#region Public Methods
		public override bool Equals(object o) {
			if (!(o is GeneratorPosition))
				return false;
			return this == (GeneratorPosition)o;
		}

		public static bool Equals (object objA, object objB) {
			if (!(objA is GeneratorPosition) || !(objB is GeneratorPosition))
				return false;
			return (GeneratorPosition)objA == (GeneratorPosition)objB;
		}

		public override string ToString() {
			//WDTDH
			return base.ToString();
		}
		#endregion

		#region Public Operators
		public static bool operator ==(GeneratorPosition gp1, GeneratorPosition gp2) {
			return gp1.index == gp2.index && gp1.offset == gp2.offset;
		}

		public static bool operator !=(GeneratorPosition gp1, GeneratorPosition gp2) {
			return !(gp1 == gp2);
		}
		#endregion
	}
}
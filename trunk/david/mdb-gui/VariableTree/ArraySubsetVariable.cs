using System;
using System.Text;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	/// <summary>
	/// Represent a subset of array elements
	/// </summary>
	public class ArraySubsetVariable: AbstractVariable
	{
		const int SubsetOptimalSize = 20;
		const int SubsetMaximalSize = 32;
		
		StackFrame stackFrame;
		TargetArrayObject obj;
		
		int[] indicesPefix;
		// The current dimension
		int dimension;
		// The bounds of current dimension
		int lowerBound; // the first valid index
		int upperBound; // the last valid index  (!)
		// The start and end of the subset
		int startIndex;
		int endIndex;
		
		public override string Name {
			get {
				StringBuilder name = new StringBuilder();
				name.Append("[");
				// The indice prefix - eg 1,2,3,
				foreach(int i in indicesPefix) {
					name.Append(i);
					name.Append(",");
				}
				// The group range - eg 100..199
				if (startIndex != lowerBound || endIndex != upperBound) {
					name.Append(startIndex);
					name.Append("..");
					name.Append(endIndex);
				} else {
					name.Append("*");
				}
				// Comas for remaining dimensions
				for(int i = 0; i < obj.Type.Rank - dimension - 1; i++) {
					name.Append(",*");
				}
				name.Append("]");
				return name.ToString();
			}
		}
		
		public override bool HasChildNodes {
			get {
				return startIndex <= endIndex;
			}
		}
		
		public override AbstractVariable[] ChildNodes {
			get {
				return GetChildNodes();
			}
		}
		
		public ArraySubsetVariable(StackFrame stackFrame, TargetArrayObject obj, int[] indicesPefix, int startIndex, int endIndex)
		{
			this.stackFrame = stackFrame;
			this.obj = obj;
			this.indicesPefix = indicesPefix;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			
			dimension = indicesPefix.Length;
			lowerBound = obj.GetLowerBound(stackFrame.Thread, dimension);
			upperBound = obj.GetUpperBound(stackFrame.Thread, dimension) - 1;

			// Uncoment to test that the whole int range can be handled
			//lowerBound = int.MinValue;
			//upperBound = int.MaxValue;
		}
		
		public ArraySubsetVariable(StackFrame stackFrame, TargetArrayObject obj, int[] indicesPefix)
			: this(stackFrame, obj, indicesPefix, 0, 0)
		{
			this.startIndex = lowerBound;
			this.endIndex = upperBound;
		}
		
		AbstractVariable GetChildNode(int index)
		{
			int[] newIndices = new int[dimension + 1];
			indicesPefix.CopyTo(newIndices, 0);
			newIndices[dimension] = index;
			
			if (newIndices.Length == obj.Type.Rank) {
				// We have reached the last dimension - create the element
				TargetObject element;
				try {
					element = obj.GetElement(stackFrame.Thread, newIndices);
				} catch {
					return new ErrorVariable(IndicesToString(newIndices), "Can not get array element");
				}
				return VariableFactory.Create(IndicesToString(newIndices), element, stackFrame);
			} else {
				// Create a subset for the next dimension
				return new ArraySubsetVariable(stackFrame, obj, newIndices);
			}
		}
		
		public static string IndicesToString(int[] indices)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for(int i = 0; i < indices.Length; i++) {
				sb.Append(indices[i]);
				if (i < indices.Length - 1) {
					sb.Append(",");
				}
			}
			sb.Append("]");
			return sb.ToString();
		}
		
		AbstractVariable[] GetChildNodes()
		{
			long childCount = (long)endIndex - (long)startIndex + 1;
			if (childCount <= SubsetMaximalSize) {
				// Return array of all childs
				AbstractVariable[] childs = new AbstractVariable[childCount];
				for(int i = 0; i < childCount; i++) {
					childs[i] = GetChildNode(startIndex + i);
				}
				return childs;
			} else {
				// Subdivide to smaller groups
				long groupSize = SubsetOptimalSize;
				
				// The number of groups must not be too big either
				// Increase groupSize if necessary
				long groupCount;
				while(true) {
					groupCount = ((childCount - 1) / groupSize) + 1; // Round up
					if (groupCount <= SubsetMaximalSize) {
						break;
					} else {
						groupSize *= SubsetOptimalSize;
					}
				}
				
				// Return the smaller groups
				AbstractVariable[] childs = new AbstractVariable[groupCount];
				for(int i = 0; i < groupCount; i++) {
					long start = startIndex + i * groupSize;
					long end = System.Math.Min(upperBound, start + groupSize - 1);
					childs[i] = new ArraySubsetVariable(stackFrame, obj, indicesPefix, (int)start, (int)end);
				}
				return childs;
			}
		}
	}
}

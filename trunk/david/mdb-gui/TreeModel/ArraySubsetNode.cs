using System;
using System.Text;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	/// <summary>
	/// Represent a subset of array elements
	/// </summary>
	public class ArraySubsetNode: AbstractNode
	{
		const int SubsetOptimalSize = 5;
		const int SubsetMaximalSize = 7;
		
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
				}
				// Comas for remaining dimensions
				name.Append(',', obj.Type.Rank - dimension - 1);
				name.Append("]");
				return name.ToString();
			}
		}
		
		public override bool HasChildNodes {
			get {
				return startIndex <= endIndex;
			}
		}
		
		public override AbstractNode[] ChildNodes {
			get {
				return GetChildNodes();
			}
		}
		
		public ArraySubsetNode(StackFrame stackFrame, TargetArrayObject obj, int[] indicesPefix, int startIndex, int endIndex)
		{
			this.stackFrame = stackFrame;
			this.obj = obj;
			this.indicesPefix = indicesPefix;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			
			dimension = indicesPefix.Length;
			lowerBound = obj.GetLowerBound(stackFrame.Thread, dimension);
			upperBound = obj.GetUpperBound(stackFrame.Thread, dimension) - 1;
		}
		
		public ArraySubsetNode(StackFrame stackFrame, TargetArrayObject obj, int[] indicesPefix)
			: this(stackFrame, obj, indicesPefix, 0, 0)
		{
			this.startIndex = lowerBound;
			this.endIndex = upperBound;
		}
		
		AbstractNode GetChildNode(int index)
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
					return new ErrorNode(IndicesToString(newIndices), "Can not get array element");
				}
				return NodeFactory.Create(IndicesToString(newIndices), element, stackFrame);
			} else {
				// Create a subset for the next dimension
				return new ArraySubsetNode(stackFrame, obj, newIndices);
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
		
		AbstractNode[] GetChildNodes()
		{
			int childCount = endIndex - startIndex + 1;
			if (childCount <= SubsetMaximalSize) {
				// Return array of all childs
				AbstractNode[] childs = new AbstractNode[childCount];
				for(int i = 0; i < childCount; i++) {
					childs[i] = GetChildNode(startIndex + i);
				}
				return childs;
			} else {
				// Subdivide to smaller groups
				int groupSize = SubsetOptimalSize;
				
				// The number of groups must not be too big either
				// Increase groupSize if necessary
				int groupCount;
				while(true) {
					groupCount = ((childCount - 1) / groupSize) + 1; // Round up
					if (groupCount <= SubsetMaximalSize) {
						break;
					} else {
						groupSize = groupSize * groupSize;
					}
				}
				
				// Return the smaller groups
				AbstractNode[] childs = new AbstractNode[groupCount];
				for(int i = 0; i < groupCount; i++) {
					int start = startIndex + i * groupSize;
					int end = Math.Min(upperBound, start + groupSize - 1);
					childs[i] = new ArraySubsetNode(stackFrame, obj, indicesPefix, start, end);
				}
				return childs;
			}
		}
	}
}

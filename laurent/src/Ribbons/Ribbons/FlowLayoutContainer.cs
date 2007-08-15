using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class FlowLayoutContainer : Container
	{
		private List<Widget> children;
		private Requisition[] childReqs;
		
		public int NChildren
		{
			get { return children.Count; }
		}
		
		public FlowLayoutContainer()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.children = new List<Gtk.Widget> ();
		}
		
		public void AddWidget (Widget w)
		{
			children.Add (w);
		}
		
		public void RemoveWidget (Widget w)
		{
			children.Remove (w);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			int n = children.Count;
			childReqs = new Requisition[n];
			for(int i = 0 ; i < n ; ++i)
			{
				childReqs[i] = children[i].SizeRequest ();
			}
			
			if(WidthRequest != -1)
			{
				if(HeightRequest == -1)
				{
					int currentLineHeight = 0, currentLineWidth = 0;
					foreach(Widget w in children)
					{
						Requisition childReq = w.SizeRequest ();
						currentLineHeight = Math.Max (childReq.Height, currentLineHeight);
						currentLineWidth += childReq.Width;
						if(currentLineWidth >= WidthRequest)
						{
							currentLineHeight = 0;
							currentLineWidth = 0;
						}
					}
					requisition.Height = currentLineHeight;
				}
			}
			else // (WidthRequest == -1)
			{
				if(HeightRequest == -1)
				{
					foreach(Widget w in children)
					{
						Requisition childReq = w.SizeRequest ();
						requisition.Height = Math.Max (childReq.Height, requisition.Height);
						requisition.Width += childReq.Width;
					}
				}
				else
				{
#if !EXPERIMENTAL
					int totalWidth = 0;
					for(int i = 0 ; i < n ; ++i)
					{
						totalWidth += childReqs[i].Width;
					}
					
					// TODO: the following algorithm a dichotomic-like search approach (lower bound: 1, upper bound: number of widgets)
					
					int lineCount = -1;
					int totalHeight = 0;
					do
					{
						++lineCount;
						int lineWidth = (int)Math.Ceiling ((double)totalWidth / lineCount);
						int currentLineWidth = 0, currentLineHeight = 0;
						for(int i = 0 ; i < n ; ++i)
						{
							currentLineWidth += childReqs[i].Width;
							if(currentLineWidth > lineWidth)
							{
								totalHeight += currentLineHeight;
								currentLineWidth = 0;
								currentLineHeight = 0;
							}
							currentLineHeight = Math.Max (childReqs[i].Height, currentLineHeight);
						}
						totalHeight += currentLineHeight;
					} while(totalHeight < HeightRequest);
					
					if(totalHeight > HeightRequest) --lineCount;
					requisition.Width = (int)Math.Ceiling ((double)totalWidth / lineCount);
#else
					int height = 0;
					for(int i = 0 ; i < n ; ++i)
					{
						height = Math.Max (childReqs[i].Height, height);
					}
					
					List<int> segments = new List<int>();
					segments.Add (0);
					segments.Add (n);
					
					for(;;)
					{
						int bestCandidateHeight = int.MaxValue;
						int bestSplit = -1;
						for(int i = 1 ; i < segments.Count ; ++i)
						{
							int oldSum, newSum;
							int splitPos = SplitWidgetsInTwo (childReqs, segments[i-1], segments[i] - 1, out oldSum, out newSum);
							int candidate = height - oldSum + newSum;
							if(newSum < oldSum && candidate < bestCandidateHeight)
							{
								bestSplit = splitPos;
								bestCandidateHeight = candidate;
							}
						}
						
						if(bestCandidateHeight < HeightRequest)
						{
							segments.Insert (~segments.BinarySearch (bestSplit), bestSplit);
						}
						else break;
					}
					
					int currentSegmentWidth = 0, currentSegmentNr = 1;
					for(int i = 0 ; i < n ; ++i)
					{
						if(i == segments[currentSegmentNr])
						{
							++currentSegmentNr;
							WidthRequest = Math.Max (currentSegmentNr, WidthRequest);
							currentSegmentNr = 0;
						}
						currentSegmentWidth += childReqs[i].Width;
					}
					requisition.Width = Math.Max (currentSegmentNr, WidthRequest);
#endif
				}
			}
		}
		
#if EXPERIMENTAL
		private int SplitWidgetsInTwo (Requisition[] Requisitions, int Index, int Length, out int PreviousSum, out int NewSum)
		{
			int[] maxLeft = new int[Length], maxRight = new int[Length];
			maxLeft[0] = Requisitions[Index].Width;
			maxRight[Length-1] = Requisitions[Index+Length-1].Width;
			PreviousSum = 0;
			for(int i = 1 ; i < Length ; ++i)
			{
				maxLeft[i] = Math.Max (maxLeft[i-1] , Requisitions[i+Index].Width);
				maxRight[Length-1-i] = Math.Max (maxLeft[Length-i] , Requisitions[Length-1-i+Index].Width);
				PreviousSum += Requisitions[i+Index].Height;
			}
			
			int ret = 0, smallestSum = int.MaxValue;
			for(int i = 0 ; i < Length ; ++i)
			{
				int sum = maxLeft[i] + maxRight[i];
				if(sum < smallestSum)
				{
					smallestSum = sum;
					ret = i + Index + 1;
				}
			}
			
			NewSum = smallestSum;
			return ret;
		}
#endif
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			int n = children.Count;
			Gdk.Rectangle childAlloc = allocation;
			int lineHeight = 0;
			for(int i = 0 ; i < n ; ++i)
			{
				childAlloc.Width = childReqs[i].Width;
				childAlloc.Height = childReqs[i].Height;
				
				if(childAlloc.X != allocation.X && childAlloc.Right > allocation.Right)
				{
					childAlloc.X = allocation.X;
					childAlloc.Y += lineHeight;
					lineHeight = 0;
				}
				
				children[i].SizeAllocate (childAlloc);
				childAlloc.X += childAlloc.Width;
				lineHeight = Math.Max (childAlloc.Height, lineHeight);
			}
		}
	}
}

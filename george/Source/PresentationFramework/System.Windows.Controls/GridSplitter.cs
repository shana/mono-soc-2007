//
// GridSplitter.cs
//
// Author:
//   George Giolfan (georgegiolfan@yahoo.com)
//
// Copyright (C) 2007 George Giolfan
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//TODO: Support preview mode.

using System.Collections;
using System.Windows.Input;
using System.Windows.Automation.Peers;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls
{
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty (Property = "PreviewStyle", StyleTargetType = typeof (Control))]
	public class GridSplitter : global::System.Windows.Controls.Primitives.Thumb
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty DragIncrementProperty = DependencyProperty.Register ("DragIncrement", typeof (double), typeof (GridSplitter), new FrameworkPropertyMetadata (1D), ValidatePositiveValue);
		public static readonly DependencyProperty KeyboardIncrementProperty = DependencyProperty.Register ("KeyboardIncrement", typeof (double), typeof (GridSplitter), new FrameworkPropertyMetadata (10D), ValidatePositiveValue);
		public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register ("PreviewStyle", typeof (Style), typeof (GridSplitter), new FrameworkPropertyMetadata ());
		public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register ("ResizeBehavior", typeof (GridResizeBehavior), typeof (GridSplitter), new FrameworkPropertyMetadata ());
		public static readonly DependencyProperty ResizeDirectionProperty = DependencyProperty.Register ("ResizeDirection", typeof (GridResizeDirection), typeof (GridSplitter), new FrameworkPropertyMetadata ());
		public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register ("ShowsPreview", typeof (bool), typeof (GridSplitter), new FrameworkPropertyMetadata ());
		#endregion
		#endregion

		#region Static Constructor
		static GridSplitter ()
		{
#if Implementation
			Theme.Load ();
#endif
			HorizontalAlignmentProperty.AddOwner (typeof (GridSplitter), new FrameworkPropertyMetadata (HorizontalAlignment.Right));
			DefaultStyleKeyProperty.OverrideMetadata (typeof (GridSplitter), new FrameworkPropertyMetadata (typeof (GridSplitter)));
		}
		#endregion

		#region Public Constructors
		public GridSplitter ()
		{
			Focusable = true;

			DragDelta += delegate (object sender, global::System.Windows.Controls.Primitives.DragDeltaEventArgs e)
			{
				Grid grid = Parent as Grid;
				if (grid == null)
					return;
				GridResizeDirection resize_direction = GetActualResizeDirection ();
				double drag_increment = DragIncrement;
				if (drag_increment == 1)
					HandleChange (grid, resize_direction, resize_direction == GridResizeDirection.Rows ? e.VerticalChange : e.HorizontalChange);
				else {
					Point mouse_position = Mouse.GetPosition (this);
					double increments = Math.Round ((resize_direction == GridResizeDirection.Rows ? mouse_position.Y : mouse_position.X) / drag_increment);
					if (increments != 0)
						HandleChange (grid, resize_direction, increments * drag_increment);
				}
			};
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public double DragIncrement {
			get { return (double)GetValue (DragIncrementProperty); }
			set { SetValue (DragIncrementProperty, value); }
		}

		public double KeyboardIncrement {
			get { return (double)GetValue (KeyboardIncrementProperty); }
			set { SetValue (KeyboardIncrementProperty, value); }
		}

		public Style PreviewStyle {
			get { return (Style)GetValue (PreviewStyleProperty); }
			set { SetValue (PreviewStyleProperty, value); }
		}

		public GridResizeBehavior ResizeBehavior {
			get { return (GridResizeBehavior)GetValue (ResizeBehaviorProperty); }
			set { SetValue (ResizeBehaviorProperty, value); }
		}

		public GridResizeDirection ResizeDirection {
			get { return (GridResizeDirection)GetValue (ResizeDirectionProperty); }
			set { SetValue (ResizeDirectionProperty, value); }
		}

		public bool ShowsPreview {
			get { return (bool)GetValue (ShowsPreviewProperty); }
			set { SetValue (ShowsPreviewProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new GridSplitterAutomationPeer(this);
#endif
		}

		protected override void OnKeyDown (KeyEventArgs e)
		{
			base.OnKeyDown (e);
			Grid grid = Parent as Grid;
			if (grid == null)
				return;
			double keyboard_increment_multiplier = 0;
			GridResizeDirection resize_direction = GetActualResizeDirection ();
			if (resize_direction == GridResizeDirection.Rows) {
				switch (e.Key) {
				case Key.Up:
					keyboard_increment_multiplier = -1;
					break;
				case Key.Down:
					keyboard_increment_multiplier = 1;
					break;
				}
			} else {
				switch (e.Key) {
				case Key.Left:
					keyboard_increment_multiplier = -1;
					break;
				case Key.Right:
					keyboard_increment_multiplier = 1;
					break;
				}
			}
			if (keyboard_increment_multiplier != 0) {
				HandleChange (grid, resize_direction, KeyboardIncrement * keyboard_increment_multiplier);
				e.Handled = true;
			}
		}

		protected override void OnLostKeyboardFocus (KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus (e);
			//WDTDH
		}

		protected override void OnRenderSizeChanged (SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged (sizeInfo);
			Cursor = GetActualResizeDirection () == GridResizeDirection.Rows ? Cursors.SizeNS : Cursors.SizeWE;
		}
		#endregion

		#region Private Methods
		GridResizeBehavior GetActualResizeBehavior (GridResizeDirection resizeDirection)
		{
			GridResizeBehavior value = ResizeBehavior;
			if (value == GridResizeBehavior.BasedOnAlignment)
				if (resizeDirection == GridResizeDirection.Rows)
					switch (VerticalAlignment) {
					case VerticalAlignment.Top:
						return GridResizeBehavior.PreviousAndCurrent;
					case VerticalAlignment.Bottom:
						return GridResizeBehavior.CurrentAndNext;
					default:
						return GridResizeBehavior.PreviousAndNext;
					} else
					switch (HorizontalAlignment) {
					case HorizontalAlignment.Left:
						return GridResizeBehavior.PreviousAndCurrent;
					case HorizontalAlignment.Right:
						return GridResizeBehavior.CurrentAndNext;
					default:
						return GridResizeBehavior.PreviousAndNext;
					} else
				return value;
		}

		GridResizeDirection GetActualResizeDirection ()
		{
			GridResizeDirection value = ResizeDirection;
			if (value == GridResizeDirection.Auto)
				if (HorizontalAlignment != HorizontalAlignment.Stretch)
					return GridResizeDirection.Columns;
				else if (VerticalAlignment != VerticalAlignment.Stretch)
					return GridResizeDirection.Rows;
				else
					return ActualHeight >= ActualWidth ? GridResizeDirection.Columns : GridResizeDirection.Rows;
			else
				return value;
		}

		static bool CheckDefinitionIndex (ICollection collection, int value)
		{
			return value < 0 || value == collection.Count;
		}

		void HandleChange (Grid grid, GridResizeDirection resizeDirection, double change)
		{
			GridResizeBehavior resize_behavior = GetActualResizeBehavior (resizeDirection);
			int position_in_grid;
			int definition_index1;
			int definition_index2;
			double definition1_actual_size;
			double definition2_actual_size;
			double minimum_size1;
			double minimum_size2;
			if (resizeDirection == GridResizeDirection.Rows) {
				position_in_grid = Grid.GetRow (this);
				switch (resize_behavior) {
				case GridResizeBehavior.CurrentAndNext:
					definition_index1 = position_in_grid;
					definition_index2 = position_in_grid + 1;
					break;
				case GridResizeBehavior.PreviousAndCurrent:
					definition_index1 = position_in_grid - 1;
					definition_index2 = position_in_grid;
					break;
				default:
					definition_index1 = position_in_grid - 1;
					definition_index2 = position_in_grid + 1;
					break;
				}
				if (CheckDefinitionIndex (grid.RowDefinitions, definition_index1))
					return;
				if (CheckDefinitionIndex (grid.RowDefinitions, definition_index2))
					return;
				RowDefinition definition1 = grid.RowDefinitions [definition_index1];
				RowDefinition definition2 = grid.RowDefinitions [definition_index2];
				switch (resize_behavior) {
				case GridResizeBehavior.PreviousAndNext:
					minimum_size1 = 0;
					minimum_size2 = 0;
					break;
				default:
					switch (VerticalAlignment) {
					case VerticalAlignment.Top:
						minimum_size1 = 0;
						minimum_size2 = ActualHeight;
						break;
					case VerticalAlignment.Bottom:
						minimum_size1 = ActualHeight;
						minimum_size2 = 0;
						break;
					default:
						minimum_size1 = ActualHeight;
						minimum_size2 = 0;
						break;
					}
					break;
				}
				definition1_actual_size = definition1.ActualHeight;
				if (definition1_actual_size + change < minimum_size1)
					change = minimum_size1 - definition1_actual_size;
				definition2_actual_size = definition2.ActualHeight;
				if (definition2_actual_size - change < minimum_size2)
					change = definition2_actual_size - minimum_size2;
				definition1.Height = new GridLength (definition1_actual_size + change);
				definition2.Height = new GridLength (definition2_actual_size - change);
			} else {
				position_in_grid = Grid.GetColumn (this);
				switch (resize_behavior) {
				case GridResizeBehavior.CurrentAndNext:
					definition_index1 = position_in_grid;
					definition_index2 = position_in_grid + 1;
					break;
				case GridResizeBehavior.PreviousAndCurrent:
					definition_index1 = position_in_grid - 1;
					definition_index2 = position_in_grid;
					break;
				default:
					definition_index1 = position_in_grid - 1;
					definition_index2 = position_in_grid + 1;
					break;
				}
				if (CheckDefinitionIndex (grid.RowDefinitions, definition_index1))
					return;
				if (CheckDefinitionIndex (grid.RowDefinitions, definition_index2))
					return;
				ColumnDefinition definition1 = grid.ColumnDefinitions [definition_index1];
				ColumnDefinition definition2 = grid.ColumnDefinitions [definition_index2];
				switch (resize_behavior) {
				case GridResizeBehavior.PreviousAndNext:
					minimum_size1 = 0;
					minimum_size2 = 0;
					break;
				default:
					switch (HorizontalAlignment) {
					case HorizontalAlignment.Left:
						minimum_size1 = 0;
						minimum_size2 = ActualWidth;
						break;
					case HorizontalAlignment.Right:
						minimum_size1 = ActualWidth;
						minimum_size2 = 0;
						break;
					default:
						minimum_size1 = ActualWidth;
						minimum_size2 = 0;
						break;
					}
					break;
				}
				definition1_actual_size = definition1.ActualWidth;
				if (definition1_actual_size + change < minimum_size1)
					change = minimum_size1 - definition1_actual_size;
				definition2_actual_size = definition2.ActualWidth;
				if (definition2_actual_size - change < minimum_size2)
					change = definition2_actual_size - minimum_size2;
				definition1.Width = new GridLength (definition1_actual_size + change);
				definition2.Width = new GridLength (definition2_actual_size - change);
			}
		}

		static bool ValidatePositiveValue (object value)
		{
			return (double)value > 0;
		}
		#endregion
	}
}
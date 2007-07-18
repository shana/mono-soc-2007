using Mono.WindowsPresentationFoundation;
using System.ComponentModel;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public class TickBar : FrameworkElement {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(TickBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty IsDirectionReversedProperty = DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(TickBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty IsSelectionRangeEnabledProperty = DependencyProperty.Register("IsSelectionRangeEnabled", typeof(bool), typeof(TickBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(TickBar), new FrameworkPropertyMetadata(100D, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(TickBar), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(TickBarPlacement), typeof(TickBar), new FrameworkPropertyMetadata(TickBarPlacement.Top, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty ReservedSpaceProperty = DependencyProperty.Register("ReservedSpace", typeof(double), typeof(TickBar), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register("SelectionEnd", typeof(double), typeof(TickBar), new FrameworkPropertyMetadata(-1D, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(double), typeof(TickBar), new FrameworkPropertyMetadata(-1D, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), typeof(TickBar), new FrameworkPropertyMetadata(1D, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty TicksProperty = DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(TickBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public TickBar() {
			SnapsToDevicePixels = true;
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		[Bindable(true)]
		public bool IsDirectionReversed {
			get { return (bool)GetValue(IsDirectionReversedProperty); }
			set { SetValue(IsDirectionReversedProperty, value); }
		}

		[Bindable(true)]
		public bool IsSelectionRangeEnabled {
			get { return (bool)GetValue(IsSelectionRangeEnabledProperty); }
			set { SetValue(IsSelectionRangeEnabledProperty, value); }
		}

		[Bindable(true)]
		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		[Bindable(true)]
		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		[Bindable(true)]
		public TickBarPlacement Placement {
			get { return (TickBarPlacement)GetValue(PlacementProperty); }
			set { SetValue(PlacementProperty, value); }
		}

		[Bindable(true)]
		public double ReservedSpace {
			get { return (double)GetValue(ReservedSpaceProperty); }
			set { SetValue(ReservedSpaceProperty, value); }
		}

		[Bindable(true)]
		public double SelectionEnd {
			get { return (double)GetValue(SelectionEndProperty); }
			set { SetValue(SelectionEndProperty, value); }
		}

		[Bindable(true)]
		public double SelectionStart {
			get { return (double)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}

		[Bindable(true)]
		public double TickFrequency {
			get { return (double)GetValue(TickFrequencyProperty); }
			set { SetValue(TickFrequencyProperty, value); }
		}

		[Bindable(true)]
		public DoubleCollection Ticks {
			get { return (DoubleCollection)GetValue(TicksProperty); }
			set { SetValue(TicksProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if (Fill == null)
				return;
			bool horizontal = Placement == TickBarPlacement.Top || Placement == TickBarPlacement.Bottom;
			if (!horizontal ^ IsDirectionReversed)
				drawingContext.PushTransform(new ScaleTransform(1, -1, ActualWidth / 2, ActualHeight / 2));
			Pen pen = new Pen(Fill, 1);
			double offset = ReservedSpace / 2;
			double size_lenght;
			Point minimum_location;
			Point maximum_location;
			if (horizontal) {
				size_lenght = ActualHeight;
				minimum_location = new Point(offset, 0);
				maximum_location = new Point(ActualWidth - offset, 0);
			} else {
				size_lenght = ActualWidth;
				minimum_location = new Point(0, offset);
				maximum_location = new Point(0, ActualHeight - offset);
			}
			Utility.DrawLine(drawingContext, !horizontal, pen, minimum_location, size_lenght);
			Utility.DrawLine(drawingContext, !horizontal, pen, maximum_location, size_lenght);
			#region Ticks
			const double TickLenghtRatio = 3D / 4;
			double tick_lenght = Math.Max(TickLenghtRatio * size_lenght, 3);
			double tick_position_lenght = size_lenght - tick_lenght;
			double size_spacing = (horizontal ? ActualWidth : ActualHeight) - ReservedSpace;
			double tick_position_spacing;
			double value_size = size_spacing / (Maximum - Minimum);
			if (Ticks != null) {
				#region Place ticks where specified
				switch (Placement) {
					case TickBarPlacement.Top:
						foreach (double tick_value in Ticks) {
							tick_position_spacing = offset + (tick_value - Minimum) * value_size;
							Utility.DrawLine(drawingContext, false, pen, new Point(tick_position_spacing, tick_position_lenght), tick_lenght);
						}
						break;
					case TickBarPlacement.Bottom:
						foreach (double tick_value in Ticks) {
							tick_position_spacing = offset + (tick_value - Minimum) * value_size;
							Utility.DrawLine(drawingContext, false, pen, new Point(tick_position_spacing, 0), tick_lenght);
						}
						break;
					case TickBarPlacement.Left:
						foreach (double tick_value in Ticks) {
							tick_position_spacing = offset + (tick_value - Minimum) * value_size;
							Utility.DrawLine(drawingContext, true, pen, new Point(tick_position_lenght, tick_position_spacing), tick_lenght);
						}
						break;
					default:
						foreach (double tick_value in Ticks) {
							tick_position_spacing = offset + (tick_value - Minimum) * value_size;
							Utility.DrawLine(drawingContext, true, pen, new Point(0, tick_position_spacing), tick_lenght);
						}
						break;
				}
				#endregion
			} else {
				#region Place ticks at intervals
				int tick_count = (int)((Maximum - Minimum) / TickFrequency);
				double tick_spacing = size_spacing / tick_count;
				switch (Placement) {
					case TickBarPlacement.Top:
						for (int tick_index = 1; tick_index <= tick_count; tick_index++) {
							tick_position_spacing = offset + tick_index * tick_spacing;
							Utility.DrawLine(drawingContext, false, pen, new Point(tick_position_spacing, tick_position_lenght), tick_lenght);
						}
						break;
					case TickBarPlacement.Bottom:
						for (int tick_index = 1; tick_index <= tick_count; tick_index++) {
							tick_position_spacing = offset + tick_index * tick_spacing;
							Utility.DrawLine(drawingContext, false, pen, new Point(tick_position_spacing, 0), tick_lenght);
						}
						break;
					case TickBarPlacement.Left:
						for (int tick_index = 1; tick_index <= tick_count; tick_index++) {
							tick_position_spacing = offset + tick_index * tick_spacing;
							Utility.DrawLine(drawingContext, true, pen, new Point(tick_position_lenght, tick_position_spacing), tick_lenght);
						}
						break;
					default:
						for (int tick_index = 1; tick_index <= tick_count; tick_index++) {
							tick_position_spacing = offset + tick_index * tick_spacing;
							Utility.DrawLine(drawingContext, true, pen, new Point(0, tick_position_spacing), tick_lenght);
						}
						break;
				}
				#endregion
			}
			#endregion
			#region Selection
			if (IsSelectionRangeEnabled) {
				//FIXME: Minor issues.
				tick_lenght = Math.Max(tick_lenght, 4);
				tick_position_lenght = size_lenght - tick_lenght - 1;
				double selection_start_position = offset + (SelectionStart - Minimum) * value_size;
				double selection_end_position = offset + (SelectionEnd - Minimum) * value_size;
				switch (Placement) {
					case TickBarPlacement.Top:
						DrawTriangle(drawingContext,
							new Point(selection_start_position, tick_position_lenght),
							new Point(selection_start_position + tick_lenght, tick_position_lenght),
							new Point(selection_start_position, size_lenght)
						);
						DrawTriangle(drawingContext,
							new Point(selection_end_position, tick_position_lenght),
							new Point(selection_end_position - tick_lenght, tick_position_lenght),
							new Point(selection_end_position, size_lenght)
						);
						break;
					case TickBarPlacement.Bottom:
						DrawTriangle(drawingContext,
							new Point(selection_start_position, -1),
							new Point(selection_start_position, tick_lenght),
							new Point(selection_start_position + tick_lenght, tick_lenght)
						);
						DrawTriangle(drawingContext,
							new Point(selection_end_position, -1),
							new Point(selection_end_position, tick_lenght),
							new Point(selection_end_position - tick_lenght, tick_lenght)
						);
						break;
					case TickBarPlacement.Left:
						DrawTriangle(drawingContext,
							new Point(tick_position_lenght, selection_start_position),
							new Point(tick_position_lenght, selection_start_position + tick_lenght),
							new Point(size_lenght, selection_start_position)
						);
						DrawTriangle(drawingContext,
							new Point(tick_position_lenght, selection_end_position),
							new Point(tick_position_lenght, selection_end_position - tick_lenght),
							new Point(size_lenght, selection_end_position)
						);
						break;
					default:
						DrawTriangle(drawingContext,
							new Point(-1, selection_start_position),
							new Point(tick_lenght, selection_start_position + tick_lenght),
							new Point(tick_lenght, selection_start_position )
						);
						DrawTriangle(drawingContext,
							new Point(-1, selection_end_position),
							new Point(tick_lenght, selection_end_position - tick_lenght),
							new Point(tick_lenght, selection_end_position)
						);
						break;
				}
			}
			#endregion
			//HACK
			if (!horizontal)
				drawingContext.Pop();
		}
		#endregion

		#region Private Methods
		void DrawTriangle(DrawingContext drawingContext, Point point1, Point point2, Point point3) {
			StreamGeometry geometry = new StreamGeometry();
			using (StreamGeometryContext context = geometry.Open()) {
				context.BeginFigure(point1, true, true);
				context.LineTo(point2, false, false);
				context.LineTo(point3, false, false);
			}
			geometry.Freeze();
			drawingContext.DrawGeometry(Fill, null, geometry);
		}
		#endregion
	}
}
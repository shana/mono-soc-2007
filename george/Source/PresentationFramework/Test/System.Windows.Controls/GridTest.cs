using NUnit.Framework;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class GridTest {
		#region Measure
		[Test]
		public void Measure() {
			new MeasureGrid();
		}

		class MeasureGrid : Grid {
			static Size measure_constraint;
			static Size measure_result;

			public MeasureGrid() {
				Children.Add(new TestButton());
				Measure(new Size(100, 100));
				Assert.AreEqual(measure_constraint.Width, 100, "1");
				Assert.AreEqual(measure_result.Width, Utility.GetEmptyButtonSize(), "2");
				Assert.AreEqual(measure_result.Height, Utility.GetEmptyButtonSize(), "3");
				Assert.AreEqual(DesiredSize.Width, Utility.GetEmptyButtonSize(), "4");
				Assert.AreEqual(DesiredSize.Height, Utility.GetEmptyButtonSize(), "5");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Measure(new Size(100, 100));
				Assert.AreEqual(measure_constraint.Width, 100, "6");
				Assert.AreEqual(measure_result.Width, Utility.GetEmptyButtonSize(), "7");
				Assert.AreEqual(measure_result.Height, Utility.GetEmptyButtonSize(), "8");
				Assert.AreEqual(DesiredSize.Width, Utility.GetEmptyButtonSize(), "9");
				Assert.AreEqual(DesiredSize.Height, Utility.GetEmptyButtonSize(), "10");
			}

			protected override Size MeasureOverride(Size availableSize) {
				return measure_result = base.MeasureOverride(availableSize);
			}

			class TestButton : global::System.Windows.Controls.Button {
				protected override Size MeasureOverride(Size constraint) {
					measure_constraint = constraint;
					return base.MeasureOverride(constraint);
				}
			}
		}
		#endregion

		#region MeasureDefinitions
		[Test]
		public void MeasureDefinitions() {
			new MeasureDefinitionsGrid();
		}

		class MeasureDefinitionsGrid : Grid {
			static Size measure_constraint;
			static Size measure_result;

			public MeasureDefinitionsGrid() {
				RowDefinitions.Add(new RowDefinition());
				ColumnDefinitions.Add(new ColumnDefinition());
				Children.Add(new TestButton());
				Measure(new Size(100, 100));
				Assert.AreEqual(measure_constraint.Width, 100, "1");
				Assert.AreEqual(measure_result.Width, Utility.GetEmptyButtonSize(), "2");
				Assert.AreEqual(measure_result.Height, Utility.GetEmptyButtonSize(), "3");
				Assert.AreEqual(DesiredSize.Width, Utility.GetEmptyButtonSize(), "4");
				Assert.AreEqual(DesiredSize.Height, Utility.GetEmptyButtonSize(), "5");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Measure(new Size(100, 100));
				Assert.AreEqual(measure_constraint.Width, 100, "6");
				Assert.AreEqual(measure_result.Width, Utility.GetEmptyButtonSize(), "7");
				Assert.AreEqual(measure_result.Height, Utility.GetEmptyButtonSize(), "8");
				Assert.AreEqual(DesiredSize.Width, Utility.GetEmptyButtonSize(), "9");
				Assert.AreEqual(DesiredSize.Height, Utility.GetEmptyButtonSize(), "10");
			}

			protected override Size MeasureOverride(Size availableSize) {
				return measure_result = base.MeasureOverride(availableSize);
			}

			class TestButton : global::System.Windows.Controls.Button {
				protected override Size MeasureOverride(Size constraint) {
					measure_constraint = constraint;
					return base.MeasureOverride(constraint);
				}
			}
		}
		#endregion

		#region MeasureAuto
		[Test]
		public void MeasureAuto() {
			new MeasureAutoGrid();
		}

		class MeasureAutoGrid : Grid {
			static Size measure_constraint;

			public MeasureAutoGrid() {
				ColumnDefinition column_definitions = new ColumnDefinition();
				column_definitions.Width = GridLength.Auto;
				ColumnDefinitions.Add(column_definitions);
				ColumnDefinitions.Add(new ColumnDefinition());
				Children.Add(new TestButton());
				Measure(new Size(100, 100));
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Width));
			}

			class TestButton : global::System.Windows.Controls.Button {
				protected override Size MeasureOverride(Size constraint) {
					measure_constraint = constraint;
					return base.MeasureOverride(constraint);
				}
			}
		}
		#endregion

		#region MeasureChildWantsASpecificSize
		[Test]
		public void MeasureChildWantsASpecificSize() {
			new MeasureChildWantsASpecificSizeGrid();
		}

		class MeasureChildWantsASpecificSizeGrid : Grid {
			public MeasureChildWantsASpecificSizeGrid() {
				ColumnDefinition column_definitions = new ColumnDefinition();
				column_definitions.Width = new GridLength(50);
				ColumnDefinitions.Add(column_definitions);
				ColumnDefinitions.Add(new ColumnDefinition());
				Children.Add(new TestButton());
				Measure(new Size(100, 100));
				Assert.AreEqual(DesiredSize.Width, 50);
			}

			class TestButton : global::System.Windows.Controls.Button {
				protected override Size MeasureOverride(Size constraint) {
					return new Size(100, 100);
				}
			}
		}
		#endregion

		[Test]
		public void ColumnDefinitions() {
			Grid g = new Grid();
			Assert.IsNotNull(g.RowDefinitions, "1");
			Assert.IsNotNull(g.ColumnDefinitions, "2");
		}

		#region NullReferenceExceptionOverridingMeasureOverrideAndNotCallingBase
		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void NullReferenceExceptionOverridingMeasureOverrideAndNotCallingBase() {
			new NullReferenceExceptionOverridingMeasureOverrideAndNotCallingBaseGrid();
		}

		class NullReferenceExceptionOverridingMeasureOverrideAndNotCallingBaseGrid : Grid {
			public NullReferenceExceptionOverridingMeasureOverrideAndNotCallingBaseGrid() {
				RowDefinitions.Add(new RowDefinition());
				Children.Add(new global::System.Windows.Controls.Button());
				Window w = new Window();
				w.Content = this;
				w.Show();
			}
			protected override Size MeasureOverride(Size availableSize) {
				return new Size();
			}
		}
		#endregion

		[Test]
		public void DefinitionSizeAfterMeasure() {
			Grid g = new Grid();
			ColumnDefinition c = new ColumnDefinition();
			c.Width = GridLength.Auto;
			g.ColumnDefinitions.Add(c);
			g.ColumnDefinitions.Add(new ColumnDefinition());
			g.Children.Add(new global::System.Windows.Controls.Button());
			g.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			//LAMESPEC
			Assert.AreEqual(c.ActualWidth, 0, "1");
			g.Arrange(new Rect(0, 0, 100, 100));
			Assert.AreEqual(c.ActualWidth, Utility.GetEmptyButtonSize(), "2");
			g.RowDefinitions.Add(new RowDefinition());
			//LAMESPEC
			Assert.AreEqual(c.ActualWidth, Utility.GetEmptyButtonSize(), "3");
			g.ColumnDefinitions.Add(new ColumnDefinition());
			Assert.AreEqual(c.ActualWidth, 0, "4");
		}

		[Test]
		public void Button() {
			Grid g = new Grid();
			g.RowDefinitions.Add(new RowDefinition());
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			g.Children.Add(b);
			g.Measure(new Size(Utility.GetEmptyButtonSize() / 2, Utility.GetEmptyButtonSize() / 2));
			g.Arrange(new Rect(0, 0, Utility.GetEmptyButtonSize() / 2, Utility.GetEmptyButtonSize() / 2));
			Assert.AreEqual(b.DesiredSize.Width, 0, "1");
			Assert.AreEqual(b.DesiredSize.Height, 0, "2");
			Assert.AreEqual(b.ActualWidth, Utility.GetEmptyButtonSize() / 2, "3");
			Assert.AreEqual(b.ActualHeight, Utility.GetEmptyButtonSize() / 2, "4");
		}

		#region Arrange
		[Test]
		public void Arrange() {
			new ArrangeGrid();
		}

		class ArrangeGrid : Grid {
			Size ArrangeFinalSize;

			public ArrangeGrid() {
				global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
				Children.Add(b);
				RowDefinitions.Add(new RowDefinition());
				Window w = new Window();
				w.Content = this;
				w.Show();
				w.Height = 0;
				Assert.AreEqual(ArrangeFinalSize.Height, 0);
			}

			protected override Size ArrangeOverride(Size finalSize) {
				ArrangeFinalSize = finalSize;
				return base.ArrangeOverride(finalSize);
			}
		}
		#endregion

		#region GridPropertiesCauseLayoutChanges
		[Test]
		public void GridPropertiesCauseLayoutChanges() {
			FrameworkPropertyMetadata column_metadata = (FrameworkPropertyMetadata) Grid.ColumnProperty.DefaultMetadata;
			Assert.IsFalse(column_metadata.AffectsArrange, "Metadata 1");
			Assert.IsFalse(column_metadata.AffectsMeasure, "Metadata 2");
			Assert.IsFalse(column_metadata.AffectsParentArrange, "Metadata 3");
			Assert.IsFalse(column_metadata.AffectsParentMeasure, "Metadata 4");
			Assert.IsFalse(column_metadata.AffectsRender, "Metadata 5");
			Assert.IsNotNull(column_metadata.PropertyChangedCallback, "Metadata 6");
			Window w = new Window();
			GridPropertiesCauseLayoutChangesGrid g = new GridPropertiesCauseLayoutChangesGrid();
			w.Width = 500;
			w.Content = g;
			ColumnDefinition c1 = new ColumnDefinition();
			c1.Width = new GridLength(100);
			g.ColumnDefinitions.Add(c1);
			ColumnDefinition c2 = new ColumnDefinition();
			c2.Width = new GridLength(200);
			g.ColumnDefinitions.Add(c2);
			Button b = new Button();
			g.Children.Add(b);
			w.Show();
			Assert.IsTrue(g.ArrangeOverrideCalled, "Arrange 1");
			g.ArrangeOverrideCalled = false;
			Assert.IsTrue(g.MeasureOverrideCalled, "Measure 1");
			g.MeasureOverrideCalled = false;
			Assert.AreEqual(b.ActualWidth, 100, "1");
			Grid.SetColumn(b, 1);
			Assert.IsFalse(g.ArrangeOverrideCalled, "Arrange 2");
			Assert.IsFalse(g.MeasureOverrideCalled, "Measure 2");
			Assert.AreEqual(b.ActualWidth, 100, "2");
			g.RowDefinitions.Add(new RowDefinition());
			Grid.SetColumn(b, 0);
			Grid.SetColumn(b, 1);
			Assert.IsFalse(g.ArrangeOverrideCalled, "Arrange 3");
			Assert.IsFalse(g.MeasureOverrideCalled, "Measure 3");
			Assert.AreEqual(b.ActualWidth, 100, "3");
		}

		class GridPropertiesCauseLayoutChangesGrid : Grid {
			public bool ArrangeOverrideCalled;
			public bool MeasureOverrideCalled;

			protected override Size ArrangeOverride(Size finalSize) {
				ArrangeOverrideCalled = true;
				return base.ArrangeOverride(finalSize);
			}
			
			protected override Size MeasureOverride(Size availableSize) {
				MeasureOverrideCalled = true;
				return base.MeasureOverride(availableSize);
			}
		}
		#endregion

		[Test]
		public void StarSizing() {
			Grid g = new Grid();
			ColumnDefinition c1 = new ColumnDefinition();
			g.ColumnDefinitions.Add(c1);
			ColumnDefinition c2 = new ColumnDefinition();
			g.ColumnDefinitions.Add(c2);
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			g.Children.Add(b);
			Canvas c = new Canvas();
			c.Children.Add(g);
			Window w = new Window();
			w.Content = c;
			w.Show();
			Assert.AreEqual(c1.ActualWidth, Utility.GetEmptyButtonSize(), "1");
			Assert.AreEqual(c2.ActualWidth, 0, "2");
		}

		[Test]
		public void DefinitionOffset() {
			Grid g = new Grid();
			ColumnDefinition c1 = new ColumnDefinition();
			c1.Width = new GridLength(100);
			g.ColumnDefinitions.Add(c1);
			ColumnDefinition c2 = new ColumnDefinition();
			g.ColumnDefinitions.Add(c2);
			Assert.AreEqual(c2.Offset, 0, "1");
			g.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(c2.Offset, 0, "2");
			g.Arrange(new Rect(0, 0, 100, 100));
			Assert.AreEqual(c2.Offset, 100, "3");
		}

		#region ShowGridLines
		[Test]
		public void ShowGridLines() {
			new ShowGridLinesGrid();
		}

		class ShowGridLinesGrid : Grid {
			public ShowGridLinesGrid() {
				Assert.AreEqual(VisualChildrenCount, 0, "1");
				ShowGridLines = true;
				Assert.AreEqual(VisualChildrenCount, 0, "2");
				RowDefinitions.Add(new RowDefinition());
				Assert.AreEqual(VisualChildrenCount, 0, "3");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(VisualChildrenCount, 1, "4");
				Visual visual = GetVisualChild(0);
				Assert.IsTrue(visual is DrawingVisual, "4 1");
				DrawingVisual drawing_visual = (DrawingVisual)visual;
				Assert.IsNull(drawing_visual.Drawing, "4 2");
				ShowGridLines = false;
				Assert.AreEqual(VisualChildrenCount, 1, "5");
				RowDefinitions.Add(new RowDefinition());
				Assert.AreEqual(VisualChildrenCount, 1, "6");
				w.Content = null;
				Assert.AreEqual(VisualChildrenCount, 1, "7");
				visual = GetVisualChild(0);
				Assert.IsTrue(visual is DrawingVisual, "8");
				Assert.IsNull(Grid.ShowGridLinesProperty.DefaultMetadata.PropertyChangedCallback, "9");
				Assert.IsNull(Grid.ShowGridLinesProperty.ValidateValueCallback, "10");
				Assert.IsFalse(Grid.ShowGridLinesProperty.DefaultMetadata is FrameworkPropertyMetadata, "11");
				Assert.AreNotSame(Grid.ShowGridLinesProperty.DefaultMetadata, Grid.ShowGridLinesProperty.GetMetadata(typeof(Grid)), "12");
				FrameworkPropertyMetadata grid_metadata = (FrameworkPropertyMetadata)Grid.ShowGridLinesProperty.GetMetadata(typeof(Grid));
				Assert.IsNotNull(grid_metadata.PropertyChangedCallback, "13");
				Assert.IsFalse(grid_metadata.AffectsArrange, "14");
				Assert.IsFalse(grid_metadata.AffectsMeasure, "15");
				Assert.IsFalse(grid_metadata.AffectsParentArrange, "16");
				Assert.IsFalse(grid_metadata.AffectsParentMeasure, "17");
				Assert.IsFalse(grid_metadata.AffectsRender, "18");
			}
		}
		#endregion

		#region ShowGridLines2
		[Test]
		public void ShowGridLines2() {
			new ShowGridLines2Grid();
		}

		class ShowGridLines2Grid : Grid {
			public ShowGridLines2Grid() {
				RowDefinitions.Add(new RowDefinition());
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(VisualChildrenCount, 0, "1");
				ShowGridLines = true;
				Assert.AreEqual(VisualChildrenCount, 0, "2");
			}
		}
		#endregion

		#region GridLinesRendererPlace
		[Test]
		public void GridLinesRendererPlace() {
			new GridLinesRendererPlaceGrid();
		}

		class GridLinesRendererPlaceGrid : Grid {
			public GridLinesRendererPlaceGrid() {
				Children.Add(new Button());
				ShowGridLines = true;
				RowDefinitions.Add(new RowDefinition());
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.IsTrue(GetVisualChild(1) is DrawingVisual);
			}
		}
		#endregion

		#region WhereIsGridLinesRendererCreated
		[Test]
		public void WhereIsGridLinesRendererCreated() {
			new WhereIsGridLinesRendererCreatedGrid();
		}

		class WhereIsGridLinesRendererCreatedGrid : Grid {
			public WhereIsGridLinesRendererCreatedGrid() {
				ShowGridLines = true;
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(VisualChildrenCount, 0, "1");
				Arrange(new Rect(0, 0, 100, 100));
				Assert.AreEqual(VisualChildrenCount, 0, "2");
				Window w = new Window();
				w.Content = this;
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(VisualChildrenCount, 0, "3");
				Arrange(new Rect(0, 0, 100, 100));
				Assert.AreEqual(VisualChildrenCount, 0, "4");
			}
		}
		#endregion

		#region WhereIsGridLinesRendererCreatedDefinition
		[Test]
		public void WhereIsGridLinesRendererCreatedDefinition() {
			new WhereIsGridLinesRendererCreatedDefinitionGrid();
		}

		class WhereIsGridLinesRendererCreatedDefinitionGrid : Grid {
			public WhereIsGridLinesRendererCreatedDefinitionGrid() {
				ShowGridLines = true;
				RowDefinitions.Add(new RowDefinition());
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(VisualChildrenCount, 0, "1");
				Arrange(new Rect(0, 0, 100, 100));
				Assert.AreEqual(VisualChildrenCount, 1, "2");
			}
		}
		#endregion

		#region WhereIsGridLinesRendererCreatedDefinition2
		[Test]
		public void WhereIsGridLinesRendererCreatedDefinition2() {
			new WhereIsGridLinesRendererCreatedDefinition2Grid();
		}

		class WhereIsGridLinesRendererCreatedDefinition2Grid : Grid {
			public WhereIsGridLinesRendererCreatedDefinition2Grid() {
				ShowGridLines = true;
				RowDefinitions.Add(new RowDefinition());
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Arrange(new Rect(0, 0, 100, 100));
			}
			
			protected override Size MeasureOverride(Size availableSize) {
				Assert.AreEqual(VisualChildrenCount, 0, "1");
				Size result = base.MeasureOverride(availableSize);
				Assert.AreEqual(VisualChildrenCount, 0, "2");
				return result;
			}
			
			protected override Size ArrangeOverride(Size finalSize) {
				Assert.AreEqual(VisualChildrenCount, 0, "3");
				Size result = base.ArrangeOverride(finalSize);
				Assert.AreEqual(VisualChildrenCount, 1, "4");
				return result;
			}
		}
		#endregion

		#region WhereIsGridLinesRendererCreatedDefinition3
		[Test]
		public void WhereIsGridLinesRendererCreatedDefinition3() {
			new WhereIsGridLinesRendererCreatedDefinition3Grid();
		}

		class WhereIsGridLinesRendererCreatedDefinition3Grid : Grid {
			public WhereIsGridLinesRendererCreatedDefinition3Grid() {
				ShowGridLines = true;
				RowDefinitions.Add(new RowDefinition());
				Arrange(new Rect(0, 0, 100, 100));
			}

			protected override Size MeasureOverride(Size availableSize) {
				Assert.AreEqual(VisualChildrenCount, 0, "1");
				Size result = base.MeasureOverride(availableSize);
				Assert.AreEqual(VisualChildrenCount, 0, "2");
				return result;
			}

			protected override Size ArrangeOverride(Size finalSize) {
				Assert.AreEqual(VisualChildrenCount, 0, "3");
				Size result = base.ArrangeOverride(finalSize);
				Assert.AreEqual(VisualChildrenCount, 1, "4");
				return result;
			}
		}
		#endregion

		#region WhereIsGridLinesRendererCreatedDefinitionChildren
		[Test]
		public void WhereIsGridLinesRendererCreatedDefinitionChildren() {
			new WhereIsGridLinesRendererCreatedDefinitionChildrenGrid();
		}

		class WhereIsGridLinesRendererCreatedDefinitionChildrenGrid : Grid {
			public WhereIsGridLinesRendererCreatedDefinitionChildrenGrid() {
				Children.Add(new TestButton(this));
				ShowGridLines = true;
				RowDefinitions.Add(new RowDefinition());
				Arrange(new Rect(0, 0, 100, 100));
			}

			public int GetVisualChildrenCount() {
				return VisualChildrenCount;
			}

			class TestButton : global::System.Windows.Controls.Button {
				WhereIsGridLinesRendererCreatedDefinitionChildrenGrid grid;

				public TestButton(WhereIsGridLinesRendererCreatedDefinitionChildrenGrid grid) {
					this.grid = grid;
				}

				protected override Size ArrangeOverride(Size arrangeBounds) {
					Assert.AreEqual(grid.GetVisualChildrenCount(), 1);
					return base.ArrangeOverride(arrangeBounds);
				}
			}
		}
		#endregion

		#region ChangingDefinitionsCauseMeasure
		[Test]
		public void ChangingDefinitionsCauseMeasure() {
			new ChangingDefinitionsCauseMeasureGrid();
		}

		class ChangingDefinitionsCauseMeasureGrid : Grid {
			bool set_called;
			bool called;

			public ChangingDefinitionsCauseMeasureGrid() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				set_called = true;
				RowDefinitions.Add(new RowDefinition());
				RowDefinitions.Add(new RowDefinition());
				Assert.IsFalse(called);
			}

			protected override Size MeasureOverride(Size constraint) {
				if (set_called)
					called = true;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		#region ChangingDefinitionsCauseMeasureChildren
		[Test]
		public void ChangingDefinitionsCauseMeasureChildren() {
			new ChangingDefinitionsCauseMeasureChildrenGrid();
		}

		class ChangingDefinitionsCauseMeasureChildrenGrid : Grid {
			bool set_called;
			bool called;

			public ChangingDefinitionsCauseMeasureChildrenGrid() {
				Children.Add(new global::System.Windows.Controls.Button());
				Window w = new Window();
				w.Content = this;
				w.Show();
				set_called = true;
				RowDefinitions.Add(new RowDefinition());
				RowDefinitions.Add(new RowDefinition());
				Assert.IsFalse(called);
			}

			protected override Size MeasureOverride(Size constraint) {
				if (set_called)
					called = true;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		#region ChangingDefinitionsCauseMeasureChildren2
		[Test]
		public void ChangingDefinitionsCauseMeasureChildren2() {
			new ChangingDefinitionsCauseMeasureChildren2Grid();
		}

		class ChangingDefinitionsCauseMeasureChildren2Grid : Grid {
			bool set_called;
			bool called;

			public ChangingDefinitionsCauseMeasureChildren2Grid() {
				Children.Add(new global::System.Windows.Controls.Button());
				Window w = new Window();
				w.Content = this;
				w.Show();
				set_called = true;
				RowDefinitions.Add(new RowDefinition());
				RowDefinitions.Add(new RowDefinition());
				ColumnDefinitions.Add(new ColumnDefinition());
				Assert.IsFalse(called);
			}

			protected override Size MeasureOverride(Size constraint) {
				if (set_called)
					called = true;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		#region ChangingDefinitionsCauseMeasureChildren3
		[Test]
		public void ChangingDefinitionsCauseMeasureChildren3() {
			new ChangingDefinitionsCauseMeasureChildren3Grid();
		}

		class ChangingDefinitionsCauseMeasureChildren3Grid : Grid {
			bool set_called;
			bool called;

			public ChangingDefinitionsCauseMeasureChildren3Grid() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				global::System.Windows.Controls.Button b1 = new global::System.Windows.Controls.Button();
				Children.Add(b1);
				global::System.Windows.Controls.Button b2 = new global::System.Windows.Controls.Button();
				Grid.SetColumn(b2, 1);
				Children.Add(b2);
				set_called = true;
				RowDefinitions.Add(new RowDefinition());
				RowDefinitions.Add(new RowDefinition());
				ColumnDefinitions.Add(new ColumnDefinition());
				Assert.IsFalse(called);
			}

			protected override Size MeasureOverride(Size constraint) {
				if (set_called)
					called = true;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		[Test]
		public void GridAttachedPropertiesOutOfRange() {
			Grid g = new Grid();
			g.RowDefinitions.Add(new RowDefinition());
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			Grid.SetRow(b, 5);
			g.Children.Add(b);
			g.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			g.Arrange(new Rect(0, 0, 100, 100));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GridRowNegative() {
			Grid.SetRow(new UIElement(), -1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GridRowSpanZero() {
			Grid.SetRowSpan(new UIElement(), 0);
		}

		#region SettingDefinitionSize
		[Test]
		public void SettingDefinitionSize() {
			new SettingDefinitionSizeGrid();
		}

		class SettingDefinitionSizeGrid : Grid {
			bool set_called;
			bool called;
			bool called_arrange;

			public SettingDefinitionSizeGrid() {
				ColumnDefinition c = new ColumnDefinition();
				ColumnDefinitions.Add(c);
				set_called = true;
				c.Width = new GridLength(100);
				Assert.IsFalse(called, "1");
				Assert.IsFalse(called_arrange, "2");
				set_called = false;
				Window w = new Window();
				w.Content = this;
				w.Show();
				set_called = true;
				c.Width = new GridLength(101);
				Assert.IsFalse(called, "3");
				Assert.IsFalse(called_arrange, "4");
			}

			protected override Size MeasureOverride(Size availableSize) {
				if (set_called)
					called = true;
				return base.MeasureOverride(availableSize);
			}

			protected override Size ArrangeOverride(Size arrangeSize) {
				if (set_called)
					called_arrange = true;
				return base.ArrangeOverride(arrangeSize);
			}
		}
		#endregion

		#region OnVisualChildrenChanged
		[Test]
		public void OnVisualChildrenChanged() {
			new OnVisualChildrenChangedGrid();
		}

		class OnVisualChildrenChangedGrid : Grid {
			bool set_called;
			int calls;

			public OnVisualChildrenChangedGrid() {
				ShowGridLines = true;
				RowDefinitions.Add(new RowDefinition());
				set_called = true;
				Arrange(new Rect(0, 0, 100, 100));
				Assert.AreEqual(calls, 1);

			}

			protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
				if (set_called)
					calls++;
				base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			}
		}
		#endregion

		#region SettingShowGridLinesCausesMeasure
		[Test]
		public void SettingShowGridLinesCausesMeasure() {
			new SettingShowGridLinesCausesMeasureGrid();
		}

		class SettingShowGridLinesCausesMeasureGrid : Grid {
			bool set_calls;
			int calls;

			public SettingShowGridLinesCausesMeasureGrid() {
				RowDefinitions.Add(new RowDefinition());
				set_calls = true;
				ShowGridLines = true;
				Assert.AreEqual(calls, 0);
			}

			protected override Size MeasureOverride(Size availableSize) {
				if (set_calls)
					calls++;
				return base.MeasureOverride(availableSize);
			}
		}

		#endregion

		#region SettingShowGridLinesCausesMeasureInWindow
		[Test]
		public void SettingShowGridLinesCausesMeasureInWindow() {
			new SettingShowGridLinesCausesMeasureInWindowGrid();
		}

		class SettingShowGridLinesCausesMeasureInWindowGrid : Grid {
			bool set_calls;
			int calls;

			public SettingShowGridLinesCausesMeasureInWindowGrid() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				RowDefinitions.Add(new RowDefinition());
				set_calls = true;
				ShowGridLines = true;
				Assert.AreEqual(calls, 0);
			}

			protected override Size MeasureOverride(Size availableSize) {
				if (set_calls)
					calls++;
				return base.MeasureOverride(availableSize);
			}
		}

		#endregion

		[Test]
		public void MinWidth() {
			Window w = new Window();
			Grid g = new Grid();
			w.Content = g;
			w.Show();
			ColumnDefinition c = new ColumnDefinition();
			g.ColumnDefinitions.Add(c);
			g.ColumnDefinitions.Add(new ColumnDefinition());
			c.MinWidth = 100;
			c.Width = new GridLength(50);
			g.Arrange(new Rect(0, 0, 200, 200));
			Assert.AreEqual(c.ActualWidth, 100);
		}

		[Test]
		public void MaxWidth() {
			Grid g = new Grid();
			ColumnDefinition c = new ColumnDefinition();
			g.ColumnDefinitions.Add(c);
			g.ColumnDefinitions.Add(new ColumnDefinition());
			c.MaxWidth = 100;
			Window w = new Window();
			w.Content = g;
			w.Show();
			Assert.AreEqual(c.ActualWidth, 100);
		}

		#region MinWidthMeasureConstraint
		[Test]
		public void MinWidthMeasureConstraint() {
			Grid g = new Grid();
			ColumnDefinition c = new ColumnDefinition();
			g.ColumnDefinitions.Add(c);
			g.ColumnDefinitions.Add(new ColumnDefinition());
			c.MinWidth = 100;
			c.Width = new GridLength(50);
			g.Children.Add(new MinWidthMeasureConstraintButton());
			Window w = new Window();
			w.Content = g;
			w.Show();
			Assert.AreEqual(MinWidthMeasureConstraintButton.MeasureConstraint.Width, 100);
		}

		class MinWidthMeasureConstraintButton : global::System.Windows.Controls.Button {
			static public Size MeasureConstraint;

			protected override Size MeasureOverride(Size constraint) {
				MeasureConstraint = constraint;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		#region MultipleColumnSpan
		[Test]
		public void MultipleColumnSpan() {
			Grid g = new Grid();
			ColumnDefinition c = new ColumnDefinition();
			c.Width = new GridLength(100);
			g.ColumnDefinitions.Add(c);
			c = new ColumnDefinition();
			c.Width = new GridLength(100);
			g.ColumnDefinitions.Add(c);
			g.ColumnDefinitions.Add(new ColumnDefinition());
			MultipleColumnSpanButton b = new MultipleColumnSpanButton();
			Grid.SetColumnSpan(b, 2);
			g.Children.Add(b);
			Window w = new Window();
			w.Content = g;
			w.Show();
			Assert.AreEqual(b.MeasureConstraint.Width, 200);
		}

		[Test]
		public void MultipleColumnSpanStar() {
			Grid g = new Grid();
			g.ColumnDefinitions.Add(new ColumnDefinition());
			g.ColumnDefinitions.Add(new ColumnDefinition());
			g.ColumnDefinitions.Add(new ColumnDefinition());
			MultipleColumnSpanButton b = new MultipleColumnSpanButton();
			Grid.SetColumnSpan(b, 2);
			g.Children.Add(b);
			Window w = new Window();
			w.Content = g;
			w.Show();
			Assert.AreEqual(b.MeasureConstraint.Width, g.ColumnDefinitions[0].ActualWidth + g.ColumnDefinitions[1].ActualWidth);
		}

		class MultipleColumnSpanButton : global::System.Windows.Controls.Button {
			public Size MeasureConstraint;
			protected override Size MeasureOverride(Size constraint) {
				MeasureConstraint = constraint;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion
	}
}
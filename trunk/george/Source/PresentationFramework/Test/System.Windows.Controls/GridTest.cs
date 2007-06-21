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
				ShowGridLines = false;
				Assert.AreEqual(VisualChildrenCount, 1, "5");
				RowDefinitions.Add(new RowDefinition());
				Assert.AreEqual(VisualChildrenCount, 1, "6");
				w.Content = null;
				Assert.AreEqual(VisualChildrenCount, 1, "7");
				Visual visual = GetVisualChild(0);
				Assert.IsTrue(visual is DrawingVisual, "8");
				Assert.IsNull(Grid.ShowGridLinesProperty.DefaultMetadata.PropertyChangedCallback, "9");
				Assert.IsNull(Grid.ShowGridLinesProperty.ValidateValueCallback, "10");
				Assert.IsFalse(Grid.ShowGridLinesProperty.DefaultMetadata is FrameworkPropertyMetadata, "11");
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
	}
}
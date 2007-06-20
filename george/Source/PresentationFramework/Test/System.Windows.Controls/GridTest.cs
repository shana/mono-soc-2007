using NUnit.Framework;
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

			public MeasureGrid() {
				Children.Add(new TestButton());
				Measure(new Size(100, 100));
				Assert.AreEqual(measure_constraint.Width, 100, "1");
				Assert.AreEqual(DesiredSize.Width, Utility.GetEmptyButtonSize(), "2");
				Assert.AreEqual(DesiredSize.Height, Utility.GetEmptyButtonSize(), "2");
			}

			class TestButton : Button {
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

			class TestButton : Button {
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

			class TestButton : Button {
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
				Children.Add(new Button());
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
			g.Children.Add(new Button());
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
			Assert.AreEqual(b.ActualWidth, Utility.GetEmptyButtonSize() / 2, "1");
			Assert.AreEqual(b.ActualHeight, Utility.GetEmptyButtonSize() / 2, "2");
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
	}
}
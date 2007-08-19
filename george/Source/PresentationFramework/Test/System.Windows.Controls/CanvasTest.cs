using NUnit.Framework;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class CanvasTest
	{
		[Test]
		public void StaticProperties ()
		{
			PropertyMetadata property_metadata = Canvas.BottomProperty.GetMetadata (typeof (Canvas));
			Assert.IsTrue (property_metadata is FrameworkPropertyMetadata, "property_metadata type");
			FrameworkPropertyMetadata framework_property_metadata = (FrameworkPropertyMetadata)property_metadata;
			Assert.IsFalse (framework_property_metadata.AffectsArrange, "AffectsArrange");
			Assert.IsFalse (framework_property_metadata.AffectsMeasure, "AffectsMeasure");
			Assert.IsFalse (framework_property_metadata.AffectsRender, "AffectsRender");
			Assert.IsFalse (framework_property_metadata.SubPropertiesDoNotAffectRender, "SubPropertiesDoNotAffectRender");
			Assert.IsFalse (framework_property_metadata.AffectsParentArrange, "AffectsParentArrange");
			Assert.IsFalse (framework_property_metadata.AffectsParentMeasure, "AffectsParentMeasure");
		}

		#region MeasureOverride
		[Test]
		public void MeasureOverride ()
		{
			new MeasureOverrideCanvas ();
		}

		class MeasureOverrideCanvas : Canvas
		{
			public MeasureOverrideCanvas ()
			{
				Children.Add (new TestControl ());
				should_set_called = true;
				Size result = MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity));
				Assert.IsTrue (called, "MeasureOverride on child called 1");
				Assert.IsTrue (double.IsInfinity (child_constraint.Width), "Child constraint width");
				Assert.IsTrue (double.IsInfinity (child_constraint.Height), "Child constraint height");
				called = false;
				Assert.AreEqual (result.Width, 0, "Width 1");
				Assert.AreEqual (result.Height, 0, "Height 1");
				result = MeasureOverride (new Size (0, 0));
				Assert.IsFalse (called, "MeasureOverride on child called 2");
				called = false;
				Assert.AreEqual (result.Width, 0, "Width 2");
				Assert.AreEqual (result.Height, 0, "Height 2");
				result = MeasureOverride (new Size (2, 2));
				Assert.IsFalse (called, "MeasureOverride on child called 3");
				called = false;
				Assert.AreEqual (result.Width, 0, "Width 3");
				Assert.AreEqual (result.Height, 0, "Height 3");
			}
			static bool called;
			static bool should_set_called;
			static Size child_constraint = new Size (123, 456);
			class TestControl : Control
			{
				protected override Size MeasureOverride (Size constraint)
				{
					if (should_set_called) {
						child_constraint = constraint;
						called = true;
					}
					return base.MeasureOverride (constraint);
				}
			}
		}
		#endregion

		#region GetLayoutClip
		[Test]
		public void GetLayoutClip ()
		{
			new GetLayoutClipCanvas ();
		}

		class GetLayoutClipCanvas : Canvas
		{
			public GetLayoutClipCanvas ()
			{
				Assert.IsFalse (ClipToBounds, "ClipToBounds");
				Assert.IsNull (GetLayoutClip (new Size (100, 100)), "1");
				ClipToBounds = false;
				Assert.IsNull (GetLayoutClip (new Size (100, 100)), "2");
				ClipToBounds = true;
				Geometry result = GetLayoutClip (new Size (100, 100));
				Assert.IsNotNull (result, "3");
				Assert.IsTrue (result.GetType () == typeof (RectangleGeometry), "3 - type");
				RectangleGeometry rectangle = result as RectangleGeometry;
				Assert.AreEqual (rectangle.Rect, new Rect (0, 0, 0, 0), "3 - Rect");

			}
		}
		#endregion
	}
}
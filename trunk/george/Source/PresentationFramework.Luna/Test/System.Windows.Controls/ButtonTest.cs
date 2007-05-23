using NUnit.Framework;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if Implementation
using Mono.Microsoft.Windows.Themes;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using Microsoft.Windows.Themes;
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
    [TestFixture]
	public class ButtonTest {

		[Test]
		public void Creation() {
			Button button = new Button();
			Assert.IsNull(button.Content, "button.Content");
			Assert.IsNull(button.ContentTemplate, "button.ContentTemplate");

			Assert.AreEqual(button.HorizontalAlignment, HorizontalAlignment.Stretch, "button.HorizontalAlignment");
			Assert.AreEqual(button.VerticalAlignment, VerticalAlignment.Stretch, "button.VerticalAlignment");
#if !Implementation
			Assert.AreEqual(button.HorizontalContentAlignment, HorizontalAlignment.Left, "button.HorizontalContentAlignment");
			Assert.AreEqual(button.VerticalContentAlignment, VerticalAlignment.Top, "button.VerticalContentAlignment");
			Assert.IsNull(button.Template, "button.Template");
			Assert.IsNull(button.Background, "button.Background");
			Assert.IsNull(button.Style, "button.Style");
#endif
			Assert.AreEqual(button.MinWidth, 0, "MinWidth");
			Assert.AreEqual(button.MinHeight, 0, "MinHeight");
		}

		[Test]
		public void AfterContentIsSet() {
			Button button = new Button();
			button.Content = "";
			Assert.IsNull(button.TemplatedParent, "button.TemplatedParent");
			#region Background
			Assert.IsInstanceOfType(typeof(LinearGradientBrush), button.Background);
			LinearGradientBrush background_linear_gradient_brush = (LinearGradientBrush)button.Background;
			Assert.AreEqual(background_linear_gradient_brush.StartPoint, new Point(0.5, 0), "background_linear_gradient_brush.StartPoint");
			Assert.AreEqual(background_linear_gradient_brush.EndPoint, new Point(0.5, 1), "background_linear_gradient_brush.EndPoint");
			Assert.AreEqual(background_linear_gradient_brush.GradientStops.Count, 2, "background_linear_gradient_brush.GradientStops.Count");
			#region GradientStop 0
			GradientStop gradient_stop0 = background_linear_gradient_brush.GradientStops[0];
			Assert.AreEqual(gradient_stop0.Color, Colors.White, "gradient_stop0.Color");
			Assert.AreEqual(gradient_stop0.Offset, 0, "gradient_stop0.Offset");
			#endregion
			#region GradientStop 1
			GradientStop gradient_stop1 = background_linear_gradient_brush.GradientStops[1];
			Assert.AreEqual(gradient_stop1.Color, Color.FromArgb(0xFF, 0xF0, 0xF0, 0xEA), "gradient_stop1.Color");
			Assert.AreEqual(gradient_stop1.Offset, 0.9, "gradient_stop1.Offset");
			#endregion
			#endregion
			#region BorderBrush
			Assert.IsInstanceOfType(typeof(SolidColorBrush), button.BorderBrush, "button.BorderBrush Type");
			SolidColorBrush border_brush = (SolidColorBrush)button.BorderBrush;
			Assert.AreEqual(border_brush.Color, Color.FromArgb(0xFF, 0x00, 0x3C, 0x74), "border_brush.Color");
			#endregion
			#region Template
			ControlTemplate template = button.Template;
			Assert.IsInstanceOfType(typeof(ControlTemplate), template, "template Type");
			Assert.AreEqual(template.TargetType, typeof(ButtonBase), "template.TargetType");
			Assert.AreEqual(template.Resources.Count, 0, "template.Resources.Count");
			Assert.AreEqual(template.Triggers.Count, 3, "template.Triggers.Count");
			#region Trigger 0
			Assert.IsInstanceOfType(typeof(Trigger), template.Triggers[0], "template.Triggers[0] Type");
			Trigger trigger0 = (Trigger)button.Template.Triggers[0];
			Assert.AreEqual(trigger0.Property, UIElement.IsKeyboardFocusedProperty, "trigger0.Property");
			Assert.AreEqual(trigger0.Value, true, "trigger0.Value");
			Assert.AreEqual(trigger0.Setters.Count, 1, "trigger0.Setters.Count");
			#region Setter 0
			Assert.IsInstanceOfType(typeof(Setter), trigger0.Setters[0], "trigger0.Setters[0] Type");
			Setter setter_0_0 = (Setter)trigger0.Setters[0];

#if !Implementation
			//FIXME: I don't understand why this fails. See the VeryStrangeThing test.
			Assert.IsTrue(setter_0_0.Property.OwnerType == typeof(ButtonChrome), "setter_0_0.Property.OwnerType 1");
			Assert.AreEqual(setter_0_0.Property.OwnerType, typeof(ButtonChrome), "setter_0_0.Property.OwnerType");
			Assert.AreEqual(setter_0_0.Property, ButtonChrome.RenderDefaultedProperty, "setter_0_0.Property");
#endif
			Assert.AreEqual(setter_0_0.TargetName, "Chrome", "setter_0_0.TargetName");
			Assert.AreEqual(setter_0_0.Value, true, "setter_0_0.Value");
			#endregion
			#endregion
			#region Trigger 1
			Assert.IsInstanceOfType(typeof(Trigger), template.Triggers[1], "template.Triggers[1] Type");
			Trigger trigger1 = (Trigger)button.Template.Triggers[1];
			Assert.AreEqual(trigger1.Property, ToggleButton.IsCheckedProperty, "trigger1.Property");
			Assert.AreEqual(trigger1.Value, true, "trigger1.Value");
			Assert.AreEqual(trigger1.Setters.Count, 1, "trigger1.Setters.Count");
			#region Setter 0
			Assert.IsInstanceOfType(typeof(Setter), trigger1.Setters[0], "trigger1.Setters[0] Type");
			Setter setter_1_0 = (Setter)trigger1.Setters[0];
#if !Implementation
			//FIXME: Probably the same thing
			Assert.AreEqual(setter_1_0.Property, ButtonChrome.RenderPressedProperty, "setter_1_0.Property");
#endif
			Assert.AreEqual(setter_1_0.TargetName, "Chrome", "setter_1_0.TargetName");
			Assert.AreEqual(setter_1_0.Value, true, "setter_1_0.Value");
			#endregion
			#endregion
			#region Trigger 2
			Assert.IsInstanceOfType(typeof(Trigger), template.Triggers[2], "template.Triggers[2] Type");
			Trigger trigger2 = (Trigger)button.Template.Triggers[2];
			Assert.AreEqual(trigger2.Property, UIElement.IsEnabledProperty, "trigger2.Property");
			Assert.AreEqual(trigger2.Value, false, "trigger2.Value");
			Assert.AreEqual(trigger2.Setters.Count, 1, "trigger2.Setters.Count");
			#region Setter 0
			Assert.IsInstanceOfType(typeof(Setter), trigger2.Setters[0], "trigger2.Setters[0] Type");
			Setter setter_2_0 = (Setter)trigger2.Setters[0];
			Assert.AreEqual(setter_2_0.Property, TextElement.ForegroundProperty, "setter_2_0.Property");
			Assert.AreEqual(setter_2_0.TargetName, null, "setter_2_0.TargetName");
			Assert.IsInstanceOfType(typeof(DynamicResourceExtension), setter_2_0.Value, "setter_2_0.Value Type");
			DynamicResourceExtension settter_2_0DynamicResourceExtension = (DynamicResourceExtension)setter_2_0.Value;
			Assert.AreEqual(settter_2_0DynamicResourceExtension.ResourceKey.ToString(), "GrayTextBrush");
			#endregion
			#endregion
			#endregion
			#region Alignment
			Assert.AreEqual(button.HorizontalAlignment, HorizontalAlignment.Stretch, "button.HorizontalAlignment");
			Assert.AreEqual(button.VerticalAlignment, VerticalAlignment.Stretch, "button.VerticalAlignment");
			Assert.AreEqual(button.HorizontalContentAlignment, HorizontalAlignment.Center, "button.HorizontalContentAlignment");
			Assert.AreEqual(button.VerticalContentAlignment, VerticalAlignment.Center, "button.VerticalContentAlignment");
			#endregion
			Assert.IsNull(button.Template.VisualTree, "button.Template.VisualTree");
		}

		[Test]
		public void Others() {
			Button button = new Button();
			Assert.IsNull(VisualTreeHelper.GetClip(button), "VisualTreeHelper.GetClip(button)");
			Assert.AreEqual(VisualTreeHelper.GetContentBounds(button), Rect.Empty, "VisualTreeHelper.GetContentBounds(button)");
			Assert.AreEqual(VisualTreeHelper.GetDescendantBounds(button), Rect.Empty, "VisualTreeHelper.GetDescendantBounds(button)");
			Assert.AreEqual(VisualTreeHelper.GetEdgeMode(button), EdgeMode.Unspecified, "VisualTreeHelper.GetEdgeMode(button)");
		}

		[Test]
#if Implementation
		[Ignore("I don't know why only this fails. It may have something to do with the way NUnit copies assemblies.")]
#endif
		public void VeryStrangeThing() {
			Button b = new Button();
			b.Content = "123";
			object x = ((Setter)((Trigger)b.Template.Triggers[0]).Setters[0]).Property;
			object y = ButtonChrome.RenderDefaultedProperty;
			Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
			Assert.AreEqual(x, y);
		}

		[Test]
		public void Measure() {
			Button b = new Button();
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 0, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height");
			b.Measure(new Size(double.PositiveInfinity, 10));
			Assert.AreEqual(b.DesiredSize.Width, 0, "Width 2");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 2");
			b.Measure(new Size(double.PositiveInfinity, 7));
			Assert.AreEqual(b.DesiredSize.Width, 0, "Width 3");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 3");
			b.Measure(new Size(7, 7));
			Assert.AreEqual(b.DesiredSize.Width, 0, "Width 4");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 4");
		}

		#region MeasureOverride
		[Test]
		public void MeasureOverride() {
			new MeasureOverrideButton();
		}

		class MeasureOverrideButton : Button {
			public MeasureOverrideButton() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "Width");
				Assert.AreEqual(result.Height, 0, "Height");
			}
		}
		#endregion
	}
}
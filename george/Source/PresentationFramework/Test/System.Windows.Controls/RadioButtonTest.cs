using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class RadioButtonTest
	{
		[Test]
		public void StaticProperties ()
		{
			Assert.AreEqual (RadioButton.GroupNameProperty.Name, "GroupName", "GroupNameProperty.Name");
			Assert.AreEqual (RadioButton.GroupNameProperty.OwnerType, typeof (RadioButton), "GroupNameProperty.OwnerType");
			Assert.AreEqual (RadioButton.GroupNameProperty.PropertyType, typeof (string), "GroupNameProperty.PropertyType");
			Assert.AreEqual (RadioButton.GroupNameProperty.DefaultMetadata.DefaultValue, string.Empty, "GroupNameProperty.DefaultMetadata.DefaultValue");
		}

		[Test]
		public void Creation ()
		{
			Label label = new Label ();
			Assert.IsFalse (label.IsTabStop, "IsTabStop");
		}

		[Test]
		public void SameNonEmptyGroupName ()
		{
			RadioButton radio_buttton_1 = new RadioButton ();
			radio_buttton_1.IsChecked = true;
			radio_buttton_1.GroupName = "Group";

			RadioButton radio_buttton_2 = new RadioButton ();
			radio_buttton_2.IsChecked = true;
			radio_buttton_2.GroupName = "Group";

			Assert.IsTrue (radio_buttton_1.IsChecked.Value);
		}

		[Test]
		public void SameNonEmptyGroupNameAddedToACanvas ()
		{
			Canvas canvas = new Canvas ();
			RadioButton radio_buttton_1 = new RadioButton ();
			canvas.Children.Add (radio_buttton_1);
			radio_buttton_1.IsChecked = true;
			radio_buttton_1.GroupName = "Group";

			RadioButton radio_buttton_2 = new RadioButton ();
			canvas.Children.Add (radio_buttton_2);
			radio_buttton_2.IsChecked = true;
			radio_buttton_2.GroupName = "Group";

			Assert.IsTrue (radio_buttton_1.IsChecked.Value);
		}

		[Test]
		public void AddedToACanvas ()
		{
			Canvas canvas = new Canvas ();
			RadioButton radio_buttton_1 = new RadioButton ();
			canvas.Children.Add (radio_buttton_1);
			radio_buttton_1.IsChecked = true;

			RadioButton radio_buttton_2 = new RadioButton ();
			canvas.Children.Add (radio_buttton_2);
			radio_buttton_2.IsChecked = true;

			Assert.IsFalse (radio_buttton_1.IsChecked.Value);
		}

		[Test]
		public void SameNonEmptyGroupNameSetAfterIsCheckedAddedToACanvasInAWindow ()
		{
			Window window = new Window ();
			Canvas canvas = new Canvas ();
			window.Content = canvas;
			window.Show ();
			RadioButton radio_buttton_1 = new RadioButton ();
			canvas.Children.Add (radio_buttton_1);
			radio_buttton_1.IsChecked = true;
			radio_buttton_1.GroupName = "Group";

			RadioButton radio_buttton_2 = new RadioButton ();
			canvas.Children.Add (radio_buttton_2);
			radio_buttton_2.IsChecked = true;
			radio_buttton_2.GroupName = "Group";

			Assert.IsTrue (radio_buttton_1.IsChecked.Value);
		}

		[Test]
		public void SameNonEmptyGroupNameSetBeforeIsCheckedAddedToACanvasInAWindow ()
		{
			Window window = new Window ();
			Canvas canvas = new Canvas ();
			window.Content = canvas;
			window.Show ();
			RadioButton radio_buttton_1 = new RadioButton ();
			canvas.Children.Add (radio_buttton_1);
			radio_buttton_1.IsChecked = true;
			radio_buttton_1.GroupName = "Group";

			RadioButton radio_buttton_2 = new RadioButton ();
			canvas.Children.Add (radio_buttton_2);
			radio_buttton_2.GroupName = "Group";
			radio_buttton_2.IsChecked = true;

			Assert.IsFalse (radio_buttton_1.IsChecked.Value);
		}

		[Test]
		public void AddedToACanvasInAWindow ()
		{
			Window window = new Window ();
			Canvas canvas = new Canvas ();
			window.Content = canvas;
			window.Show ();
			RadioButton radio_buttton_1 = new RadioButton ();
			canvas.Children.Add (radio_buttton_1);
			radio_buttton_1.IsChecked = true;

			RadioButton radio_buttton_2 = new RadioButton ();
			canvas.Children.Add (radio_buttton_2);
			radio_buttton_2.IsChecked = true;

			Assert.IsFalse (radio_buttton_1.IsChecked.Value);
		}

		[Test]
		public void AddedToACanvasAfterSettingIsChecked ()
		{
			RadioButton b1 = new RadioButton ();
			b1.IsChecked = true;

			RadioButton b2 = new RadioButton ();
			b2.IsChecked = true;

			Canvas c = new Canvas ();
			c.Children.Add (b1);
			c.Children.Add (b2);

			Assert.IsTrue (b1.IsChecked.Value, "1");
			Assert.IsTrue (b2.IsChecked.Value, "2");
		}

		[Test]
		public void AddedToACanvasAfterSettingIsCheckedInAWindow ()
		{
			RadioButton b1 = new RadioButton ();
			b1.IsChecked = true;

			RadioButton b2 = new RadioButton ();
			b2.IsChecked = true;

			Canvas c = new Canvas ();
			c.Children.Add (b1);
			c.Children.Add (b2);

			Window w = new Window ();
			w.Content = c;
			w.Show ();

			Assert.IsTrue (b1.IsChecked.Value, "1");
			Assert.IsTrue (b2.IsChecked.Value, "2");
		}

		#region OnChecked
		[Test]
		public void OnChecked ()
		{
			Canvas c = new Canvas ();
			OnCheckedRadioButton b1 = new OnCheckedRadioButton ();
			c.Children.Add (b1);
			b1.IsChecked = true;
			OnCheckedRadioButton b2 = new OnCheckedRadioButton ();
			c.Children.Add (b2);
			b2.IsChecked = true;
			Assert.IsTrue (b1.IsChecked.Value);
		}

		class OnCheckedRadioButton : RadioButton
		{
			protected override void OnChecked (RoutedEventArgs e)
			{
			}
		}
		#endregion

		#region Order
		[Test]
		public void Order ()
		{
			Canvas c = new Canvas ();
			OrderRadioButton b1 = new OrderRadioButton (1);
			c.Children.Add (b1);
			b1.IsChecked = true;

			OrderRadioButton b2 = new OrderRadioButton (2);
			c.Children.Add (b2);
			b2.IsChecked = true;

			Assert.AreEqual (OrderRadioButton.Last, 2);
		}

		class OrderRadioButton : RadioButton
		{

			int id;
			static public int Last;

			public OrderRadioButton (int id)
			{
				this.id = id;
			}

			protected override void OnChecked (RoutedEventArgs e)
			{
				base.OnChecked (e);
				Last = id;
			}

			protected override void OnUnchecked (RoutedEventArgs e)
			{
				base.OnUnchecked (e);
				Last = id;
			}
		}
		#endregion
	}
}
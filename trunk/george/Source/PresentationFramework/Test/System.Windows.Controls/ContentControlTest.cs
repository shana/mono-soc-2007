using NUnit.Framework;
using System.Collections;
using System.Windows.Markup;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ContentControlTest
	{
		#region LogicalChildren
		[Test]
		public void LogicalChildren ()
		{
			new LogicalChildrenContentControl ();
		}

		class LogicalChildrenContentControl : ContentControl
		{
			public LogicalChildrenContentControl ()
			{
				IEnumerator c = LogicalChildren;
				//LAMESPEC
				Assert.IsNotNull (c, "1");
				Assert.IsFalse (c.MoveNext (), "2");
				Content = "1";
				c = LogicalChildren;
				Assert.IsTrue (c.MoveNext (), "3");
				Assert.AreEqual (c.Current, "1", "4");
				Assert.IsFalse (c.MoveNext (), "5");
				Content = "2";
				c = LogicalChildren;
				Assert.IsTrue (c.MoveNext (), "6");
				Assert.AreEqual (c.Current, "2", "7");
				Assert.IsFalse (c.MoveNext (), "8");
				Content = null;
				c = LogicalChildren;
				Assert.IsFalse (c.MoveNext (), "9");
			}
		}
		#endregion

		#region IAddChildMembersCallTheOtherMembers
		[Test]
		public void IAddChildMembersCallTheOtherMembers ()
		{
			new IAddChildMembersCallTheOtherMembersContentControl ();
		}

		class IAddChildMembersCallTheOtherMembersContentControl : ContentControl
		{
			int add_child_calls;
			int add_text_calls;

			public IAddChildMembersCallTheOtherMembersContentControl ()
			{
				IAddChild add_child = (IAddChild)this;
				Assert.AreEqual (add_child_calls, 0, "1");
				add_child.AddChild (1);
				Assert.AreEqual (add_child_calls, 1, "2");
				Content = null;
				Assert.AreEqual (add_text_calls, 0, "3");
				add_child.AddText ("1");
				Assert.AreEqual (add_text_calls, 1, "4");
			}

			protected override void AddChild (object value)
			{
				add_child_calls++;
				base.AddChild (value);
			}

			protected override void AddText (string text)
			{
				add_text_calls++;
				base.AddText (text);
			}
		}
		#endregion

		[Test]
		public void ShouldSerializeContent ()
		{
			ContentControl c = new ContentControl ();
			Assert.IsFalse (c.ShouldSerializeContent (), "1");
			Assert.IsFalse (c.ShouldSerializeContent (), "1 1");
			c.Content = null;
			Assert.IsTrue (c.ShouldSerializeContent (), "2");
			c.Content = "";
			Assert.IsTrue (c.ShouldSerializeContent (), "3");
			c.Content = null;
			Assert.IsTrue (c.ShouldSerializeContent (), "4");
			c.Content = 1;
			Assert.IsTrue (c.ShouldSerializeContent (), "5");
		}

		[Test]
		public void ShouldSerializeContent2 ()
		{
			ContentControl c = new ContentControl ();
			Assert.IsFalse (c.ShouldSerializeContent (), "1");
			Assert.IsFalse (c.ShouldSerializeContent (), "1 1");
			c.SetValue (ContentControl.ContentProperty, null);
			Assert.IsTrue (c.ShouldSerializeContent (), "2");
			c.SetValue (ContentControl.ContentProperty, "");
			Assert.IsTrue (c.ShouldSerializeContent (), "3");
			c.SetValue (ContentControl.ContentProperty, null);
			Assert.IsTrue (c.ShouldSerializeContent (), "4");
			c.SetValue (ContentControl.ContentProperty, 1);
			Assert.IsTrue (c.ShouldSerializeContent (), "5");
		}

		[Test]
		public void ContentProperty ()
		{
			Assert.IsNull (ContentControl.ContentProperty.ValidateValueCallback, "1");
			PropertyMetadata metadata = ContentControl.ContentProperty.GetMetadata (typeof (ContentControl));
			Assert.IsNull (metadata.CoerceValueCallback, "2");
			Assert.IsNotNull (metadata.PropertyChangedCallback, "3");
		}

		#region AddChildAddText
		[Test]
		public void AddChildAddText ()
		{
			new AddChildAddTextContentControl ();
		}

		class AddChildAddTextContentControl : ContentControl
		{
			public AddChildAddTextContentControl ()
			{
				Assert.IsNull (Content, "1");
				AddChild (1);
				Assert.AreEqual (Content, 1, "2");
				Content = null;
				Assert.IsNull (Content, "3");
				AddText ("1");
				Assert.AreEqual (Content, "1", "4");
				try {
					AddChild (2);
					Assert.Fail ("5");
				} catch (InvalidOperationException ex) {
					Assert.AreEqual (ex.Message, "Content of a ContentControl must be a single element.", "6");
				}
				try {
					AddText ("2");
					Assert.Fail ("7");
				} catch (InvalidOperationException ex) {
					Assert.AreEqual (ex.Message, "Content of a ContentControl must be a single element.", "8");
				}
			}
		}
		#endregion
	}
}
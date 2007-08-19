using NUnit.Framework;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
#if Implementation
	[Ignore ("Not implementable now")]
#endif
	public class ItemsPresenterTest
	{
		#region OnApplyTemplate
		#region OnApplyTemplateCallsGetVisualChild
		[Test]
		public void OnApplyTemplateCallsGetVisualChild ()
		{
			new OnApplyTemplateCallsGetVisualChildItemsPresenter ();
		}

		class OnApplyTemplateCallsGetVisualChildItemsPresenter : ItemsPresenter
		{
			int calls;
			int argument;

			public OnApplyTemplateCallsGetVisualChildItemsPresenter ()
			{
				Assert.AreEqual (calls, 0, "1");
				try {
					OnApplyTemplate ();
				} catch {
				}
				Assert.AreEqual (calls, 1, "2");
				Assert.AreEqual (argument, 0, "3");
			}

			protected override Visual GetVisualChild (int index)
			{
				calls++;
				argument = index;
				return base.GetVisualChild (index);
			}
		}
		#endregion

		#region VisualTreeMustBeASingleElement
		[Test]
		public void VisualTreeMustBeASingleElement ()
		{
			new VisualTreeMustBeASingleElementItemsPresenter ();
		}

		class VisualTreeMustBeASingleElementItemsPresenter : ItemsPresenter
		{
			public VisualTreeMustBeASingleElementItemsPresenter ()
			{
				try {
					OnApplyTemplate ();
					Assert.Fail ("1");
				} catch (InvalidOperationException ex) {
					Assert.AreEqual (ex.Message, "VisualTree of ItemsPanelTemplate must be a single element.", "2");
				}
			}

			protected override Visual GetVisualChild (int index)
			{
				return null;
			}
		}
		#endregion

		#region VisualTreeMustBeASingleElement2
		[Test]
		public void VisualTreeMustBeASingleElement2 ()
		{
			new VisualTreeMustBeASingleElement2ItemsPresenter ();
		}

		class VisualTreeMustBeASingleElement2ItemsPresenter : ItemsPresenter
		{
			public VisualTreeMustBeASingleElement2ItemsPresenter ()
			{
				try {
					OnApplyTemplate ();
					Assert.Fail ("1");
				} catch (InvalidOperationException ex) {
					Assert.AreEqual (ex.Message, "VisualTree of ItemsPanelTemplate must be a single element.", "2");
				}
			}

			protected override Visual GetVisualChild (int index)
			{
				return new FrameworkElement ();
			}
		}
		#endregion

		#region VisualTreeMustBeASingleElement3
		[Test]
		public void VisualTreeMustBeASingleElement3 ()
		{
			new VisualTreeMustBeASingleElement3ItemsPresenter ();
		}

		class VisualTreeMustBeASingleElement3ItemsPresenter : ItemsPresenter
		{
			public VisualTreeMustBeASingleElement3ItemsPresenter ()
			{
				try {
					OnApplyTemplate ();
					Assert.Fail ("1");
				} catch (InvalidOperationException ex) {
					Assert.AreEqual (ex.Message, "VisualTree of ItemsPanelTemplate must be a single element.", "2");
				}
			}

			protected override int VisualChildrenCount {
				get {
					return 1;
				}
			}

			protected override Visual GetVisualChild (int index)
			{
				return new FrameworkElement ();
			}
		}
		#endregion

		#region VisualTreeMustBeASingleElement4
		[Test]
		public void VisualTreeMustBeASingleElement4 ()
		{
			new VisualTreeMustBeASingleElement4ItemsPresenter ();
		}

		class VisualTreeMustBeASingleElement4ItemsPresenter : ItemsPresenter
		{
			public VisualTreeMustBeASingleElement4ItemsPresenter ()
			{
				AddVisualChild (new StackPanel ());
				Assert.AreEqual (VisualChildrenCount, 0, "1");
			}
		}
		#endregion
		#endregion
	}
}
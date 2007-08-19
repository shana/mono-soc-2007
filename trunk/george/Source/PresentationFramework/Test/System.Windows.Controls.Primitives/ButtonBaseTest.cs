using NUnit.Framework;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class ButtonBaseTest
	{
		[Test]
		public void StaticProperties ()
		{
			Assert.IsTrue (ButtonBase.IsPressedProperty.ReadOnly, "IsPressedProperty.ReadOnly");
		}

		#region OnMouseLeftButtonDownNullReferenceException
		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void OnMouseLeftButtonDownNullReferenceException ()
		{
			new OnMouseLeftButtonDownNullReferenceExceptionButtonBase ();
		}

		class OnMouseLeftButtonDownNullReferenceExceptionButtonBase : ButtonBase
		{
			public OnMouseLeftButtonDownNullReferenceExceptionButtonBase ()
			{
				OnMouseLeftButtonDown (null);
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInvalidOperationException
		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void OnMouseLeftButtonDownInvalidOperationException ()
		{
			new OnMouseLeftButtonDownInvalidOperationExceptionButtonBase ();
		}

		class OnMouseLeftButtonDownInvalidOperationExceptionButtonBase : ButtonBase
		{
			public OnMouseLeftButtonDownInvalidOperationExceptionButtonBase ()
			{
				OnMouseLeftButtonDown (new MouseButtonEventArgs (Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left));
			}
		}
		#endregion

		#region OnMouseLeftButtonDown
		[Test]
		public void OnMouseLeftButtonDown ()
		{
			new OnMouseLeftButtonDownButtonBase ();
		}

		class OnMouseLeftButtonDownButtonBase : ButtonBase
		{
			public OnMouseLeftButtonDownButtonBase ()
			{
				MouseButtonEventArgs e = new MouseButtonEventArgs (Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;

				Assert.IsFalse (IsPressed, "IsPressed - Before");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - Before");
				Assert.IsFalse (IsFocused, "IsFocused - Before");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - Before");
				OnMouseLeftButtonDown (e);
				Assert.IsFalse (IsPressed, "IsPressed - After");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - After");
				Assert.IsTrue (IsFocused, "IsFocused - After");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - After");
			}

			protected override void OnClick ()
			{
				base.OnClick ();
				Assert.IsFalse (IsPressed, "IsPressed - OnClick");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - OnClick");
				Assert.IsTrue (IsFocused, "IsFocused - OnClick");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - OnClick");
			}
		}
		#endregion

		#region OnMouseLeftButtonDownAddedToACanvas
		[Test]
		public void OnMouseLeftButtonDownAddedToACanvas ()
		{
			new OnMouseLeftButtonDownAddedToACanvasButtonBase ();
		}

		class OnMouseLeftButtonDownAddedToACanvasButtonBase : ButtonBase
		{
			public OnMouseLeftButtonDownAddedToACanvasButtonBase ()
			{
				Canvas canvas = new Canvas ();
				canvas.Children.Add (this);

				MouseButtonEventArgs e = new MouseButtonEventArgs (Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				Assert.IsNotNull (Parent, "Parent");

				Assert.IsFalse (IsPressed, "IsPressed - Before");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - Before");
				Assert.IsFalse (IsFocused, "IsFocused - Before");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - Before");
				OnMouseLeftButtonDown (e);
				Assert.IsFalse (IsPressed, "IsPressed - After");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - After");
				Assert.IsTrue (IsFocused, "IsFocused - After");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - After");
			}
		}
		#endregion

		#region OnMouseLeftButtonDownAddedToAWindow
		[Test]
		public void OnMouseLeftButtonDownAddedToAWindow ()
		{
			new OnMouseLeftButtonDownAddedToAWindowButtonBase ();
		}

		class OnMouseLeftButtonDownAddedToAWindowButtonBase : ButtonBase
		{
			public OnMouseLeftButtonDownAddedToAWindowButtonBase ()
			{
				Window window = new Window ();
				window.Content = this;
				window.Show ();

				MouseButtonEventArgs e = new MouseButtonEventArgs (Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				Assert.IsNotNull (Parent, "Parent");

				Assert.IsFalse (IsPressed, "IsPressed - Before");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - Before");
				Assert.IsFalse (IsFocused, "IsFocused - Before");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - Before");
				OnMouseLeftButtonDown (e);
				Assert.IsFalse (IsPressed, "IsPressed - After");
				Assert.IsFalse (IsMouseCaptured, "IsMouseCaptured - After");
				Assert.AreEqual (Mouse.Captured, null, "Mouse.Captured - After");
				Assert.IsTrue (IsFocused, "IsFocused - After");
			}
		}
		#endregion

		#region ClickEventCommandOrder
		[Test]
		public void ClickEventCommandOrder ()
		{
			new ClickEventCommandOrderButtonBase ();
		}

		class ClickEventCommandOrderButtonBase : ButtonBase
		{
			public ClickEventCommandOrderButtonBase ()
			{
				Command = new TestCommand ();
				Click += OnClick;
				OnClick ();
			}

			void OnClick (object sender, RoutedEventArgs e)
			{
				Assert.IsFalse (((TestCommand)Command).Executed);
			}

			class TestCommand : ICommand
			{
				public bool Executed;

				public bool CanExecute (object parameter)
				{
					return true;
				}

				public event EventHandler CanExecuteChanged;

				public void Execute (object parameter)
				{
					Executed = true;
				}
			}
		}
		#endregion

		#region Command
		[Test]
		public void Command ()
		{
			new CommandButtonBase ();
		}

		class CommandButtonBase : ButtonBase
		{
			public CommandButtonBase ()
			{
				Command = new TestCommand ();
				CommandParameter = Parameter;
				OnClick ();
				Assert.IsTrue (Executed, "Executed");
			}

			static object Parameter;
			static bool Executed;

			class TestCommand : ICommand
			{
				public bool CanExecute (object parameter)
				{
					return true;
				}

				public event EventHandler CanExecuteChanged;

				public void Execute (object parameter)
				{
					Assert.AreEqual (parameter, Parameter, "Parameter");
					Executed = true;
				}
			}
		}
		#endregion

		#region OnAccessKey
		[Test]
		public void OnAccessKey ()
		{
			new OnAccessKeyButtonBase ();
		}

		class OnAccessKeyButtonBase : ButtonBase
		{
			bool on_click_called;

			public OnAccessKeyButtonBase ()
			{
				Content = "_1";
				AccessKeyManager.ProcessKey (null, "1", false);
				Assert.IsFalse (on_click_called);
			}

			protected override void OnClick ()
			{
				base.OnClick ();
				on_click_called = true;
			}
		}
		#endregion

		[Test]
		public void AccessKeyManagerTest ()
		{
			SimpleButtonBase b = new SimpleButtonBase ();
			Assert.IsFalse (AccessKeyManager.IsKeyRegistered (null, "1"), "Before");
			b.Content = "_1";
			Assert.IsFalse (AccessKeyManager.IsKeyRegistered (null, "1"), "After");
		}

		[Test]
		public void Alignment ()
		{
			SimpleButtonBase b = new SimpleButtonBase ();
			Assert.AreEqual (b.HorizontalContentAlignment, HorizontalAlignment.Left, "HorizontalContentAlignment - Before");
			Assert.AreEqual (b.VerticalContentAlignment, VerticalAlignment.Top, "VerticalContentAlignment - Before");
			Assert.AreEqual (b.HorizontalAlignment, HorizontalAlignment.Stretch, "HorizontalAlignment - Before");
			Assert.AreEqual (b.VerticalAlignment, VerticalAlignment.Stretch, "VerticalAlignment - Before");
			b.Content = new Image ();
			Assert.AreEqual (b.HorizontalContentAlignment, HorizontalAlignment.Left, "HorizontalContentAlignment - After");
			Assert.AreEqual (b.VerticalContentAlignment, VerticalAlignment.Top, "VerticalContentAlignment - After");
			Assert.AreEqual (b.HorizontalAlignment, HorizontalAlignment.Stretch, "HorizontalAlignment - After");
			Assert.AreEqual (b.VerticalAlignment, VerticalAlignment.Stretch, "VerticalAlignment - After");
		}

		class SimpleButtonBase : ButtonBase
		{
		}
	}
}
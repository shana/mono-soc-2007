/* vi:set ts=4 sw=4: */
//
// ControlDesignerTest
//
// Authors:
//	  Ivan N. Zlatev (contact i-nZ.net)
//
// (C) 2007 Ivan N. Zlatev

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using NUnit.Core;
using NUnit.Framework;

#if !MS_NET
using Mono.Design;
#endif

namespace Mono.Design.Test
{

	[TestFixture]
	public class ControlDesignerTest : ControlDesigner
	{
		private DesignerTestHelper _helper;

		public ControlDesignerTest()
		{
			_helper = new DesignerTestHelper ();
		}

		[SetUp]
		public void InitializeSurface ()
		{
			_helper.InitializeDesignSurface ();
		}

		[TearDown]
		public void DisposeSurface ()
		{
			_helper.DisposeDesignSurface ();
		}

		[Test]
		public void InitializeTest ()
		{
			Button button = (Button) _helper.CreateControl (typeof (TestButton), null);

			Assert.IsFalse ((bool)_helper.GetValue (button, "Locked"), "#1");
			Assert.IsTrue ((bool)_helper.GetValue (button, "Visible"), "#2");
			Assert.IsTrue ((bool)_helper.GetValue (button, "Enabled"), "#3");
			Assert.IsFalse ((bool)_helper.GetValue (button, "AllowDrop"), "#4");
			Assert.IsTrue (button.Enabled, "#5");
			Assert.IsTrue (button.Visible, "#6");
			Assert.IsFalse (button.AllowDrop, "#7");
#if NET_2_0
			ControlDesigner buttonDesigner = _helper.IDesignerHost.GetDesigner (button) as ControlDesigner;
			Assert.AreEqual (null, buttonDesigner.InternalControlDesigner (20));
			Assert.AreEqual (0, buttonDesigner.NumberOfInternalControlDesigners ());
#endif

		}

		[Test]
		public void SelectionRulesTest ()
		{
			Button button = (Button) _helper.CreateControl (typeof (TestButton), null);
			ControlDesigner designer = _helper.IDesignerHost.GetDesigner (button) as ControlDesigner;

			SelectionRules selectionRules = SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible;
			Assert.AreEqual (selectionRules, designer.SelectionRules, "#1");

			button.Dock = DockStyle.Left;
			selectionRules = SelectionRules.RightSizeable | SelectionRules.Visible;
			Assert.AreEqual (selectionRules, designer.SelectionRules, "#2");

			button.Dock = DockStyle.Fill;
			selectionRules = SelectionRules.Visible;
			Assert.AreEqual (selectionRules, designer.SelectionRules, "#3");
		}

		[Test]
		public void CanBeParentedToTest ()
		{
			ControlDesigner rootPanel = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel),  null));
			ControlDesigner childPanel = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel), rootPanel.Control));
			ControlDesigner rootButton = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestButton), null));
			ControlDesigner childPanelButton = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestButton), childPanel.Control));
			ControlDesigner rootPanelButton = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestButton), rootPanel.Control));

			Assert.IsFalse (rootButton.CanBeParentedTo (childPanelButton), "#1");
			Assert.IsFalse (((ControlDesigner)_helper.IDesignerHost.GetDesigner (_helper.IDesignerHost.RootComponent)).CanBeParentedTo (rootButton), "#2");
			Assert.IsTrue (childPanelButton.CanBeParentedTo (childPanel), "#3");
			Assert.IsTrue (childPanel.CanBeParentedTo (childPanel), "#4");
			Assert.IsFalse (childPanel.CanBeParentedTo (childPanelButton), "#5");
			Assert.IsTrue (rootPanelButton.CanBeParentedTo (childPanel), "#6");
			Assert.IsTrue (childPanel.CanBeParentedTo (rootPanel), "#7");
			Assert.IsFalse (rootPanel.CanBeParentedTo (childPanel), "#8");
		}

		[Test]
		public void LocationTest ()
		{
			Panel rootPanel = (Panel) _helper.CreateControl (typeof (TestPanel),  null);
			Button childButton = (Button) _helper.CreateControl (typeof (TestButton), rootPanel);
			rootPanel.AutoScroll = true;
			_helper.SetValue (rootPanel, "Location",  new Point (50, 50));
			_helper.SetValue (rootPanel, "Size", new Size (200, 200));
			_helper.SetValue (childButton, "Location", new Point (100, 100));
			Assert.AreEqual (new Point (100, 100), (Point) _helper.GetValue (childButton, "Location"), "#1");
		}

		[Test]
		public void AssociatedComponentsTest ()
		{
			ControlDesigner rootPanel = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel),  null));
			ControlDesigner childPanel = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel), rootPanel.Control));
			Button b = new Button ();
			rootPanel.Control.Controls.Add (b);
			Assert.AreEqual (1, rootPanel.AssociatedComponents.Count, "#1");
			IComponent[] array = new IComponent[rootPanel.AssociatedComponents.Count];
			rootPanel.AssociatedComponents.CopyTo (array, 0);
			Assert.AreEqual (childPanel.Component, array[0],"#2");
		}

#if NET_2_0
		[Test]
		public void ParentComponentTest ()
		{
			ControlDesigner rootPanel = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel),  null));
			ControlDesignerTest childButton = (ControlDesignerTest) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestButton), rootPanel.Control));
			Assert.AreEqual (rootPanel.Control, childButton.Control.Parent, "#1");
			Assert.AreEqual (childButton.ParentComponent, rootPanel.Component, "#2");
		}
#endif
	}
}

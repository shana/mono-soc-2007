/* vi:set ts=4 sw=4: */
//
// ParentControlDesignerTest
//
// Authors:
//	  Ivan N. Zlatev <contact i-nZ.net>
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
	public class ParentControlDesignerTest : ParentControlDesigner
	{

		private DesignerTestHelper _helper;

		public ParentControlDesignerTest()
		{
			_helper = new DesignerTestHelper ();
		}

#if MS_NET
		protected override void Dispose (bool disposing)
		{
			// ms crashes here during tests, more than likely because they have many internal
			// interfaces, which I don't have in my design surface.
		}
#endif

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
			Panel panel = (Panel) _helper.CreateControl (typeof (TestPanel), null);

			Assert.IsFalse ((bool)_helper.GetValue (panel, "AllowDrop"), "#1");
#if MS_NET && NET_1_1
			Assert.IsTrue ((bool)_helper.GetValue (panel, "SnapToGrid"), "#2");
			Assert.IsTrue ((bool)_helper.GetValue (panel, "DrawGrid"), "#3");
			Assert.AreEqual (new Size (8, 8), (Size)_helper.GetValue (panel, "GridSize"), "#4");
#endif
			Assert.IsTrue (panel.AllowDrop, "#5");
		}

		protected override Point DefaultControlLocation {
			get { return new Point (3, 3); }
		}

#if MS_NET && NET_1_1
		[Test]
		public void GridPropertiesTest ()
		{
			Panel rootPanel = (Panel) _helper.CreateControl (typeof (TestPCDPanel),  null);
			_helper.SetValue (rootPanel, "SnapToGrid", true);
			_helper.SetValue (rootPanel, "DrawGrid", true);
			_helper.SetValue (rootPanel, "GridSize", new Size (10, 10));
	
			Panel childPanel = (Panel) _helper.CreateControl (typeof (TestPanel),  rootPanel);
			Assert.AreEqual (new Point (0, 0), (Point) _helper.GetValue (childPanel, "Location"), "#1");
			Assert.AreEqual (new Size (8, 8), (Size) _helper.GetValue (childPanel, "GridSize"), "#2");
			Assert.IsTrue ((bool) _helper.GetValue (childPanel, "SnapToGrid"), "#3");
			Assert.IsTrue ((bool) _helper.GetValue (childPanel, "DrawGrid"), "#4");

			// Test properties' values population among the children
			// 
			_helper.SetValue (rootPanel, "SnapToGrid", false);
			_helper.SetValue (rootPanel, "GridSize", new Size (5, 5));
			_helper.SetValue (rootPanel, "DrawGrid", false);

			Assert.AreEqual (new Size (5, 5), (Size) _helper.GetValue (childPanel, "GridSize"), "#5");
			Assert.IsFalse ((bool) _helper.GetValue (childPanel, "SnapToGrid"), "#6");
			Assert.IsFalse ((bool) _helper.GetValue (childPanel, "DrawGrid"), "#7");

			Panel childPanelTwo = (Panel) _helper.CreateControl (typeof (TestPanel),  rootPanel);
			Assert.AreEqual (new Point (3, 3), (Point) _helper.GetValue (childPanelTwo, "Location"), "#8");
			Assert.AreEqual (new Size (5, 5), (Size) _helper.GetValue (childPanelTwo, "GridSize"), "#9");
			Assert.IsFalse ((bool) _helper.GetValue (childPanelTwo, "SnapToGrid"), "#10");
			Assert.IsFalse ((bool) _helper.GetValue (childPanelTwo, "DrawGrid"), "#11");
		}
#endif

		[Test]
		public void CreateToolTest ()
		{
			// essentially a DesignerTestHelper.CreateControl test, because of the code I use there
			ParentControlDesigner rootPanel = (ParentControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel),  null));
			ParentControlDesigner childPanel = (ParentControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel), rootPanel.Control));
			Assert.IsNotNull (rootPanel, "#1");
			Assert.IsNotNull (childPanel,  "#2");
		}

		[Test]
		public void CanParentTest ()
		{
			ParentControlDesigner rootPanel = (ParentControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel),  null));
			ParentControlDesigner childPanel = (ParentControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestPanel), rootPanel.Control));
			ControlDesigner childPanelButton = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestButton), childPanel.Control));
			ControlDesigner rootPanelButton = (ControlDesigner) _helper.IDesignerHost.GetDesigner (_helper.CreateControl (typeof (TestButton), rootPanel.Control));

			Assert.IsFalse (childPanel.CanParent ((Control)_helper.IDesignerHost.RootComponent), "#1");
			Assert.IsTrue (childPanel.CanParent (childPanel), "#2");
			Assert.IsTrue (childPanel.CanParent (childPanelButton), "#3");
			Assert.IsTrue (childPanel.CanParent (rootPanelButton), "#4");
			Assert.IsFalse (childPanel.CanParent (rootPanel), "#5");
			Assert.IsTrue (rootPanel.CanParent (childPanel), "#6");
			Assert.IsTrue (rootPanel.CanParent (rootPanelButton), "#7");
		}
	}
}

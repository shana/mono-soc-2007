/* vi:set ts=4 sw=4: */
//
// DocumentDesignerTest
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
using System.Drawing.Design;
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

namespace Mono.Design.Test {

	[TestFixture]
	public class DocumentDesignerTest : DocumentDesigner {

		private DesignerTestHelper _helper;

		public DocumentDesignerTest()
		{
			_helper = new DesignerTestHelper ();
		}

		[SetUp]
		public void InitializeSurface ()
		{
			_helper.InitializeDesignSurface (typeof (TestPanel));
		}

		[TearDown]
		public void DisposeSurface ()
		{
			_helper.DisposeDesignSurface ();
		}

		[Test]
		public void InitializeTest ()
		{
			Panel panel = (Panel) ((IContainer)_helper.IDesignerHost).Components[0]; // the root control
			DocumentDesignerTest designer = _helper.IDesignerHost.GetDesigner (panel) as DocumentDesignerTest;

			SelectionRules rules = SelectionRules.BottomSizeable | SelectionRules.RightSizeable | SelectionRules.Visible;
			Assert.AreEqual (rules, designer.SelectionRules, "#1");
			Assert.AreEqual (new Point (0, 0), (Point) _helper.GetValue (panel, "Location"), "#2");
#if NET_2_0
			Assert.AreEqual (ViewTechnology.Default, ((IRootDesigner)designer).SupportedTechnologies[0], "#3");
			Assert.IsTrue (((IRootDesigner)designer).GetView (ViewTechnology.Default) is Control, "#4");
#else
			Assert.AreEqual (ViewTechnology.WindowsForms, ((IRootDesigner)designer).SupportedTechnologies[0], "#3");
			Assert.IsTrue (((IRootDesigner)designer).GetView (ViewTechnology.WindowsForms) is Control, "#4");
#endif
		}

		[Test]
		public void LocationTest ()
		{
			Panel panel = (Panel) ((IContainer)_helper.IDesignerHost).Components[0]; // the root control

			_helper.SetValue (panel, "Location", new Point (100, 100));
			Assert.AreEqual (new Point (100, 100), (Point) _helper.GetValue (panel, "Location"), "#1");
			Assert.AreEqual (new Point (15, 15), panel.Location, "#2");
		}

		[Test]
		public void GetToolSupportedTest ()
		{
			Panel panel = (Panel) ((IContainer)_helper.IDesignerHost).Components[0]; // the root control
			DocumentDesignerTest designer = _helper.IDesignerHost.GetDesigner (panel) as DocumentDesignerTest;

			Assert.IsTrue (designer.GetToolSupported (new ToolboxItem (typeof (Form))), "#1");
		}
	}
}

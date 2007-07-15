/* vi:set ts=4 sw=4: */
//
// ComponentDesignerTest
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
using System.Threading;
using System.Windows.Forms;
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

	[Designer("Mono.Design.Test.ComponentDesignerTest, Mono.Design.Test", typeof(IDesigner))]
	public class TreeDesignerTestComponent : Component
	{
	}

	[TestFixture]
	public class ComponentDesignerTest : ComponentDesigner
	{

#if NET_2_0
		private DesignerTestHelper _helper;
#endif

		public ComponentDesignerTest()
		{
#if NET_2_0
			_helper = new DesignerTestHelper ();
#endif
		}

#if MS_NET
		protected override void Dispose (bool disposing)
		{
			// ms crashes here during tests, more than likely because they have many internal
			// interfaces, which I don't have in my design surface.
		}
#endif

		[Test]
		public void ShadowPropertiesTest()
		{
			Button button = new Button ();
			this.Initialize (button);

			try {
				object o = this.ShadowProperties["Visible"];
				if(o != null) o = null; // just a workaround not to get a not-used-variable warning
				Assert.Fail ("#1");
			} catch {}

			button.Visible = false;
			this.ShadowProperties["Visible"] = true;
			Assert.IsFalse (button.Visible, "#2");
			Assert.IsTrue ((bool)ShadowProperties["Visible"], "#3");
			this.ShadowProperties["Visible"] = false;
			Assert.IsFalse ((bool)ShadowProperties["Visible"], "#4");
		}

		[Test]
		public void InitializeTest ()
		{
			Button button = new Button ();
			Component c = new Component ();
			this.Initialize (c);
			this.Initialize (button);
			Assert.AreEqual (button, this.Component, "#1");
		}

		[Test]
		public void IDesignerFilterTest ()
		{
			Button button = new Button ();
			this.Initialize (button);
			Assert.IsNotNull (TypeDescriptor.GetProperties (typeof (ComponentDesignerTest))["NewFilteredProperty"], "#1");
		}

		protected override void PreFilterProperties (IDictionary properties) 
		{
			properties["NewFilteredProperty"] = TypeDescriptor.CreateProperty (typeof (ComponentDesignerTest), "NewFilteredProperty", typeof (bool), new Attribute[0] {});
		}

		private bool _newProperty = false;

		public bool NewFilteredProperty {
			get { return _newProperty; }
			set { _newProperty = value; }
		}

#if NET_2_0

		[Test]
		public void ITreeDesignerTest ()
		{
			_helper.InitializeDesignSurface ();
			IDesignerHost host = _helper.IDesignerHost;
			IComponent root = host.RootComponent;
			IDesigner rootDesigner = host.GetDesigner (root);
			IComponent child = null;
			child = host.CreateComponent (typeof (TreeDesignerTestComponent)) as IComponent;
			ComponentDesignerTest childDesigner = host.GetDesigner (child) as ComponentDesignerTest;

			Assert.AreEqual (root, childDesigner.ParentComponent, "#1");
			Assert.AreEqual (rootDesigner, ((ITreeDesigner)childDesigner).Parent, "#2");
			Assert.IsTrue (childDesigner.AssociatedComponents.Count == 0, "#3");
			Assert.IsTrue (((ITreeDesigner)childDesigner).Children.Count == 0, "#4");
			_helper.DisposeDesignSurface ();
		}
#endif
	}
}

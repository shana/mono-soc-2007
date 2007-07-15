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

#if NET_2_0

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

namespace Mono.Design.Test 
{

	[TestFixture]
	public class DesignSurfaceTest : DesignSurface 
	{

		private DesignerTestHelper _helper;

		public DesignSurfaceTest()
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
		public void ServicesTest ()
		{
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IExtenderProviderService)), "#1");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IExtenderListService)), "#2");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (ISelectionService)), "#3");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (DesignSurface)), "#4");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IComponentChangeService)), "#5");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IDesignerHost)), "#6");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IContainer)), "#7");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IServiceContainer)), "#8");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (ITypeDescriptorFilterService)), "#9");
		}

		[Test]
		public void ServiceContainerTest ()
		{
			// test non-replaceable service container.
			IServiceContainer servContainer = _helper.DesignSurface.GetService (typeof (IServiceContainer)) as IServiceContainer;
			try{
				servContainer.RemoveService (typeof (IDesignerHost));
				servContainer.RemoveService (typeof (IComponentChangeService));
				servContainer.RemoveService (typeof (IContainer));
				servContainer.RemoveService (typeof (IServiceContainer));
				Assert.Fail ("#1");
			} catch {
			}

			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IDesignerHost)), "#2");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IComponentChangeService)), "#3");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IContainer)), "#4");
			Assert.IsNotNull (_helper.DesignSurface.GetService (typeof (IServiceContainer)), "#5");
		}

		[Test]
		public void SiteServicesTest ()
		{
			Control control = (Control) _helper.CreateControl (typeof (TestButton),  null);
			Assert.IsNotNull (control.Site.GetService (typeof (IDictionaryService)), "#1");
			Assert.IsNotNull (control.Site.GetService (typeof (INestedContainer)), "#2");
			Assert.IsNotNull (control.Site.GetService (typeof (IServiceContainer)), "#3");

			// The following two tests show that the DT Site:
			// 	* doesn't offer its implementation of IServiceContainer as a site-specific service
			//  * offers the added site-specific services via IServiceProvider
			//
			((IServiceContainer) control.Site).AddService (typeof (DesignSurfaceTest), new DesignSurfaceTest ());
			Assert.IsNotNull (control.Site.GetService (typeof (DesignSurfaceTest)), "#4");
			Assert.AreEqual (control.Site.GetService (typeof (IServiceContainer)),
								_helper.DesignSurface.GetService (typeof (IServiceContainer)), "#5");

			IDictionaryService dictionary = control.Site.GetService (typeof (IDictionaryService)) as IDictionaryService;
			dictionary.SetValue ("val", 15);
			Assert.AreEqual (15, (int) dictionary.GetValue ("val"), "#6");
			Assert.AreEqual ("val", (string) dictionary.GetKey (15), "#7");

			dictionary.SetValue ("val", 16);
			Assert.AreEqual (16, (int) dictionary.GetValue ("val"), "#6");
		}

		[Test]
		public void CreateComponentTest ()
		{
			Control control = (Control) _helper.CreateControl (typeof (TestButton),  null);
			Assert.IsNotNull (control, "#1");
			Assert.IsNotNull (_helper.IDesignerHost.GetDesigner (control), "#2");
		}

		[Test]
		public void DestroyComponentTest ()
		{
			Control control = (Control) _helper.CreateControl (typeof (TestButton),  null);
			_helper.IDesignerHost.DestroyComponent (control);
			Assert.IsNull (_helper.IDesignerHost.GetDesigner (control), "#2");
		}

		// TypeDescriptor queres the component's site for ITypeDescriptorFilterService 
		// then invokes ITypeDescriptorFilterService.* before retrieveing props/event/attributes, 
		// which then invokes the IDesignerFilter implementation of the component, which adds
		// and modifies props/events/attr.
		// 
		[Test]
		public void TypeDescriptorFilterTest ()
		{
			Control control = (Control) _helper.CreateControl (typeof (TestButton),  null);
			try {
				_helper.GetValue (control, "Locked");
			} catch {
				Assert.Fail ("#1"); // "Locked" DT property has not been added
			}
		}

		[Test]
		public void ContainerTest ()
		{
			TestButton control = new TestButton ();
			_helper.IDesignerHost.Container.Add (control, "SomeName");
			Assert.AreEqual ("SomeName", control.Site.Name, "#1");
			Assert.IsNotNull (_helper.IDesignerHost.GetDesigner (control), "#2");
			Assert.AreEqual (control, _helper.IDesignerHost.Container.Components["SomeName"], "#3");

			_helper.IDesignerHost.Container.Remove (control);
			Assert.IsNull (_helper.IDesignerHost.GetDesigner (control), "#4");
			Assert.IsNull (_helper.IDesignerHost.Container.Components["SomeName"], "#5");
		}

		[Test]
		public void NestedContainerTest ()
		{
			Control control = (Control) _helper.CreateControl (typeof (TestButton),  null);
			Control nestedControl = new TestButton ();
			
			// The following two tests show that the DT Site:
			//  * offers the added site-specific services via IServiceProvider
			// 	* doesn't offer its implementation of IServiceContainer as a site-specific service
			//  * GetService is routed through the owner
			// 
			INestedContainer nestedContainer = control.Site.GetService (typeof (INestedContainer)) as INestedContainer;
			nestedContainer.Add (nestedControl);

			((IServiceContainer) nestedControl.Site).AddService (typeof (DesignSurfaceTest), new DesignSurfaceTest ());
			Assert.IsNotNull (nestedControl.Site.GetService (typeof (DesignSurfaceTest)), "#1");
			Assert.AreEqual (nestedControl.Site.GetService (typeof (IServiceContainer)), 
							 _helper.DesignSurface.GetService (typeof (IServiceContainer)), "#2");
			Assert.IsNotNull (nestedControl.Site.GetService (typeof (DesignSurface)), "#3");
		}

		[Test]
		public void SelectionServiceTest ()
		{
			ISelectionService selection = _helper.DesignSurface.GetService (typeof (ISelectionService)) as ISelectionService;
			Assert.AreEqual (_helper.IDesignerHost.RootComponent, selection.PrimarySelection, "#1");
		}
	}
}
#endif

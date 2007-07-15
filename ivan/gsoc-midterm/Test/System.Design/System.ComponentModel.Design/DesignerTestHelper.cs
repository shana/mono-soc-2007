/* vi:set ts=4 sw=4: */
//
// DesignerTest
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing.Design;

#if !MS_NET
using Mono.Design;
#endif

namespace Mono.Design.Test
{

	

#if !MS_NET
	//FIXME: get rid of this bit once FormDocumentDesigner is implemented
	[Designer("Mono.Design.DocumentDesigner, Mono.Design", typeof(IRootDesigner))]
#endif
	public class TestForm : Form
	{
	}

	[Designer ("Mono.Design.Test.ControlDesignerTest, Mono.Design.Test", typeof (IDesigner))]
	public class TestButton : Button
	{
	}

	public class TestParentControlDesigner : ParentControlDesigner
	{
	}

	[Designer ("Mono.Design.Test.DocumentDesignerTest, Mono.Design.Test", typeof (IRootDesigner))]
	[Designer ("Mono.Design.Test.TestParentControlDesigner, Mono.Design.Test", typeof (IDesigner))]
	public class TestPanel : Panel
	{
	}

	[Designer ("Mono.Design.Test.ParentControlDesignerTest, Mono.Design.Test", typeof (IDesigner))]
	public class TestPCDPanel : Panel
	{
	}

	internal class DesignerTestHelper
	{

		public DesignerTestHelper ()
		{
		}

		public object GetValue (object component, string propertyName)
		{
		   return this.GetValue (component, propertyName, null);
		}
		
		public object GetValue (object component, string propertyName, Type propertyType)
		{
			PropertyDescriptor prop = TypeDescriptor.GetProperties (component)[propertyName] as PropertyDescriptor;
			if (prop == null)
				throw new InvalidOperationException ("Property \"" + propertyName + "\" is missing on " + 
													 component.GetType().AssemblyQualifiedName);
			if (propertyType != null && !propertyType.IsAssignableFrom (prop.PropertyType))
					throw new InvalidOperationException ("Types do not match: " + prop.PropertyType.AssemblyQualifiedName +
														 " : " + propertyType.AssemblyQualifiedName);
			return prop.GetValue (component);
		}

		public void SetValue (object component, string propertyName, object value)
		{
			PropertyDescriptor prop = TypeDescriptor.GetProperties (component)[propertyName] as PropertyDescriptor;

			if (prop == null)
					throw new InvalidOperationException ("Property \"" + propertyName + "\" is missing on " + 
														 component.GetType().AssemblyQualifiedName);
			if (!prop.PropertyType.IsAssignableFrom (value.GetType ()))
					throw new InvalidOperationException ("Types do not match: " + value.GetType ().AssemblyQualifiedName +
														 " : " + prop.PropertyType.AssemblyQualifiedName);
			if (!prop.IsReadOnly)
				prop.SetValue (component, value);
		}

		private Control _parentedControl;

		// parent null denodes parenting to the root component
		//
		public Control CreateControl (Type controlType, Control parent)
		{
			if (_surface == null)
				throw new Exception ("Helper designer surface not initialized.");

			if (parent == null)
				parent = IDesignerHost.RootComponent as Control;

			ParentControlDesigner parentDesigner = IDesignerHost.GetDesigner (parent) as ParentControlDesigner;
			if (parentDesigner == null)
				throw new Exception ("parent is not handled by a ParentControlDesigner.");

			ToolboxItem tbItem = new ToolboxItem (controlType);
			tbItem.ComponentsCreated += new ToolboxComponentsCreatedEventHandler (OnToolboxComponentCreated);
			tbItem.CreateComponents (this.IDesignerHost);
			parent.Controls.Add (_parentedControl);
			tbItem.ComponentsCreated -= new ToolboxComponentsCreatedEventHandler (OnToolboxComponentCreated);
			return _parentedControl;
		}

		private void OnToolboxComponentCreated (object sender, ToolboxComponentsCreatedEventArgs args)
		{
			_parentedControl = args.Components[0] as Control;
		}

#if NET_2_0
		private DesignSurface _surface;
#else
		private Mono.Design.Test.DesignerHost _surface;
#endif
		private TestForm _mainView;

		public void InitializeDesignSurface ()
		{
			InitializeDesignSurface (typeof (TestForm));
		}

		public void InitializeDesignSurface (Type rootControlType)
		{
			if (_surface == null) {
				_mainView = new TestForm ();
#if NET_2_0
				_surface = new DesignSurface ();
				_surface.BeginLoad (rootControlType);
				_mainView.Controls.Add ((Control)_surface.View);
#else
				_surface = new Mono.Design.Test.DesignerHost ();
				Control rootControl = (Control) Activator.CreateInstance (rootControlType, BindingFlags.CreateInstance | BindingFlags.Public
																| BindingFlags.Instance, null,  null, null);
				_surface.Add (rootControl, null);
				_mainView.Controls.Add (rootControl);
#endif
				_mainView.Controls[0].Visible = true;
				_mainView.Show ();
			}
		}

		public void DisposeDesignSurface ()
		{
			if (_surface != null) {
				_mainView.Close ();
				_surface = null;
			}
		}

#if NET_2_0
		public IDesignerHost IDesignerHost {
			get { return _surface.GetService (typeof (IDesignerHost)) as IDesignerHost; }
		}

		public DesignSurface DesignSurface { 
			get { return _surface; }
		}
#else
		public IDesignerHost IDesignerHost {
			get { return (IDesignerHost)_surface; }
		}
#endif
	}
}

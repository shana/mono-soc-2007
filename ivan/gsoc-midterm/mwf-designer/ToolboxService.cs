/* vi:set ts=4 sw=4: */
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
using System.Windows.Forms;
using System.Drawing.Design;

namespace Designer
{
	// just a basic implementation for testing
	//
	internal class ToolboxService : ListBox, IToolboxService
	{

		public ToolboxService ()
		{
		}

		public System.Drawing.Design.CategoryNameCollection CategoryNames {
			get {
				return null;
			}
		}

		public string SelectedCategory {
			get {
				return null;
			}
			set {
			}
		}

		public void AddCreator (System.Drawing.Design.ToolboxItemCreatorCallback creator, string format, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void AddCreator (System.Drawing.Design.ToolboxItemCreatorCallback creator, string format)
		{
		}

		public void AddLinkedToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, string category, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void AddLinkedToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void AddToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, string category)
		{
			this.AddToolboxItem (toolboxItem);
		}

		public void AddToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			this.Items.Add (toolboxItem);
		}

		public System.Drawing.Design.ToolboxItem DeserializeToolboxItem (object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return null;
		}

		public System.Drawing.Design.ToolboxItem DeserializeToolboxItem (object serializedObject)
		{
			return null;
		}

		public System.Drawing.Design.ToolboxItem GetSelectedToolboxItem (System.ComponentModel.Design.IDesignerHost host)
		{
			return this.GetSelectedToolboxItem ();
		}

		public System.Drawing.Design.ToolboxItem GetSelectedToolboxItem ()
		{
			if (base.SelectedIndex == -1)
				return null;
			else
				return (ToolboxItem)base.SelectedItem;
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems (string category, System.ComponentModel.Design.IDesignerHost host)
		{
			return this.GetToolboxItems ();
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems (string category)
		{
			return this.GetToolboxItems ();
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems (System.ComponentModel.Design.IDesignerHost host)
		{
			return this.GetToolboxItems ();
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems ()
		{
			ToolboxItem[] items = new ToolboxItem[base.Items.Count];
			base.Items.CopyTo (items, 0);
			return new ToolboxItemCollection (items);
		}

		public bool IsSupported (object serializedObject, System.Collections.ICollection filterAttributes)
		{
			return false;
		}

		public bool IsSupported (object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return false;
		}

		public bool IsToolboxItem (object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return false;
		}

		public bool IsToolboxItem (object serializedObject)
		{
			return false;
		}

		public void RemoveCreator (string format, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void RemoveCreator (string format)
		{
		}

		public void RemoveToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, string category)
		{
			this.RemoveToolboxItem (toolboxItem);
		}

		public void RemoveToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			base.Items.Remove (toolboxItem);
		}

		public void SelectedToolboxItemUsed ()
		{
			// XXX: Causes MWF to go booom badly :-)
			//base.SelectedIndex = -1;
		}

		public object SerializeToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			return null;
		}

		public bool SetCursor ()
		{
			return false;
		}

		public void SetSelectedToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			base.SelectedItem = toolboxItem;
		}

		private bool ShouldSerializeItems ()
		{
			return false;
		}
	}
}

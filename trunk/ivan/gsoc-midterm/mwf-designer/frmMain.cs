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
using System.Drawing;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

using Mono.Design;

namespace Designer
{
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PropertyGrid _propertyGrid;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel _panelDesignerView;
		private System.Windows.Forms.Splitter splitter2;
		private ToolboxService _toolbox;
		private System.Windows.Forms.Label lblSelectedComponent;
		private IContainer components;

		public frmMain ()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent ();

			InitializeSurface ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose ();
				}
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.components = new System.ComponentModel.Container ();
			this._propertyGrid = new System.Windows.Forms.PropertyGrid ();
			this.splitter1 = new System.Windows.Forms.Splitter ();
			this.panel1 = new System.Windows.Forms.Panel ();
			this.splitter2 = new System.Windows.Forms.Splitter ();
			this.lblSelectedComponent = new System.Windows.Forms.Label ();
			this._panelDesignerView = new System.Windows.Forms.Panel ();
			this._toolbox = new ToolboxService ();
			this.panel1.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// _propertyGrid
			// 
			this._propertyGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this._propertyGrid.Location = new System.Drawing.Point (0, 142);
			this._propertyGrid.Name = "_propertyGrid";
			this._propertyGrid.Size = new System.Drawing.Size (224, 312);
			this._propertyGrid.TabIndex = 0;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point (588, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size (4, 478);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add (this._toolbox);
			this.panel1.Controls.Add (this.splitter2);
			this.panel1.Controls.Add (this._propertyGrid);
			this.panel1.Controls.Add (this.lblSelectedComponent);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point (592, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size (224, 478);
			this.panel1.TabIndex = 2;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point (0, 138);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size (224, 4);
			this.splitter2.TabIndex = 1;
			this.splitter2.TabStop = false;
			// 
			// lblSelectedComponent
			// 
			this.lblSelectedComponent.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblSelectedComponent.Location = new System.Drawing.Point (0, 454);
			this.lblSelectedComponent.Name = "lblSelectedComponent";
			this.lblSelectedComponent.Size = new System.Drawing.Size (224, 24);
			this.lblSelectedComponent.TabIndex = 3;
			this.lblSelectedComponent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _panelDesignerView
			// 
			this._panelDesignerView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._panelDesignerView.Location = new System.Drawing.Point (0, 0);
			this._panelDesignerView.Name = "_panelDesignerView";
			this._panelDesignerView.Size = new System.Drawing.Size (588, 478);
			this._panelDesignerView.TabIndex = 3;
			// 
			// _toolbox
			// 
			this._toolbox.BackColor = System.Drawing.SystemColors.Control;
			this._toolbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._toolbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._toolbox.Font = new System.Drawing.Font ("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._toolbox.IntegralHeight = false;
			this._toolbox.ItemHeight = 16;
			this._toolbox.Location = new System.Drawing.Point (0, 0);
			this._toolbox.Name = "_toolbox";
			this._toolbox.SelectedCategory = null;
			this._toolbox.Size = new System.Drawing.Size (224, 138);
			this._toolbox.Sorted = true;
			this._toolbox.TabIndex = 2;
			this._toolbox.DoubleClick += new EventHandler (this.Toolbox_MouseDoubleClick);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size (5, 14);
			this.ClientSize = new System.Drawing.Size (816, 478);
			this.Controls.Add (this._panelDesignerView);
			this.Controls.Add (this.splitter1);
			this.Controls.Add (this.panel1);
			this.Font = new System.Drawing.Font ("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "frmMain";
			this.Text = "THIS IS JUST FOR TESTING PURPOSES! - Middle-click for dragging a container control";
			this.panel1.ResumeLayout (false);
			this.ResumeLayout (false);

		}
		#endregion

		[STAThread]
		static void Main ()
		{
			Application.Run (new frmMain ());
		}

		private IServiceContainer serviceContainer = null;

		private void InitializeSurface ()
		{
			DesignSurface surface = new DesignSurfaceManager ().CreateDesignSurface ();

			surface.BeginLoad (typeof (MyForm));
			this._panelDesignerView.Controls.Add ((Control)surface.View);
			((Control)surface.View).Dock = DockStyle.Fill;
			this.serviceContainer = surface.GetService (typeof (IServiceContainer)) as IServiceContainer;
			serviceContainer.AddService (typeof (IToolboxService), _toolbox);

			ISelectionService s = (ISelectionService)serviceContainer.GetService (typeof (ISelectionService));
			s.SelectionChanged += new EventHandler (OnSelectionChanged);

			PopulateToolbox (_toolbox);
		}

		private void PopulateToolbox (IToolboxService toolbox)
		{
			toolbox.AddToolboxItem (new ToolboxItem (typeof (MyButton)));
			toolbox.AddToolboxItem (new ToolboxItem (typeof (MyPanel)));
		}

		private void OnSelectionChanged (object sender, System.EventArgs e)
		{
			ISelectionService selectionServ = serviceContainer.GetService (typeof (ISelectionService)) as ISelectionService;

			object[] selection;
			if (selectionServ.SelectionCount == 0)
				_propertyGrid.SelectedObject = null;
			else {
				selection = new object[selectionServ.SelectionCount];
				selectionServ.GetSelectedComponents ().CopyTo (selection, 0);
				_propertyGrid.SelectedObjects = selection;
			}

			if (selectionServ.PrimarySelection == null)
				lblSelectedComponent.Text = "";
			else {
				IComponent component = (IComponent)selectionServ.PrimarySelection;
				lblSelectedComponent.Text = component.Site.Name + " (" + component.GetType ().Name + ")";
			}
		}


		private void Toolbox_MouseDoubleClick (object sender, EventArgs e)
		{
			IDesignerHost host = this.serviceContainer.GetService (typeof (IDesignerHost)) as IDesignerHost;
			IToolboxService tb = this.serviceContainer.GetService (typeof (IToolboxService)) as IToolboxService;
			((IToolboxUser)(DocumentDesigner)host.GetDesigner (host.RootComponent)).ToolPicked (tb.GetSelectedToolboxItem ());
		}

	}
}

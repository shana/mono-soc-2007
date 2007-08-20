//
// EditorProcess.cs:
//
// Authors:
//   Ivan N. Zlatev <contact@i-nz.net>
//
// Copyright (C) 2007 Ivan N. Zlatev
//
//
// This source code is licenced under The MIT License:
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
using Gtk;
using Gdk;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;

using MonoDevelop.Core.Execution;
using MonoDevelop.Core;
using MonoDevelop.DesignerSupport.Toolbox;

using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Mono.Design;

namespace WinFormsAddin
{
	public class EditorProcess : MonoDevelop.DesignerSupport.RemoteDesignerProcess
	{
		private Widget _gtkContainer;
		private Control _mwfContainer;
		private string _designerFile;
		private Thread _editorProcess;

		public EditorProcess ()
		{
		}
		
		public void Initialize (string designerFile)
		{
			if (designerFile == null)
				throw new ArgumentNullException ("designerFile");
			_designerFile = designerFile;

			_editorProcess = new Thread (InitializeRunner);
			_editorProcess.Start ();
		}

		private void InitializeRunner ()
		{
			StartGuiThread ();
			RunMWFThread ();
            while (_mwfContainer == null) {}
			while (!_mwfContainer.IsHandleCreated) { } // wait for the mwf handle


			// Hopefully by the time LoadGui is done the MWF Application.Run
			// will be done with whatever it's doing, because else strange things
			// might happen.
			Gtk.Application.Invoke (delegate { 
				InitializeGTK ();
			});
            while (_gtkContainer == null) {}
			while (_gtkContainer.Handle == IntPtr.Zero) { } // wait for the gtk handle

			bool parented = false;
			Gtk.Application.Invoke ( delegate { 
				Gdk.Window window = Gdk.Window.ForeignNew ((uint) _mwfContainer.Handle);
				window.Reparent (_gtkContainer.GdkWindow, 0, 0);
				parented = true;
			});
			while (!parented) { }

			EventHandler loadSurfaceDelegate = delegate {
				DesignSurface surface = new DesignSurface ();
				((IServiceContainer)surface.GetService (typeof (IServiceContainer))).AddService (typeof (ITypeResolutionService),
																								 new TypeResolutionService ());
				surface.BeginLoad (new MDDesignerLoader (_designerFile));
				if (surface.IsLoaded) {
					_mwfContainer.Controls.Add ((Control)surface.View);
					_mwfContainer.Refresh ();
				}
			};
			_mwfContainer.Invoke (loadSurfaceDelegate);

			_gtkContainer.SizeAllocated += delegate (object o, SizeAllocatedArgs args) {
				EventHandler resizeDelegate = delegate {
					_mwfContainer.Width = args.Allocation.Width;
					_mwfContainer.Height = args.Allocation.Height;
				};
				_mwfContainer.Invoke (resizeDelegate);
			};	

			EventHandler resizeNow = delegate {
				_mwfContainer.Width = _gtkContainer.Allocation.Width;
				_mwfContainer.Height = _gtkContainer.Allocation.Height;
			};
			_mwfContainer.Invoke (resizeNow);
		}

		private void RunMWFThread ()
		{
			Thread t = new Thread (InitializeMWF); 
			t.Start ();
			while (!t.IsAlive) {};
		}
		
		private void InitializeMWF ()
		{
			Control mwfContainer = new Panel ();
			mwfContainer.BackColor = System.Drawing.Color.Yellow;
			mwfContainer.CreateControl ();
			_mwfContainer = mwfContainer;
			System.Windows.Forms.Application.Run ();
		}
		
		void InitializeGTK ()
		{
			Widget viewFrame = new Gtk.Frame ();
			base.DesignerWidget = viewFrame;
			base.PropertyGridWidget = new Gtk.Label ("PropertyGrid");
			_gtkContainer = viewFrame;
		}

		private bool _disposed = false;
		public override void Dispose ()
		{		
			if (!_disposed) {
				EventHandler dispose = delegate { 
					System.Windows.Forms.Application.Exit ();
				};
				_mwfContainer.Invoke (dispose);
				base.Dispose ();
				_disposed = true;
				_editorProcess.Join ();
			}
		}

		protected override void HandleError (Exception e)
		{
//          //remove the grid in case it was the source of the exception, as GTK# expose exceptions can fire repeatedly
//          //also user should not be able to edit things when showing exceptions
//          if (propertyGrid != null) {
//              Gtk.Container parent = propertyGrid.Parent as Gtk.Container;
//              if (parent != null)
//                  parent.Remove (propertyGrid);
//
//              propertyGrid.Destroy ();
//              propertyGrid = null;
//          }
//
//          //show the error message
			base.HandleError (e);
		}
	}
}

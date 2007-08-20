//
// DesignSurfaceDisplayBinding.cs
//
// Authors:
// 	 Ivan N. Zlatev <contact@i-nz.net>
// 	 
// Based on AspNetEditViewContent by:
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
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
using System.ComponentModel;
using Gtk;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.DesignerSupport.Toolbox;
using MonoDevelop.DesignerSupport;
using MonoDevelop.DesignerSupport.PropertyGrid;

using Mono.Design;
using System.ComponentModel.Design.Serialization;

namespace WinFormsAddin
{

	internal class DesignSurfaceViewFrame: Frame, ICustomPropertyPadProvider
	{

		private Widget _propertyGrid;

		public DesignSurfaceViewFrame (Widget propertyGrid)
		{
			_propertyGrid = propertyGrid;

			this.CanFocus = true;
			this.Shadow = ShadowType.None;
			this.BorderWidth = 0;
		}

		Gtk.Widget ICustomPropertyPadProvider.GetCustomPropertyWidget ()
		{
			return _propertyGrid;
		}

		void ICustomPropertyPadProvider.DisposeCustomPropertyWidget ()
		{
			_propertyGrid.Dispose ();
		}
	}

	public class DesignSurfaceViewContent : AbstractSecondaryViewContent //, IToolboxConsumer
	{
		private string _designerFile;
		private DesignSurfaceViewFrame _surfaceFrame;
		Gtk.Frame _propertyGridFrame;
		private EditorProcess _editorProcess;
		Gtk.Socket _designerSocket;
		Gtk.Socket _propGridSocket;
		private bool _loaded;

		internal DesignSurfaceViewContent (string designerFile)
		{
			_loaded = false;
			_designerFile = designerFile;
			_propertyGridFrame = new Gtk.Frame ();
			_propertyGridFrame.Show ();
			_surfaceFrame = new DesignSurfaceViewFrame (_propertyGridFrame);
			_surfaceFrame.Show ();
		}

		public override Gtk.Widget Control {
			get {return _surfaceFrame; }
		}
		
		public override string TabPageLabel {
			get { return GettextCatalog.GetString ("Design"); }
		}

		public override void Selected ()
		{
			if (!_loaded) {
				InitializeView ();
				_loaded = true;
			}
		}

		public override void Deselected ()
		{
			if (_loaded) {
				_editorProcess.Dispose ();
				this.DisposeView ();
				_loaded = false;
			}
		}

		private void InitializeView ()
		{
			_designerSocket = new Gtk.Socket ();
			_designerSocket.Show ();
			_surfaceFrame.Add (_designerSocket);
			_surfaceFrame.ShowAll ();
			
			_propGridSocket = new Gtk.Socket ();
			_propGridSocket.Show ();
			_propertyGridFrame.Add (_propGridSocket);
			
			_editorProcess = (EditorProcess) Runtime.ProcessService.CreateExternalProcessObject (typeof (EditorProcess), false);

			if (_designerSocket.IsRealized)
				_editorProcess.AttachDesigner (_designerSocket.Id);
			if (_propGridSocket.IsRealized)
				_editorProcess.AttachPropertyGrid (_propGridSocket.Id);
			_designerSocket.Realized += delegate { _editorProcess.AttachDesigner (_designerSocket.Id); };
			_propGridSocket.Realized += delegate { _editorProcess.AttachPropertyGrid (_propGridSocket.Id); };

			_editorProcess.Initialize (_designerFile);
		}

		private void DisposeView()
		{			
			if (_loaded) {
				if (_editorProcess != null) {
					_editorProcess.Dispose ();
					_editorProcess = null;
				}

				if (_propGridSocket != null) {
					_propGridSocket.Dispose ();
					_propGridSocket = null;
				}
				
				if (_designerSocket != null) {
					_surfaceFrame.Remove (_designerSocket);
					_designerSocket.Dispose ();
					_designerSocket = null;
				}
				_loaded = false;
			}
		}
	}
}

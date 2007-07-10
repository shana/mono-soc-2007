//
// Authors:
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using Gtk;
using System;
using Mono.Data.Sql;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.GlueGenerator
{
	public partial class GenerateGlueDialog : Gtk.Dialog
	{
		private ConnectionComboBox comboConnection;
		private ProjectDirectoryComboBox comboObjectLayerLocation;
		private ProjectDirectoryComboBox comboDbLayerLocation;
		
		private int currentStep = 0;
		
		public GenerateGlueDialog()
		{
			this.Build();
			
			//step 1
			comboConnection = new ConnectionComboBox ();
			comboObjectLayerLocation = new ProjectDirectoryComboBox ();
			comboDbLayerLocation = new ProjectDirectoryComboBox ();
			
			comboConnection.Changed += new EventHandler (DefaultChangedHandler);
			comboObjectLayerLocation.Changed += new EventHandler (ObjectLayerLocationChanged);
			comboDbLayerLocation.Changed += new EventHandler (DatabaseLayerLocationChanged);
			entryObjectLayerNamespace.Changed += new EventHandler (DefaultChangedHandler);
			entryDbLayerNamespace.Changed += new EventHandler (DefaultChangedHandler);
			
			tableStep1.Attach (comboConnection, 1, 2, 0, 1, AttachOptions.Expand, AttachOptions.Fill, 0, 0);
			tableStep1.Attach (comboObjectLayerLocation, 1, 2, 1, 2, AttachOptions.Expand, AttachOptions.Fill, 0, 0);
			tableStep1.Attach (comboDbLayerLocation, 1, 2, 3, 4, AttachOptions.Expand, AttachOptions.Fill, 0, 0);
			
			//step 2
			
			//step 3
			
			//step 4
			
			//step 5
		}

		protected virtual void CancelClicked (object sender, EventArgs e)
		{
			Respond (ResponseType.Cancel);
			Destroy ();
		}

		protected virtual void BackClicked (object sender, EventArgs e)
		{
			if (currentStep > 0) currentStep--;
			notebook.Page = currentStep;
		}

		protected virtual void ForwardClicked (object sender, EventArgs e)
		{
			if (currentStep < 4) {
				currentStep++;
				notebook.Page = currentStep;
			} else {
				//TODO: check if everything is filled in correctly
				
				Respond (ResponseType.Ok);
				Destroy ();
			}
		}
		
		private void ObjectLayerLocationChanged (object sender, EventArgs e)
		{
			if (entryObjectLayerNamespace.Text.Length <= 0) {
				entryObjectLayerNamespace.Text = GetNamespaceName (
					comboObjectLayerLocation.SelectedProject,
					comboObjectLayerLocation.SelectedDirectory
				);
			}
			
			CheckCurrentStep ();
		}
		
		private void DatabaseLayerLocationChanged (object sender, EventArgs e)
		{
			if (entryDbLayerNamespace.Text.Length <= 0) {
				entryDbLayerNamespace.Text = GetNamespaceName (
					comboDbLayerLocation.SelectedProject,
					comboDbLayerLocation.SelectedDirectory
				);
			}
			
			CheckCurrentStep ();
		}
		
		private void DefaultChangedHandler (object sender, EventArgs e)
		{
			CheckCurrentStep ();
		}

		private string GetNamespaceName (Project proj, string directory)
		{
			if (directory != null)
				return String.Concat (proj.DefaultNamespace, ".", System.IO.Path.GetDirectoryName (directory));
			return proj.DefaultNamespace;
		}
		
		private void CheckCurrentStep ()
		{
			bool val = ValidateCurrentStep ();
			buttonNext.Sensitive = val;
		}
		
		private bool ValidateCurrentStep ()
		{
			switch (currentStep) {
				case 0:
					if (comboConnection.Active < 0) return false;
					if (comboDbLayerLocation.Active < 0) return false;
					if (comboObjectLayerLocation.Active < 0) return false;
					if (entryDbLayerNamespace.Text.Length <= 0) return false;
					if (entryObjectLayerNamespace.Text.Length <= 0) return false;
					
					return true;
				case 1:
					return false;
				case 2:
					return false;
				case 3:
					return false;
				case 4:
					return false;
				default:
					return false;
			}
		}
	}
}

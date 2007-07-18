//
// Measures.Ui.MainWindow class
//
// Authors:
//	Néstor Salceda <nestor.salceda@gmail.com>
//
// 	(C) 2007 Néstor Salceda
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
using System.Collections;
using Gtk;
using Glade;
using Mono.Cecil;

namespace Measures.Ui {
	
	public class MainWindow {
		Glade.XML xmlMainWindow;
		[Widget] Window mainWindow;
		[Widget] TreeView measuresTreeView;
		[Widget] Expander expander1;
		TreeStore measuresTreeStore;
		IEnumerable measures;

		public MainWindow () 
		{
			xmlMainWindow = new Glade.XML (null,"measures.glade", "mainWindow", null);
			xmlMainWindow.Autoconnect (this);
			measuresTreeView.AppendColumn ("Unit", new CellRendererText (), "text", 0);
			measuresTreeView.Selection.Changed += new EventHandler (OnTreeSelectionChanged);
			mainWindow.ShowAll ();
		}
		
		//Disabling warning 0169 because this code will be called at 
		//runtime with glade.
		#pragma warning disable 0169
		private void OnWindowDeleteEvent (object sender, DeleteEventArgs args) 
		{
			Application.Quit ();
		}
		#pragma warning restore 0169
		
		private FileFilter CreateAssemblyFilter () {
			FileFilter fileFilter = new FileFilter ();
			fileFilter.Name = "Assemblies";
			fileFilter.AddPattern ("*.dll");
			fileFilter.AddPattern ("*.exe");
			return fileFilter;
		}
		
		//Disabling warning 0169 because this code will be called at
		//runtime with glade.
		#pragma warning disable 0169
		private void OnOpenToolButtonClicked (object sender, EventArgs args) 
		{
			FileChooserDialog fileChooser = new FileChooserDialog (
				"Choose an assembly for measure",
				mainWindow,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Open", ResponseType.Accept);
			fileChooser.Filter = CreateAssemblyFilter ();
			if (fileChooser.Run () == (int) ResponseType.Accept) {
				FillTreeView (new MeasureCalculator ().ProcessMeasures (AssemblyFactory.GetAssembly (fileChooser.Filename)));
			}
			fileChooser.Destroy ();
		}
		#pragma warning restore 0169

		private void FillTreeView (IEnumerable results) 
		{
			measures = results;
			measuresTreeStore = new TreeStore (typeof (string));

			foreach (TypeMeasure typeMeasure in results) {
				TreeIter parent = measuresTreeStore.AppendValues (typeMeasure.Name);	
				foreach (MethodMeasure methodMeasure in typeMeasure.MethodMeasures) {
					measuresTreeStore.AppendValues (parent, methodMeasure.Name);
				}
			}

			measuresTreeView.Model = measuresTreeStore;
		}

		private void OnTreeSelectionChanged (object sender, EventArgs args) 
		{
			TreeIter treeIter;
			if (measuresTreeView.Selection.GetSelected (out treeIter)) {
				TreeIter parent;
				if (measuresTreeStore.IterParent (out parent, treeIter)) {
					string type = measuresTreeStore.GetValue (parent, 0).ToString ();
					string method = measuresTreeStore.GetValue (treeIter, 0).ToString ();
					ShowMethodInformation (type, method);
				}
				else {
					string type = measuresTreeStore.GetValue (treeIter, 0).ToString ();
					ShowTypeInformation (type);
				}
			}
		}

		private TypeMeasure FindTypeMeasure (string type) 
		{
			foreach (TypeMeasure typeMeasure in measures) {
				if (String.Compare (typeMeasure.Name, type) == 0)
					return typeMeasure;
			}
			return null;
		}

		private MethodMeasure FindMethodMeasure (string type, string method) 
		{
			TypeMeasure typeMeasure = FindTypeMeasure (type);
			foreach (MethodMeasure methodMeasure in typeMeasure.MethodMeasures) {
				if (String.Compare (methodMeasure.Name, method) == 0) 
					return methodMeasure;
			}
			return null;
		}

		private void CleanExpander () 
		{
			if (expander1.Child != null)
				expander1.Remove (expander1.Child);
		}
		
		private void ShowMethodInformation (string type, string method) 
		{
			CleanExpander ();
			expander1.Child = new MethodMeasureWidget (FindMethodMeasure (type, method)).Widget;
			expander1.ShowAll ();
		}

		private void ShowTypeInformation (string type) {
			CleanExpander ();
			expander1.Child = new TypeMeasureWidget (FindTypeMeasure (type)).Widget;
			expander1.ShowAll ();

		}
	}
}
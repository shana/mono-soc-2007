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

		private void OnWindowDeleteEvent (object sender, DeleteEventArgs args) 
		{
			Application.Quit ();
		}
		
		private FileFilter CreateAssemblyFilter () {
			FileFilter fileFilter = new FileFilter ();
			fileFilter.Name = "Assemblies";
			fileFilter.AddPattern ("*.dll");
			fileFilter.AddPattern ("*.exe");
			return fileFilter;
		}

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

		private void ShowMethodInformation (string type, string method) 
		{
			MethodMeasure methodMeasureToShow = null;
			foreach (TypeMeasure typeMeasure in measures) {
				if (String.Compare (typeMeasure.Name, type) == 0) {
					foreach (MethodMeasure methodMeasure in typeMeasure.MethodMeasures) {
						if (String.Compare (methodMeasure.Name, method) == 0) {
							methodMeasureToShow = methodMeasure; 
						}
					}
				}
			}
			if (expander1.Child != null)
				expander1.Remove (expander1.Child);
			expander1.Child = new MethodMeasureWidget (methodMeasureToShow).Widget;
			expander1.ShowAll ();
		}

		private void ShowTypeInformation (string type) {
			TypeMeasure typeMeasureToShow = null;
			foreach (TypeMeasure typeMeasure in measures) {
				if (String.Compare (typeMeasure.Name, type) == 0)
					typeMeasureToShow = typeMeasure;
			}
			if (expander1.Child != null)
				expander1.Remove (expander1.Child);
			expander1.Child = new TypeMeasureWidget (typeMeasureToShow).Widget;
			expander1.ShowAll ();

		}
	}
}

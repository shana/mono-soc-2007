//
// Measures.Ui.FindDialog class
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
using Measures;

namespace Measures.Ui {
	
	public class FindDialog {
		Glade.XML xmlFindDialog;
		IEnumerable measures; 
		IEnumerable results;
		[Widget] Dialog findDialog;
		[Widget] RadioButton linesPerMethodRadioButton;
		[Widget] RadioButton parametersPerMethodRadioButton;
		[Widget] RadioButton numberOfLinesRadioButton;
		[Widget] RadioButton numberOfParametersRadioButton;
		[Widget] RadioButton numberOfFieldsRadioButton;
		[Widget] Button okButton;
		[Widget] Entry minimumValueEntry;

		public FindDialog (IEnumerable measures) 
		{
			xmlFindDialog = new Glade.XML (null,"measures.glade", "findDialog", null);
			xmlFindDialog.Autoconnect (this);
			this.measures = measures;
		}

		public void ShowDialog () 
		{
			findDialog.Run ();
		}

		public IEnumerable Results {
			get {
				return results;
			}
		}

		//Disabling warning 0169 because this code will be called at
		//runtime with glade.
		#pragma warning disable 0169
		private void OnDialogResponse (object sender, ResponseArgs args) 
		{
			if (args.ResponseId == ResponseType.Ok) {
				MeasureFinder measureFinder = new MeasureFinder ();
				int minimumValue = Int32.Parse (minimumValueEntry.Text);
				if (linesPerMethodRadioButton.Active) {
					results = measureFinder.FindByLinesPerMethod (measures, minimumValue);
				}
				else if (parametersPerMethodRadioButton.Active) {
					results = measureFinder.FindByParametersPerMethod (measures, minimumValue);
				}
				else if (numberOfLinesRadioButton.Active) {
					results = measureFinder.FindByNumberOfLines (measures, minimumValue);
				}
				else if (numberOfParametersRadioButton.Active) {
					results = measureFinder.FindByNumberOfParameters (measures, minimumValue);
				}
				else if (numberOfFieldsRadioButton.Active) {
					results = measureFinder.FindByNumberOfFields (measures, minimumValue);
				}
			}
			findDialog.Destroy ();
		}
		#pragma warning restore 0169
		
		//Disabling warning 0169 because this code will be called at
		//runtime with glade.
		#pragma warning disable 0169
		private void OnVBoxEvent (object sender, WidgetEventArgs args) 
		{
			okButton.Sensitive = ValidateUpperLimit ();
		}
		#pragma warning restore 0169

		private bool ValidateUpperLimit () 
		{
			//I get an 0117 if I do the TryParse
			try {
				Int32.Parse (minimumValueEntry.Text);
				return true;
			}
			catch (FormatException) {
				return false;
			}
		}
	}
}

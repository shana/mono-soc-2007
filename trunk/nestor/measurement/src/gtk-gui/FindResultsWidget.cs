//
// Measures.Ui.FindResultsWidget class
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
	
	public class FindResultsWidget {
		Glade.XML xmlFindResultsWidget;
		[Widget] Table table4;
		[Widget] Label countTypes;
		[Widget] Label countMethods;

		public FindResultsWidget (IEnumerable results) 
		{
			xmlFindResultsWidget = new Glade.XML (null,"measures.glade", "table4", null);
			xmlFindResultsWidget.Autoconnect (this);
			countTypes.Text = CountTypes (results).ToString ();
			countMethods.Text = CountMethods (results).ToString ();
		}

		private int CountTypes (IEnumerable results) 
		{
			return ((ICollection) results).Count;
		}
		
		private int CountMethods (IEnumerable results) 
		{
			int counter = 0;
			foreach (TypeMeasure typeMeasure in results) {
				counter += ((ICollection) typeMeasure.MethodMeasures).Count;
			}
			return counter;
		}

		public Widget Widget {
			get {
				return table4;
			}
		}
	}
}

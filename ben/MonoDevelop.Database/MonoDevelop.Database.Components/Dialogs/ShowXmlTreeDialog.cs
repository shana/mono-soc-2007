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
using System.Xml;
using System.Collections.Generic;

namespace MonoDevelop.Database.Components
{
	public partial class ShowXmlTreeDialog : Gtk.Dialog
	{
		private ListStore store;
		
		public ShowXmlTreeDialog (string xml)
		{
			this.Build();
			BuildTree ();
			LoadXml (xml);
			
			ShowAll ();
		}
		
		private void BuildTree ()
		{
			//FIXME:
			//tree.HeadersVisible = false;
			
			store = new ListStore (typeof (Gdk.Pixbuf), typeof (string));
			//tree.Model = store;
			
			TreeViewColumn col = new TreeViewColumn ();
			
			CellRendererPixbuf pixbufRenderer = new CellRendererPixbuf ();
			col.PackStart (pixbufRenderer, false);
			col.AddAttribute (pixbufRenderer, "pixbuf", 0);
			
			CellRendererText textRenderer = new CellRendererText ();
			col.PackStart (textRenderer, true);
			col.AddAttribute (textRenderer, "text", 1);
			
			//tree.AppendColumn (col);
		}
		
		private void LoadXml (string xml)
		{
			XmlDocument doc = new XmlDocument ();
			try {
				doc.Load (xml);
				
				//TODO: create tree
			} catch {
				//hboxError.Visible = true;
			}
		}
		
		protected virtual void CloseClicked (object sender, System.EventArgs e)
		{
			Destroy ();
		}
	}
}

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace umbraco.editorControls.simpleEditor
{
	/// <summary>
	/// Summary description for simpleEditor.
	/// </summary>
	public class SimpleEditor : System.Web.UI.WebControls.TextBox, interfaces.IDataEditor
	{
		private interfaces.IData _data;


		public SimpleEditor(interfaces.IData Data) 
		{
			_data = Data;
		}

		public virtual bool TreatAsRichTextEditor 
		{
			get {return false;}
		}

		public bool ShowLabel 
		{
			get {return true;}
		}

		public Control Editor {get{return this;}}

		public void Save() 
		{
			_data.Value = this.Text;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			base.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
			base.Rows = 8;
			if (this.CssClass == "")
				this.CssClass = "umbEditorTextField";
			if (!Page.IsPostBack) 
			{
				if (_data != null && _data.Value != null)
					Text = _data.Value.ToString();
			}

			base.Attributes.Add("onSelect", "storeCaret(this);");
			base.Attributes.Add("onClick", "storeCaret(this);");
			base.Attributes.Add("onKeyUp", "storeCaret(this);");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "simpleEditorJs", "<script>" + 
	"var tempCaretEl;" + 
	"	function storeCaret (editEl) {" + 
	"		if (editEl.createTextRange)" + 
	"			editEl.currRange = document.selection.createRange().duplicate();" + 
	"	}" + 
	"function insertLink(el) {" + 
	"var theLink = prompt('Enter URL for link here:','http://');" + 
	"insertTag(el, 'a', ' href=\"' + theLink + '\"');" + 
	"}" + 
	"function insertTag (el, tag, param) {" + 
	"  if (el.currRange) {" + 
	"    el.currRange.text = '<' + tag + param + '>' + el.currRange.text + '<\\/' +" + 
	"tag + '>';" + 
	"    el.currRange.select();" + 
	"  }" + 
	"}" + 
	"</script>");
		}

		protected override void Render(HtmlTextWriter output)
		{
			output.WriteLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td>");
			output.WriteLine(	"<p align=\"right\" style=\"margin: 0px; padding:0px\"><a href=\"javascript:insertLink(document.getElementById('" + this.ClientID + "'))\">" +
	"<img src=\"/Umbraco_client/simpleEditor/images/link.gif\" border=\"0\" align=\"right\" />" +
	"</a>" +
	"<a href=\"javascript:insertTag(document.getElementById('" + this.ClientID + "'), 'em', '')\">" +
	"<img src=\"/Umbraco_client/simpleEditor/images/italic.gif\" border=\"0\" style=\"margin-left: 3px; margin-right: 3px;\" align=\"right\" />" +
	"</a>" +
	" <a href=\"javascript:insertTag(document.getElementById('" + this.ClientID + "'), 'strong', '')\">" +
	"<img src=\"/Umbraco_client/simpleEditor/images/bold.gif\" border=\"0\" align=\"right\" />" +
	"</a>" +
	"<br/></p>");
			base.Render(output);
			output.WriteLine("</td></tr></table>");
		}
	}
}
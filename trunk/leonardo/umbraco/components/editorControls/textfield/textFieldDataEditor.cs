using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace umbraco.editorControls.textfield
{
	public class TextFieldEditor : System.Web.UI.WebControls.TextBox, interfaces.IDataEditor
	{
		private interfaces.IData _data;


		public TextFieldEditor(interfaces.IData Data) {
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
			if (this.CssClass == "")
				this.CssClass = "umbEditorTextField";
			if (!Page.IsPostBack && _data != null && _data.Value != null) 
			{
				Text = _data.Value.ToString();
			}
		}

		protected override void Render(HtmlTextWriter output)
		{
			base.Render(output);
		}
	}
}
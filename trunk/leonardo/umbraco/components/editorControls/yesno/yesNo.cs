using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace umbraco.editorControls
{
	/// <summary>
	/// Generates a radiolist of yes and no for boolean fields
	/// </summary>
	public class yesNo : System.Web.UI.WebControls.RadioButtonList, interfaces.IDataEditor
	{
	
		private interfaces.IData _data;
		public yesNo(interfaces.IData Data) {
			_data = Data;
		}
		private String _text;

		public Control Editor {
			get 
			{
				return this;
			}
		}
		public virtual bool TreatAsRichTextEditor 
		{
			get {return false;}
		}
		public bool ShowLabel 
		{
			get {return true;}
		}

		public override String Text
		{
			get {
				if (!Page.IsPostBack && _data != null && _data.Value != null)
					_text = _data.Value.ToString();
				return _text;
			
			}
			// set {_text = value;}
		}

		public void Save() 
		{
				string value = "";
				if (this.Items[0].Selected)
					value = "1";
				else
					value = "0";
			_data.Value = value;
			
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			base.Items.Add(new ListItem(UI.Text("yes"), "1"));
			base.Items.Add(new ListItem(UI.Text("no"), "0"));
		}

	
		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param Name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			if (Text != null) {
				if (_text == "1") 
				{
					this.Items[0].Selected = true;
					this.Items[1].Selected = false;
				} 
				else 
				{
					this.Items[0].Selected = false;
					this.Items[1].Selected = true;
				}
				}

			base.Render(output);
		}
	}
}

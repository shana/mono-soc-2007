using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Globalization;

namespace umbraco.editorControls
{
	/// <summary>
	/// Summary description for dateField.
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:dateField runat=server></{0}:dateField>")]
	public class dateField : controls.datePicker, interfaces.IDataEditor
	{

		interfaces.IData _data;

		public dateField(interfaces.IData Data) {
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

		public Control Editor {
			get {return this;}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if (!Page.IsPostBack) 
			{
				if (_data != null && _data.Value != null && _data.Value.ToString() != "")
					Text = _data.Value.ToString();
				else
					base.EmptyDateAsDefault = true;

			}
		}

		public void Save() 
		{
			try 
			{
				if (System.Web.HttpContext.Current.Request.Form[this.ClientID] != "") 
				{
					DateTime tempDate = DateTime.Parse(System.Web.HttpContext.Current.Request.Form[this.ClientID]);
					this.Text = tempDate.ToLongDateString() + " " + tempDate.ToLongTimeString();
					System.Data.SqlTypes.SqlDateTime sqlDate = new System.Data.SqlTypes.SqlDateTime(tempDate);
					_data.Value = sqlDate;
				} else
					_data.Value = null;
			} 
			catch {
				_data.Value = null;
			}
		}
	
	
		public new string Text 
		{
			get
			{
				return base.DateTime.ToString();
			}

			set
			{
				try 
				{
					
					base.DateTime = Convert.ToDateTime(value);;
				} 
				catch {}
			}
		}


		protected override void OnInit(EventArgs e)
		{
			//base.ShowTime = false;
			base.CustomMinutes = "00, 05, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55";
			base.OnInit (e);
		}



		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param Name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter writer)
		{
			base.Render (writer);
		}
	}
}

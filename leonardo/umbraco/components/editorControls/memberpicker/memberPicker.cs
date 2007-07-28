using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.editorControls
{
	/// <summary>
	/// Summary description for memberPicker.
	/// </summary>
	public class memberPicker : System.Web.UI.WebControls.DropDownList, interfaces.IDataEditor
	{

		
		private String _text;

		interfaces.IData _data;
		public memberPicker(interfaces.IData Data)
		{
			_data = Data;
		}

		public virtual bool TreatAsRichTextEditor 
		{
			get {return false;}
		}
		public virtual bool ShowLabel 
		{
			get {return true;}
		}

		public Control Editor {
			get {return this;}
		}
		
		public override string Text
		{
			get
			{
				if (!Page.IsPostBack) 
				{
					_text = _data.Value.ToString();
				}
				return _text;
			}
		}
		public void Save() 
		{
				// Loop through the items, and write the ones selected
				_text = "";
				foreach(ListItem li in base.Items) 
				{
					if (li.Selected) 
					{
						_text = li.Value;
						break;
					}
						
				}
				_data.Value = _text;
				
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			SqlDataReader dropdownData = SqlHelper.ExecuteReader(umbraco.GlobalSettings.DbDSN, 
				CommandType.Text, "select id, text from umbracoNode where nodeObjectType = '39EB0F98-B348-42A1-8662-E7EB18487560' order by text");
			base.DataValueField = "id";
			base.DataTextField = "text";
			base.DataSource = dropdownData;
			base.DataBind();
			base.Items.Insert(0, new ListItem(ui.Text("choose") + "...",""));

			// Iterate on the control items and mark fields by match them with the Text property!
			foreach(ListItem li in base.Items) 
			{
				if ((","+Text+",").IndexOf(","+li.Value.ToString()+",") > -1)
					li.Selected = true;
			}
			dropdownData.Close();
		}
	
		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			base.Render(output);
		}
	}
}

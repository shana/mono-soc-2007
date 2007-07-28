using System;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;

namespace umbraco.macroRenderings
{
	/// <summary>
	/// Summary description for content.
	/// </summary>
	public class media : System.Web.UI.HtmlControls.HtmlInputHidden, interfaces.IMacroGuiRendering
	{
		string _value = "";

		public bool ShowCaption 
		{
			get {return true;}
		}

		public override string Value 
		{
			get {return _value;}
			set 
			{
				_value = value;
			}
		}

		public media()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			string label = "";
			if (this.Value != "") 
			{
				SqlDataReader pageName = SqlHelper.ExecuteReader(umbraco.GlobalSettings.DbDSN, 
					CommandType.Text, "select text as nodeName from umbracoNode where id = " + this.Value);
				if (pageName.Read())
					label = pageName.GetString(pageName.GetOrdinal("nodeName")) + "<br/>";
				pageName.Close();
			}
			writer.WriteLine("<b><span id=\"label" + this.ID + "\">" + label + "</span></b>");
			
			writer.WriteLine("<a href=\"javascript:saveTreepickerValue('media','" + this.ID + "');\">Choose item</a>");
			writer.WriteLine("<input type=\"hidden\" name=\"" + this.ID + "\" value=\"" + this.Value + "\"/>");
		}

	}
}

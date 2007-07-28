using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;

namespace umbraco.dialogs
{
	/// <summary>
	/// Summary description for umbracoField.
	/// </summary>
	public partial class umbracoField : BasePages.UmbracoEnsuredPage
	{
		string[] preValuesSource = {"@pageID", "@parentID", "@level", "@writerID", "@nodeType", "@template", "@sortOrder", "@createDate", "@updateDate", "@pageName", "@urlName", "@writerName", "@nodeTypeAlias", "@path"};

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			SqlDataReader dataTypes = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, 
				CommandType.Text, "select distinct alias from cmsPropertyType order by alias");

			fieldPicker.DataTextField = "alias";
			fieldPicker.DataValueField = "alias";
			fieldPicker.DataSource = dataTypes;
			fieldPicker.DataBind();
			fieldPicker.Attributes.Add("onChange", "document.forms[0].field.value = document.forms[0].fieldPicker[document.forms[0].fieldPicker.selectedIndex].value;");
			dataTypes.Close();

			SqlDataReader dataTypes2 = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, 
				CommandType.Text, "select distinct alias from cmsPropertyType order by alias");
			altFieldPicker.DataTextField = "alias";
			altFieldPicker.DataValueField = "alias";
			altFieldPicker.DataSource = dataTypes2;
			altFieldPicker.DataBind();
			altFieldPicker.Attributes.Add("onChange", "document.forms[0].useIfEmpty.value = document.forms[0].altFieldPicker[document.forms[0].altFieldPicker.selectedIndex].value;");
			dataTypes2.Close();

			fieldPicker.Items.Insert(0,new ListItem(ui.Text("general", "choose", base.getUser())));
			altFieldPicker.Items.Insert(0,new ListItem(ui.Text("general", "choose", base.getUser())));

			// Pre values
			foreach (string s in preValuesSource) 
			{
				fieldPicker.Items.Add(new ListItem(s, s.Replace("@","")));
				altFieldPicker.Items.Add(new ListItem(s, s.Replace("@","")));
			}

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

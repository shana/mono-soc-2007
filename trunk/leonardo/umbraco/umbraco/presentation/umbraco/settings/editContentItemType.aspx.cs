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

namespace umbraco.cms.presentation.settings
{
	/// <summary>
	/// Summary description for editContentItemType.
	/// </summary>
	public partial class editContentItemType : BasePages.UmbracoEnsuredPage
	{
		private cms.businesslogic.ContentType ct;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ct = new cms.businesslogic.contentitem.ContentItemType(int.Parse(Request.QueryString["id"]));
			controls.ContentTypeControl tmp = new controls.ContentTypeControl(ct,"TabView1");
			plc.Controls.Add(tmp);
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

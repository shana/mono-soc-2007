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

namespace umbraco.presentation.webservices.ajax_aspx_calls
{
	/// <summary>
	/// Summary description for delete.
	/// </summary>
	public partial class delete : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (BasePages.BasePage.ValidateUserContextID(BasePages.BasePage.umbracoUserContextID)) 
			{
				int nodeId = -1;
				string alias = "";
				if (helper.Request("nodeId") != "")
					try 
					{
						nodeId = int.Parse(helper.Request("nodeId"));
					} 
					catch 
					{
						// Pages using strings as id's like xslts
						alias = helper.Request("nodeId");
					}

				presentation.create.dialogHandler_temp.Delete(helper.Request("nodeType"), nodeId, alias);
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

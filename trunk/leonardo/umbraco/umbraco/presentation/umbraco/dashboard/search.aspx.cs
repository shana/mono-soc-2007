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
using System.Web.Services;
using System.Xml;

namespace dashboardUtilities
{
	/// <summary>
	/// Summary description for search.
	/// </summary>
	public partial class search : umbraco.BasePages.UmbracoEnsuredPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (umbraco.helper.Request("q").Trim() != "" && this.getUser() != null) 
			{
				string temp = "";

				webService ws = new webService();
				ws.Url = umbraco.helper.GetBaseUrl(System.Web.HttpContext.Current) + this.UmbracoPath + "/webService.asmx";
				XmlNode result = ws.GetDocumentsBySearch(umbraco.helper.Request("q"), this.getUser().StartNodeId, umbraco.BasePages.BasePage.umbracoUserContextID);
				if (result != null) 
				{
					foreach (XmlNode x in result.ChildNodes) 
					{
						temp += (x.Attributes.GetNamedItem("nodeName").Value.Replace("'","\\'").Replace("\"","\\\"").Replace("\n", "").Replace("\t", "").Replace("\r", "") + "|||" + x.Attributes.GetNamedItem("id").Value + "\n");
						//						temp += ("new ComboBoxItem('" + x.Attributes.GetNamedItem("nodeName").Value.Replace("'","\\'").Replace("\"","\\\"").Replace("\n", "").Replace("\t", "").Replace("\r", "") + "'," + x.Attributes.GetNamedItem("id").Value + ")\n");
					}
					if (temp.Length > 0)
						Response.Write (temp.Substring(0, temp.Length-1));
					else
						Response.Write("0");
				}
   				else
					Response.Write ("error");
			}
			else
				Response.Write("0");
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

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

using System.Threading;

namespace umbraco.presentation.webservices.aspx_ajax_calls
{
	/// <summary>
	/// Summary description for progressStatus.
	/// </summary>
	public partial class progressStatus : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(GetResult));
			t.Start();
		}


		private void GetResult() 
		{
			try 
			{
				Response.Write(Application[helper.Request("key")].ToString());
			} 
			catch 
			{
				Response.Write("");
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

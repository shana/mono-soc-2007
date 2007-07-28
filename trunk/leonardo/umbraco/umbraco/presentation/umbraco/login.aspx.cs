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
using umbraco.BusinessLogic;

namespace umbraco.cms.presentation
{
	/// <summary>
	/// Summary description for login.
	/// </summary>
	public partial class login : BasePages.BasePage
	{
		protected umbWindow treeWindow;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender (e);
			Button1.Text = ui.Text("general", "login");
			Panel1.Text = ui.Text("general", "welcome");
			Panel1.Style.Add("padding","10px;");
			username.Text = ui.Text("general", "username");
			password.Text = ui.Text("general", "password");	

			// Add bottom and top texts
			TopText.Text = ui.Text("login", "topText");	
			BottomText.Text = ui.Text("login", "bottomText", DateTime.Now.Year.ToString(), null);	

			
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

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			if (BusinessLogic.User.validateCredentials(lname.Text,passw.Text)) 
			{
				doLogin(new BusinessLogic.User(lname.Text,passw.Text));
                if(string.IsNullOrEmpty(Request["redir"]))
                    Response.Redirect("umbraco.aspx");
                else
                    Response.Redirect(Request["redir"]);
			}
		}
	}
}

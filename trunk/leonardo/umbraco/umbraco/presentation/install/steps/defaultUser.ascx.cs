namespace umbraco.presentation.install.steps
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for defaultUser.
	/// </summary>
	public partial class defaultUser : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Disable back/forward buttons
			Page.FindControl("next").Visible = false;
			Page.FindControl("back").Visible = false;

			BusinessLogic.User u = BusinessLogic.User.GetUser(0);

			if (u.NoConsole || u.Disabled) 
			{
				identifyResult.Text = "<b>The Default user has been disabled or has no access to umbraco!</b><br/><br/>No further actions needs to be taken. Click <b>Next</b> to proceed.";
				Page.FindControl("next").Visible = true;
			}
			else if (u.Password != "default") 
			{
				identifyResult.Text = "<b>The Default user's password has been successfully changed since the installation!</b><br/><br/>No further actions needs to be taken. Click <b>Next</b> to proceed.";
				Page.FindControl("next").Visible = true;
			}
			else 
			{
				identifyResult.Text = "<b>The Default users’ password needs to be changed!</b>";
				changeForm.Visible = true;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void changePassword_Click(object sender, System.EventArgs e)
		{
			BusinessLogic.User.GetUser(0).Password = password.Text;
			passwordChanged.Visible = true;
			identify.Visible = false;
			changeForm.Visible = false;
			Page.FindControl("next").Visible = true;
		}
	}
}

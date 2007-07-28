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

namespace umbraco.presentation.install
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class _default : System.Web.UI.Page // This is intentionally change from a protected page to a non protected
	{

		private string _installStep = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			step.Value = _installStep;
		}

		private void loadContent() 
		{
			Response.Redirect("./default.aspx?installStep=" + step.Value, true);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			_installStep = helper.Request("installStep");
			if (_installStep == "")
				_installStep = "welcome";
			PlaceHolderStep.Controls.Add(new System.Web.UI.UserControl().LoadControl(GlobalSettings.Path + "/../install/steps/" + _installStep + ".ascx"));
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		protected void next_Click(object sender, System.EventArgs e)
		{
			switch (step.Value) 
			{
				case "welcome":
					step.Value = "detect";
					loadContent();
					break;
				case "detect":
					step.Value = "validatePermissions";
					loadContent();
					break;
				case "upgradeIndex":
					step.Value = "validatePermissions";
					loadContent();
					break;
				case "validatePermissions":
					step.Value = "defaultUser";
					loadContent();
					break;
				case "defaultUser":
					step.Value = "theend";
					loadContent();
					next.Text = "Close";
					break;
				case "theend":
					Response.Redirect("http://umbraco.org/redir/getting-started", true);
					break;
				default:
					break;
			}
		}
	}
}

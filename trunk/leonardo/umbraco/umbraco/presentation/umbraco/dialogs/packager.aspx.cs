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
using System.Xml;
using System.Xml.XPath;

namespace umbraco.dialogs
{
	/// <summary>
	/// Summary description for packager.
	/// </summary>
	public partial class packager : BasePages.UmbracoEnsuredPage
	{
		
		private Control configControl;

		

		private cms.businesslogic.macro.Packager p = new cms.businesslogic.macro.Packager();
		private string tempFileName = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!IsPostBack)
            {
                ButtonInstall.Attributes.Add(
                    "onClick",
                    "this.style.display = 'none'; document.getElementById('installingMessage').style.display = 'block'; document.getElementById('pleaseWaitIcon').src = '../images/wait.gif'; return true;");
            }

            if (umbraco.helper.Request("guid") != "")
            {
                tempFile.Value = p.Import(p.Fetch(new Guid(umbraco.helper.Request("guid"))));
                updateSettings();
            }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			if (helper.Request("config") != "") 
			{
				drawConfig();
			}
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
			tempFileName = Guid.NewGuid().ToString() + ".umb";
			string fileName = GlobalSettings.StorageDirectory + System.IO.Path.DirectorySeparatorChar + tempFileName;
			file1.PostedFile.SaveAs(Server.MapPath(fileName));
			tempFile.Value = p.Import(tempFileName);
            updateSettings();
			
		}

        private void updateSettings()
        {
            PanelUpload.Visible = false;
            PanelAccept.Visible = true;
            LabelName.Text = p.Name + " Version: " + p.Version;
            LabelMore.Text = "<a href=\"" + p.Url + "\" target=\"_blank\">" + p.Url + "</a>";
            LabelAuthor.Text = "<a href=\"" + p.AuthorUrl + "\" target=\"_blank\">" + p.Author + "</a>";
            LabelLicense.Text = "<a href=\"" + p.LicenseUrl + "\" target=\"_blank\">" + p.License + "</a>";
            if (p.ReadMe != "")
                readme.Text = "<div style=\"border: 1px solid #999; padding: 5px; overflow: scroll; width: 370px; height: 160px;\">" + p.ReadMe + "</div>";
            else
                readme.Text = "<span style=\"color: #999\">No information</span>";
        }

		protected void ButtonInstall_Click(object sender, System.EventArgs e)
		{
			p.LoadConfig(tempFile.Value);
			p.Install(tempFile.Value);
			
			if (p.Control != null && p.Control != "") 
			{
				Response.Redirect("packager.aspx?config=" + Server.UrlEncode(p.Control), true);
			} 
			else 
			{
				succes.Visible = true;
				PanelAccept.Visible = false;
			}
		}

		private void drawConfig() 
		{
			PanelUpload.Visible = false;
			optionalControl.Visible = true;
			succes.Visible = false;
			PanelAccept.Visible = false;

			configControl = new System.Web.UI.UserControl().LoadControl(GlobalSettings.Path + "/.." + helper.Request("config"));
			configControl.ID = "packagerConfigControl";
			optionalControl.Controls.Add(configControl);
		}
	}
}

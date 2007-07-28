namespace umbraco.presentation.install.steps
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using System.Data.SqlClient;
	using Microsoft.ApplicationBlocks.Data;

	/// <summary>
	///		Summary description for detect.
	/// </summary>
	public partial class detect : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			businesslogic.DbVersion dbVersion = businesslogic.detect.GetDatabaseVersion();			
			version.Text = dbVersion.ToString();

			// Hide all buttons and text
			hideAll();


			if (dbVersion == businesslogic.DbVersion.v3)
				v3.Visible = true;
			else 
			{
				// Disable back/forward buttons
				Page.FindControl("next").Visible = false;
				Page.FindControl("back").Visible = false;

				if (dbVersion == businesslogic.DbVersion.v21 ||
					dbVersion == businesslogic.DbVersion.v20 ||
					dbVersion == businesslogic.DbVersion.v21rc ||
                    dbVersion == businesslogic.DbVersion.v211) 
				{
					upgrade.Visible = true;
					other.Visible = true;
				} 
				else if (dbVersion == businesslogic.DbVersion.None)
				{
					install.Visible = true;
					none.Visible = true;
				} 
				else 
				{
					retry.Visible = true;
					error.Visible = true;
				}
			}
		}

		private void hideAll() 
		{
			v3.Visible = false;
			other.Visible = false;
			none.Visible = false;
			error.Visible = false;
			install.Visible = false;
			retry.Visible = false;
			upgrade.Visible = false;
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

		protected void upgrade_Click(object sender, System.EventArgs e)
		{
			// Load and execute sql
			string theFile = "";
			string sql = "";
		    businesslogic.DbVersion dbVer = businesslogic.detect.GetDatabaseVersion();
			if (dbVer == businesslogic.DbVersion.v20) 
			{
				theFile = Server.MapPath(GlobalSettings.Path + "/../install/sql/20_upgrade.txt");
				sql = businesslogic.helper.loadTextFile(theFile);
				processSql(sql);
			}

            if (dbVer == businesslogic.DbVersion.v21rc) 
			{
				theFile = Server.MapPath(GlobalSettings.Path + "/../install/sql/21rc_upgrade.txt");
				sql = businesslogic.helper.loadTextFile(theFile);
				processSql(sql);
			}
            if (dbVer != businesslogic.DbVersion.v211)
            {
                theFile = Server.MapPath(GlobalSettings.Path + "/../install/sql/211_upgrade.txt");
                sql = businesslogic.helper.loadTextFile(theFile);
                processSql(sql);
            }
            theFile = Server.MapPath(GlobalSettings.Path + "/../install/sql/v3_upgrade_complete.txt");
            sql = businesslogic.helper.loadTextFile(theFile);
            processSql(sql);

            if (dbVer != businesslogic.DbVersion.v211)
		        Response.Redirect("default.aspx?installStep=upgradeIndex", true);
            else
                Response.Redirect("default.aspx?installStep=defaultUser", true);

			((HtmlInputHidden) Page.FindControl("step")).Value = "upgradeIndex";
			((Button) Page.FindControl("next")).Visible = true;
			upgrade.Visible = false;

			hideAll();
			identify.Visible = false;
			confirms.Visible = true;
			upgradeConfirm.Visible = true;
		
		}

		protected void install_Click(object sender, System.EventArgs e)
		{
			// Load and execute sql
			string theFile = Server.MapPath(GlobalSettings.Path + "/../install/sql/30clean.txt");
			string sql = businesslogic.helper.loadTextFile(theFile);
			processSql(sql);

			((HtmlInputHidden) Page.FindControl("step")).Value = "validatePermissions";
			((Button) Page.FindControl("next")).Visible = true;
			install.Visible = false;

			hideAll();
			identify.Visible = false;
			confirms.Visible = true;
			installConfirm.Visible = true;

		}

		private void processSql(string sql) 
		{
			string[] statements = sql.Split("|".ToCharArray());
			foreach (string statement in statements) 
			{
				SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, statement);
			}
		}
	}
}

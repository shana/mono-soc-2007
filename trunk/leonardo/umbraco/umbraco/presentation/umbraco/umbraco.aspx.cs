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
using System.Xml;
using System.Xml.XPath;

namespace umbraco.cms.presentation
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class _umbraco : BasePages.UmbracoEnsuredPage
	{
		protected umbWindow UmbWindow1;
		protected System.Web.UI.WebControls.PlaceHolder bubbleText;
	
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			

			// Load user module icons ..
			foreach (BusinessLogic.Application a in this.getUser().Applications) 
			{
				plcIcons.Controls.Add(new LiteralControl(
					"<img src=\"images/tray/" + a.icon + "\" width=\"49\" height=\"36\" alt=\"" + ui.Text("sections", a.alias, base.getUser()) + "\" border=\"0\" onClick=\"shiftApp('" + a.alias + "', '" + ui.Text("sections", a.alias, this.getUser()) + "');\" style=\"cursor:pointer;_cursor: hand;\">"));
			}

			// Load globalized labels
			PlaceHolderAppIcons.Text = ui.Text("main", "sections", base.getUser());
			treeWindow.Text = ui.Text("main", "tree", base.getUser());


			// Version check goes here!
			bool disableVersionCheck = false;
			if (GlobalSettings.DisableVersionCheck != null)
				disableVersionCheck = bool.Parse(GlobalSettings.DisableVersionCheck);

			if (!disableVersionCheck) { // && Request.Cookies["updateCheck"] == null) {
				XmlDocument versionDoc = new XmlDocument();
				XmlTextReader versionReader = new XmlTextReader(Server.MapPath(GlobalSettings.Path + "/config/version.xml"));
				versionDoc.Load(versionReader);
				versionReader.Close();

				// Find current versions
				int versionMajor, versionMinor, versionPatch;
				versionMajor = Convert.ToInt32(versionDoc.SelectSingleNode("/version/major").FirstChild.Value);
				versionMinor = Convert.ToInt32(versionDoc.SelectSingleNode("/version/minor").FirstChild.Value);
				versionPatch = Convert.ToInt32(versionDoc.SelectSingleNode("/version/patch").FirstChild.Value);

				try 
				{
					/*
					umbracoUpdater.Check umbracoUpdater = new umbracoUpdater.Check();
					string latestVersion = umbracoUpdater.CheckVersion(versionMajor,versionMinor,versionPatch);
					if (latestVersion != "")
						bubbleText.Controls.Add(new LiteralControl("setTimeout('umbSpeechBubble(\"Info\", \"" + ui.Text("update", "updateAvailable", base.getUser()) + "\", \"<a href=http://umbraco.org/download class=nolink target=_blank>" + ui.Text("update", "updateDownloadText", latestVersion, base.getUser()) + "</a>\")', 1000);"));
					else
						Trace.Write("UmbracoUpdate", "This version is up to date!");
						*/
				} 
				catch (Exception updaterException) 
				{
					Trace.Warn("UmbracoUpdate", "Error verifying version", updaterException);
					bubbleText.Controls.Add(new LiteralControl("setTimeout('umbSpeechBubble(\"Info\", \"" + ui.Text("update", "updateNoServer", this.getUser()) + "\", \"" + ui.Text("update", "updateNoServerError", this.getUser())+ "\")', 1000);"));
				}

				Response.Cookies["updateCheck"].Value = "1";
				Response.Cookies["updateCheck"].Expires = DateTime.Now.AddDays(1);
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

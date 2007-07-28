using System;
using umbraco.BusinessLogic;

namespace umbraco.BasePages
{
	/// <summary>
	/// Summary description for BasePage1.
	/// </summary>
	public class UmbracoEnsuredPage : BasePage
	{
		public UmbracoEnsuredPage()
		{

		}

        private bool _redirectToUmbraco;
        /// <summary>
        /// If true then umbraco will force any window/frame to reload umbraco in the main window
        /// </summary>
        public bool RedirectToUmbraco
        {
            get { return _redirectToUmbraco; }
            set { _redirectToUmbraco = value; }
        }
	
        public bool ValidateUserApp(string app)
        {

            foreach(Application uApp in getUser().Applications)
                if (uApp.alias == app)
                    return true;
            return false;
        }

        public bool ValidateUserNodeTreePermissions(string Path, string Action)
        {
            string permissions = getUser().GetPermissions(Path);
                        if (permissions.IndexOf(Action) > -1 && (Path.Contains("-20") || (","+Path+",").Contains("," + getUser().StartNodeId.ToString() + ",")))
                            return true;

            Log.Add(LogTypes.LoginFailure, getUser(), -1, "Insufient permissions in UmbracoEnsuredPage: '" + Path + "', '" + permissions + "', '" + Action + "'");
            return false;
        }

		public static BusinessLogic.User CurrentUser {
			get 
			{
				if (BasePage.umbracoUserContextID != "")
					return BusinessLogic.User.GetUser(BasePage.GetUserId(BasePage.umbracoUserContextID));
				else
					return BusinessLogic.User.GetUser(0);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			try {
				ensureContext();
			}
			catch 
			{
                // Some umbraco pages should not be loaded on timeout, but instead reload the main application in the top window. Like the treeview for instance
                if (RedirectToUmbraco)
                    Response.Redirect(GlobalSettings.Path+"/logout.aspx?");
                else
                    Response.Redirect(GlobalSettings.Path + "/logout.aspx?redir=" + Server.UrlEncode(Request.RawUrl));
            }

			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ui.Culture(this.getUser()));
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
		}
	}
}
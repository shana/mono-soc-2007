using System;
using Umbraco;
using Umbraco.BusinessLogic;

namespace Umbraco.BasePages
{
	/// <summary>
	/// Summary description for BasePage1.
	/// </summary>
	public class UmbracoEnsuredPage : BasePage
	{
        private bool _redirectToUmbraco;
        /// <summary>
        /// If true then Umbraco will force any window/frame to reload Umbraco in the main window
        /// </summary>
        public bool RedirectToUmbraco
        {
            get { return _redirectToUmbraco; }
            set { _redirectToUmbraco = value; }
        }
	
        public bool ValidateUserApp(string app)
        {

            foreach(Application uApp in ValidatedUser.Applications)
                if (uApp.alias == app)
                    return true;
            return false;
        }

        public bool ValidateUserNodeTreePermissions(string Path, string Action)
        {
            string permissions = ValidatedUser.GetPermissions(Path);
                        if (permissions.IndexOf(Action) > -1 && (Path.Contains("-20") || (","+Path+",").Contains("," + ValidatedUser.StartNodeId.ToString() + ",")))
                            return true;

            Log.Add(LogTypes.LoginFailure, ValidatedUser, -1, "Insufient permissions in UmbracoEnsuredPage: '" + Path + "', '" + permissions + "', '" + Action + "'");
            return false;
        }

		public static BusinessLogic.User CurrentUser {
			get 
			{
				if (BasePage.UmbracoUserContextID != "")
					return BusinessLogic.User.GetUser(BasePage.GetUserId(BasePage.UmbracoUserContextID));
				else
					return BusinessLogic.User.GetUser(0);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			try {
				EnsureContext();
			}
			catch 
			{
                // Some Umbraco pages should not be loaded on timeout, but instead reload the main application in the top window. Like the treeview for instance
                if (RedirectToUmbraco)
                    Response.Redirect(GlobalSettings.Path+"/logout.aspx?");
                else
                    Response.Redirect(GlobalSettings.Path + "/logout.aspx?redir=" + Server.UrlEncode(Request.RawUrl));
            }

			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(UIHelper.GetUserCulture(this.ValidatedUser));
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
		}
	}
}
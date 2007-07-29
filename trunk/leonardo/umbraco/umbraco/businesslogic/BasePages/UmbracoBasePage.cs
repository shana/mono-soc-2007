using System;
using System.Globalization;
using System.Threading;
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

		/// <summary>
		/// Validates the user app.
		/// </summary>
		/// <param name="app">The app.</param>
		/// <returns></returns>
		public bool ValidateUserApp(string app)
		{
			foreach (Application uApp in ValidatedUser.Applications)
				if (uApp.Alias == app)
					return true;
			return false;
		}

		/// <summary>
		/// Validates the user node tree permissions.
		/// </summary>
		/// <param name="Path">The path.</param>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public bool ValidateUserNodeTreePermissions(string Path, string Action)
		{
			string permissions = ValidatedUser.GetPermissions(Path);
			if (permissions.IndexOf(Action) > -1 &&
			    (Path.Contains("-20") || ("," + Path + ",").Contains("," + ValidatedUser.StartNodeId.ToString() + ",")))
				return true;

			Log.Add(LogTypes.LoginFailure, ValidatedUser, -1,
			        "Insufient permissions in UmbracoEnsuredPage: '" + Path + "', '" + permissions + "', '" + Action + "'");
			return false;
		}

		/// <summary>
		/// Gets the current user.
		/// </summary>
		/// <value>The current user.</value>
		public static User CurrentUser
		{
			get
			{
				if (UmbracoUserContextID != string.Empty)
					return BusinessLogic.User.GetUser(GetUserId(UmbracoUserContextID));
				else
					return BusinessLogic.User.GetUser(0);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			try
			{
				EnsureContext();
			}
			catch (ArgumentException)
			{
				// Some Umbraco pages should not be loaded on timeout, but instead reload the main application in the top window. Like the treeview for instance
				if (RedirectToUmbraco)
					Response.Redirect(GlobalSettings.Path + "/logout.aspx?");
				else
					Response.Redirect(GlobalSettings.Path + "/logout.aspx?redir=" + Server.UrlEncode(Request.RawUrl));
			}

			Thread.CurrentThread.CurrentCulture = new CultureInfo(UIHelper.GetUserCulture(ValidatedUser));
			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
		}
	}
}
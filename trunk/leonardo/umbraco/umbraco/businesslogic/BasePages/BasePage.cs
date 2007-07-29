using System;
using System.Data;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using Umbraco;
using Umbraco.BusinessLogic;

namespace Umbraco.BasePages
{
	/// <summary>
	/// Implements the base class for a web page
	/// </summary>
	public class BasePage : Page
	{
		private User _user;
		private bool _userisValidated = false;

		// ticks per minute 600,000,000 
		private static long _ticksPrMinute = 600000000;
		private static int _umbracoTimeOutInMinutes = GlobalSettings.TimeOutInMinutes;
		protected string UmbracoPath = GlobalSettings.Path;
		protected int uid = 0;
		protected long timeout = 0;

		private bool overrideClientTarget = true;

		/// <summary>
		/// Refreshes the page.
		/// </summary>
		/// <param name="Seconds">The seconds.</param>
		public void RefreshPage(int Seconds)
		{
			ClientScript.RegisterClientScriptBlock(GetType(), "Refresh",
												   "<script>\nsetTimeout('document.location.reload()', " + Seconds * 1000 +
												   ");\n</script>");
		}

		/// <summary>
		/// Validates the user.
		/// </summary>
		private void ValidateUser()
		{
			if ((UmbracoUserContextID != ""))
			{
				uid = GetUserId(UmbracoUserContextID);
				timeout = GetTimeout(UmbracoUserContextID);

				if (timeout > DateTime.Now.Ticks)
				{
					_user = BusinessLogic.User.GetUser(uid);
					_userisValidated = true;
					UpdateLogin();
				}
				else
				{
					throw new ArgumentException("User has timed out!!");
				}
			}
			else
				throw new ArgumentException("The user has no Umbraco contextid - try logging in");
		}

		/// <summary>
		/// Gets the user id.
		/// </summary>
		/// <param name="userContextId">The Umbraco user context ID.</param>
		/// <returns></returns>
		public static int GetUserId(string userContextId)
		{
			try
			{
				// TODO: SQL
				if (HttpRuntime.Cache["UmbracoUserContext" + userContextId] == null)
				{
					HttpRuntime.Cache.Insert(
						"UmbracoUserContext" + userContextId,
						int.Parse(
							Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
								GlobalSettings.DbDSN,
								CommandType.Text,
								"select userID from umbracoUserLogins where contextID = '" +
								SqlHelper.SafeString(userContextId) + "'"
								).ToString()
							),
						null,
						Cache.NoAbsoluteExpiration, new TimeSpan(0, (_umbracoTimeOutInMinutes / 10), 0));
				}

				return (int)HttpRuntime.Cache["UmbracoUserContext" + userContextId];
			}
			catch
			{
				return -1;
			}
		}


		// Added by NH to use with webservices authentications
		/// <summary>
		/// Validates the user context ID.
		/// </summary>
		/// <param name="userContextId">The Umbraco user context ID.</param>
		/// <returns></returns>
		public static bool ValidateUserContextID(string userContextId)
		{
			if ((userContextId != ""))
			{
				int uid = GetUserId(userContextId);
				long timeout = GetTimeout(userContextId);

				if (timeout > DateTime.Now.Ticks)
				{
					return true;
				}
				else
				{
					Log.Add(LogTypes.Logout, BusinessLogic.User.GetUser(uid), -1, "");

					return false;
				}
			}
			else
				return false;
		}

		/// <summary>
		/// Gets the timeout.
		/// </summary>
		/// <param name="userContextId">The Umbraco user context ID.</param>
		/// <returns></returns>
		private static long GetTimeout(string userContextId)
		{
			// TODO: SQL
			if (HttpRuntime.Cache["UmbracoUserContextTimeout" + userContextId] == null)
			{
				HttpRuntime.Cache.Insert(
					"UmbracoUserContextTimeout" + userContextId,
					long.Parse(
						Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
							GlobalSettings.DbDSN,
							CommandType.Text,
							string.Format("select timeout from umbracoUserLogins where contextID = '{0}'",
										  SqlHelper.SafeString(userContextId))
							).ToString()
						),
					null,
					DateTime.Now.AddMinutes(_umbracoTimeOutInMinutes / 10), Cache.NoSlidingExpiration);
			}

			return (long)HttpRuntime.Cache["UmbracoUserContextTimeout" + userContextId];
		}

		// Changed to public by NH to help with webservice authentication
		/// <summary>
		/// Gets or sets the Umbraco user context ID.
		/// </summary>
		/// <value>The Umbraco user context ID.</value>
		public static string UmbracoUserContextID
		{
			get
			{
				if (HttpContext.Current.Request.Cookies.Get("UserContext") != null)
					return HttpContext.Current.Request.Cookies.Get("UserContext").Value;
				else
					return string.Empty;
			}
			set
			{
				if (HttpContext.Current.Request.Cookies["UserContext"] != null)
				{
					HttpContext.Current.Response.Cookies.Clear();
				}
				// Create new cookie.
				HttpCookie c = new HttpCookie("UserContext");
				c.Name = "UserContext";
				c.Value = value;
				c.Expires = DateTime.Now.AddDays(1);
				HttpContext.Current.Response.Cookies.Add(c);
			}
		}


		/// <summary>
		/// Clears the login.
		/// </summary>
		public void ClearLogin()
		{
			UmbracoUserContextID = string.Empty;
		}

		/// <summary>
		/// Updates the login.
		/// </summary>
		private void UpdateLogin()
		{
			if (timeout - (((_ticksPrMinute * _umbracoTimeOutInMinutes) * 0.8)) < DateTime.Now.Ticks)
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(
					GlobalSettings.DbDSN,
					CommandType.Text,
					"update umbracoUserLogins set timeout = '" +
					(DateTime.Now.Ticks + (_ticksPrMinute * _umbracoTimeOutInMinutes)).ToString() +
					"' where contextID = '" + SqlHelper.SafeString(UmbracoUserContextID) + "'"
					);
		}

		/// <summary>
		/// Logins the user.
		/// </summary>
		/// <param name="user">The user.</param>
		public static void LoginUser(User user)
		{
			Guid retVal = Guid.NewGuid();
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
																	   "insert into umbracoUserLogins (contextID, userID, timeout) values ('" +
																	   retVal.ToString() + "','" + user.Id + "','" +
																	   (DateTime.Now.Ticks +
																		(_ticksPrMinute * _umbracoTimeOutInMinutes)).ToString() +
																	   "') ");
			UmbracoUserContextID = retVal.ToString();
			Log.Add(LogTypes.Login, user, -1, "");
		}


		/// <summary>
		/// Gets information about the user making the page request.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="T:System.Security.Principal.IPrincipal"/> that represents the user making the page request.</returns>
		public User ValidatedUser
		{
			get
			{
				if (!_userisValidated)
				{
					ValidateUser();
				}
				return _user;
			}
		}

		public bool OverrideClientTarget
		{
			get { return overrideClientTarget; }
			set { overrideClientTarget = value; }
		}

		/// <summary>
		/// Ensures that the user is valid on the specified context.
		/// </summary>
		public void EnsureContext()
		{
			ValidateUser();
		}

		/// <summary>
		/// Creates a Speech Bubble (a Balloon Tooltip)
		/// </summary>
		/// <param name="icon">The icon.</param>
		/// <param name="header">The header.</param>
		/// <param name="body">The body.</param>
		public void SpeechBubble(SpeechBubbleIcon icon, string header, string body)
		{
			if (Request.QueryString["external"] == null)
				ClientScript.RegisterClientScriptBlock(GetType(), Guid.NewGuid().ToString(),
													   "<script>top.umbSpeechBubble('" + icon.ToString() + "', '" +
													   header.Replace("'", "\\'") + "', '" + body.Replace("'", "\\'") +
													   "');</script>");
		}

		/// <summary>
		/// Reloads the parent node.
		/// </summary>
		public void ReloadParentNode()
		{
			ClientScript.RegisterClientScriptBlock(GetType(), Guid.NewGuid().ToString(),
												   "<script>top.reloadParentNode();</script>");
		}

		/// <summary>
		/// Indicates the type of the Speech Bubble Icon
		/// </summary>
		public enum SpeechBubbleIcon
		{
			/// <summary>
			/// Save Icon
			/// </summary>
			Save,
			/// <summary>
			/// Information icon
			/// </summary>
			Information,
			/// <summary>
			/// Error icon
			/// </summary>
			Error
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (OverrideClientTarget)
				ClientTarget = "uplevel";
		}
	}
}
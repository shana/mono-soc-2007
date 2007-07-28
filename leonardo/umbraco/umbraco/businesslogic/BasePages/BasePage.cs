using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BusinessLogic;

namespace umbraco.BasePages
{
    public class BasePage : System.Web.UI.Page
    {
        private User _user;
        private bool _userisValidated = false;

        // ticks per minute 600,000,000 
        private static long _ticksPrMinute = 600000000;
        private static int _umbracoTimeOutInMinutes = GlobalSettings.TimeOutInMinutes;
        protected string UmbracoPath = GlobalSettings.Path;
        protected int uid = 0;
        protected long timeout = 0;

        public BasePage()
        {
        }


        public void RefreshPage(int Seconds)
        {
            ClientScript.RegisterClientScriptBlock(GetType(), "Refresh",
                                                   "<script>\nsetTimeout('document.location.reload()', " + Seconds*1000 +
                                                   ");\n</script>");
        }

        private void validateUser()
        {
            if ((umbracoUserContextID != ""))
            {
                uid = GetUserId(umbracoUserContextID);
                timeout = GetTimeout(umbracoUserContextID);

                if (timeout > DateTime.Now.Ticks)
                {
                    _user = BusinessLogic.User.GetUser(uid);
                    _userisValidated = true;
                    updateLogin();
                }
                else
                {
                    throw new ArgumentException("User has timed out!!");
                }
            }
            else
                throw new ArgumentException("The user has no umbraco contextid - try logging in");
        }

        public static int GetUserId(string umbracoUserContextID)
        {
            try
            {
                if (System.Web.HttpRuntime.Cache["UmbracoUserContext" + umbracoUserContextID] == null)
                {
                    System.Web.HttpRuntime.Cache.Insert(
                        "UmbracoUserContext" + umbracoUserContextID, 
                        int.Parse(
                        SqlHelper.ExecuteScalar(
                            GlobalSettings.DbDSN,
                            CommandType.Text,
                            "select userID from umbracoUserLogins where contextID = '" +
                            sqlHelper.safeString(umbracoUserContextID) + "'"
                            ).ToString()
                        ),
                        null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration,
    new TimeSpan(0, (int) (_umbracoTimeOutInMinutes / 10), 0));

                    
                }

                return (int) System.Web.HttpRuntime.Cache["UmbracoUserContext" + umbracoUserContextID];

            }
            catch
            {
                return -1;
            }
        }


        // Added by NH to use with webservices authentications
        public static bool ValidateUserContextID(string umbracoUserContextID)
        {
            if ((umbracoUserContextID != ""))
            {
                int uid = GetUserId(umbracoUserContextID);
                long timeout = GetTimeout(umbracoUserContextID);

                if (timeout > DateTime.Now.Ticks)
                {
                    return true;
                }
                else
                {
                    BusinessLogic.Log.Add(BusinessLogic.LogTypes.Logout, BusinessLogic.User.GetUser(uid), -1, "");

                    return false;
                }
            }
            else
                return false;
        }

        private static long GetTimeout(string umbracoUserContextID)
        {
            if (System.Web.HttpRuntime.Cache["UmbracoUserContextTimeout" + umbracoUserContextID] == null)
            {
                System.Web.HttpRuntime.Cache.Insert(
                    "UmbracoUserContextTimeout" + umbracoUserContextID,
                    long.Parse(
                        SqlHelper.ExecuteScalar(
                            GlobalSettings.DbDSN,
                            CommandType.Text,
                            string.Format("select timeout from umbracoUserLogins where contextID = '{0}'",
                                          sqlHelper.safeString(umbracoUserContextID))
                            ).ToString()
                        ),
                    null,
                    DateTime.Now.AddMinutes(_umbracoTimeOutInMinutes/10), System.Web.Caching.Cache.NoSlidingExpiration);


            }

            return (long)System.Web.HttpRuntime.Cache["UmbracoUserContextTimeout" + umbracoUserContextID];

        }

        // Changed to public by NH to help with webservice authentication
        public static string umbracoUserContextID
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.Get("UserContext") != null)
                    return System.Web.HttpContext.Current.Request.Cookies.Get("UserContext").Value;
                else
                    return "";
            }
            set
            {
                // Clearing all old cookies before setting a new one.
                try
                {
                    if (System.Web.HttpContext.Current.Request.Cookies["UserContext"] != null)
                    {
                        System.Web.HttpContext.Current.Response.Cookies.Clear();
                    }
                }
                catch
                {
                }
                // Create new cookie.
                System.Web.HttpCookie c = new System.Web.HttpCookie("UserContext");
                c.Name = "UserContext";
                c.Value = value;
                c.Expires = DateTime.Now.AddDays(1);
                System.Web.HttpContext.Current.Response.Cookies.Add(c);
            }
        }


        public void ClearLogin()
        {
            umbracoUserContextID = "";
        }

        private void updateLogin()
        {
            // only call update if more than 1/10 of the timeout has passed
            if (timeout - (((_ticksPrMinute * _umbracoTimeOutInMinutes) * 0.8)) < DateTime.Now.Ticks)
                SqlHelper.ExecuteNonQuery(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "update umbracoUserLogins set timeout = '" +
                    (DateTime.Now.Ticks + (_ticksPrMinute*_umbracoTimeOutInMinutes)).ToString() +
                    "' where contextID = '" + sqlHelper.safeString(umbracoUserContextID) + "'"
                    );
        }

        public static void doLogin(User u)
        {
            Guid retVal = Guid.NewGuid();
            SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                      "insert into umbracoUserLogins (contextID, userID, timeout) values ('" +
                                      retVal.ToString() + "','" + u.Id + "','" +
                                      (DateTime.Now.Ticks + (_ticksPrMinute*_umbracoTimeOutInMinutes)).ToString() +
                                      "') ");
            umbracoUserContextID = retVal.ToString();
            BusinessLogic.Log.Add(BusinessLogic.LogTypes.Login, u, -1, "");
        }


        public User getUser()
        {
            if (!_userisValidated) validateUser();
            return _user;
        }

        public void ensureContext()
        {
            validateUser();
        }

        public void speechBubble(speechBubbleIcon i, string header, string body)
        {
            if (Request.QueryString["external"] == null)
                ClientScript.RegisterClientScriptBlock(GetType(), Guid.NewGuid().ToString(),
                                                       "<script>top.umbSpeechBubble('" + i.ToString() + "', '" +
                                                       header.Replace("'", "\\'") + "', '" + body.Replace("'", "\\'") +
                                                       "');</script>");
        }

        public void reloadParentNode()
        {
            ClientScript.RegisterClientScriptBlock(GetType(), Guid.NewGuid().ToString(),
                                                   "<script>top.reloadParentNode();</script>");
        }

        public enum speechBubbleIcon
        {
            save,
            info,
            error
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (OverrideClientTarget)
                ClientTarget = "uplevel";
        }

        public bool OverrideClientTarget = true;
    }
}
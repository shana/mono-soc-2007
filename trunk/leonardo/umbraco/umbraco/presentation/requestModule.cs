using System;
using System.ComponentModel;
using System.Threading;
using System.Web;
using umbraco.BusinessLogic;

namespace umbraco.presentation
{
    /// <summary>
    /// Summary description for requestModule.
    /// </summary>
    public class requestModule : IHttpModule
    {
        protected Timer publishingTimer;
        protected Timer pingTimer;

        private HttpApplication mApp;
        private IContainer components = null;

        protected void ApplicationStart(HttpApplication HttpApp)
        {
            try
            {
                Log.Add(LogTypes.System, User.GetUser(0), -1, "Application started at " + DateTime.Now);
            }
            catch
            {
            }

            // Check for configured key, checking for currentversion to ensure that a request with
            // no httpcontext don't set the whole app in configure mode
            if (GlobalSettings.CurrentVersion != null && !GlobalSettings.Configured)
            {
                HttpApp.Application["umbracoNeedConfiguration"] = true;
            }
            else
            {
            }

            /* This section is needed on start-up because timer objects
                 * might initialize before these are initialized without a traditional
                 * request, and therefore lacks information on application paths */

            /* Initialize SECTION END */

            // add current default url
            HttpApp.Application["umbracoUrl"] = string.Format("{0}:{1}{2}",
                                                              HttpApp.Context.Request.ServerVariables["SERVER_NAME"],
                                                              HttpApp.Context.Request.ServerVariables["SERVER_PORT"],
                                                              GlobalSettings.Path);

            // Start ping / keepalive timer
            pingTimer = new Timer(new TimerCallback(keepAliveService.PingUmbraco), HttpApp.Context, 60000, 300000);

            // Start publishingservice
            publishingTimer =
                new Timer(new TimerCallback(publishingService.CheckPublishing), HttpApp.Context, 600000, 60000);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {

            if (GlobalSettings.UseDirectoryUrls)
            {
                HttpContext context = mApp.Context;
                string path = context.Request.Path;
                int asmx = path.IndexOf(".asmx/", StringComparison.OrdinalIgnoreCase);
                if (asmx >= 0)
                    context.RewritePath(
                        path.Substring(0, asmx + 5),
                        path.Substring(asmx + 5),
                        context.Request.QueryString.ToString());

            }


            if (HttpContext.Current.Request.Path.ToLower().IndexOf(".aspx") > -1 ||
                HttpContext.Current.Request.Path.ToLower().IndexOf(".") == -1)
            {
                // Check if path or script is reserved!
                bool urlIsReserved = false;

                // validate configuration
                if (mApp.Application["umbracoNeedConfiguration"] == null)
                    mApp.Application["umbracoNeedConfiguration"] = !GlobalSettings.Configured;

                string[] reservedUrls = GlobalSettings.ReservedUrls.Split(',');
                for (int i = 0; i < reservedUrls.Length; i++)
                    if (reservedUrls[i] != "" &&
                        (HttpContext.Current.Request.Path).ToLower().StartsWith(reservedUrls[i].Trim().ToLower()))
                        urlIsReserved = true;

                string[] reservedPaths = GlobalSettings.ReservedPaths.Split(',');
                for (int i = 0; i < reservedPaths.Length; i++)
                    if ((HttpContext.Current.Request.Path).ToLower().StartsWith(reservedPaths[i].Trim().ToLower()))
                        urlIsReserved = true;

                if (!urlIsReserved)
                {
                    bool test = (bool) mApp.Application["umbracoNeedConfiguration"];
                    if (mApp.Application["umbracoNeedConfiguration"] != null &&
                        (bool) mApp.Application["umbracoNeedConfiguration"])
                        HttpContext.Current.Response.Redirect(
                            string.Format("{0}/../install/default.aspx?redir=true&url={1}",
                                          GlobalSettings.Path, HttpContext.Current.Request.Path.ToLower()), true);
                    else if (UmbracoSettings.EnableSplashWhileLoading && content.Instance.isInitializing)
                        HttpContext.Current.RewritePath(string.Format("{0}/../config/splashes/booting.aspx",
                                                                      GlobalSettings.Path));
                    else
                    {
                        string path = HttpContext.Current.Request.Path;
                        string query = HttpContext.Current.Request.Url.Query;
                        if (query != null && query != "")
                        {
                            // Clean umbPage from querystring, caused by .NET 2.0 default Auth Controls
                            if (query.IndexOf("umbPage") > 0)
                            {
                                query += "&";
                                path = query.Substring(9, query.IndexOf("&") - 9);
                                query = query.Substring(query.IndexOf("&") + 1, query.Length - query.IndexOf("&") - 1);
                            }
                            else if (query.Length > 0)
                                query = query.Substring(1, query.Length - 1);

                            if (query.Length > 0)
                            {
                                HttpContext.Current.Items["VirtualUrl"] = path + "?" + query;
                                HttpContext.Current.RewritePath(string.Format("{0}/../default.aspx?umbPage={1}&{2}",
                                                                              GlobalSettings.Path, path, query));
                            }
                        }
                        if (HttpContext.Current.Items["VirtualUrl"] == null)
                        {
                            HttpContext.Current.Items["VirtualUrl"] = path;
                            HttpContext.Current.RewritePath(string.Format("{0}/../default.aspx?umbPage={1}",
                                                                          GlobalSettings.Path, path));
                        }
                    }
                }
            }
        }


        protected void Application_Error(Object sender, EventArgs e)
        {
            if (GlobalSettings.Configured)
            {
                if (mApp.Context.Request != null)
                    Log.Add(LogTypes.Error, User.GetUser(0), -1,
                            string.Format("At {0} (Refered by: {1}): {2}",
                                          mApp.Context.Request.RawUrl, mApp.Context.Request.UrlReferrer,
                                          mApp.Context.Server.GetLastError().InnerException));
                else
                    Log.Add(LogTypes.Error, User.GetUser(0), -1,
                            "No Context available -> " + mApp.Context.Server.GetLastError().InnerException);
            }
        }

        #region IHttpModule Members

        ///<summary>
        ///Initializes a module and prepares it to handle requests.
        ///</summary>
        ///
        ///<param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application </param>
        public void Init(HttpApplication context)
        {
            InitializeComponent();

            ApplicationStart(context);
            context.BeginRequest += new EventHandler(Application_BeginRequest);
            context.Error += new EventHandler(Application_Error);
            mApp = context;
        }


        private void InitializeComponent()
        {
            components = new Container();
        }

        ///<summary>
        ///Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
        ///</summary>
        ///
        public void Dispose()
        {
        }

        #endregion
    }
}
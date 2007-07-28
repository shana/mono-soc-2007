using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace umbraco
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	/// 
	public partial class WebForm1 : Page
	{
		protected override void Render(HtmlTextWriter output)
		{
			// Get content
			TextWriter tempWriter = new StringWriter();
			base.Render(new HtmlTextWriter(tempWriter));
			string pageContents = tempWriter.ToString();

			pageContents = template.ParseInternalLinks(pageContents);

			// Parse javascript without types
			pageContents =
				pageContents.Replace("<script language=\"javascript\">", "<script language=\"javascript\" type=\"text/javascript\">");

			// Parse form name
			if(HttpContext.Current.Items["VirtualUrl"] != null)
			{
				Regex formReplace = new Regex("action=\"default.aspx([^\"]*)\"");
				if(formReplace.ToString() != "")
					output.Write(
						formReplace.Replace(pageContents, "action=\"" + Convert.ToString(HttpContext.Current.Items["VirtualUrl"]) + "\""));
			}
			else
				base.Render(output);
		}

		public Control pageContent = new Control();

		protected void Page_Load(object sender, EventArgs e)
		{
			if(!String.IsNullOrEmpty(Request["umbDebugShowTrace"]))
			{
				if(GlobalSettings.DebugMode)
				{
					PanelDebug.Visible = true;
				} else
				{
                    Page.Trace.IsEnabled = false;
                    PanelDebug.Visible = false;
				}
			}
			else
				Page.Trace.IsEnabled = false;
		}

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();

            if (!UmbracoSettings.EnableSplashWhileLoading || !content.Instance.isInitializing)
            {
                Trace.Write("umbracoInit", "handling request");

                // we need to replace umbruntime prefix

                Guid version = Guid.Empty;

                string tmp = requestHandler.cleanUrl();

                if (tmp != "")
                {
                    // Check numeric
                    string tryIntParse = tmp.Replace("/", "").Replace(".aspx", string.Empty);
                    int result;
                    if (int.TryParse(tryIntParse, out result))
                    {
                        tmp = tmp.Replace(".aspx", string.Empty);

                        // Check for request
                        if (!string.IsNullOrEmpty(Request["umbVersion"]))
                        {
                            // Security check
                            BasePages.UmbracoEnsuredPage bp = new BasePages.UmbracoEnsuredPage();
                            bp.ensureContext();
                            version = new Guid(Request["umbVersion"]);
                        }
                    }
                }

                page umbPage = null;
                requestHandler umbRequest = null;

                if (version != Guid.Empty)
                {
                    HttpContext.Current.Items["pageID"] = tmp.Replace("/", "");
                    umbPage = new page(int.Parse(tmp.Replace("/", "")), version);
                }
                else
                {
                    umbRequest = new requestHandler(content.Instance.XmlContent, tmp);
                    Trace.Write("umbracoInit", "Done handling request");
                    if (umbRequest.currentPage != null)
                    {
                        HttpContext.Current.Items["pageID"] = umbRequest.currentPage.Attributes.GetNamedItem("id").Value;
                        umbPage = new page(umbRequest.currentPage);
                    }
                }

                // TODO: Find 404 error
                if (umbPage != null)
                {
                    string tempCulture = umbPage.GetCulture();
                    if (tempCulture != "")
                    {
                        System.Web.HttpContext.Current.Trace.Write("default.aspx", "Culture changed to " + tempCulture);
                        System.Threading.Thread.CurrentThread.CurrentCulture =
                            System.Globalization.CultureInfo.CreateSpecificCulture(tempCulture);
                        System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    }

                    umbPage.RenderPage(umbPage.Template);
                    layoutControls.umbracoPageHolder umbPageHolder =
                        (layoutControls.umbracoPageHolder)Page.FindControl("umbPageHolder");
                    umbPageHolder.Populate(umbPage);

                    // Stat
                    if (GlobalSettings.EnableStat)
                        if (Session["umbracoSessionId"] != null)
                        {
                            // If session just has been initialized we should use the cookie from the response object
                            if (Session["umbracoSessionId"] == null)
                                cms.businesslogic.stat.Session.AddEntry(new Guid(Session["umbracoSessionId"].ToString()), umbPage.PageID);
                            else
                                cms.businesslogic.stat.Session.AddEntry(new Guid(Session["umbracoSessionId"].ToString()), umbPage.PageID);
                        }
                }
                else
                {
                    // If there's no published content, show friendly error
                    if (umbraco.content.Instance.XmlContent.SelectSingleNode("/root/node") == null)
                        Response.Redirect("config/splashes/noNodes.htm");
                    else
                    {
                        Response.StatusCode = 404;
                        if (umbRequest != null)
                            HttpContext.Current.Response.Write("No node found (" + Request.Url + ", '" + umbRequest.PageXPathQuery + "')");
                        else
                            HttpContext.Current.Response.Write("No node found (" + Request.Url + ")");
                    }
				}
			}
			else {
                Response.Redirect("config/splashes/booting.aspx?orgUrl=" + Request.Url);
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
	}
}

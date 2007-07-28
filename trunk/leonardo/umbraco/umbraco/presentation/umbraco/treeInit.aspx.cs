using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BasePages;
using umbraco.cms.businesslogic;
using umbraco.interfaces;

namespace umbraco.cms.presentation
{
    /// <summary>
    /// Summary description for TreeInit.
    /// </summary>
    public partial class TreeInit : UmbracoEnsuredPage
    {
        protected PlaceHolder PlaceHolderJavascript;

        protected void Page_Load(object sender, EventArgs e)
        {


            // Initialize the tree
            Random rnd = new Random();
            String appAlias = Request.QueryString["app"];

            string isDialog = "", dialogMode = "";
            if (Request.QueryString["isDialog"] != null)
                isDialog = Request.QueryString["isDialog"];
            if (Request.QueryString["dialogMode"] != null)
                dialogMode = Request.QueryString["dialogMode"];

            if (appAlias == "" || appAlias == null) appAlias = "content";

            // Validate permissions
            if (!ValidateUserApp(appAlias))
                throw new ArgumentException("The current user doesn't have access to this application. Please contact the system administrator.");

            // Special initialize for content and medias
            if (appAlias.ToLower() == "content" || appAlias.ToLower() == "media" || appAlias.ToLower() == "users")
            {
                int startNodeId = -1;
                if (appAlias == "content")
                {
                    appAlias = "Content";
                    startNodeId = getUser().StartNodeId;
                }
                else if (appAlias == "media")
                {
                    startNodeId = getUser().StartMediaId;
                    appAlias = "Media";
                }

                string strTxt = ui.Text("sections", appAlias.ToLower(), base.getUser());

                if (startNodeId > 0)
                    try
                    {
                        strTxt = new CMSNode(startNodeId).Text;
                    }
                    catch
                    {
                        strTxt = ui.Text("errors", "startNodeDoesNotExists", base.getUser());
                    }

                // Check if we're at root
                string link = "";
                string cruds = "";
                if (appAlias.ToLower() == "users")
                {
                    cruds = "C,L";
                }
                else if (startNodeId == -1 && isDialog == "")
                {
                    link = "javascript:parent.openDashboard('" + appAlias + "\');";
                    if (appAlias.ToLower() == "media")
                        cruds = "CDS,L";
                    else
                        cruds = "CDS,B,L";
                }
                else
                {
                    cruds = "CS,L";
                    link = "javascript:open" + appAlias + "(" + startNodeId + ");";
                }

                // Check for disabling context menu
                if (helper.Request("contextMenu") == "false")
                    cruds = "";

                PlaceHolderTree.Controls.Add(
                    new LiteralControl("<script>\n" +
                                       "var tree = new WebFXLoadTree(\"" + strTxt + "\", \"tree.aspx?rnd=" +
                                       Convert.ToString(rnd.Next()) + "&id=" + startNodeId + "&treeType=" +
                                       appAlias.ToLower() + "&contextMenu=" + helper.Request("contextMenu") +
                                       "&isDialog=" + isDialog + "&dialogMode=" + dialogMode + "&app=" + appAlias +
                                       "\", \"\", \"" + link + "\", \"\", \"\", \"\", \"" + appAlias.ToLower() + "\", " +
                                       startNodeId + ", \"" + cruds + "\");\n" +
                                       "document.writeln(tree);\n" +
                                       "</script>"));
            }
            else
            {
                PlaceHolderTree.Controls.Add(
                    new LiteralControl("<script>\n" +
                                       "var tree = new WebFXLoadTree(\"" + ui.Text("sections", appAlias, base.getUser()) +
                                       "\", \"tree.aspx?rnd=" + Convert.ToString(rnd.Next()) + "&contextMenu=" +
                                       helper.Request("contextMenu") + "&app=" + appAlias +
                                       "\", \"\", \"javascript:parent.openDashboard('" + appAlias +
                                       "\');\", \"\", \"\", \"\", \"" + appAlias.ToLower() + "\", 0, \"C,L\");\n" +
                                       "document.writeln(tree);\n" +
                                       "</script>"));
            }

            // Load all javascript for current view
            StringBuilder Javascript = new StringBuilder();
            using (SqlDataReader treeData = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
                                                                    CommandType.Text,
                                                                    "select treeHandlerAssembly, treeHandlerType from umbracoAppTree where appAlias = @appAlias order by treeSortOrder",
                                                                    new SqlParameter("@appAlias", appAlias)))
            {
                while (treeData.Read())
                {
                    String treeAssembly = treeData.GetString(treeData.GetOrdinal("treeHandlerAssembly"));
                    String treeType = treeData.GetString(treeData.GetOrdinal("treeHandlerType"));
                    try
                    {
                        // Create an instance of the type by loading it from the assembly,
                        // and pass the Tree-XML-object as an argument
                        Assembly assembly =
                            Assembly.LoadFrom(Server.MapPath(GlobalSettings.Path + "/../bin/" + treeAssembly + ".dll"));
                        Type type = assembly.GetType(treeAssembly + "." + treeType);
                        if (type == null)
                            continue;
                        ITree typeInstance = Activator.CreateInstance(type) as ITree;
                        if (typeInstance != null)
                        {
                            typeInstance.RenderJS(ref Javascript);
                        }
                        else
                        {
                            Trace.Warn("umbTree",
                                       "Type doesn't exist or is not umbraco.ITree ('" + treeAssembly + "." + treeType +
                                       "')");
                        }
                    }
                    catch (Exception treeException)
                    {
                        Trace.Warn("umbTree", "Error creating type '" + treeAssembly + "." + treeType + "'",
                                   treeException);
                    }
                }
            }

            PlaceHolderJavascript.Controls.Add(
                new LiteralControl(Javascript.ToString())
                );
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.RedirectToUmbraco = true;
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
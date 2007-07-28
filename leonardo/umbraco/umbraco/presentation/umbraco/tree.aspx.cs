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
using System.Xml;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace  umbraco.cms.presentation
{
	/// <summary>
	/// Summary description for tree.
	/// </summary>
	/// 



	public partial class tree : umbraco.BasePages.UmbracoEnsuredPage
	{

		public XmlDocument Tree = new XmlDocument();
		private System.Text.StringBuilder Javascript = new System.Text.StringBuilder();
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Tree.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><tree/>");

			// Put user code to initialize the page here
			if (Request.QueryString["treeType"] == null)
				InitTree(Request.QueryString["app"]);
			else
				LoadTree(Request.QueryString["app"], Request.QueryString["treeType"]);

			Response.Write(Tree.InnerXml);
		}

		private void InitTree(String appAlias) 
		{
			// Load the tree elements from the app
			using (SqlDataReader treeData = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
				CommandType.Text, "select treeSilent, treeAlias, treeTitle, TreeIconClosed, TreeIconOpen, treeHandlerAssembly, treeHandlerType from umbracoAppTree where appAlias = @appAlias and TreeInitialize = 1 order by treeSortOrder", new SqlParameter("@appAlias", appAlias)))
			{
				Random rnd = new Random();
				XmlNode root = Tree.DocumentElement;
				while(treeData.Read())
				{
					string xtraCruds = "";
					string treeCaption = ui.Text("treeHeaders", treeData.GetString(treeData.GetOrdinal("treeAlias")));
					if (treeCaption.Length > 0 && treeCaption.Substring(0, 1) == "[")
						treeCaption = treeData.GetString(treeData.GetOrdinal("treeTitle"));

					if(treeData.GetString(treeData.GetOrdinal("treeAlias")).ToLower() == "macros")
						xtraCruds = ",X";
					else if(treeData.GetString(treeData.GetOrdinal("treeAlias")).ToLower() == "nodetypes")
						xtraCruds = ",8";

					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("menu", "C" + xtraCruds + ",L");
					treeElement.SetAttribute("text", treeCaption);
					treeElement.SetAttribute("action", "");
					treeElement.SetAttribute("src",
						"tree.aspx?rnd=" + Convert.ToString(rnd.Next()) + "&app=" + appAlias + "&treeType=" +
						treeData.GetString(treeData.GetOrdinal("treeAlias")));
					treeElement.SetAttribute("icon", treeData.GetString(treeData.GetOrdinal("TreeIconClosed")));
					treeElement.SetAttribute("openIcon", treeData.GetString(treeData.GetOrdinal("TreeIconOpen")));
					treeElement.SetAttribute("nodeType", "init" + treeData.GetString(treeData.GetOrdinal("treeAlias")));
					treeElement.SetAttribute("nodeID", "init");
					root.AppendChild(treeElement);
				}
			}
		}


		private void LoadTree(String appAlias, String treeAlias) 
		{
			// Load info on the treeType
			using (SqlDataReader treeData = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
				CommandType.Text, "select treeSilent, treeAlias, treeTitle, TreeIconClosed, TreeIconOpen, treeHandlerAssembly, treeHandlerType from umbracoAppTree where appAlias = @appAlias and treeAlias = @treeAlias", new SqlParameter("@appAlias", appAlias), new SqlParameter("@treeAlias", treeAlias)))
			{
				String treeAssembly = "";
				String treeType = "";
				if(treeData.Read())
				{
					treeAssembly = treeData.GetString(treeData.GetOrdinal("treeHandlerAssembly"));
					treeType = treeData.GetString(treeData.GetOrdinal("treeHandlerType"));
					try
					{
						// Create an instance of the type by loading it from the assembly,
						// and pass the Tree-XML-object as an argument
						Assembly assembly = Assembly.LoadFrom(Server.MapPath(GlobalSettings.Path + "/../bin/" + treeAssembly + ".dll"));
						//					object[] treeObject = new object [] {(object) Tree, (object) Request.QueryString["id"]};
						Type type = assembly.GetType(treeAssembly + "." + treeType);
						interfaces.ITree typeInstance = Activator.CreateInstance(type) as interfaces.ITree;
						if(typeInstance != null)
						{
							typeInstance.id = Convert.ToInt32(Request.QueryString["id"]);
							typeInstance.app = appAlias;
							typeInstance.Render(ref Tree);
						}
						else
						{
							Trace.Warn("umbTree", "Type doesn't exist or is not umbraco.ITree ('" + treeAssembly + "." + treeType + "')");
						}
					}
					catch(Exception e)
					{
						string wholeExcp = "";
						while(e.InnerException != null)
						{
							wholeExcp += "--------------------------------\n";
							wholeExcp += e.InnerException.ToString() + ", \n";
							e = e.InnerException;
						}
						Trace.Warn("umbTree", "Error creating type '" + treeAssembly + "." + treeType + "': " + wholeExcp, e);
					}
				}
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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using umbraco.cms.businesslogic.template;
using System.Xml.Xsl;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

using umbraco.cms.businesslogic.web;
using umbraco.presentation.nodeFactory;

namespace umbraco.cms.presentation
{
	/// <summary>
	/// Summary description for test.
	/// </summary>
	public partial class test : BasePages.UmbracoEnsuredPage
	{

		private ArrayList permissions = new ArrayList();

		private static void generateXmlDocument(int parentId, System.Xml.XmlNode parentNode) 
		{
			if (parents.ContainsKey(parentId)) 
			{
				ArrayList children = (ArrayList) parents[parentId];
				foreach (int i in children) 
				{
					System.Xml.XmlNode childNode = (System.Xml.XmlNode) nodes[i];
					parentNode.AppendChild(childNode);
					generateXmlDocument(i, childNode);
				}
			}
		}

		private static Hashtable nodes = new Hashtable();
		private static Hashtable parents = new Hashtable();

		private void appendDocumentType(int Id, Hashtable Existing, XmlDocument xd, ref XmlElement root) 
		{
			DocumentType dt = new DocumentType(Id);
			if (!Existing.ContainsKey(Id)) 
			{
				root.AppendChild(dt.ToXml(xd));
				Existing.Add(Id, "");
				foreach (int i in dt.AllowedChildContentTypeIDs)
					appendDocumentType(i, Existing, xd, ref root);
			}

		}

		protected void Page_Load(object sender, System.EventArgs e) 
		{
		    businesslogic.Dictionary.DictionaryItem item = new umbraco.cms.businesslogic.Dictionary.DictionaryItem(int.Parse(umbraco.helper.Request("id")));
            if (!item.IsTopMostItem())
		        Response.Write("*" + item.Parent.key + "*");
            else 
                Response.Write("Item doesn't have a parent...");

            // TEsting document/node exception
            //new Document(1048);
            /*
            // testing translate
            cms.businesslogic.translation.Translation.MakeNew(
                new umbraco.cms.businesslogic.CMSNode(1060),
                new umbraco.BusinessLogic.User(0),
                new umbraco.BusinessLogic.User(2),
                new umbraco.cms.businesslogic.language.Language(2),
                "Test translation task", false, true);
            */
            // testing tasks

            // creation
            /*
            cms.businesslogic.task.Task t = new cms.businesslogic.task.Task();
            t.Comment = "This is just a test";
            t.Node = new umbraco.cms.businesslogic.CMSNode(1060);
            t.ParentUser = new umbraco.BusinessLogic.User(0);
            t.User = new umbraco.BusinessLogic.User(2);
            t.Type = new umbraco.cms.businesslogic.task.TaskType(1);
            t.Save();
             * 
             * 

            // update
            cms.businesslogic.task.Task t = new umbraco.cms.businesslogic.task.Task(5);
            t.Node = new umbraco.cms.businesslogic.CMSNode(1061);
            t.Save();
            */


            //            cms.businesslogic.translation.Translation.MakeNew(new umbraco.cms.businesslogic.CMSNode(1052), getUser(), new umbraco.BusinessLogic.User(2), new umbraco.cms.businesslogic.language.Language(2), "Testing...");
//			Response.ContentType = "text/xml";
/*
            cms.businesslogic.macro.Packager p = new umbraco.cms.businesslogic.macro.Packager();
            try
            {
                p.Fetch(new Guid(umbraco.library.Request("guid")));
            }
            catch (Exception ee)
            {
                Response.Write(ee.ToString());
            }
*/
/*			// Testing xml script of document types
			Hashtable ht = new Hashtable();
			XmlDocument xd = new XmlDocument();
			XmlElement root = xd.CreateElement("DocumentTypes");
			DocumentType dt = DocumentType.GetByAlias(helper.Request("alias"));
			appendDocumentType(dt.Id, ht, xd, ref root);
			Response.Write(root.OuterXml);

			// Testing xml script of templates
			ht = new Hashtable();
			root = xd.CreateElement("Templates");
			foreach(cms.businesslogic.template.Template t in cms.businesslogic.template.Template.getAll())
				root.AppendChild(t.ToXml(xd));
			Response.Write(root.OuterXml);

			// Testing xml script of stylesheets
			Hashtable ht = new Hashtable();
			XmlDocument xd = new XmlDocument();
			XmlElement root = xd.CreateElement("Stylesheets");
			foreach(cms.businesslogic.web.StyleSheet s in cms.businesslogic.web.StyleSheet.GetAll())
				root.AppendChild(s.ToXml(xd));
			Response.Write(root.OuterXml);

*/




//			cms.businesslogic.index.Indexer.ReIndex();

/*			Server.ScriptTimeout = 10000;
			XmlDocument xd = new XmlDocument();
			SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, 
				"select nodeId from cmsdocument where published = 1");
			while (dr.Read())
				new Document(int.Parse(dr["nodeId"].ToString())).XmlGenerate(xd);
			dr.Close();
*/
//			Response.Write (SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select datantext from cmsPropertyData inner join cmsPropertyType on propertytypeId = cmsPropertyType.id where versionID = @versionId and alias = @alias", new SqlParameter("@versionId", new Guid(helper.Request("versionId"))), new SqlParameter("@alias", helper.Request("alias"))).ToString());

//			foreach(cms.businesslogic.datatype.DataType dt in cms.businesslogic.datatype.DataType.GetAll())
//				types.Items.Add(new ListItem(dt.Text, dt.Id.ToString()));

//			tab.Items.Add(new ListItem("Forum contents", "test"));

			
//			foreach(cms.businesslogic.member.Member m in cms.businesslogic.member.Member.getMemberFromFirstLetter("a".ToCharArray()[0]))
//				Response.Write (m.Text + "<br/>");

/*			
			XmlDocument xd = new XmlDocument();


			foreach(Guid g in cms.businesslogic.web.Document.getAllUniquesFromObjectType(new Guid("c66ba18e-eaf3-4cff-8a22-41b16d66a972"))) 
			{
				cms.businesslogic.web.Document d = new cms.businesslogic.web.Document(g);
				if (d.Published) 
				{
					try 
					{
						Response.Write("Rendering xml for " + d.Text + "<br/>");
						d.ToXml(xd, false);
					} 
					catch (Exception de)
					{
						Response.Write("Error in (" + d.Id.ToString() + ", " + d.Text + ") <br/>");
					}
				}
			}
*/

/*			XmlDocument xd = new XmlDocument();
			SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select nodeId from cmsContent where contentType = -100117");
			while (dr.Read())

			{
				try 
				{
					cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(int.Parse(dr["nodeId"].ToString()));
					d.ToXml(xd, true);
					Response.Write(d.Text + "<br/>");
					Response.Flush();
				} 
				catch (Exception ee) {
					Response.Write("<span class=\"red\">Error: " + ee.ToString() + "</span><br/>");
				}
			}
*/			/*SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select top 300 * from umbracoLog order by id desc");
			DataGrid1.DataSource = dr;
			DataGrid1.DataBind();
			dr.Close();
*/
			
			// New xml storage test
//			int id = int.Parse(helper.Request("id"));
//			cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(id);
//			d.ToXml(new XmlDocument(), true);

//			Response.Write ("</ul>");

//			Response.Write(Server.HtmlEncode(d.ToXml(new XmlDocument(), false).OuterXml));

			// Gentle test
/*			BusinessLogic.GentleLog g = new BusinessLogic.GentleLog(1);
			Response.Write("*" + g.Header + "*");
			g.Header = "Opdateret via gentle med new statement...";
			g.Persist();
			BusinessLogic.GentleLog g2 = new BusinessLogic.GentleLog(BusinessLogic.LogTypes.Debug, "Niels tester via gentle...");
*/
			/*			BusinessLogic.GentleLog g = BusinessLogic.GentleLog.Retrieve(1);
			Response.Write("*" + g.Header + "*");
			g.Header = "Gentle testing...";
			g.Persist();

			// Gentle - make new
			BusinessLogic.GentleLog g2 = new BusinessLogic.GentleLog(BusinessLogic.LogTypes.Debug, "Niels tester via gentle...");
			g2.Persist();
			Response.Write ("*" + g2.Header + "*");
			g2.LogComment = "Her kan man skrive alt muligt...";
			g2.NodeId = 1000;
			g2.Persist();
*/
//			cms.businesslogic.relation.Relation.MakeNew(1068, 1058, new cms.businesslogic.relation.RelationType(1), "");
//			cms.businesslogic.relation.Relation.MakeNew(1048, 1068, new cms.businesslogic.relation.RelationType(2), "");
			//Response.Write(Server.HtmlEncode(((IHasXmlNode) library.GetRelatedNodesAsXml(1068).Current).GetNode().OuterXml));
			
/*			foreach (cms.businesslogic.relation.Relation r in cms.businesslogic.relation.Relation.GetRelations(1068)) 
			{
				Response.Write(r.Parent.Text + " -> " + r.Child.Text + "<br/>");
			}
*/

			//			Response.Write (bool.Parse(GlobalSettings.EditXhtmlMode).ToString());

//			Response.Write(new BusinessLogic.User(2).GetPermissions(1054) + "<br/>");
//			Response.Write(new BusinessLogic.User(2).GetPermissions(1055) + "<br/>");
//			Response.Write(new BusinessLogic.User(2).GetPermissions(1056) + "<br/>");

//			BusinessLogic.Permission.UpdateCruds(new BusinessLogic.User(2), new cms.businesslogic.CMSNode(1054), "-");
//			BusinessLogic.Permission.UpdateCruds(new BusinessLogic.User(2), new cms.businesslogic.CMSNode(1058), "CD");
//			BusinessLogic.Permission.UpdateCruds(new BusinessLogic.User(2), new cms.businesslogic.CMSNode(1059), "C");

//			cms.businesslogic.member.MemberGroup mg = cms.businesslogic.member.MemberGroup.MakeNew("test", new BusinessLogic.User(0));
//			cms.businesslogic.web.Access.ProtectPage(1091, 1203, 1203);
//			cms.businesslogic.web.Access.AddMemberGroupToDocument(1091, mg.Id);

			//umbGroup.InstallUmbGroup i = new umbGroup.InstallUmbGroup();

			
//			cms.businesslogic.member.MemberGroup.MakeNew("profesionelle", new BusinessLogic.User(0));
//			cms.businesslogic.web.Access.ProtectPage(1053, 1079, 1079);
//			cms.businesslogic.web.Access.AddMemberGroupToDocument(1053, 1081);
//			foreach (cms.businesslogic.web.Document d in new cms.businesslogic.web.Document(1066).Children)
//				for (int i=0;i<300;i++) 
//					cms.businesslogic.web.Document.MakeNew("Ajax demo " + i.ToString(), new cms.businesslogic.web.DocumentType(1065), new BusinessLogic.User(0), d.Id);
//			if((ReleaseExpireTimer.checkContent == null))
//			else
//				Response.Write("no");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);


			/*

			HtmlTable ht = new HtmlTable();
			ht.CellPadding = 4;

			HtmlTableRow captions = new HtmlTableRow();
			captions.Cells.Add(new HtmlTableCell());

			foreach (main.IAction a in BusinessLogic.Actions.Action.GetAll()) 
			{
				if (a.CanBePermissionAssigned) 
				{
					HtmlTableCell hc = new HtmlTableCell();
					hc.Attributes.Add("class", "guiDialogTinyMark");
					hc.Controls.Add(new LiteralControl(ui.Text("actions", a.Alias)));
					captions.Cells.Add(hc);
				}
			}
			ht.Rows.Add(captions);

			foreach (BusinessLogic.User u in BusinessLogic.User.getAll()) 
			{
				if (!u.Disabled) 
				{
					HtmlTableRow hr = new HtmlTableRow();

					HtmlTableCell hc = new HtmlTableCell();
					hc.Attributes.Add("class", "guiDialogTinyMark");
					hc.Controls.Add(new LiteralControl(u.Name));
					hr.Cells.Add(hc);

					foreach (main.IAction a in BusinessLogic.Actions.Action.GetAll()) 
					{
						CheckBox c = new CheckBox();
						c.ID = u.Id + "_" + a.Letter;
						if (a.CanBePermissionAssigned) 
						{
							if (u.GetPermissions(-1).IndexOf(a.Letter) > -1)
								c.Checked = true;
							HtmlTableCell cell = new HtmlTableCell();
							cell.Style.Add("text-align", "center");
							cell.Controls.Add(c);
							permissions.Add(c);
							hr.Cells.Add(cell);
						}
							
					}
					ht.Rows.Add(hr);
				}
			}
			PlaceHolder1.Controls.Add(ht);
*/
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
/*
		private void Button1_Click(object sender, System.EventArgs e)
		{
			// First off - load all users
			Hashtable allUsers = new Hashtable();
			foreach (BusinessLogic.User u in BusinessLogic.User.getAll()) 
				if (!u.Disabled)
					allUsers.Add(u.Id, "");

			cms.businesslogic.CMSNode node = new cms.businesslogic.CMSNode(int.Parse(TextBox1.Text));
			foreach (CheckBox c in permissions) 
			{
				// Update the user with the new permission
				if (c.Checked)
					allUsers[int.Parse(c.ID.Substring(0,c.ID.IndexOf("_")))] = (string) allUsers[int.Parse(c.ID.Substring(0,c.ID.IndexOf("_")))] + c.ID.Substring(c.ID.IndexOf("_")+1, c.ID.Length-c.ID.IndexOf("_")-1);
			}


			// Loop through the users and update their Cruds...
			IDictionaryEnumerator uEnum = allUsers.GetEnumerator();
			while (uEnum.MoveNext()) 
			{
				string cruds = "-";
				if (uEnum.Value.ToString().Trim() != "")
					cruds = uEnum.Value.ToString();

				BusinessLogic.Permission.UpdateCruds(new BusinessLogic.User(int.Parse(uEnum.Key.ToString())), node, cruds);
			}
		}

	*/
	}
}


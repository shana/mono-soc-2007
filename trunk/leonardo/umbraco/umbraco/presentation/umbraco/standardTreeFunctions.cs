using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

using Microsoft.ApplicationBlocks.Data;

using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.cache;
using umbraco.cms.businesslogic.contentitem;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.language;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;

using Template=umbraco.cms.businesslogic.template.Template;

namespace umbraco
{

	#region content and media

	public class loadContent : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			if(HttpContext.Current.Request.QueryString["functionToCall"] != null)
			{
				Javascript.Append("function openContent(id) {\n");
				Javascript.Append(HttpContext.Current.Request.QueryString["functionToCall"] + "(id)\n");
				Javascript.Append("}\n");
			}
			else if(HttpContext.Current.Request.QueryString["isDialog"] == null)
			{
				Javascript.Append(
					@"
function openContent(id) {
	parent.right.document.location.href = 'editContent.aspx?id=' + id;
}
");
			}
			else
			{
				Javascript.Append(
					@"
function openContent(id) {
	if (parent.opener)
		parent.opener.dialogHandler(id);
	else
		parent.dialogHandler(id);	
}

");
			}
		}

		public void Render(ref XmlDocument Tree)
		{
			// CRUDS that is used in this rendering
			string useCruds = "CD,MO,SK,UIR,P,Z,T5";

			// Get user and check if its administrator og editor
			UmbracoEnsuredPage bp = new UmbracoEnsuredPage();
			User u = bp.getUser();
			//string userTypeName = u.UserType.Name.ToLower();
			//string defaultCruds = u.UserType.DefaultPermissions;

			// Check if we're used for at general dialog purpose
			string isDialogMode = "";
			string dialogMode = "";
			string hideContextMenu;
			bool isRecycleBin = false;
			if(!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["isRecycleBin"]))
				isRecycleBin = bool.Parse(HttpContext.Current.Request.QueryString["isRecycleBin"]);

			hideContextMenu = helper.Request("contextMenu");

			if(HttpContext.Current.Request.QueryString["isDialog"] != null)
				if(HttpContext.Current.Request.QueryString["isDialog"] != "")
				{
					isDialogMode = "true";
					if(HttpContext.Current.Request.QueryString["dialogMode"] != null)
						dialogMode = HttpContext.Current.Request.QueryString["dialogMode"];
				}
			//library lib = new library();

			Document[] docs;

			if(_id == -1)
			{
				docs = Document.GetRootDocuments();
			}
			else
			{
				docs = Document.GetChildrenForTree(_id);
				//				docs = new cms.businesslogic.web.Document(_id).Children;
			}

			XmlNode root = Tree.DocumentElement;
			foreach(Document dd in docs)
			{
				// CRUDS...
				string cruds = "";
				string allCruds = u.GetPermissions(dd.Path);

				// Check for update/view rights
				if(allCruds.IndexOf("A") > -1)
				{
					foreach(char c in useCruds.ToCharArray())
						if(allCruds.IndexOf(c) > -1 || (c.ToString() == "," && !cruds.EndsWith(",")))
							cruds += c.ToString();

					// A user is allowed to delete their own stuff
					if(dd.UserId == u.Id && cruds.IndexOf("D") == -1)
						cruds += "D";

					XmlElement treeElement = Tree.CreateElement("tree");

					treeElement.SetAttribute("nodeID", dd.Id.ToString());
					treeElement.SetAttribute("text", dd.Text);
					if(!dd.Published || isRecycleBin) treeElement.SetAttribute("iconClass", "umbraco-tree-icon-grey");

					if(dd.Published)
					{
						if(Math.Round(new TimeSpan(dd.UpdateDate.Ticks - dd.VersionDate.Ticks).TotalSeconds, 0) > 1)
							treeElement.SetAttribute("notPublished", "true");
								//dd.VersionDate.ToString() + ", " + dd.UpdateDate.ToString() + " (" + library.DateDiff(dd.VersionDate, dd.UpdateDate, "s").ToString() + ")");
					}

					if(Access.IsProtected(dd.Id, dd.Path))
						treeElement.SetAttribute("isProtected", "true");

					// Check for dialog behaviour
					if(isDialogMode == "" || dialogMode == "id")
						treeElement.SetAttribute("action", string.Format("javascript:openContent({0});", dd.Id));
					else
					{
						string nodeLink = library.NiceUrl(dd.Id);
						if(nodeLink == "")
						{
							nodeLink = "/" + dd.Id;
							if(!GlobalSettings.UseDirectoryUrls)
								nodeLink += ".aspx";
						}

						// 2.1.1 - render urls on the fly
						nodeLink = string.Format("{{localLink:{0}}}", dd.Id);
						//						nodeLink = "{#" + dd.Id.ToString() + "," + nodeLink + "}";

						treeElement.SetAttribute("action", string.Format("javascript:openContent('{0}');", nodeLink));
					}

					// special cruds for recycle bin
					if(isRecycleBin)
						cruds = "M,D";

					if(dd.HasChildren)
					{
						if(hideContextMenu == "")
							treeElement.SetAttribute("menu", cruds + ",L,Q");
						else
							treeElement.SetAttribute("menu", "");

						treeElement.SetAttribute("src",
							"tree.aspx?contextMenu=" + hideContextMenu + "&isDialog=" + isDialogMode + "&dialogMode=" + dialogMode + "&app=" +
							_app + "&id=" + dd.Id + "&treeType=" + HttpContext.Current.Request.QueryString["treeType"] +
							"&isRecycleBin=" + isRecycleBin + "&rnd=" + Guid.NewGuid());
					}
					else
					{
						if(hideContextMenu == "")
							treeElement.SetAttribute("menu", cruds.Replace("S,", "") + ",L,Q");
						else
							treeElement.SetAttribute("menu", "");
						treeElement.SetAttribute("rootSrc",
							"tree.aspx?app=" + _app + "&id=" + dd.Id + "&treeType=content&rnd=" + Guid.NewGuid());
						treeElement.SetAttribute("src", "");
					}
					if (dd.ContentTypeIcon != null)
					{
						treeElement.SetAttribute("icon", dd.ContentTypeIcon);
						treeElement.SetAttribute("openIcon", dd.ContentTypeIcon);
					}

					treeElement.SetAttribute("nodeType", "content");
					root.AppendChild(treeElement);
				}
			}

			if(_id == -1 && isDialogMode.ToLower() != "true")
			{
				// Add Recyle Bin
				XmlElement treeElement = Tree.CreateElement("tree");

				treeElement.SetAttribute("nodeID", "-20");
				treeElement.SetAttribute("text", ui.Text("recycleBin"));

				if(new RecycleBin(Document._objectType).Smells())
				{
					treeElement.SetAttribute("icon", "../tree/bin.png");
					treeElement.SetAttribute("openIcon", "../tree/bin.png");
					treeElement.SetAttribute("menu", "N,L");
					treeElement.SetAttribute("src",
						"tree.aspx?isRecycleBin=true&app=" + _app + "&id=-20&treeType=" +
						HttpContext.Current.Request.QueryString["treeType"] + "&rnd=" + Guid.NewGuid());
				}
				else
				{
					treeElement.SetAttribute("icon", "../tree/bin_empty.png");
					treeElement.SetAttribute("openIcon", "../tree/bin.png");
				}

				treeElement.SetAttribute("nodeType", "recyleBin");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadMedia : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			if(HttpContext.Current.Request.QueryString["functionToCall"] != null)
			{
				Javascript.Append("function openMedia(id) {\n");
				Javascript.Append(HttpContext.Current.Request.QueryString["functionToCall"] + "(id)\n");
				Javascript.Append("}\n");
			}
			else if(HttpContext.Current.Request.QueryString["isDialog"] == null)
			{
				Javascript.Append(
					@"
function openMedia(id) {
	parent.right.document.location.href = 'editMedia.aspx?id=' + id;
}
");
			}
			else
			{
				Javascript.Append(
					@"
function openMedia(id) {
	if (parent.opener)
		parent.opener.dialogHandler(id);
	else
		parent.dialogHandler(id);	
}
");
			}
		}

		public void Render(ref XmlDocument Tree)
		{
			// Check if we're used for at general dialog purpose
			string isDialogMode = "";
			string dialogMode = "";
			string hideContextMenu;

			hideContextMenu = helper.Request("contextMenu");

			if(HttpContext.Current.Request.QueryString["isDialog"] != null)
				if(HttpContext.Current.Request.QueryString["isDialog"] != "")
				{
					isDialogMode = "true";
					if(HttpContext.Current.Request.QueryString["dialogMode"] != null)
						dialogMode = HttpContext.Current.Request.QueryString["dialogMode"];
				}
			//library lib = new library();

			Media[] docs;

			if(_id == -1)
			{
				docs = Media.GetRootMedias();
			}
			else
			{
				docs = new Media(_id).Children;
			}

			XmlNode root = Tree.DocumentElement;
			foreach(Media dd in docs)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", dd.Id.ToString());
				treeElement.SetAttribute("text", dd.Text);

				// Check for dialog behaviour
				if((isDialogMode == "" || dialogMode == "id"))
				{
					if(hideContextMenu == "")
						treeElement.SetAttribute("menu", "CMDS,L,Q");
					else
						treeElement.SetAttribute("menu", "");
					treeElement.SetAttribute("action", "javascript:openMedia(" + dd.Id + ");");
				}
				else
				{
					if(hideContextMenu == "")
						treeElement.SetAttribute("menu", "L,Q");
					else
						treeElement.SetAttribute("menu", "");

					Guid uploadGuid = new Guid("5032a6e6-69e3-491d-bb28-cd31cd11086c");
					string nodeLink = dd.Id.ToString();

					foreach(Property p in dd.getProperties)
					{
						if(p.PropertyType.DataTypeDefinition.DataType.Id == uploadGuid && p.Value.ToString() != "")
						{
							nodeLink = p.Value.ToString();
							break;
						}
					}

					treeElement.SetAttribute("action", "javascript:openMedia('" + nodeLink + "');");
				}
				if(dd.HasChildren)
				{
					treeElement.SetAttribute("src",
						"tree.aspx?isDialog=" + isDialogMode + "&dialogMode=" + dialogMode + "&app=" + _app + "&id=" + dd.Id +
						"&treeType=" + HttpContext.Current.Request.QueryString["treeType"] + "&contextMenu=" + hideContextMenu + "&rnd=" +
						Guid.NewGuid());
				}
				else
				{
					treeElement.SetAttribute("rootSrc",
						"tree.aspx?app=" + _app + "&id=" + dd.Id + "&treeType=media" + "&contextMenu=" + hideContextMenu +
						"&rnd=" + Guid.NewGuid());
					treeElement.SetAttribute("src", "");
				}

				if (dd.ContentType != null)
				{
					treeElement.SetAttribute("icon", dd.ContentType.IconUrl);
					treeElement.SetAttribute("openIcon", dd.ContentType.IconUrl);
				}

				treeElement.SetAttribute("nodeType", "media");
				root.AppendChild(treeElement);
			}
		}
	}

	#endregion content and media

	#region settings

	public class loadTemplates : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openTemplate(id) {
	parent.right.document.location.href = 'settings/editTemplate.aspx?templateID=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			foreach(Template t in Template.getAll())
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", t.Id.ToString());
				treeElement.SetAttribute("text", t.Text);
				treeElement.SetAttribute("action", "javascript:openTemplate(" + t.Id + ");");
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingTemplate.gif");
				treeElement.SetAttribute("openIcon", "settingTemplate.gif");
				treeElement.SetAttribute("nodeType", "templates");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadNodeTypes : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openNodeType(id) {
	parent.right.document.location.href = 'settings/editNodeTypeNew.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			foreach(DocumentType dt in DocumentType.GetAll)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", dt.Id.ToString());
				treeElement.SetAttribute("text", dt.Text);
				treeElement.SetAttribute("menu", "9,D");
				treeElement.SetAttribute("action", "javascript:openNodeType(" + dt.Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingDataType.gif");
				treeElement.SetAttribute("openIcon", "settingDataType.gif");
				treeElement.SetAttribute("nodeType", "nodeType");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadMediaTypes : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openMediaType(id) {
	parent.right.document.location.href = 'settings/editMediaType.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			foreach(MediaType dt in MediaType.GetAll)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", dt.Id.ToString());
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("text", dt.Text);
				treeElement.SetAttribute("action", string.Format("javascript:openMediaType({0});", dt.Id));
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingDataType.gif");
				treeElement.SetAttribute("openIcon", "settingDataType.gif");
				treeElement.SetAttribute("nodeType", "mediaType");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadStylesheets : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
			function openStylesheet(id) {
				parent.right.document.location.href = 'settings/stylesheet/editStylesheet.aspx?id=' + id;
			}
			");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;
			foreach(StyleSheet n in StyleSheet.GetAll())
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("menu", "CDS,L");
				treeElement.SetAttribute("nodeID", n.Id.ToString());
				treeElement.SetAttribute("text", n.Text);
				treeElement.SetAttribute("action", "javascript:openStylesheet(" + n.Id + ");");
				if(n.HasChildren)
					treeElement.SetAttribute("src",
						"tree.aspx?app=" + _app + "&id=" + n.Id + "&treeType=stylesheetProperty" + "&rnd=" +
						Guid.NewGuid());
				else
					treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingCss.gif");
				treeElement.SetAttribute("openIcon", "settingCss.gif");
				treeElement.SetAttribute("nodeType", "stylesheet");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadStylesheetProperty : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
			function openStylesheetProperty(id) {
				parent.right.document.location.href = 'settings/stylesheet/property/editStylesheetProperty.aspx?id=' + id;
			}
			");
		}

		public void Render(ref XmlDocument Tree)
		{
			StyleSheet sn = new StyleSheet(_id);
			XmlNode root = Tree.DocumentElement;

			foreach(StylesheetProperty n in sn.Properties)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", n.Id.ToString());
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("text", n.Text);
				treeElement.SetAttribute("action", "javascript:openStylesheetProperty(" + n.Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingCssItem.gif");
				treeElement.SetAttribute("openIcon", "settingCssItem.gif");
				treeElement.SetAttribute("nodeType", "stylesheetProperty");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadScripts : ITree
	{
		#region TreeI Members

		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
			function openScriptEditor(id) {
			parent.right.document.location.href = 'settings/scripts/editScript.aspx?file=' + id;
			}
		");
		}

		public void Render(ref XmlDocument Tree)
		{
			String orgPath = "";
			String path;
			if(HttpContext.Current.Request.QueryString["folder"] != null)
			{
				orgPath = HttpContext.Current.Request.QueryString["folder"];
				path = HttpContext.Current.Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + orgPath + "/");
				//orgPath += "/";
			}
			else
				path = HttpContext.Current.Server.MapPath(UmbracoSettings.ScriptFolderPath + "/");

			XmlNode root = Tree.DocumentElement;

			DirectoryInfo dirInfo = new DirectoryInfo(path);
			DirectoryInfo[] dirInfos = dirInfo.GetDirectories();
			foreach(DirectoryInfo dir in dirInfos)
			{
				if((dir.Attributes & FileAttributes.Hidden) == 0)
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", dir.FullName);
					treeElement.SetAttribute("text", dir.Name);
					treeElement.SetAttribute("action", "");
					treeElement.SetAttribute("menu", "D,C,L");
					treeElement.SetAttribute("src",
						string.Format("tree.aspx?app={0}&id=1&folder={1}&treeType=scripts&rnd={2}", this._app, dir.Name, Guid.NewGuid()));
					treeElement.SetAttribute("icon", "folder.gif");
					treeElement.SetAttribute("openIcon", "folder_o.gif");
					treeElement.SetAttribute("nodeType", "scriptsFolder");
					root.AppendChild(treeElement);
				}
			}

			FileInfo[] fileInfo = dirInfo.GetFiles("*.*");
			string fileTypes = UmbracoSettings.ScriptFileTypes;

			foreach(FileInfo file in fileInfo)
			{
				if((file.Attributes & FileAttributes.Hidden) == 0 && fileTypes.Contains(file.Extension.Trim('.')))
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", file.FullName);
					treeElement.SetAttribute("text", file.Name);
					if(orgPath != "")
						treeElement.SetAttribute("action", "javascript:openScriptEditor('" + dirInfo.Name + "/" + file.Name + "');");
					else
						treeElement.SetAttribute("action", "javascript:openScriptEditor('" + file.Name + "');");
					treeElement.SetAttribute("src", "");
					treeElement.SetAttribute("menu", "D");
					treeElement.SetAttribute("icon", "developerXslt.gif");
					treeElement.SetAttribute("openIcon", "developerXslt.gif");
					treeElement.SetAttribute("nodeType", "scripts");
					root.AppendChild(treeElement);
				}
			}
		}
	}

	#endregion

	public class loadDictionary : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
			function openDictionaryItem(id) {
				parent.right.document.location.href = 'settings/editDictionaryItem.aspx?id=' + id;
			}
			");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;
			Dictionary.DictionaryItem[] tmp;
			if(HttpContext.Current.Request.QueryString["parentkey"] == null)
				tmp = Dictionary.getTopMostItems;
			else
				tmp = new Dictionary.DictionaryItem(int.Parse(HttpContext.Current.Request.QueryString["parentkey"])).Children;

			foreach(Dictionary.DictionaryItem di in tmp)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", di.id.ToString());
				treeElement.SetAttribute("menu", "CD,L");
				treeElement.SetAttribute("text", di.key);
				treeElement.SetAttribute("action", string.Format("javascript:openDictionaryItem({0});", di.id));
				if(di.hasChildren)
					treeElement.SetAttribute("src",
						"tree.aspx?parentkey=" + di.id + "&app=" + _app + "&treeType=" +
						HttpContext.Current.Request.QueryString["treeType"] + "&rnd=" + Guid.NewGuid());
				treeElement.SetAttribute("icon", "settingDataType.gif");
				treeElement.SetAttribute("openIcon", "settingDataType.gif");
				treeElement.SetAttribute("nodeType", "DictionaryItem");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadcontentItemType : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openContentItemType(id) {
	parent.right.document.location.href = 'settings/editContentItemType.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			foreach(ContentItemType dt in ContentItemType.GetAll)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", dt.Id.ToString());
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("text", dt.Text);
				treeElement.SetAttribute("action", "javascript:openContentItemType(" + dt.Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingDataType.gif");
				treeElement.SetAttribute("openIcon", "settingDataType.gif");
				treeElement.SetAttribute("nodeType", "contentItemType");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadLanguages : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openLanguage(id) {
	parent.right.document.location.href = 'settings/editLanguage.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			foreach(Language l in Language.getAll)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", l.id.ToString());
				treeElement.SetAttribute("text", l.FriendlyName);
				treeElement.SetAttribute("action", "javascript:openLanguage(" + l.id + ");");
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingLanguage.gif");
				treeElement.SetAttribute("openIcon", "settingLanguage.gif");
				treeElement.SetAttribute("nodeType", "language");
				root.AppendChild(treeElement);
			}
		}
	}

	#endregion

	#region developer

	public class loadCache : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			Hashtable ht = Cache.ReturnCacheItemsOrdred();

			foreach(string key in ht.Keys)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", key);
				treeElement.SetAttribute("text", key + " (" + ((ArrayList)ht[key]).Count + ")");
				treeElement.SetAttribute("action", "#");
				treeElement.SetAttribute("src",
					"tree.aspx?app=" + _app + "&id=1&cacheTypeName=" + key + "&treeType=cacheItem" + "&rnd=" +
					Guid.NewGuid());
				treeElement.SetAttribute("icon", "developerCacheTypes.gif");
				treeElement.SetAttribute("openIcon", "developerCacheTypes.gif");
				treeElement.SetAttribute("menu", "L");
				treeElement.SetAttribute("nodeType", "cache");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadCacheItem : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openCacheItem(id) {
	parent.right.document.location.href = 'developer/viewCacheItem.aspx?key=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;

			Hashtable ht = Cache.ReturnCacheItemsOrdred();

			ArrayList a = (ArrayList)ht[HttpContext.Current.Request.QueryString["cacheTypeName"]];

			for(int i = 0; i < a.Count; i++)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", a[i].ToString());
				treeElement.SetAttribute("text", a[i].ToString());
				treeElement.SetAttribute("action", "javascript:openCacheItem('" + a[i] + "');");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("menu", "");
				treeElement.SetAttribute("icon", "developerCacheItem.gif");
				treeElement.SetAttribute("openIcon", "developerCacheItem.gif");
				treeElement.SetAttribute("nodeType", "cache");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadMacros : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openMacro(id) {
	parent.right.document.location.href = 'developer/editMacro.aspx?macroID=' + id;
}

function createPackage() {
	newName = window.open('dialogs/packagerCreate.aspx?rnd='+parent.returnRandom(), 'packager', 'width=530,height=450,scrollbars=auto');
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			using(SqlDataReader macros = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
				CommandType.Text, "select id, macroName from cmsMacro order by macroName"))
			{
				XmlNode root = Tree.DocumentElement;

				while (macros.Read())
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", macros.GetInt32(macros.GetOrdinal("id")).ToString());
					treeElement.SetAttribute("text", macros.GetString(macros.GetOrdinal("macroName")));
					treeElement.SetAttribute("action",
						"javascript:openMacro(" + macros.GetInt32(macros.GetOrdinal("id")) + ");");
					treeElement.SetAttribute("src", "");
					treeElement.SetAttribute("menu", "D");
					treeElement.SetAttribute("icon", "developerMacro.gif");
					treeElement.SetAttribute("openIcon", "developerMacro.gif");
					treeElement.SetAttribute("nodeType", "macro");
					root.AppendChild(treeElement);
				}
			}
		}
	}

	public class loadXslt : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openXslt(id) {
	parent.right.document.location.href = 'developer/editXslt.aspx?file=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			String orgPath = "";
			String path = "";
			if(HttpContext.Current.Request.QueryString["folder"] != null)
			{
				orgPath = HttpContext.Current.Request.QueryString["folder"];
				path = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../xslt/" + orgPath);
				orgPath += "/";
			}
			else
				path = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../xslt/");

			XmlNode root = Tree.DocumentElement;
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			DirectoryInfo[] dirInfos = dirInfo.GetDirectories();
			foreach(DirectoryInfo dir in dirInfos)
			{
				if((dir.Attributes & FileAttributes.Hidden) == 0)
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", "-1");
					treeElement.SetAttribute("text", dir.Name);
					treeElement.SetAttribute("action", "");
					treeElement.SetAttribute("menu", "D");
					treeElement.SetAttribute("src",
						"tree.aspx?app=" + _app + "&id=1&folder=" + dir.Name + "&treeType=xslt" + "&rnd=" + Guid.NewGuid());
					treeElement.SetAttribute("icon", "folder.gif");
					treeElement.SetAttribute("openIcon", "folder_o.gif");
					treeElement.SetAttribute("nodeType", "xsltFolder");
					root.AppendChild(treeElement);
				}
			}

			FileInfo[] fileInfo = dirInfo.GetFiles("*.xsl*");
			foreach(FileInfo file in fileInfo)
			{
				if((file.Attributes & FileAttributes.Hidden) == 0)
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", file.FullName);
					treeElement.SetAttribute("text", file.Name);
					if(orgPath != "")
						treeElement.SetAttribute("action", "javascript:openXslt('" + dirInfo.Name + "/" + file.Name + "');");
					else
						treeElement.SetAttribute("action", "javascript:openXslt('" + file.Name + "');");
					treeElement.SetAttribute("src", "");
					treeElement.SetAttribute("menu", "D");
					treeElement.SetAttribute("icon", "developerXslt.gif");
					treeElement.SetAttribute("openIcon", "developerXslt.gif");
					treeElement.SetAttribute("nodeType", "xslt");
					root.AppendChild(treeElement);
				}
			}
		}
	}

	public class loadPython : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"function openPython(id) {parent.right.document.location.href = 'developer/editPython.aspx?file=' + id;}");
		}

		public void Render(ref XmlDocument Tree)
		{
			string orgPath = "";
			string path = "";
			if(HttpContext.Current.Request.QueryString["folder"] != null)
			{
				orgPath = HttpContext.Current.Request.QueryString["folder"];
				path = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../python/" + orgPath);
				orgPath += "/";
			}
			else
			{
				path = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../python/");
			}
			XmlNode root = Tree.DocumentElement;
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			DirectoryInfo[] dirInfos = dirInfo.GetDirectories();
			foreach(DirectoryInfo dir in dirInfos)
			{
				if((dir.Attributes & FileAttributes.Hidden) == 0)
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", "-1");
					treeElement.SetAttribute("text", dir.Name);
					treeElement.SetAttribute("action", "");
					treeElement.SetAttribute("menu", "D");
					treeElement.SetAttribute("src",
						"tree.aspx?app=" + _app + "&id=1&folder=" + dir.Name + "&treeType=xslt" + "&rnd=" + Guid.NewGuid());
					treeElement.SetAttribute("icon", "folder.gif");
					treeElement.SetAttribute("openIcon", "folder_o.gif");
					treeElement.SetAttribute("nodeType", "pythonFolder");
					root.AppendChild(treeElement);
				}
			}
			FileInfo[] fileInfo = dirInfo.GetFiles("*.py*");
			foreach(FileInfo file in fileInfo)
			{
				if((file.Attributes & FileAttributes.Hidden) == 0)
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("nodeID", file.FullName);
					treeElement.SetAttribute("text", file.Name);
					if(!((orgPath == "")))
					{
						treeElement.SetAttribute("action", "javascript:openPython('" + dirInfo.Name + "/" + file.Name + "');");
					}
					else
					{
						treeElement.SetAttribute("action", "javascript:openPython('" + file.Name + "');");
					}
					treeElement.SetAttribute("src", "");
					treeElement.SetAttribute("menu", "D");
					treeElement.SetAttribute("icon", "developerPython.gif");
					treeElement.SetAttribute("openIcon", "developerPython.gif");
					treeElement.SetAttribute("nodeType", "python");
					root.AppendChild(treeElement);
				}
			}
		}
	}

	public class loadDataTypes : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openDataType(id) {
	parent.right.document.location.href = 'developer/editDataType.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			XmlNode root = Tree.DocumentElement;
			foreach(DataTypeDefinition dt in DataTypeDefinition.GetAll())
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("nodeID", dt.Id.ToString());
				treeElement.SetAttribute("text", dt.Text);
				treeElement.SetAttribute("action", "javascript:openDataType(" + dt.Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("icon", "settingDatatype.gif");
				treeElement.SetAttribute("openIcon", "settingDatatype.gif");
				treeElement.SetAttribute("nodeType", "datatype");
				root.AppendChild(treeElement);
			}
		}
	}

	#endregion

	#region "Users"

	public class loadUsers : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openUser(id) {
	parent.right.document.location.href = 'users/editUser.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			User[] users = User.getAll();
			XmlNode root = Tree.DocumentElement;
			foreach(User u in users)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				if(u.Id != 0)
					treeElement.SetAttribute("menu", "E");

				string textAdd = "";
				if(u.Disabled)
				{
					treeElement.SetAttribute("iconClass", "umbraco-tree-icon-grey");
				}
				treeElement.SetAttribute("nodeID", u.Id.ToString());
				treeElement.SetAttribute("text", u.Name + textAdd);
				treeElement.SetAttribute("action", "javascript:openUser(" + u.Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "settingDataType.gif");
				treeElement.SetAttribute("openIcon", "settingDataType.gif");
				treeElement.SetAttribute("nodeType", "user");
				root.AppendChild(treeElement);
			}
		}
	}

	#endregion

	#region "Members & membertypes"

	public class loadMembers : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openMember(id) {
	parent.right.document.location.href = 'members/editMember.aspx?id=' + id;
}
");
			Javascript.Append(
				@"
function openContentItem(id) {
	parent.right.document.location.href = 'ContentItem/edit.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			string letter = "";
			string ContentItemParent = "";
			if(HttpContext.Current.Request.QueryString.ToString().IndexOf("letter") >= 0)
			{
				letter = HttpContext.Current.Request.QueryString.Get("letter");
			}
			if(HttpContext.Current.Request.QueryString.ToString().IndexOf("ContentItemParent") >= 0)
			{
				ContentItemParent = HttpContext.Current.Request.QueryString.Get("ContentItemParent");
			}
			// letter = ;

			XmlNode root = Tree.DocumentElement;
			if(letter != "")
			{
				if(ContentItemParent != "") // show contentitems owned by the specific member!
				{
					CMSNode c = new CMSNode(int.Parse(ContentItemParent));
					foreach(CMSNode cn in c.ChildrenOfAllObjectTypes)
					{
						XmlElement treeElement = Tree.CreateElement("tree");
						treeElement.SetAttribute("menu", "D,L");
						treeElement.SetAttribute("nodeID", cn.Id.ToString());
						treeElement.SetAttribute("text", cn.Text);
						// treeElement.SetAttribute("action", "javascript:openMember(" + m.Id.ToString() + ");");
						treeElement.SetAttribute("action", "javascript:openContentItem(" + cn.Id + ");");
						if(!cn.HasChildren)
						{
							treeElement.SetAttribute("src", "");
						}
						else
						{
							treeElement.SetAttribute("src",
								"tree.aspx?letter=" + letter + "&app=" + _app + "&treeType=" +
								HttpContext.Current.Request.QueryString["treeType"] + "&ContentItemParent=" + cn.Id + "&rnd=" + Guid.NewGuid());
						}
						treeElement.SetAttribute("icon", "doc.gif");
						treeElement.SetAttribute("openIcon", "doc.gif");
						treeElement.SetAttribute("nodeType", "contentItem");
						root.AppendChild(treeElement);
					}
				}
				else // list all members with selected first character.
				{
					//if letters equals Others show members that not starts with a through z
					if(letter.Equals("Others"))
					{
						foreach(Member m in Member.getAllOtherMembers())
						{
							XmlElement treeElement = Tree.CreateElement("tree");

							treeElement.SetAttribute("nodeID", m.Id.ToString());
							treeElement.SetAttribute("text", m.Text);
							treeElement.SetAttribute("action", "javascript:openMember(" + m.Id + ");");
							if(!m.HasChildren)
							{
								treeElement.SetAttribute("src", "");
								treeElement.SetAttribute("menu", "D");
							}
							else
							{
								treeElement.SetAttribute("src",
									"tree.aspx?letter=" + letter + "&app=" + _app +
									"&treeType=" +
									HttpContext.Current.Request.QueryString["treeType"] +
									"&ContentItemParent=" + m.Id + "&rnd=" +
									Guid.NewGuid());
								treeElement.SetAttribute("menu", "D,L");
							}
							treeElement.SetAttribute("icon", "member.gif");
							treeElement.SetAttribute("openIcon", "member.gif");
							treeElement.SetAttribute("nodeType", "member");
							root.AppendChild(treeElement);
						}
					}
					else
					{
						foreach(Member m in Member.getMemberFromFirstLetter(letter.ToCharArray()[0]))
						{
							XmlElement treeElement = Tree.CreateElement("tree");

							treeElement.SetAttribute("nodeID", m.Id.ToString());
							treeElement.SetAttribute("text", m.Text);
							treeElement.SetAttribute("action", "javascript:openMember(" + m.Id + ");");
							if(!m.HasChildren)
							{
								treeElement.SetAttribute("src", "");
								treeElement.SetAttribute("menu", "D");
							}
							else
							{
								treeElement.SetAttribute("src",
									"tree.aspx?letter=" + letter + "&app=" + _app +
									"&treeType=" +
									HttpContext.Current.Request.QueryString["treeType"] +
									"&ContentItemParent=" + m.Id + "&rnd=" +
									Guid.NewGuid());
								treeElement.SetAttribute("menu", "D,L");
							}
							treeElement.SetAttribute("icon", "member.gif");
							treeElement.SetAttribute("openIcon", "member.gif");
							treeElement.SetAttribute("nodeType", "member");
							root.AppendChild(treeElement);
						}
					}
				}
			}
			else
			{
				for(int i = 97; i < 123; i++)
				{
					XmlElement treeElement = Tree.CreateElement("tree");
					treeElement.SetAttribute("menu", "L");
					treeElement.SetAttribute("nodeID", "1002");
					treeElement.SetAttribute("text", ((char)i).ToString());
					treeElement.SetAttribute("action", "javascript:void();");
					treeElement.SetAttribute("src", "");
					treeElement.SetAttribute("icon", "folder.gif");
					treeElement.SetAttribute("openIcon", "folder.gif");
					treeElement.SetAttribute("nodeType", "member");
					treeElement.SetAttribute("src",
						"tree.aspx?letter=" + ((char)i) + "&app=" + _app + "&treeType=" +
						HttpContext.Current.Request.QueryString["treeType"] + "&rnd=" + Guid.NewGuid());
					root.AppendChild(treeElement);
				}
				//Add folder named "Others"
				XmlElement treeElementOther = Tree.CreateElement("tree");
				treeElementOther.SetAttribute("menu", "L");
				treeElementOther.SetAttribute("nodeID", "1002");
				treeElementOther.SetAttribute("text", "Others");
				treeElementOther.SetAttribute("action", "javascript:void();");
				treeElementOther.SetAttribute("src", "");
				treeElementOther.SetAttribute("icon", "folder.gif");
				treeElementOther.SetAttribute("openIcon", "folder.gif");
				treeElementOther.SetAttribute("nodeType", "member");
				treeElementOther.SetAttribute("src",
					"tree.aspx?letter=Others&app=" + _app + "&treeType=" +
					HttpContext.Current.Request.QueryString["treeType"] + "&rnd=" +
					Guid.NewGuid());
				root.AppendChild(treeElementOther);
			}
		}
	}

	public class loadMemberTypes : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openMemberType(id) {
	parent.right.document.location.href = 'members/editMemberType.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			MemberType[] MemberTypes = MemberType.GetAll;
			XmlNode root = Tree.DocumentElement;
			for(int i = 0; i < MemberTypes.Length; i++)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("nodeID", MemberTypes[i].Id.ToString());
				treeElement.SetAttribute("text", MemberTypes[i].Text);
				treeElement.SetAttribute("action", "javascript:openMemberType(" + MemberTypes[i].Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "membertype.gif");
				treeElement.SetAttribute("openIcon", "membertype.gif");
				treeElement.SetAttribute("nodeType", "memberType");
				root.AppendChild(treeElement);
			}
		}
	}

	public class loadMemberGroups : ITree
	{
		private int _id;
		private string _app;

		public int id
		{
			set { _id = value; }
		}

		public string app
		{
			set { _app = value; }
		}

		public void RenderJS(ref StringBuilder Javascript)
		{
			Javascript.Append(
				@"
function openMemberGroup(id) {
	parent.right.document.location.href = 'members/editMemberGroup.aspx?id=' + id;
}
");
		}

		public void Render(ref XmlDocument Tree)
		{
			MemberGroup[] MemberGroups = MemberGroup.GetAll;
			XmlNode root = Tree.DocumentElement;
			for(int i = 0; i < MemberGroups.Length; i++)
			{
				XmlElement treeElement = Tree.CreateElement("tree");
				treeElement.SetAttribute("menu", "D");
				treeElement.SetAttribute("nodeID", MemberGroups[i].Id.ToString());
				treeElement.SetAttribute("text", MemberGroups[i].Text);
				treeElement.SetAttribute("action", "javascript:openMemberGroup(" + MemberGroups[i].Id + ");");
				treeElement.SetAttribute("src", "");
				treeElement.SetAttribute("icon", "membergroup.gif");
				treeElement.SetAttribute("openIcon", "membergroup.gif");
				treeElement.SetAttribute("nodeType", "memberGroup");
				root.AppendChild(treeElement);
			}
		}
	}

	#endregion
}
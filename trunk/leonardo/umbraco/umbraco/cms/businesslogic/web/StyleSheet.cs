using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

using System.Xml;

namespace Umbraco.Cms.BusinessLogic.web
{
	/// <summary>
	/// Summary description for StyleSheet.
	/// </summary>
	public class StyleSheet : CMSNode
	{

		private string _filename = "";
		private string _content = "";

		static Guid moduleObjectType = new Guid("9f68da4f-a3a8-44c2-8226-dcbd125e4840");

		public string Filename 
		{
			get {return _filename;}
			set {
				_filename = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update cmsStylesheet set filename = '" + _filename + "' where nodeId = " + base.Id.ToString());
			}
		}

		public string Content 
		{
			get {return _content;}
			set {
				_content = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update cmsStylesheet set content = '" + _content.Replace("'","''") + "' where nodeId = " + base.Id.ToString());
			}
		}

		public StylesheetProperty[] Properties 
		{
			get {
				BusinessLogic.console.IconI[] tmp = this.ChildrenOfAllObjectTypes;
			
				StylesheetProperty[] retVal = new StylesheetProperty[tmp.Length];
				for (int i = 0; i < tmp.Length;i++)
					retVal[i] = new StylesheetProperty(this.ChildrenOfAllObjectTypes[i].UniqueId);
				return retVal;
			}
		}

		//static bool isInitialized = false;

		public StyleSheet(Guid id) : base(id)
		{
			setupStyleSheet();
		}

		public StyleSheet(int id) : base(id)
		{
			setupStyleSheet();
		}

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
            base.Save();
        }


		private void setupStyleSheet() 
		{
			// Get stylesheet data
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "select filename, content from cmsStylesheet where nodeid = " + base.Id.ToString());
			if (dr.Read()) 
			{
				if (!dr.IsDBNull(dr.GetOrdinal("filename")))
					_filename = dr.GetString(dr.GetOrdinal("filename"));
				if (!dr.IsDBNull(dr.GetOrdinal("content")))
					_content = dr.GetString(dr.GetOrdinal("content"));
			}
			dr.Close();
		}

		public static StyleSheet MakeNew(BusinessLogic.User user, string Text, string FileName, string Content) 
		{
			
			// Create the Umbraco node
			CMSNode newNode = CMSNode.MakeNew(-1, moduleObjectType, user.Id, 1, Text, Guid.NewGuid());

			// Create the stylesheet data
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "insert into cmsStylesheet (nodeId, filename, content) values ('" + newNode.Id.ToString() + "','" + FileName + "',@content)", new SqlParameter("@content", Content));
			
			return new StyleSheet(newNode.UniqueId);
		}

		public static StyleSheet[] GetAll() {
			Guid[] topNodeIds = CMSNode.TopMostNodeIds(moduleObjectType);
			StyleSheet[] retval = new StyleSheet[topNodeIds.Length];
			for (int i = 0; i < topNodeIds.Length;i++) retval[i] = new StyleSheet(topNodeIds[i]);
			return retval;
		}

		public StylesheetProperty AddProperty(string Alias, BusinessLogic.User u) {
			return StylesheetProperty.MakeNew(Alias,this,u);
		}

		new public void delete() {
			System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../css/"+this.Text+".css"));
			foreach (StylesheetProperty p in this.Properties)
				p.delete();
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"delete from cmsStylesheet where nodeId = @nodeId", new SqlParameter("@nodeId", this.Id));
			base.delete();		
		}

		public void saveCssToFile() {
			System.IO.StreamWriter SW;
			SW=System.IO.File.CreateText(System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../css/"+this.Text+".css"));
			string tmpCss;
			tmpCss = "/* GENERAL STYLES */\n";
			tmpCss += this.Content + "\n\n";
			tmpCss += "/* EDITOR PROPERTIES */\n";
			foreach (StylesheetProperty p in this.Properties){
				tmpCss += p.ToString()+ "\n";
			}
			SW.Write(tmpCss);
			SW.Close();
		}

		public XmlElement ToXml(XmlDocument xd) 
		{
			XmlElement doc = xd.CreateElement("Stylesheet");
			doc.AppendChild(XmlHelper.addTextNode(xd, "Name", this.Text));
			doc.AppendChild(XmlHelper.addTextNode(xd, "FileName", this.Filename));
			doc.AppendChild(XmlHelper.addCDataNode(xd, "Content", this.Content));

			if (this.Properties.Length > 0) 
			{
				XmlElement props = xd.CreateElement("Properties");
				foreach(StylesheetProperty sp in this.Properties) 
				{
					XmlElement prop = xd.CreateElement("Property");
					prop.AppendChild(XmlHelper.addTextNode(xd, "Name", sp.Text));
					prop.AppendChild(XmlHelper.addTextNode(xd, "Alias", sp.Alias));
					prop.AppendChild(XmlHelper.addTextNode(xd, "Value", sp.value));
					props.AppendChild(prop);
				}
				doc.AppendChild(props);
			}


			return doc;
		}
	}
}
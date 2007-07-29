using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

using System.Collections;
using System.Xml;
using SqlHelper=Umbraco.SqlHelper;

namespace Umbraco.Cms.BusinessLogic.template
{
	/// <summary>
	/// Summary description for Template.
	/// </summary>
	public class Template :CMSNode
	{
		private static Guid _objectType = new Guid("6fbde604-4178-42ce-a10b-8a2600a2f07d");
		private string _OutputContentType;
		private string _design;
		private string _alias;
		private int _mastertemplate;
	    private static bool _templateAliasesInitialized = false;

        private static Hashtable _templateAliases = new Hashtable();

        public static Hashtable TemplateAliases
        {
            get { return _templateAliases; }
            set { _templateAliases = value; }
        }
	

		public Template(int id) :  base(id)
		{
			//
			// TODO: Add constructor logic here
			//
			setupTemplate();
		}

		public Template(Guid id) :  base(id)
		{
			//
			// TODO: Add constructor logic here
			//
			setupTemplate();
		}

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
            base.Save();
        }

        public string GetRawText()
        {
            return base.Text;
        }

        public override string Text
        {
            get
            {
                string tempText = base.Text;
                if (!tempText.StartsWith("#"))
                    return tempText;
                else
                {
                    language.Language lang = language.Language.GetByCultureCode(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
                    if (lang != null)
                    {
                        if (Dictionary.DictionaryItem.hasKey(tempText.Substring(1, tempText.Length - 1)))
                        {
                            Dictionary.DictionaryItem di = new Dictionary.DictionaryItem(tempText.Substring(1, tempText.Length - 1));
                            if (di != null)
                                return di.Value(lang.id);
                        }
                    }

                    return "[" + tempText + "]";
                }
            }
            set
            {
                base.Text = value;
            }
        }

		public string OutputContentType
		{
			get {  return _OutputContentType; }
			set { _OutputContentType = value; }    
		}

		private void setupTemplate() 
		{
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString,CommandType.Text,"Select alias,design,master from cmsTemplate where nodeId = " + this.Id);
			dr.Read();
			_alias = dr["alias"].ToString();
			_design = dr["design"].ToString();
			_mastertemplate = int.Parse(dr["master"].ToString());
			dr.Close();
		}
		
	
		public string Alias 
		{
			get {return _alias;}
			set 
			{
				_alias = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Update cmsTemplate set alias = @alias where NodeId = " + this.Id, new SqlParameter("@alias", _alias));
			    _templateAliasesInitialized = false;
                initTemplateAliases();
            }
		
		}
		
		public bool HasMasterTemplate 
		{
			get{return (_mastertemplate >0 );}
		}

		public int MasterTemplate 
		{
			get{return _mastertemplate;}
			set{
				_mastertemplate = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Update cmsTemplate set master = " + value + " where NodeId = " + this.Id);
			}
		}

		public string Design {
			get {return _design;}
			set {
			    _design = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Update cmsTemplate set design = '" + value.Replace("'","''") + "' where NodeId = " + this.Id);
			}
		}

		public XmlElement ToXml(XmlDocument xd) 
		{
			XmlElement doc = xd.CreateElement("Template");
			doc.AppendChild(XmlHelper.addTextNode(xd, "Name", this.Text));
			doc.AppendChild(XmlHelper.addTextNode(xd, "Alias", this.Alias));
			if (this.MasterTemplate != 0)
				doc.AppendChild(XmlHelper.addTextNode(xd, "Master", new Template(this.MasterTemplate).Alias));
			else
				doc.AppendChild(XmlHelper.addTextNode(xd, "Master", ""));
			doc.AppendChild(XmlHelper.addCDataNode(xd, "Design", this.Design));


			return doc;
		}

		public static Template MakeNew(string Name, BusinessLogic.User u) {
			// CMSNode MakeNew(int parentId, Guid objectType, int userId, int level, string text, Guid uniqueId)
			CMSNode n = CMSNode.MakeNew(-1,_objectType, u.Id, 1, Name, Guid.NewGuid());
			if (Name.Length > 100)
				Name = Name.Substring(0, 95) + "...";
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Insert into cmsTemplate (NodeId, Alias, design,master) Values (" + n.Id + ",'"+ SqlHelper.SafeString(Name) +"',' ',0)");
			return new Template(n.Id);
		}

		public static Template GetByAlias(string Alias) 
		{
			try 
			{
				return new Template(int.Parse(
					Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select nodeId from cmsTemplate where alias = @alias", new SqlParameter("@alias", Alias)).ToString()));
			} 
			catch 
			{
				return null;
			}
		}

		public static Template[] getAll()  {
			Guid[] ids = CMSNode.TopMostNodeIds(_objectType);

			SortedList initRetVal = new SortedList();
			for (int i = 0; i < ids.Length; i++) 
			{
				Template t = new Template(ids[i]);
				initRetVal.Add(t.Text + t.UniqueId, t);
			}

			Template[] retVal = new Template[ids.Length];

			IDictionaryEnumerator ide = initRetVal.GetEnumerator(); 
	
			int count = 0;
			while(ide.MoveNext())
			{ 
				retVal[count] = (Template) ide.Value;
				count++;
			} 

			return retVal;
		}

        public static int GetTemplateIdFromAlias(string alias)
        {
            initTemplateAliases();
            if (TemplateAliases.ContainsKey(alias))
                return (int) TemplateAliases[alias];
            else
                return 0;
        }

        private static void initTemplateAliases()
        {
            if (!_templateAliasesInitialized)
            {
                _templateAliases.Clear();
                foreach(Template t in getAll())
                    TemplateAliases.Add(t.Alias.ToLower(), t.Id);
                
                _templateAliasesInitialized = true;
            }
        }

		new public void delete() {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"delete from cmsTemplate where NodeId ="+ this.Id);
			base.delete();
		}

		
	}
}
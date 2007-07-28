using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco
{
	/// <summary>
	/// Summary description for standardTasks.
	/// </summary>
	/// 


	public class XsltTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}


		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			string template = _alias.Substring(0, _alias.IndexOf("|||"));
			string fileName = _alias.Substring(_alias.IndexOf("|||")+3, _alias.Length-_alias.IndexOf("|||")-3).Replace(" ", "");
			string xsltTemplateSource = System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.Path+"/xslt/templates/" + template);
			string xsltNewFilename = System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.Path+"/../xslt/" + fileName + ".xslt");
			System.IO.File.Copy(xsltTemplateSource, xsltNewFilename, false);

			// Create macro?
			if (ParentID == 1) 
			{
				cms.businesslogic.macro.Macro m = 
					cms.businesslogic.macro.Macro.MakeNew(
					helper.SpaceCamelCasing(_alias.Substring(_alias.IndexOf("|||")+3, _alias.Length-_alias.IndexOf("|||")-3)));
				m.Xslt = fileName + ".xslt";
			}

			return true;
		}

		public bool Delete() 
		{
			System.Web.HttpContext.Current.Trace.Warn("", "*" + Alias + "*");
//			try 
//			{
				System.IO.File.Delete(Alias);
//			} 
//			catch {}
			return true;
		}

		public XsltTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

    public class PythonTasks : interfaces.ITask
    {
        private string _alias;
        private int _parentID;
        private int _typeID;
        private int _userID;

        public int UserId
        {
            set
            {
                _userID = value;
            }
        }

        public int TypeID
        {
            get
            {
                return _typeID;
            }
            set
            {
                _typeID = value;
            }
        }

        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value;
            }
        }

        public int ParentID
        {
            get
            {
                return _parentID;
            }
            set
            {
                _parentID = value;
            }
        }

        public bool Save()
        {
            string pythonNewFilename = System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../python/" + _alias + ".py");
            System.IO.FileStream fs = new System.IO.FileStream(pythonNewFilename, System.IO.FileMode.Create);
            fs.Flush();
            fs.Close();
            return true;
        }

        public bool Delete()
        {
            System.Web.HttpContext.Current.Trace.Warn("", "*" + Alias + "*");
            System.IO.File.Delete(Alias);
            return true;
        }

        public PythonTasks()
        {
        }
    }

    public class ScriptTasks : interfaces.ITask
    {
        private string _alias;
        private int _parentID;
        private int _typeID;
        private int _userID;

        public int UserId
        {
            set { _userID = value; }
        }
        public int TypeID
        {
            set { _typeID = value; }
            get { return _typeID; }
        }


        public string Alias
        {
            set { _alias = value; }
            get { return _alias; }
        }

        public int ParentID
        {
            set { _parentID = value; }
            get { return _parentID; }
        }

        public bool Save()
        {
            string[] scriptFileAr = _alias.Split('¤');

            string path = scriptFileAr[0];
            string fileName = scriptFileAr[1];
            string fileType = scriptFileAr[2];
            int createFolder = ParentID;

            if (createFolder == 1)
            {
                System.IO.Directory.CreateDirectory(path + @"\" + fileName);
            }
            else
            {
                System.IO.File.Create(path + @"\" + fileName + "." + fileType).Close();
            }
            return true;
        }

        public bool Delete()
        {
            if (System.IO.File.Exists(_alias))
                System.IO.File.Delete(_alias);
            else
            {
                if (System.IO.Directory.Exists(_alias))
                    System.IO.Directory.Delete(_alias, true);
            }
            BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Delete, umbraco.BasePages.UmbracoEnsuredPage.CurrentUser, -1, _alias + " Deleted");
            return true;
        }
    }

	public class macroTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}


		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,
				CommandType.Text, "insert into cmsMacro (macroAlias, macroName) values (@alias, @name)", new SqlParameter("@alias", _alias.Replace(" ", "")), new SqlParameter("@name", _alias));
			return true;
		}

		public bool Delete() 
		{
			// Release cache
			System.Web.Caching.Cache macroCache = System.Web.HttpRuntime.Cache;
			if (macroCache["umbMacro" + ParentID.ToString()] != null) 
			{
				macroCache.Remove("umbMacro" + ParentID.ToString());
			}

			// Clear cache!
			macro.ClearAliasCache();
			new cms.businesslogic.macro.Macro(ParentID).Delete();
			return true;
		}

		public macroTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
	
	public class MediaTypeTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			cms.businesslogic.media.MediaType.MakeNew(BusinessLogic.User.GetUser(_userID),Alias.Replace("'","''"));
			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.media.MediaType(_parentID).delete();
			return false;
		}

		public MediaTypeTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class nodetypeTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			cms.businesslogic.web.DocumentType dt = cms.businesslogic.web.DocumentType.MakeNew(BusinessLogic.User.GetUser(_userID),Alias.Replace("'","''"));
			dt.IconUrl = "folder.gif";

			// Create template?
			if (ParentID == 1) 
			{
				cms.businesslogic.template.Template[] t = 
					{cms.businesslogic.template.Template.MakeNew(_alias, BusinessLogic.User.GetUser(_userID))};
				dt.allowedTemplates = t;
				dt.DefaultTemplate = t[0].Id;
			}

			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.web.DocumentType(ParentID).delete();
			return false;
		}

		public nodetypeTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

    public class templateTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			cms.businesslogic.template.Template.MakeNew(Alias,BusinessLogic.User.GetUser(_userID));
			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.template.Template(_parentID).delete();
			return false;
		}

		public templateTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
	
	public class mediaTasks : interfaces.ITaskReturnUrl
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;
		private string _returnUrl = "";

		public int UserId
		{
			set {_userID = value;}
		}

		public string ReturnUrl 
		{
			get {return _returnUrl;}
		}

		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}

		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set 
			{
				_parentID = value;
			}
			get 
			{
				return _parentID;
			}
		}

		public bool Save() 
		{
			cms.businesslogic.media.MediaType dt = new cms.businesslogic.media.MediaType(TypeID);
			cms.businesslogic.media.Media m = cms.businesslogic.media.Media.MakeNew(Alias,dt, BusinessLogic.User.GetUser(_userID),ParentID);
			_returnUrl = "editMedia.aspx?id=" + m.Id.ToString();

			return true;			
		}

		public bool Delete() 
		{
			cms.businesslogic.media.Media d = new cms.businesslogic.media.Media(ParentID);
			// Remove all files
			interfaces.IDataType uploadField = new cms.businesslogic.datatype.controls.Factory().GetNewObject(new Guid("5032a6e6-69e3-491d-bb28-cd31cd11086c"));
			foreach (cms.businesslogic.property.Property p in d.getProperties)
				if (p.PropertyType.DataTypeDefinition.DataType.Id == uploadField.Id &&
					p.Value.ToString() != "" && 
					System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(p.Value.ToString()))
					)
					System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(p.Value.ToString()));


			// Log
			BasePages.UmbracoEnsuredPage bp = new BasePages.UmbracoEnsuredPage();
			BusinessLogic.Log.Add(BusinessLogic.LogTypes.Delete, bp.getUser(), d.Id, "");

			d.delete();
			return true;

		}

		public bool Sort()
		{
			return false;
		}

		public mediaTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

    public class contentTasks : interfaces.ITaskReturnUrl
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;
		private string _returnUrl = "";

		public int UserId
		{
			set {_userID = value;}
		}

		public string ReturnUrl 
		{
			get {return _returnUrl;}
		}

		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}

		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {
				_parentID = value;
			}
			get {
				return _parentID;
			}
		}

		public bool Save() 
		{
			cms.businesslogic.web.DocumentType dt = new cms.businesslogic.web.DocumentType(TypeID);
			cms.businesslogic.web.Document d = cms.businesslogic.web.Document.MakeNew(Alias,dt, BusinessLogic.User.GetUser(_userID),ParentID);
            _returnUrl = "editContent.aspx?id=" + d.Id.ToString();
			return true;			
		}

		public bool Delete() 
		{
			cms.businesslogic.web.Document d = new cms.businesslogic.web.Document(ParentID);
			
			// Log
			BasePages.UmbracoEnsuredPage bp = new BasePages.UmbracoEnsuredPage();
			BusinessLogic.Log.Add(BusinessLogic.LogTypes.Delete, bp.getUser(), d.Id, "");

			library.UnPublishSingleNode(d.Id);

            d.delete();

			return true;

		}

		public bool Sort()
		{
			return false;
		}

		public contentTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

    public class userTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			// Hot damn HACK > user is allways UserType with id  = 1  = administrator ???
			// temp password deleted by NH
			BusinessLogic.User.MakeNew(Alias, Alias, "", BusinessLogic.UserType.GetUserType(1));
			return true;
		}

		public bool Delete() 
		{
			BusinessLogic.User u = BusinessLogic.User.GetUser(ParentID);
			u.Disabled = true;
			return true;
		}

		public userTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class DataTypeTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
		
			cms.businesslogic.datatype.DataTypeDefinition.MakeNew(BusinessLogic.User.GetUser(_userID), Alias);
			return true;
		}

		public bool Delete() 
		{
			cms.businesslogic.datatype.DataTypeDefinition.GetDataTypeDefinition(ParentID).delete();
			return true;
		}

		public DataTypeTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}


	public class contentItemTypeTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
		
			cms.businesslogic.contentitem.ContentItemType.MakeNew(BusinessLogic.User.GetUser(_userID), Alias);
			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.contentitem.ContentItemType(_parentID).delete();
			return true;
		}

		public contentItemTypeTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}



	public class MemberGroupTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
		
			cms.businesslogic.member.MemberGroup.MakeNew(Alias, BusinessLogic.User.GetUser(_userID));
			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.member.MemberGroup(_parentID).delete();
			return true;
		}

		public MemberGroupTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class MemberTypeTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
		
			cms.businesslogic.member.MemberType.MakeNew(BusinessLogic.User.GetUser(_userID), Alias);
			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.member.MemberType(_parentID).delete();
			return true;
		}

		public MemberTypeTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class StylesheetTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
		
			cms.businesslogic.web.StyleSheet.MakeNew(BusinessLogic.User.GetUser(_userID),Alias,"","");
			return true;
		}

		public bool Delete() 
		{
			cms.businesslogic.web.StyleSheet s = new cms.businesslogic.web.StyleSheet(ParentID);
			s.delete();
			return true;
		}


	}

	public class stylesheetPropertyTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			try 
			{
				cms.businesslogic.web.StyleSheet s = new cms.businesslogic.web.StyleSheet(ParentID);
				s.AddProperty(Alias,BusinessLogic.User.GetUser(_userID));
			} 
			catch {
				throw new ArgumentException("DER ER SKET EN FEJL MED AT OPRETTE NOGET MED ET PARENT ID : " + ParentID);
			}
			return true;
		}

		public bool Delete() 
		{
			cms.businesslogic.web.StylesheetProperty sp = new cms.businesslogic.web.StylesheetProperty(ParentID);
			cms.businesslogic.web.StyleSheet s = sp.StyleSheet();
			s.saveCssToFile();
			sp.delete();
			
			return true;
		}


	}

	
	public class memberTasks : interfaces.ITaskReturnUrl
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;
		private string _returnUrl = "";

		public int UserId
		{
			set {_userID = value;}
		}

		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}

		public string ReturnUrl 
		{
			get {return _returnUrl;}
		}

		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set 
			{
				_parentID = value;
				// NASTY HACK ON NASTY HACK§!!
				if (_parentID == 1) _parentID = -1;
			}
			get 
			{
				return _parentID;
			}
		}

		public bool Save() 
		{
			cms.businesslogic.member.MemberType dt = new cms.businesslogic.member.MemberType(TypeID);
			cms.businesslogic.member.Member m = cms.businesslogic.member.Member.MakeNew(Alias,dt, BusinessLogic.User.GetUser(_userID));
			_returnUrl = "members/editMember.aspx?id=" + m.Id.ToString();

			return true;			
		}

		public bool Delete() 
		{
			cms.businesslogic.member.Member d = new cms.businesslogic.member.Member(ParentID);
			d.delete();
			return true;

		}

		public bool Sort()
		{
			return false;
		}

		public memberTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class contentItemTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}

		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}

		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set 
			{
				_parentID = value;
			}
			get 
			{
				return _parentID;
			}
		}

		public bool Save() 
		{
			// TODO : fix it!!
			return true;			
		}

		public bool Delete() 
		{
			cms.businesslogic.contentitem.ContentItem d = new cms.businesslogic.contentitem.ContentItem(ParentID);
			
            // v3.0 - moving to recycle bin instead of deletion
            //d.delete();
            d.Move(-20);
			return true;

		}

		public bool Sort()
		{
			return false;
		}

		public contentItemTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class dictionaryTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}

		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}

		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set 
			{
				_parentID = value;
				// NASTY HACK ON NASTY HACK§!!
				// if (_parentID == 1) _parentID = -1;
			}
			get 
			{
				return _parentID;
			}
		}

		public bool Save() 
		{
			// Create new dictionary item if name no already exist
			if (ParentID > 0) 
				cms.businesslogic.Dictionary.DictionaryItem.addKey(Alias,"",new cms.businesslogic.Dictionary.DictionaryItem(ParentID).key);
			else
				cms.businesslogic.Dictionary.DictionaryItem.addKey(Alias,"");
			
			return true;			
		}

		public bool Delete() 
		{
			new cms.businesslogic.Dictionary.DictionaryItem(ParentID).delete();
			return true;
		}

		public bool Sort()
		{
			return false;
		}

		public dictionaryTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class languageTasks : interfaces.ITask
	{

		private string _alias;
		private int _parentID;
		private int _typeID;
		private int _userID;

		public int UserId
		{
			set {_userID = value;}
		}
		public int TypeID 
		{
			set {_typeID = value;}
			get {return _typeID;}
		}


		public string Alias 
		{
			set {_alias = value;}
			get {return _alias;}
		}

		public int ParentID 
		{
			set {_parentID = value;}
			get {return _parentID;}
		}

		public bool Save() 
		{
			cms.businesslogic.language.Language.MakeNew(Alias);
			return true;
		}

		public bool Delete() 
		{
			new cms.businesslogic.language.Language(ParentID).Delete();
			return false;
		}

		public languageTasks()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
	
}

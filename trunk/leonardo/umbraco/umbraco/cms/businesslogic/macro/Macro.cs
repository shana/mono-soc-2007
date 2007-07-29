using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Xml;
using SqlHelper=Umbraco.SqlHelper;

namespace Umbraco.Cms.BusinessLogic.macro
{
	/// <summary>
	/// The Macro component are one of the Umbraco essentials, used for drawing dynamic content in the public website of Umbraco.
	/// 
	/// A Macro is a placeholder for either a xsl transformation, a custom .net control or a .net usercontrol.
	/// 
	/// The Macro is representated in templates and content as a special html element, which are being parsed out and replaced with the
	/// output of either the .net control or the xsl transformation when a page is being displayed to the visitor.
	/// 
	/// A macro can have a variety of properties which are used to transfer userinput to either the usercontrol/custom control or the xsl
	/// 
	/// </summary>
	public class Macro		
	{

		int _id;
		bool _useInEditor;
		int _refreshRate;
		string _alias;
		string _name;
		string _assembly;
		string _type;
		string _xslt;
		MacroProperty[] _properties;

		/// <summary>
		/// id
		/// </summary>
		public int Id 
		{
			get {return _id;}
		}
		
		/// <summary>
		/// If set to true, the macro can be inserted on documents using the richtexteditor.
		/// </summary>
		public bool UseInEditor 
		{
			get {return _useInEditor;}
			set 
			{
				_useInEditor = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroUseInEditor = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}

		/// <summary>
		/// The cache refreshrate - the maximum amount of time the macro should remain cached in the Umbraco
		/// runtime layer.
		/// 
		/// The macro caches are refreshed whenever a document is changed
		/// </summary>
		public int RefreshRate
		{
			get {return _refreshRate;}
			set 
			{
				_refreshRate = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroRefreshRate = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}

		/// <summary>
		/// The alias of the macro - are used for retrieving the macro when parsing the <?UMBRACO_MACRO></?UMBRACO_MACRO> element,
		/// by using the alias instead of the Id, it's possible to distribute macroes from one installation to another - since the id
		/// is given by an autoincrementation in the database table, and might be used by another macro in the foreing Umbraco
		/// </summary>
		public string Alias
		{
			get {return _alias;}
			set 
			{
				_alias = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroAlias = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}
		
		/// <summary>
		/// The userfriendly Name
		/// </summary>
		public string Name
		{
			get {return _name;}
			set 
			{
				_name = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroName = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}

		/// <summary>
		/// If the macro is a wrapper for a custom control, this is the assemly Name from which to load the macro
		/// 
		/// specified like: /bin/mydll (without the .dll extension)
		/// </summary>
		public string Assembly
		{
			get {return _assembly;}
			set 
			{
				_assembly = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroScriptAssembly = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}

		/// <summary>
		/// The relative path to the usercontrol
		/// 
		/// Specified like: /usercontrols/myusercontrol.ascx (with the .ascx postfix)
		/// </summary>
		public string Type
		{
			get {return _type;}
			set 
			{
				_type = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroScriptType = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}

		/// <summary>
		/// The xsl file used to transform content
		/// 
		/// Umbraco assumes that the xslfile is present in the "/xslt" folder
		/// </summary>
		public string Xslt
		{
			get {return _xslt;}
			set 
			{
				_xslt = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update cmsMacro set macroXSLT = @macroAlias where id = @id", new SqlParameter("@macroAlias", value), new SqlParameter("@id", this.Id));
			}
		}

		/// <summary>
		/// Properties which are used to send parameters to the xsl/usercontrol/customcontrol of the macro
		/// </summary>
		public MacroProperty[] Properties
		{
			get {return _properties;}
		}

		/// <summary>
		/// Macro initializer
		/// </summary>
		public Macro() {}

		/// <summary>
		/// Macro initializer
		/// </summary>
		/// <param Name="Id">The id of the macro</param>
		public Macro(int Id)
		{
			_id = Id;
			setup();
		}


        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public virtual void Save()
        {
        }


		/// <summary>
		/// Deletes the current macro
		/// </summary>
		public void Delete() 
		{
			foreach(MacroProperty p in this.Properties)
				p.Delete();
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from cmsMacro where id = @id", new SqlParameter("@id", this._id));

		}

		private void setup() 
		{
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select macroUseInEditor, macroRefreshRate, macroAlias, macroName, macroScriptType, macroScriptAssembly, macroXSLT from cmsMacro where id = @id", new SqlParameter("@id", _id)))
			{
				if(dr.Read())
				{
					_useInEditor = bool.Parse(dr["macroUseInEditor"].ToString());
					_refreshRate = int.Parse(dr["macroRefreshRate"].ToString());
					_alias = dr["macroAlias"].ToString();
					_name = dr["macroName"].ToString();
					_assembly = dr["macroScriptAssembly"].ToString();
					_type = dr["macroScriptType"].ToString();
					_xslt = dr["macroXSLT"].ToString();
					_properties = MacroProperty.GetProperties(_id);
				}
			}
		}

		/// <summary>
		/// Get an xmlrepresentation of the macro, used for exporting the macro to a package for distribution
		/// </summary>
		/// <param Name="xd">Current xmldocument context</param>
		/// <returns>An xmlrepresentation of the macro</returns>
		public XmlElement ToXml(XmlDocument xd) {

			XmlElement doc = xd.CreateElement("macro");

			// info section
			doc.AppendChild(XmlHelper.AddTextNode(xd, "Name", this.Name));
			doc.AppendChild(XmlHelper.AddTextNode(xd, "alias", this.Alias));
			doc.AppendChild(XmlHelper.AddTextNode(xd, "scriptType", this.Type));
			doc.AppendChild(XmlHelper.AddTextNode(xd, "scriptAssembly", this.Assembly));
			doc.AppendChild(XmlHelper.AddTextNode(xd, "xslt", this.Xslt));
			doc.AppendChild(XmlHelper.AddTextNode(xd, "useInEditor", this.UseInEditor.ToString()));
			doc.AppendChild(XmlHelper.AddTextNode(xd, "refreshRate", this.RefreshRate.ToString()));

			// properties
			XmlElement props = xd.CreateElement("properties");
			foreach (MacroProperty p in this.Properties)
				props.AppendChild(p.ToXml(xd));
			doc.AppendChild(props);

			return doc;
		}


		#region STATICS

		/// <summary>
		/// Creates a new macro given the Name
		/// </summary>
		/// <param Name="Name">Userfriendly Name</param>
		/// <returns>The newly macro</returns>
		public static Macro MakeNew(string Name) 
		{
			return new Macro( int.Parse(
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN,
				CommandType.Text, "SET NOCOUNT ON; insert into cmsMacro (macroAlias, macroName) values ('" + Umbraco.SqlHelper.SafeString(Name.Replace(" ", "")) + "','" + SqlHelper.SafeString(Name) + "') select @@identity as id").ToString()));
		}

		/// <summary>
		/// Retrieve all macroes
		/// </summary>
		/// <returns>A list of all macroes</returns>
		public static Macro[] GetAll() 
		{
			int total = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select count(*) from cmsMacro").ToString());
			int count = 0;
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select id from cmsMacro order by macroName");
			Macro[] retval = new Macro[total];
			while (dr.Read()) 
			{
				retval[count] =  new Macro(int.Parse(dr["id"].ToString()));
				count++;
			}
			dr.Close();
			return retval;
		}

		/// <summary>
		/// Static contructor for retrieving a macro given an alias
		/// </summary>
		/// <param Name="Alias">The alias of the macro</param>
		/// <returns>If the macro with the given alias exists, it returns the macro, else null</returns>
		public static Macro GetByAlias(string Alias) 
		{
			try 
			{
				return new Macro(int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select id from cmsMacro where macroAlias = @alias", new SqlParameter("@alias", Alias)).ToString()));
			} 
			catch 
			{
				return null;
			}
		}
		#endregion
	}
}

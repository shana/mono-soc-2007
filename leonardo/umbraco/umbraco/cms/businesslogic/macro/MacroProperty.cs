using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Xml;

namespace Umbraco.Cms.BusinessLogic.macro
{
	/// <summary>
	/// The macro property is used by macroes to communicate/transfer userinput to an instance of a macro.
	/// 
	/// It contains information on which type of data is inputted aswell as the userinterface used to input data
	/// 
	/// A MacroProperty uses it's MacroPropertyType to define which underlaying component should be used when
	/// rendering the MacroProperty editor aswell as which datatype its containing.
	/// </summary>
	public class MacroProperty
	{

		int _id;
		int _sortOrder;
		bool _public;
		string _alias;
		string _name;
		Cms.BusinessLogic.macro.MacroPropertyType _type;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param Name="Id">Id</param>
		public MacroProperty(int Id)
		{
			_id = Id;
			setup();
		}

		/// <summary>
		/// The sortorder
		/// </summary>
		public int SortOrder 
		{
			get {return _sortOrder;}
		}

		/// <summary>
		/// If set to true, the user will be presented with an editor to input data.
		/// 
		/// If not, the field can be manipulated by a default value given by the MacroPropertyType, this is s
		/// </summary>
		public bool Public
		{
			get {return _public;}
		}

		/// <summary>
		/// The alias if of the macroproperty, this is used in the special macro element
		/// <?UMBRACO_MACRO macroAlias="value"></?UMBRACO_MACRO>
		/// 
		/// </summary>
		public string Alias
		{
			get {return _alias;}
		}

		/// <summary>
		/// The userfriendly Name
		/// </summary>
		public string Name
		{
			get {return _name;}
		}

		/// <summary>
		/// The basetype which defines which component is used in the UI for editing content
		/// </summary>
		public Cms.BusinessLogic.macro.MacroPropertyType Type 
		{
			get {return _type;}
		}

		private void setup() 
		{
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select macroPropertyHidden, macroPropertyType, macroPropertySortOrder, macroPropertyAlias, macroPropertyName from cmsMacroProperty where id = @id", new SqlParameter("@id", _id)))
			{
				if(dr.Read())
				{
					_public = bool.Parse(dr["macroPropertyHidden"].ToString());
					_sortOrder = int.Parse(dr["macroPropertySortOrder"].ToString());
					_alias = dr["macroPropertyAlias"].ToString();
					_name = dr["macroPropertyName"].ToString();
					_type = new MacroPropertyType(int.Parse(dr["macroPropertyType"].ToString()));
				}
			}
		}

		/// <summary>
		/// Deletes the current macroproperty
		/// </summary>
		public void Delete() 
		{
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from cmsMacroProperty where id = @id", new SqlParameter("@id", this._id));
		}

		/// <summary>
		/// Retrieve a Xmlrepresentation of the MacroProperty used for exporting the Macro to the package
		/// </summary>
		/// <param Name="xd">XmlDocument context</param>
		/// <returns>A xmlrepresentation of the object</returns>
		public XmlElement ToXml(XmlDocument xd) 
		{
			XmlElement doc = xd.CreateElement("property");

			doc.Attributes.Append(XmlHelper.addAttribute(xd, "Name", this.Name));
			doc.Attributes.Append(XmlHelper.addAttribute(xd, "alias", this.Alias));
			doc.Attributes.Append(XmlHelper.addAttribute(xd, "show", this.Public.ToString()));
			doc.Attributes.Append(XmlHelper.addAttribute(xd, "propertyType", this.Type.Alias));
			
			return doc;
		}

		#region STATICS

		/// <summary>
		/// Retieve all MacroProperties of a macro
		/// </summary>
		/// <param Name="MacroId">Macro identifier</param>
		/// <returns>All MacroProperties of a macro</returns>
		public static MacroProperty[] GetProperties(int MacroId) 
		{
			int totalProperties = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select count(*) from cmsMacroProperty where macro = @macroID", new SqlParameter("@macroID", MacroId)).ToString());
			int count = 0;
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select id from cmsMacroProperty where macro = @macroId order by macroPropertySortOrder", new SqlParameter("@macroId", MacroId)))
			{
				MacroProperty[] retval = new MacroProperty[totalProperties];
				while(dr.Read())
				{
					retval[count] = new MacroProperty(int.Parse(dr["id"].ToString()));
					count++;
				}
				return retval;
			}
		}

		/// <summary>
		/// Creates a new MacroProperty on a macro
		/// </summary>
		/// <param Name="M">The macro</param>
		/// <param Name="show">Will the editor be able to input data</param>
		/// <param Name="alias">The alias of the property</param>
		/// <param Name="name">Userfriendly MacroProperty Name</param>
		/// <param Name="propertyType">The MacroPropertyType of the property</param>
		/// <returns></returns>
		public static MacroProperty MakeNew(Macro M, bool show, string alias, string name, MacroPropertyType propertyType) 
		{
			return new MacroProperty( int.Parse(
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN,
				CommandType.Text, "SET NOCOUNT ON; insert into cmsMacroProperty (macro, macroPropertyHidden, macropropertyAlias, macroPropertyName, macroPropertyType) values (@macro, @show, @alias, @Name, @type) select @@identity as id", new SqlParameter("@macro", M.Id), new SqlParameter("@show", show), new SqlParameter("@alias", alias), new SqlParameter("@Name", name), new SqlParameter("@type", propertyType.Id)).ToString()));
		}

		#endregion

	}
}

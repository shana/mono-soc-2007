using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.Cms.BusinessLogic.macro
{
	/// <summary>
	/// The MacroPropertyType class contains information on the assembly and class of the 
	/// IMacroGuiRendering component and basedatatype
	/// 
	/// TODO: implement interface/abstract factory pattern
	/// </summary>
	public class MacroPropertyType
	{
		int _id;
		string _alias;
		string _assembly;
		string _type;
		string _baseType;

		/// <summary>
		/// Identifier
		/// </summary>
		public int Id 
		{
			get {return _id;}
		}

		/// <summary>
		/// The alias of the MacroPropertyType
		/// </summary>
		public string Alias 
		{get {return _alias;}}

		/// <summary>
		/// The assembly (without the .dll extension) used to retrieve the component at runtime
		/// </summary>
		public string Assembly 
		{get {return _assembly;}}

		/// <summary>
		/// The MacroPropertyType
		/// </summary>
		public string Type 
		{get {return _type;}}

		/// <summary>
		/// The IMacroGuiRendering component (namespace.namespace.Classname)
		/// </summary>
		public string BaseType 
		{get {return _baseType;}}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param Name="Id">Identifier</param>
		public MacroPropertyType(int Id)
		{
			_id = Id;
			setup();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param Name="Alias">The alias of the MacroPropertyType</param>
		public MacroPropertyType(string Alias) 
		{
			_id = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select id from cmsMacroPropertyType where macroPropertyTypeAlias = @alias", new SqlParameter("@alias", Alias)).ToString());
			setup();
		}

		private void setup() 
		{
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select macroPropertyTypeAlias, macroPropertyTypeRenderAssembly, macroPropertyTypeRenderType, macroPropertyTypeBaseType from cmsMacroPropertyType where id = @id", new SqlParameter("@id", _id)))
			{
				if(dr.Read())
				{
					_alias = dr["macroPropertyTypeAlias"].ToString();
					_assembly = dr["macroPropertyTypeRenderAssembly"].ToString();
					_type = dr["macroPropertyTypeRenderType"].ToString();
					_baseType = dr["macroPropertyTypeBaseType"].ToString();
				}
			}
		}
	}
}

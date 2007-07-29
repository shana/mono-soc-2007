using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.editorControls
{
	/// <summary>
	/// Summary description for cms.Umbraco.Cms.BusinessLogic.datatype.DefaultDataKeyValue.
	/// </summary>
    public class DefaultDataKeyValue : Cms.BusinessLogic.datatype.DefaultData
	{
		public DefaultDataKeyValue(Cms.BusinessLogic.datatype.BaseDataType DataType)  : base(DataType)
		{}
		/// <summary>
		/// Ov
		/// </summary>
		/// <param Name="d"></param>
		/// <returns></returns>
		
		public override System.Xml.XmlNode ToXMl(System.Xml.XmlDocument d)
		{
			// Get the value from 
			string v = "";
			try 
			{
				SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
					umbraco.GlobalSettings.DbDSN,
					CommandType.Text,
					"Select [value] from cmsDataTypeprevalues where id in (" + Value.ToString() +")");

				while (dr.Read()) {
					if (v.Length == 0)
						v += dr["value"].ToString();
					else
						v += "," + dr["value"].ToString();
				}
				dr.Close();
			} 
			catch {}
			return d.CreateCDataSection(v);
		}
	}
}

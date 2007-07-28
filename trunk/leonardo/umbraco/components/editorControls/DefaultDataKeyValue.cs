using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.editorControls
{
	/// <summary>
	/// Summary description for cms.businesslogic.datatype.DefaultDataKeyValue.
	/// </summary>
    public class DefaultDataKeyValue : cms.businesslogic.datatype.DefaultData
	{
		public DefaultDataKeyValue(cms.businesslogic.datatype.BaseDataType DataType)  : base(DataType)
		{}
		/// <summary>
		/// Ov
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		
		public override System.Xml.XmlNode ToXMl(System.Xml.XmlDocument d)
		{
			// Get the value from 
			string v = "";
			try 
			{
				SqlDataReader dr = SqlHelper.ExecuteReader(
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

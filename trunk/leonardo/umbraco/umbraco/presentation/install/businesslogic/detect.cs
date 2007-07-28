using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.presentation.install.businesslogic
{
	/// <summary>
	/// Class to detect umbraco version
	/// </summary>
	public class detect
	{
		public detect()
		{
		}

		public static DbVersion GetDatabaseVersion() 
		{
            // Test for 3
            try
            {
                SqlHelper.ExecuteNonQuery(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "select description from cmsContentType");
                return DbVersion.v3;
            }
            catch { }

            // Test for 2.1.1
            try
            {
                SqlHelper.ExecuteNonQuery(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "select newest from cmsDocument");
                return DbVersion.v211;
            }
            catch { }
			

			// Test for 2.1 release
			try 
			{
				SqlHelper.ExecuteNonQuery(
					GlobalSettings.DbDSN,
					CommandType.Text,
					"select alias from umbracoRelationType");
				return DbVersion.v21;
			} 
			catch {}
			
			// Test for 2.1 RC
			try 
			{
				SqlHelper.ExecuteNonQuery(
					GlobalSettings.DbDSN,
					CommandType.Text,
					"select validationRegExp from cmsPropertyType");
				return DbVersion.v21rc;
			} 
			catch {} 

			// Test for 2.0
			try 
			{
				SqlHelper.ExecuteNonQuery(
					GlobalSettings.DbDSN,
					CommandType.Text,
					"select id from cmsMacro");
				return DbVersion.v20;
			} 
			catch {} 

			// Test for empty database
			try 
			{
				SqlHelper.ExecuteNonQuery(
					GlobalSettings.DbDSN,
					CommandType.Text,
					"select id from sysobjects");
				return DbVersion.None;
			} 
			catch {} 

			return DbVersion.Unavailable;



		}
	}

	public enum DbVersion 
	{
		Unavailable,
		None,
		v20,
		v21rc,
		v21,
		v211,
        v3
	}
}

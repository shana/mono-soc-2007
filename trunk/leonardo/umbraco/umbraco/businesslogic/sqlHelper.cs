using System;

namespace umbraco
{
	/// <summary>
	/// Summary description for sqlHelper.
	/// </summary>
	public class sqlHelper
	{
		public sqlHelper()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string safeString(string text) 
		{
			return text.Replace("'", "''");
		}
	}
}

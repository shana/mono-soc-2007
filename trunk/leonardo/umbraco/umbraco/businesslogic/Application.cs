using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.BusinessLogic
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{
		private static string _ConnString = GlobalSettings.DbDSN;

		private string _name;
		private string _alias;
		private string _icon;

		public Application()
		{
		}
		public Application(string name,string alias, string icon)
		{
			this.name = name;
			this.alias = alias;
			this.icon = icon;
		}

		public string name 
		{
			get {return _name;}
			set {_name = value;}
		}

		public string alias 
		{
			get {return _alias;}
			set {_alias = value;}
		}

		public string icon 
		{
			get {return _icon;}
			set {_icon = value;}
		}

		public static Application[] getAll() {
			SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString,CommandType.Text,"Select appAlias, appIcon, appName from umbracoApp");
			System.Collections.ArrayList tmp = new System.Collections.ArrayList();

			while (dr.Read()) {
				tmp.Add(new Application(dr["appName"].ToString(),dr["appAlias"].ToString(), dr["appIcon"].ToString()));
			}
			dr.Close();
			Application[] retval = new Application[tmp.Count];
			for (int i = 0;i<tmp.Count;i++) retval[i] = (Application) tmp[i];
			return retval;
		}
	}
}

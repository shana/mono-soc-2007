using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Umbraco.BusinessLogic
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{
		private static string _connString = GlobalSettings.DbDSN;

		private string _name;
		private string _alias;
		private string _icon;

		/// <summary>
		/// Initializes a new instance of the <see cref="Application"/> class.
		/// </summary>
		public Application()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Application"/> class.
		/// </summary>
		/// <param Name="name">The Name.</param>
		/// <param Name="alias">The alias.</param>
		/// <param Name="icon">The icon.</param>
		public Application(string name, string alias, string icon)
		{
			Name = name;
			Alias = alias;
			Icon = icon;
		}

		/// <summary>
		/// Gets or sets the Name.
		/// </summary>
		/// <value>The Name.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>The alias.</value>
		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public string Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		public static Application[] GetAll()
		{
			// TODO: SQL
			SqlDataReader dr =
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_connString, CommandType.Text,
				                                                         "Select appAlias, appIcon, appName from umbracoApp");
			ArrayList tmp = new ArrayList();

			while (dr.Read())
			{
				tmp.Add(new Application(dr["appName"].ToString(), dr["appAlias"].ToString(), dr["appIcon"].ToString()));
			}
			dr.Close();
			Application[] retval = new Application[tmp.Count];
			for (int i = 0; i < tmp.Count; i++) retval[i] = (Application) tmp[i];
			return retval;
		}
	}
}
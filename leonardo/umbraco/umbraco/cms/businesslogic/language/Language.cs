using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using Microsoft.ApplicationBlocks.Data;

using Umbraco.Cms.BusinessLogic.cache;

namespace Umbraco.Cms.BusinessLogic.language
{
	/// <summary>
	/// The language class contains methods for creating and modifing installed languages.
	/// 
	/// A language is used internal in the Umbraco console for displaying languagespecific text and 
	/// in the public website for language/country specific representation of ex. date/time, currencies.
	/// 
	/// Besides by using the built in Dictionary you are able to store language specific bits and pieces of translated text
	/// for use in templates.
	/// </summary>
	public class Language
	{
		private int _id;
		private string _name = "";
		private string _friendlyName;
		private string _cultureAlias;

		private static string _ConnString = GlobalSettings.DbDSN;
		private static object getLanguageSyncLock = new object();

		/// <param Name="id">Id of the language</param>
		public Language(int id)
		{
			_id = id;
			_cultureAlias = Cache.GetCacheItem<string>("UmbracoLanguage" + id, getLanguageSyncLock, TimeSpan.FromMinutes(60),
				delegate
				{
					return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
						"select languageISOCode from umbracoLanguage where id = @LanguageId", new SqlParameter("@LanguageId", id)).ToString();
				});

			updateNames();
		}

		/// <summary>
		/// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
		/// </summary>
		public virtual void Save()
		{
			Cache.ClearCacheItem("UmbracoLanguage" + id);
		}

		private void updateNames()
		{
			try
			{
				CultureInfo ci = new CultureInfo(_cultureAlias);
				_friendlyName = ci.DisplayName;
			}
			catch
			{
				_friendlyName = _name + "(unknown Culture)";
			}
		}

		/// <summary>
		/// Creates a new language given the culture code - ie. da-dk  (denmark)
		/// </summary>
		/// <param Name="CultureCode">Culturecode of the language</param>
		public static void MakeNew(string CultureCode)
		{
			if(new CultureInfo(CultureCode) != null)
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
					"insert into umbracoLanguage (languageISOCode) values (@CultureCode)",
					new SqlParameter("@CultureCode", CultureCode));
		}

		/// <summary>
		/// Deletes the current Language.
		/// 
		/// Notice: this can have various sideeffects - use with care.
		/// </summary>
		public void Delete()
		{
			Cache.ClearCacheItem("UmbracoLanguage" + id);
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from umbracoLanguage where id = @id",
				new SqlParameter("@id", id));
		}

		/// <summary>
		/// Method for accessing all installed languagess
		/// </summary>
		public static Language[] getAll
		{
			get
			{
				List<Language> tmp = new List<Language>();

				using(SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "select id from umbracoLanguage"))
				{
					while(dr.Read())
						tmp.Add(new Language(int.Parse(dr["id"].ToString())));
				}

				return tmp.ToArray();
			}
		}

		public static Language GetByCultureCode(String CultureCode)
		{
			if(new CultureInfo(CultureCode) != null)
			{
				object sqlLangId =
					Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
						"select id from umbracoLanguage where languageISOCode = @CultureCode",
						new SqlParameter("@CultureCode", CultureCode));
				if(sqlLangId != null)
				{
					int langId;
					if(int.TryParse(sqlLangId.ToString(), out langId))
					{
						return new Language(langId);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// The id used by Umbraco to identify the language
		/// </summary>
		public int id
		{
			get { return _id; }
		}

		/// <summary>
		/// The culture code of the language: ie. Danish/Denmark da-dk
		/// </summary>
		public string CultureAlias
		{
			get { return _cultureAlias; }
			set
			{
				_cultureAlias = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
					"update umbracoLanguage set languageISOCode = @cultureAlias where id = @id", new SqlParameter("@id", id),
					new SqlParameter("@cultureAlias", _cultureAlias));
				updateNames();
			}
		}

		/// <summary>
		/// The user friendly Name of the language/country
		/// </summary>
		public string FriendlyName
		{
			get { return _friendlyName; }
		}
	}
}
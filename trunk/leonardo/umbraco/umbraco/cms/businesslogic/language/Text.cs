using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.Cms.BusinessLogic.language
{
	/// <summary>
	/// Item class contains method for accessing language translated text, its a generic component which
	/// can be used for storing language translated content, all items are associated to an unique identifier (Guid)
	/// 
	/// The data is cached and are usable in the public website.
	/// 
	/// Primarily used by the built-in dictionary
	/// 
	/// </summary>
	public class Item
	{
		private static Hashtable _items = Hashtable.Synchronized(new Hashtable());
		private static bool isInitialized = false;
		private static string _Connstring = GlobalSettings.DbDSN;
		
		private static void ensureData() 
		{
			if (!isInitialized) 
			{
				// load all data
				SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_Connstring,CommandType.Text,"Select LanguageId, UniqueId,[value] from cmsLanguageText order by UniqueId");
							
				while(dr.Read()) 
				{
					string LanguageId = dr["LanguageId"].ToString();
					string UniqueId = dr["UniqueId"].ToString();
					string text = dr["value"].ToString();		
					updateCache(int.Parse(LanguageId),new Guid(UniqueId),text);
				}
				isInitialized = true;
				dr.Close();
			}
		}
		
		private static void updateCache(int LanguageId, Guid key, string text) 
		{
			// test if item already exist in items and update internal data or insert new internal data
			if (_items.ContainsKey(key)) 
			{
				System.Collections.Hashtable languagevalues = (System.Collections.Hashtable) _items[key];
				// check if the current language key is used
				if(languagevalues.ContainsKey(LanguageId)) 
				{
					languagevalues[LanguageId] = text;
				} 
				else 
				{
					languagevalues.Add(LanguageId, text);
				}
			}	
			else 
			{
				// insert 
				Hashtable languagevalues = Hashtable.Synchronized(new Hashtable());
				languagevalues.Add(LanguageId, text);
				_items.Add(key,languagevalues);
			}
		}


		/// <summary>
		/// Retrieves the value of a languagetranslated item given the key
		/// </summary>
		/// <param Name="key">Unique identifier</param>
		/// <param Name="LanguageId">Umbraco languageid</param>
		/// <returns>The language translated text</returns>
		public static string Text(Guid key, int LanguageId) 
		{
			if (hasText(key,LanguageId))
				return ((System.Collections.Hashtable) _items[key])[LanguageId].ToString();
			else
				throw new ArgumentException("Key being requested does not exist");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param Name="key">Unique identifier</param>
		/// <param Name="LanguageId">Umbraco language id</param>
		/// <returns>returns True if there is a value associated to the unique identifier with the specified language</returns>
		public static bool hasText(Guid key, int LanguageId) 
		{
			ensureData();
			if (_items.ContainsKey(key)) 
			{
				System.Collections.Hashtable tmp = (System.Collections.Hashtable) _items[key];
				return tmp.ContainsKey(LanguageId);
			}
			return false;
		}
		/// <summary>
		/// Updates the value of the language translated item, throws an exeption if the
		/// key does not exist
		/// </summary>
		/// <param Name="LanguageId">Umbraco language id</param>
		/// <param Name="key">Unique identifier</param>
		/// <param Name="value">The new dictionaryvalue</param>
		
		public static void setText(int LanguageId, Guid key, string value) 
		{
			ensureData();
			if(!hasText(key,LanguageId)) throw new ArgumentException("Key does not exist");

			updateCache(LanguageId, key, value);
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_Connstring, CommandType.Text, "Update cmsLanguageText set [value] = @value where LanguageId = @languageId And UniqueId = @key",
                new SqlParameter("@value", value),
                new SqlParameter("@languageId", LanguageId),
                new SqlParameter("@key", key));
		}

		/// <summary>
		/// Adds a new languagetranslated item to the collection
		/// 
		/// </summary>
		/// <param Name="LanguageId">Umbraco languageid</param>
		/// <param Name="key">Unique identifier</param>
		/// <param Name="value"></param>
		public static void addText(int LanguageId, Guid key, string value)
		{
			ensureData();
			if(hasText(key,LanguageId)) throw new ArgumentException("Key being add'ed already exists");
			updateCache(LanguageId, key, value);
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_Connstring, CommandType.Text, "Insert Into cmsLanguageText (languageId,UniqueId,[value]) values (@languageId, @key, @value)",
                new SqlParameter("@languageId", LanguageId),
                new SqlParameter("@key", key),
                new SqlParameter("@value", value));
		}
		/// <summary>
		/// Removes all languagetranslated texts associated to the unique identifier.
		/// </summary>
		/// <param Name="key">Unique identifier</param>
		public static void removeText(Guid key) 
		{
			// remove from cache
			_items.Remove(key);
			// remove from database
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_Connstring, CommandType.Text, "Delete from cmsLanguageText where UniqueId =  @key",
                new SqlParameter("@key", key));		
		}
	}
}
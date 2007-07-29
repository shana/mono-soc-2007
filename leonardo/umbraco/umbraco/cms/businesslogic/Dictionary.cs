using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using Umbraco.Cms.BusinessLogic.language;

namespace Umbraco.Cms.BusinessLogic
{
    /// <summary>
    /// The Dictionary is used for storing and retrieving language translated textpieces in Umbraco. It uses
    /// Cms.Umbraco.Cms.BusinessLogic.language.Item class as storage and can be used from the public website of Umbraco
    /// all text are cached in memory.
    /// </summary>
    public class Dictionary
    {
        private static bool cacheIsEnsured = false;
        private static Hashtable DictionaryItems = new Hashtable();
        private static string _ConnString = GlobalSettings.DbDSN;
        private static Guid topLevelParent = new Guid("41c7638d-f529-4bff-853e-59a0c2fb1bde");

        private static void ensureCache()
        {
            if (!cacheIsEnsured)
            {
                SqlDataReader dr =
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "Select id, [key] from cmsDictionary");

                while (dr.Read())
                {
                    Guid tmp = new Guid(dr["id"].ToString());
                    string key = dr["key"].ToString();
                    DictionaryItems.Add(key, tmp);
                }
                dr.Close();
                cacheIsEnsured = true;
            }
        }

        /// <summary>
        /// Retrieve a list of toplevel DictionaryItems
        /// </summary>
        public static DictionaryItem[] getTopMostItems
        {
            get
            {
                ArrayList tmp = new ArrayList();
                SqlDataReader dr =
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                            "Select [Key] from cmsDictionary where parent = '" +
                                            topLevelParent.ToString() + "' order by [key]");
                while (dr.Read())
                {
                    tmp.Add(dr["key"]);
                }
                dr.Close();
                DictionaryItem[] retval = new DictionaryItem[tmp.Count];
                for (int i = 0; i < tmp.Count; i++) retval[i] = new DictionaryItem(tmp[i].ToString());
                return retval;
            }
        }

        /// <summary>
        /// A DictionaryItem is basically a key/value pair (key/language key/value) which holds the data
        /// associated to a key in various language translations
        /// </summary>
        public class DictionaryItem
        {
            private Guid _uniqueId;
            private string _key;


            public DictionaryItem(string key)
            {
                ensureCache();
                if (hasKey(key))
                {
                    _uniqueId = (Guid) DictionaryItems[key];
                    _key = key;
                }
                else throw new ArgumentException("No key " + key + " exists in dictionary");
            }

            public DictionaryItem(Guid id)
            {
                string key =
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                            "Select [key] from cmsDictionary where id = @id",
                                            new SqlParameter("@id", id)).ToString();

                ensureCache();
                if (hasKey(key))
                {
                    _uniqueId = (Guid) DictionaryItems[key];
                    _key = key;
                }
                else throw new ArgumentException("No key " + key + " exists in dictionary");
            }

            public DictionaryItem(int id)
            {
                string key =
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                            "Select [key] from cmsDictionary where pk = " + id.ToString()).ToString();

                ensureCache();
                if (hasKey(key))
                {
                    _uniqueId = (Guid) DictionaryItems[key];
                    _key = key;
                }
                else throw new ArgumentException("No key " + key + " exists in dictionary");
            }

            private DictionaryItem _parent;

            public bool IsTopMostItem()
            {
                return (Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                                "Select parent from cmsDictionary where pk = " +
                                                id.ToString()).ToString() == topLevelParent.ToString());
            }

            public DictionaryItem Parent
            {
                get
                {
                    if (_parent == null)
                    {
                        Guid parentGuid = new Guid(
                            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                                    "Select parent from cmsDictionary where pk = " +
                                                    id.ToString()).ToString());
                        if (parentGuid != topLevelParent)
                            _parent =
                                new DictionaryItem(parentGuid);
                        else
                            throw new ArgumentException("Top most dictionary items doesn't have a parent");
                    }

                    return _parent;
                }
            }

            public int id
            {
                get
                {
                    return
                        int.Parse(
                            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                                    "Select pk from cmsDictionary where [key] = '" + key + "'").ToString
                                ());
                }
            }


            public DictionaryItem[] Children
            {
                get
                {
                    ArrayList tmp = new ArrayList();
                    SqlDataReader dr =
                        Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                                "Select [Key] from cmsDictionary where parent = '" + _uniqueId + "'");
                    while (dr.Read())
                    {
                        tmp.Add(dr["key"]);
                    }
                    dr.Close();
                    DictionaryItem[] retval = new DictionaryItem[tmp.Count];
                    for (int i = 0; i < tmp.Count; i++) retval[i] = new DictionaryItem(tmp[i].ToString());
                    return retval;
                }
            }

            public static bool hasKey(string key)
            {
                ensureCache();
                return DictionaryItems.ContainsKey(key);
            }

            public bool hasChildren
            {
                get
                {
                    return (
                               int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString,
                                                                 CommandType.Text,
                                                                 "select count([key]) as tmp from cmsDictionary where parent = '" +
                                                                 _uniqueId.ToString() + "'").ToString()) > 0);
                }
            }

            public string key
            {
                get { return _key; }
                set
                {
                    if (!hasKey(value))
                    {
                        object tmp = DictionaryItems[key];
                        DictionaryItems.Remove(key);
                        _key = value;

                        DictionaryItems.Add(key, tmp);
                        Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                                  "Update cmsDictionary set [key] = '" + value + "'");
                    }
                    else
                        throw new ArgumentException("New value of key already exists (is key)");
                }
            }

            public string Value(int languageId)
            {
                if (Item.hasText(_uniqueId, languageId))
                    return Item.Text(_uniqueId, languageId);

                return "";
            }

            public void setValue(int languageId, string value)
            {
                if (Item.hasText(_uniqueId, languageId))
                    Item.setText(languageId, _uniqueId, value);
                else
                    Item.addText(languageId, _uniqueId, value);
            }

            public string Value()
            {
                return Item.Text(_uniqueId, 1);
            }

            public void setValue(string value)
            {
                if (Item.hasText(_uniqueId, 0))
                    Item.setText(0, _uniqueId, value);
                else
                    Item.addText(0, _uniqueId, value);
            }

            public static void addKey(string key, string defaultValue, string parentKey)
            {
                ensureCache();
                if (hasKey(parentKey))
                {
                    createKey(key, new DictionaryItem(parentKey)._uniqueId, defaultValue);
                }
                else
                    throw new ArgumentException("Parentkey doesnt exist");
            }

            public static void addKey(string key, string defaultValue)
            {
                ensureCache();
                createKey(key, topLevelParent, defaultValue);
            }

            public void delete()
            {
                // delete recursive
                foreach (DictionaryItem dd in Children)
                    dd.delete();

                // remove all language values from key
                Item.removeText(_uniqueId);

                // Remove key from cache
                DictionaryItems.Remove(key);

                // remove key from database
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "delete from cmsDictionary where [key] ='" + key + "'");
            }

            private static void createKey(string key, Guid parentId, string defaultValue)
            {
                if (!hasKey(key))
                {
                    Guid newId = Guid.NewGuid();
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "Insert into cmsDictionary (id,parent,[key]) values ('" + newId.ToString() +
                                              "','" + parentId.ToString() + "','" + key + "')");
                    DictionaryItems.Add(key, newId);
                    new DictionaryItem(key).setValue(defaultValue);
                }
                else
                {
                    throw new ArgumentException("Key being added already exist!");
                }
            }
        }
    }
}
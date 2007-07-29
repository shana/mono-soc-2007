using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using Umbraco.BusinessLogic;
using Umbraco.BusinessLogic.Actions;
using Umbraco.Cms.BusinessLogic.cache;
using Umbraco.Cms.BusinessLogic.language;
using Umbraco.interfaces;
using SqlHelper=Umbraco.SqlHelper;

namespace Umbraco.Cms.BusinessLogic.web
{
    /// <summary>
    /// Summary description for Domain.
    /// </summary>
    public class Domain
    {
        private Language _language;
        private string _name;
        private int _root;
        private int _id;

		public Domain(int Id)
        {
            initDomain(Id);
        }

        public Domain(string DomainName)
        {
            object result = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
				"select id from umbracoDomains where domainName = @DomainName",
				new SqlParameter("@DomainName", DomainName));
            if (result == null || !(result is int))
                throw new Exception(string.Format("Domain Name '{0}' does not exists", DomainName));
            initDomain((int) result);
        }

        private void initDomain(int id)
        {
            using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
				"select domainDefaultLanguage, domainRootStructureID, domainName from umbracoDomains where id = @ID",
				new SqlParameter("@ID", id)))
            {
                if (dr.Read())
                {
                    _id = id;
                    _language = new Language(int.Parse(dr["domainDefaultLanguage"].ToString()));
                    _name = dr["domainName"].ToString();
                    _root = int.Parse(dr["domainRootStructureID"].ToString());
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                        string.Format("update umbracoDomains set domainName = '{0}' where id = {1}",
                                                      Umbraco.SqlHelper.SafeString(value), _id));
                _name = value;
            }
        }

        public Language Language
        {
            get { return _language; }
            set
            {
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                        string.Format(
                                            "update umbracoDomains set domainDefaultLanguage = {0} where id = {1}",
                                            value.id, _id));
                _language = value;
            }
        }

        public int RootNodeId
        {
            get { return _root; }
            set
            {
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                        string.Format(
                                            "update umbracoDomains set domainRootStructureID = '{0}' where id = {1}",
                                            value, _id));
                _root = value;
            }
        }

        public int Id
        {
            get { return _id; }
        }

        public void Delete()
        {
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                    string.Format("delete from umbracoDomains where id = {0}", Id));
        	InvalidateCache();
        }

        #region Statics

		private static void InvalidateCache()
		{
			Cache.ClearCacheItem("UmbracoDomainList");
		}

		private static object getDomainsSyncLock = new object();
        private static List<Domain> GetDomains()
        {
			return Cache.GetCacheItem<List<Domain>>("UmbracoDomainList", getDomainsSyncLock, TimeSpan.FromMinutes(30),
        		delegate
        		{
        			List<Domain> result = new List<Domain>();
        			using(SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
						"select id, domainName from umbracoDomains"))
        			{
        				while(dr.Read())
        				{
        					string domainName = dr["domainName"] as string;
        					int domainId = (int)dr["id"];

							if (result.Find(delegate(Domain d) { return d.Name == domainName; }) == null)
								result.Add(new Domain(domainId));
							else
        						Log.Add(LogTypes.Error, User.GetUser(0), -1,
        							string.Format("Domain already exists in list ({0})", domainName));
        				}
        			}
        			return result;
        		});
        }

        public static Domain GetDomain(string DomainName)
        {
        	return GetDomains().Find(delegate(Domain d) { return d.Name == DomainName; });
        }

        public static int GetRootFromDomain(string DomainName)
        {
        	Domain d = GetDomain(DomainName);
			if (d == null) return -1;
        	return d._root;
        }

        public static Domain[] GetDomainsById(int nodeId)
        {
			return GetDomains().FindAll(delegate(Domain d) { return d._root == nodeId; }).ToArray();
        }

        public static bool Exists(string DomainName)
        {
			return GetDomain(DomainName) != null;
        }

        public static void MakeNew(string DomainName, int RootNodeId, int LanguageId)
        {
            if (Exists(DomainName.ToLower()))
                throw new Exception("Domain " + DomainName + " already exists!");
            else
            {
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                        string.Format(
                                            "insert into umbracoDomains (domainDefaultLanguage, domainRootStructureID, domainName) values ({0}, {1},'{2}')",
                                            LanguageId, RootNodeId, SqlHelper.SafeString(DomainName.ToLower())));
            	InvalidateCache();
            }
        }

        #endregion
    }

    public class DomainDeleteHandler : IActionHandler
    {
        #region IActionHandler Members

        public bool Execute(Document documentObject, IAction action)
        {
            foreach (Domain d in Domain.GetDomainsById(documentObject.Id))
            {
                d.Delete();
            }
            // TODO:  Add DomainDeleteHandler.Execute implementation
            return true;
        }

        public IAction[] ReturnActions()
        {
            // TODO:  Add DomainDeleteHandler.ReturnActions implementation
            IAction[] _retVal = {new ActionDelete()};
            return _retVal;
        }

        public string HandlerName()
        {
            // TODO:  Add DomainDeleteHandler.HandlerName implementation
            return "DomainDeleteHandler";
        }

        #endregion
    }
}
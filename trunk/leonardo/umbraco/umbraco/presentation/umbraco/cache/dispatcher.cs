using System;
using System.Xml;

namespace umbraco.presentation.cache
{
	/// <summary>
	/// Summary description for dispatcher.
	/// </summary>
	public class dispatcher
	{
		private static string _login = BusinessLogic.User.GetUser(UmbracoSettings.DistributedCallUser).LoginName;
		private static string _password = BusinessLogic.User.GetUser(UmbracoSettings.DistributedCallUser).Password;
		

		public dispatcher()
		{
		}

		public static void Refresh(Guid uniqueIdentifier, int Id) 
		{
			try 
			{
				foreach (XmlNode n in UmbracoSettings.DistributionServers.SelectNodes("./server")) 
				{
					CacheRefresher cr = new CacheRefresher();
					cr.Url = "http://" + xmlHelper.GetNodeValue(n) + GlobalSettings.Path + "/webservices/cacheRefresher.asmx";
					cr.RefreshById(uniqueIdentifier, Id, _login, _password);
				}
			} 
			catch (Exception ee)
			{
				BusinessLogic.Log.Add(
					BusinessLogic.LogTypes.Error,
					BusinessLogic.User.GetUser(0),
					-1,
					"Error refreshing '" + new Factory().GetNewObject(uniqueIdentifier).Name + "' with id '" + Id.ToString() + "', error: '" + ee.ToString() + "'");
			}
		}

		public static void Refresh(Guid uniqueIdentifier, Guid Id) 
		{
			try 
			{
				foreach (XmlNode n in UmbracoSettings.DistributionServers.SelectNodes("/server")) 
				{
					CacheRefresher cr = new CacheRefresher();
					cr.Url = "http://" + xmlHelper.GetNodeValue(n) + GlobalSettings.Path + "/webservices/cacheRefresher.asmx";
					cr.RefreshByGuid(uniqueIdentifier, Id, _login, _password);
				}
			} 
			catch (Exception ee)
			{
				BusinessLogic.Log.Add(
					BusinessLogic.LogTypes.Error,
					BusinessLogic.User.GetUser(0),
					-1,
					"Error refreshing '" + new Factory().GetNewObject(uniqueIdentifier).Name + "' with id '" + Id.ToString() + "', error: '" + ee.ToString() + "'");
			}
		}

		public static void RefreshAll(Guid uniqueIdentifier) 
		{
			try {
				foreach (XmlNode n in UmbracoSettings.DistributionServers.SelectNodes("/server")) 
				{
					CacheRefresher cr = new CacheRefresher();
					cr.Url = "http://" + xmlHelper.GetNodeValue(n) + GlobalSettings.Path + "/webservices/cacheRefresher.asmx";
					cr.RefreshAll(uniqueIdentifier, _login, _password);
				}
			} 
			catch (Exception ee)
			{
				BusinessLogic.Log.Add(
					BusinessLogic.LogTypes.Error,
					BusinessLogic.User.GetUser(0),
					-1,
					"Error refreshing all in '" + new Factory().GetNewObject(uniqueIdentifier).Name + "', error: '" + ee.ToString() + "'");
			}

		}


	}
}

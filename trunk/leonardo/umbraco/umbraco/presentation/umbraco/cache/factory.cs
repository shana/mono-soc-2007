using System;
using System.Web;

using umbraco.BusinessLogic.Utils;
using umbraco.interfaces;
using System.Collections.Generic;

namespace umbraco.presentation.cache
{
	/// <summary>
	/// Summary description for factory.
	/// </summary>
	public class Factory
	{
		#region Declarations

		private static string _pluginFolder = "";
		private static readonly Dictionary<Guid, Type> _refreshers = new Dictionary<Guid, Type>();

		#endregion

		#region Constructors

		static Factory()
		{
			Initialize();
		}

		#endregion

		#region Properties

		public static string PluginFolder
		{
			get { return _pluginFolder; }
		}

		#endregion

		#region Methods

		private static void Initialize()
		{
			// Updated to use reflection  26-08-2004, NH
			// Add'ed plugin-folder setting key in web.config
			_pluginFolder = GlobalSettings.Path + "/../bin";

			string[] types = TypeResolver.GetAssignablesFromType<ICacheRefresher>(
				HttpContext.Current.Server.MapPath(_pluginFolder), "*.dll");
			foreach(string type in types)
			{
				Type t = Type.GetType(type, false, true);
				if(t == null)
					continue;
				ICacheRefresher typeInstance = Activator.CreateInstance(t) as ICacheRefresher;
				if(typeInstance == null)
					continue;
				_refreshers.Add(typeInstance.UniqueIdentifier, t);
				//HttpContext.Current.Trace.Write("cache.factory", " + Adding cacheRefresher '" + typeInstance.Name);
			}
		}

		public ICacheRefresher CacheRefresher(Guid CacheRefresherId)
		{
			return GetNewObject(CacheRefresherId);
		}

		public ICacheRefresher GetNewObject(Guid CacheRefresherId)
		{
			ICacheRefresher newObject = Activator.CreateInstance(_refreshers[CacheRefresherId]) as ICacheRefresher;
			return newObject;
		}

		public ICacheRefresher[] GetAll()
		{
			ICacheRefresher[] retVal = new ICacheRefresher[_refreshers.Count];
			int c = 0;

			foreach(ICacheRefresher cr in _refreshers.Values)
			{
				retVal[c] = GetNewObject(cr.UniqueIdentifier);
				c++;
			}

			return retVal;
		}

		#endregion
	}
}
using System;
using System.Web.Caching;

namespace Umbraco.Cms.BusinessLogic
{
	/// <summary>
	/// This class implements helper methods to access the cache
	/// </summary>
	internal class CacheHelper
	{
		/// <summary>
		/// This delegates stores a referenc to the method that executes the real get action
		/// </summary>
		public delegate TT GetCacheItemDelegate<TT>();

		/// <summary>
		/// Gets the cache item.
		/// </summary>
		/// <typeparam name="TT">The type of the T.</typeparam>
		/// <param name="cacheKey">The cache key.</param>
		/// <param name="syncLock">The sync lock.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="refreshAction">The refresh action.</param>
		/// <param name="cacheDependency">The cache dependency.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="getCacheItem">The get cache item.</param>
		/// <returns></returns>
		public static TT GetCacheItem<TT>(string cacheKey, object syncLock,
			CacheItemPriority priority, CacheItemRemovedCallback refreshAction,
			CacheDependency cacheDependency, TimeSpan timeout, GetCacheItemDelegate<TT> getCacheItem)
		{
			object result = System.Web.HttpRuntime.Cache.Get(cacheKey);
			if (result == null)
			{
				lock (syncLock)
				{
					result = System.Web.HttpRuntime.Cache.Get(cacheKey);
					if (result == null)
					{
						result = getCacheItem();
						System.Web.HttpRuntime.Cache.Add(cacheKey, result, cacheDependency,
							DateTime.Now.Add(timeout), TimeSpan.Zero, priority, refreshAction);
					}
				}
			}
			return (TT)result;
		}

	}
}

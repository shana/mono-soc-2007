using System;
using System.Web;
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
		public delegate TCacheItem GetCacheItemDelegate<TCacheItem>();

		/// <summary>
		/// Gets the cache item.
		/// </summary>
		/// <typeparam name="TCacheItem">The type of the T.</typeparam>
		/// <param name="cacheKey">The cache key.</param>
		/// <param name="syncLock">The sync lock.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="refreshAction">The refresh action.</param>
		/// <param name="cacheDependency">The cache dependency.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="getCacheItem">The get cache item.</param>
		/// <returns></returns>
		public static TCacheItem GetCacheItem<TCacheItem>(string cacheKey, object syncLock,
		                                  CacheItemPriority priority, CacheItemRemovedCallback refreshAction,
		                                  CacheDependency cacheDependency, TimeSpan timeout,
		                                  GetCacheItemDelegate<TCacheItem> getCacheItem)
		{
			object result = HttpRuntime.Cache.Get(cacheKey);
			if (result == null)
			{
				lock (syncLock)
				{
					result = HttpRuntime.Cache.Get(cacheKey);
					if (result == null)
					{
						result = getCacheItem();
						HttpRuntime.Cache.Add(cacheKey, result, cacheDependency,
						                      DateTime.Now.Add(timeout), TimeSpan.Zero, priority, refreshAction);
					}
				}
			}
			return (TCacheItem) result;
		}
	}
}
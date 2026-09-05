using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Web.Core
{
    public class HelperCache
    {
        static readonly ObjectCache Cache = MemoryCache.Default;
        /// <summary>
        /// Thêm cache vào Memory
        /// </summary>
        /// <param name="objectToCache"></param>
        /// <param name="key"></param>
        /// <param name="time">Hours</param>
        public static void AddCache(object objectToCache, string key)
        {
            Cache.Add(key, objectToCache, DateTime.Now.AddHours(1));
        }
        /// <summary>
        /// Lấy cache từ Memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCache<T>(string key) where T : class
        {
            try
            {
                return (T)Cache[key];
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Xóa cache
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key)
        {
            Cache.Remove(key);
        }
        /// <summary>
        /// Xóa tất cả cache
        /// </summary>
        public static void ClearAllCache()
        {
            foreach (var cache in Cache)
            {
                Cache.Remove(cache.Key);
            }
        }
    }
}
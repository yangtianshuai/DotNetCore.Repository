using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Repository.Cache
{
    public static class RedisCacheExtension
    {
        /// <summary>
        /// 获取对象缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var buffer = cache.Get(key);
            if (buffer != null)
            {                
                var json = Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }

        /// <summary>
        /// 获取对象缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var buffer = await cache.GetAsync(key);
            if (buffer != null)
            {
                var json = Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }
        /// <summary>
        /// 获取或者创建缓存
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key">键值</param>
        /// <param name="factory">回溯方法</param>
        /// <returns></returns>
        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> factory, TimeSpan timespan)
        {
            var buffer = await cache.GetAsync(key);
            if (buffer == null)
            {               
                var data = await factory.Invoke();
                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    await cache.SetAsync(key, Encoding.UTF8.GetBytes(json)
                        , new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = timespan
                        });
                }
                else
                {
                    await cache.SetAsync(key, buffer, timespan);
                }              
                return data;
            }
            else
            {
                var json = Encoding.UTF8.GetString(buffer);
                var temp = JsonConvert.DeserializeObject<TItem>(json);
                return JsonConvert.DeserializeObject<TItem>(json);
            }
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="timespan">绝对超期时间</param>
        public static void Set(this IDistributedCache cache, string key, object obj, TimeSpan timespan)
        {
            var json = JsonConvert.SerializeObject(obj);
            cache.Set(key, Encoding.UTF8.GetBytes(json), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timespan
            });
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="timespan">绝对超期时间</param>
        public static async Task SetAsync(this IDistributedCache cache, string key, object obj, TimeSpan timespan)
        {
            var json = JsonConvert.SerializeObject(obj);
            await cache.SetAsync(key
                , Encoding.UTF8.GetBytes(json)
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timespan
                });
        }
    }
}
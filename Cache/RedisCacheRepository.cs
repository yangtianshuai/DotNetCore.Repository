using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetCore.Repository.Cache
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public abstract class RedisCacheRepository<T> : CacheRepository<T> where T : class
    {
        private readonly IDistributedCache _cache;

        public RedisCacheRepository(DbContext context, IDistributedCache cache) : base(context)
        {
            _cache = cache;
        }        

        protected override IEnumerable<T> GetCache(int? minutes = null)
        {
            if (minutes == null) minutes = outTimeMinutes;
            return GetCache(null, minutes.Value);
        }

        protected override IEnumerable<T> GetCache(Expression<Func<T, bool>> where)
        {
            return GetCache(where, outTimeMinutes);
        }

        protected override IEnumerable<T> GetCache(Expression<Func<T, bool>> where, int minutes)
        {
            string cacheType = GetCacheType();
            var cache = _cache.Get<IEnumerable<T>>(cacheType);
            if (cache == null)
            {
                this.IsCache = true;
                cache = base.List(null);
                minutes = outTimeMinutes;
                var span = TimeSpan.FromSeconds(minutes * 60 + new Random().Next(300));
                _cache.Set(cacheType, cache, span);
            }
            return cache;
        }

        protected override async Task<IEnumerable<T>> GetCacheAsync(int? minutes = null)
        {
            if (minutes == null) minutes = outTimeMinutes;
            return await GetCacheAsync(null, minutes.Value);
        }

        protected override async Task<IEnumerable<T>> GetCacheAsync(Expression<Func<T, bool>> where)
        {
            return await GetCacheAsync(where, outTimeMinutes);
        }

        protected override async Task<IEnumerable<T>> GetCacheAsync(Expression<Func<T, bool>> where, int minutes)
        {
            string cacheType = GetCacheType();
            //绝对过期增加随机防止发生缓存雪崩
            var timespan = TimeSpan.FromSeconds(minutes * 60 + new Random().Next(300));
            var datas = await _cache.GetOrCreateAsync(cacheType, async () =>
            {
                this.IsCache = true;                
                return await this.ListAsync(where);
            }, timespan);
            return datas;
        }

        protected override string GetCacheType()
        {
            var type = typeof(T);           
            return "R" + type.FullName.GetHashCode().ToString();
        }

        protected override async void UpdateCache()
        {
            await _cache.RemoveAsync(GetCacheType());
        }
       
    }
}
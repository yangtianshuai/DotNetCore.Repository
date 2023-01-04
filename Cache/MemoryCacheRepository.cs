using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCore.Repository.Cache
{
    public class MemoryCacheRepository<T> : CacheRepository<T> where T : class
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheRepository(DbContext context, IMemoryCache cache) : base(context)
        {
            this._cache = cache;
        }

        protected override async Task<IEnumerable<T>> GetCacheAsync(int? minutes)
        {
            if (minutes == null) minutes = outTimeMinutes;
            return await GetCacheAsync(null, minutes.Value);
        }

        protected override async Task<IEnumerable<T>> GetCacheAsync(Expression<Func<T, bool>> where = null)
        {
            return await GetCacheAsync(where, outTimeMinutes);
        }

        protected override async Task<IEnumerable<T>> GetCacheAsync(Expression<Func<T, bool>> where, int minutes)
        {
            string cacheType = GetCacheType();
            var datas = await _cache.GetOrCreateAsync(cacheType, async entry =>
            {
                this.IsCache = true;
                var span = TimeSpan.FromSeconds(minutes * 60 + new Random().Next(300));
                //绝对过期增加随机防止发生缓存雪崩
                entry.SetAbsoluteExpiration(span);
                return await base.ListAsync(where);
            });
            return datas;
        }
        protected override void UpdateCache()
        {
            _cache.Remove(GetCacheType());
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
            var cacheType = GetCacheType();
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

        protected override string GetCacheType()
        {
            var type = typeof(T);
            return "M" + type.FullName.GetHashCode().ToString();
        }
    }
}

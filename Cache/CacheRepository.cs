using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCore.Repository.Cache
{
    /// <summary>
    /// 缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CacheRepository<T> : Repository<T> where T : class
    {       
        protected readonly int outTimeMinutes = 60;
        public bool IsCache { get; set; }

        public CacheRepository(DbContext context) : base(context)
        {
            
        }

        #region --查询 
        public override T FirstOrDefault(Expression<Func<T, bool>> where)
        {
            if (IsCache)
            {
                var list = GetCache().ToList();
                if (list != null)
                {
                    return list.FirstOrDefault(where.Compile());
                }
            }
            return base.FirstOrDefault(where);
        }

        public override async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            if (IsCache)
            {
                var list = (await GetCacheAsync()).ToList();
                if (list != null)
                {
                    return list.FirstOrDefault(where.Compile());
                }
            }
            return await base.FirstOrDefaultAsync(where);
        }

        public override TResult FirstOrDefault<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {
            return base.FirstOrDefault(select, predicate);
        }

        public override async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {
            return await base.FirstOrDefaultAsync(select, predicate);
        }

        public override IEnumerable<T> List(Expression<Func<T, bool>> where = null)
        {
            if (IsCache)
            {
                var list = GetCache().ToList();
                if (list != null && where != null)
                {
                    return list.Where(where.Compile());
                }
            }
            return base.List(where);
        }

        public override IEnumerable<TResult> List<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {
            return base.List(select, predicate);
        }

        public override async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> where = null)
        {
            if (IsCache)
            {
                var list = (await GetCacheAsync()).ToList();
                if (list != null && where != null)
                {
                    return list.Where(where.Compile());
                }
            }
            return await base.ListAsync(where);
        }

        public override async Task<IEnumerable<TResult>> ListAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {
            return await base.ListAsync(select, predicate);
        }

        public override T SingleOrDefault(Expression<Func<T, bool>> where)
        {
            if (IsCache)
            {
                var list = GetCache().ToList();
                if (list != null)
                {
                    return list.SingleOrDefault(where.Compile());
                }
            }
            return base.SingleOrDefault(where);
        }

        public override TResult SingleOrDefault<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {
            return base.SingleOrDefault(select, predicate);
        }

        public override async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            if (IsCache)
            {
                var list = (await GetCacheAsync()).ToList();
                if (list != null)
                {
                    return list.SingleOrDefault(where.Compile());
                }
            }
            return await base.SingleOrDefaultAsync(where);
        }

        public override async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {
            return await base.SingleOrDefaultAsync(select, predicate);
        }
        #endregion

        protected abstract IEnumerable<T> GetCache(int? minutes = null);

        protected abstract IEnumerable<T> GetCache(Expression<Func<T, bool>> where);

        protected abstract IEnumerable<T> GetCache(Expression<Func<T, bool>> where, int minutes);

        protected abstract Task<IEnumerable<T>> GetCacheAsync(int? minutes);
        protected abstract Task<IEnumerable<T>> GetCacheAsync(Expression<Func<T, bool>> where = null);

        protected abstract Task<IEnumerable<T>> GetCacheAsync(Expression<Func<T, bool>> where, int minutes);

        protected abstract string GetCacheType();

        protected bool HasCache(string key)
        {
            return GetCacheType() == key;
        }

        protected abstract void UpdateCache();
       
        public override bool SaveChanges(bool refresh = false)
        {
            var flag = base.SaveChanges();
            if (flag || refresh)
            {
                UpdateCache();
            }
            return flag;
        }

        public override async Task<bool> SaveChangesAsync(bool refresh = false)
        {
            var flag = await base.SaveChangesAsync();
            if (flag || refresh)
            {
                UpdateCache();
            }
            return flag;
        }
    }
}
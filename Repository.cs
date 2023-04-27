using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace DotNetCore.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private DbContext _context;     
        private DbSet<T> _dbSet;
        private bool tracking = true;

        private Dictionary<string, IRepository> _repositories;

        public Repository(DbContext context)
        {
            _context = context;                 
            _dbSet = _context.Set<T>();
        }  
        
        public Repository<T> AsNoTracking()
        {           
            this.tracking = false;
            return this;
        }

        #region --新增
        public void Add(T item)
        {            
            _dbSet.Add(item);
        }

        public async Task AddAsync(T item)
        {
            await _dbSet.AddAsync(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            _dbSet.AddRange(items);
        }

        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            await _dbSet.AddRangeAsync(items);
        }
        #endregion

        #region --更新
        public void Update(T item)
        {
            _dbSet.Update(item);
        }

        public void Update(Expression<Func<T, bool>> where, Action<T> action)
        {
            var list = this.List(where);
            foreach (var model in list)
            {
                if (action != null)
                {
                    action(model);
                }
                this.Update(model);
            }
        }       

        public async Task UpdateAsync(Expression<Func<T, bool>> where, Action<T> action)
        {
            var list = await this.ListAsync(where);
            foreach (var model in list)
            {
                if (action != null)
                {
                    action(model);
                }
                this.Update(model);
            }
        }
        #endregion

        #region --删除
        public void Delete(T item)
        {
            _dbSet.Remove(item);
        }
        public void Delete(IEnumerable<T> items)
        {
            _dbSet.RemoveRange(items);
        }
        public void Delete(Expression<Func<T, bool>> where)
        {
            var list = List(where);
            if (list.Any())
            {
                Delete(list);
            }
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> where)
        {
            var list = await ListAsync(where);
            if (list.Any())
            {
                Delete(list);
            }
        }
        #endregion

        #region --查询
        public virtual IQueryable<T> Query(Expression<Func<T, bool>> where)
        {
            if (where == null)
            {
                if (this.tracking)
                {
                    return _dbSet.AsTracking();
                }
                else
                {
                    return _dbSet.AsNoTracking();
                }                
            }
            if (this.tracking)
            {
                return _dbSet.Where(where).AsTracking();
            }
            else
            {
                return _dbSet.Where(where).AsNoTracking();
            }
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> where)
        {
            return Query(where).FirstOrDefault();
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await Query(where).FirstOrDefaultAsync();
        }

        public virtual TResult FirstOrDefault<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {           
            return Query(predicate).Select(select).FirstOrDefault();
        }

        public virtual async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {           
            return await Query(predicate).Select(select).FirstOrDefaultAsync();
        }

        public virtual IEnumerable<T> List(Expression<Func<T, bool>> where = null)
        {            
            return Query(where);
        }

        public virtual IEnumerable<TResult> List<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {           
            return Query(predicate).Select(select).ToList();
        }

        public virtual async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> where = null)
        {            
            return await Query(where).ToListAsync();
        }

        public virtual async Task<IEnumerable<TResult>> ListAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {           
            return await Query(predicate).Select(select).ToListAsync();
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> where)
        {
            return Query(where).SingleOrDefault();
        }

        public virtual TResult SingleOrDefault<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {            
            return Query(predicate).Select(select).SingleOrDefault();
        }

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await Query(where).SingleOrDefaultAsync();
        }

        public virtual async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null)
        {           
            return await Query(predicate).Select(select).SingleOrDefaultAsync();
        }
        #endregion

        public IRepository Union(IRepository repository)
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, IRepository>();
            }
            var typeName = repository.GetType().Name;
            if (!this._repositories.ContainsKey(typeName))
            {
                this._repositories.Add(typeName, repository);
            }
            return this;
        }

        public virtual bool SaveChanges(bool refresh = false)
        {
            this.tracking = true;
            if (this._repositories != null && this._repositories.Count > 0)
            {
                return this.SaveChanges(refresh, this._repositories.Values.ToArray());
            }
            else
            {
                return _context.SaveChanges() > 0;
            }
        }

        public virtual async Task<bool> SaveChangesAsync(bool refresh = false)
        {
            this.tracking = true;
            if (this._repositories != null && this._repositories.Count > 0)
            {
                return await this.SaveChangesAsync(refresh, this._repositories.Values.ToArray());
            }
            else
            {
                return await _context.SaveChangesAsync() > 0;
            }
        }

        public bool SameContext(DbContext context)
        {
            var connect = _context.Database.GetDbConnection();
            return connect.Equals(context.Database.GetDbConnection());
        }

        public DbContext GetContext()
        {
            return this._context;
        }

        public DbSet<T> GetDbSet()
        {
            return this._dbSet;
        }

        private bool SaveChanges(bool refresh = false, params IRepository[] repositories)
        {
            //using (TransactionScope scope = new TransactionScope())
            //{
            //    foreach (var repository in repositories)
            //    {
            //        if (repository.GetContext().SaveChanges(false) <= 0)
            //        {
            //            //跳过
            //            break;
            //        }
            //    }
            //    scope.Complete();
            //    foreach (var repository in repositories)
            //    {
            //        if (repository.GetContext().AcceptAllChanges())
            //        {
            //            //跳过
            //            break;
            //        }
            //    }
            //}
            var count = 0;
            foreach (var repository in repositories)
            {
                if (repository.SameContext(this._context))
                {
                    count++;
                }
                if (repository.SaveChanges(refresh))
                {
                    count++;
                }
            }
            if (this.SaveChanges(refresh))
            {
                count++;
            }
            return count == (repositories.Count() + 1);

        }
        /// <summary>
        /// 多个数据库提交保存
        /// 注意：需开启msdts服务
        /// </summary>
        /// <param name="repositories"></param>
        /// <returns></returns>
        private async Task<bool> SaveChangesAsync(bool refresh = false, params IRepository[] repositories)
        {
            if (repositories == null)
            {
                return false;
            }

            var count = 0;
            foreach (var repository in repositories)
            {
                if (repository.SameContext(this._context))
                {
                    count++;
                    continue;
                }
                if (await repository.SaveChangesAsync())
                {
                    count++;
                }
            }
            this._repositories.Clear();
            this._repositories = null;
            if (await this.SaveChangesAsync())
            {
                count++;
            }
            return count == (repositories.Count() + 1);

        }
    }
}
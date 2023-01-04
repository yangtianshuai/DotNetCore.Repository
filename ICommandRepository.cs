using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCore.Repository
{
    public interface ICommandRepository<T> where T : class
    {
        void Add(T item);
        Task AddAsync(T item);
        void AddRange(IEnumerable<T> items);
        Task AddRangeAsync(IEnumerable<T> items);

        void Update(T item);       
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="where">修改数据的查询条件</param>
        /// <param name="action"></param>
        void Update(Expression<Func<T, bool>> where, Action<T> action);
        /// <summary>
        /// 异步修改
        /// </summary>
        /// <param name="where">修改数据的查询条件</param>
        /// <param name="action"></param>
        Task UpdateAsync(Expression<Func<T, bool>> where, Action<T> action);

        void Delete(T item);
        void Delete(IEnumerable<T> items);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where">删除数据的查询条件</param>
        void Delete(Expression<Func<T, bool>> where);
        /// <summary>
        /// 异步删除
        /// </summary>
        /// <param name="where">删除数据的查询条件</param>
        Task DeleteAsync(Expression<Func<T, bool>> where);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCore.Repository
{
    public interface IQueryRepository<T> where T : class
    {
        IEnumerable<T> List(Expression<Func<T, bool>> where = null);
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> where = null);

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="select">结果表达式</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        IEnumerable<TResult> List<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="select">结果表达式</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> ListAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        IQueryable<T> Query(Expression<Func<T, bool>> where);

        T FirstOrDefault(Expression<Func<T, bool>> where);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 获取第一个或者默认值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="select">结果表达式</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        TResult FirstOrDefault<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// 方法暂时不支持
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null);

        T SingleOrDefault(Expression<Func<T, bool>> where);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where);

        TResult SingleOrDefault<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// 方法暂时不支持
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> predicate = null);
    }
}

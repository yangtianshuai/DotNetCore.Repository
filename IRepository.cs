﻿using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DotNetCore.Repository
{
    public interface IRepository<T> : IRepository, IQueryRepository<T>, ICommandRepository<T> where T : class
    {
        /// <summary>
        /// 获取当前Context
        /// </summary>
        /// <returns></returns>
        DbContext GetContext();
        /// <summary>
        /// 获取DbSet
        /// </summary>
        /// <returns></returns>
        DbSet<T> GetDbSet();
        /// <summary>
        /// 开启不追踪
        /// </summary>
        /// <returns></returns>
        Repository<T> AsNoTracking();
    }

    public interface IRepository
    {
        bool SameContext(DbContext context);
        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        IRepository Union(IRepository repository);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        bool SaveChanges();
        /// <summary>
        /// 数据库提交
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveChangesAsync();
    }
}
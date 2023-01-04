using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetCore.Repository
{
    public static class DbContextExtension
    {
        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            
            return cmd;
        }
        /// <summary>
        /// 执行Sql（非查询）
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>
        /// <param name="close"></param>
        /// <returns></returns>
        public static async Task<bool> SqlExecuteAsync(this DatabaseFacade facade, string sql, bool close = false)
        {
            try
            {
                var command = CreateCommand(facade, sql, out DbConnection conn);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 执行Sql（非查询）
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>
        /// <param name="close"></param>
        /// <returns></returns>
        public static bool SqlExecute(this DatabaseFacade facade, string sql, bool close = false)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Sql查询
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>        
        /// <returns></returns>
        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, bool close = false)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn);
         
            var reader = command.ExecuteReader();
            var dt = new DataTable();           
            dt.Load(reader);
            if(close)
            {
                reader.Close();
            }           
            conn.Close();
            return dt;
        }

        /// <summary>
        /// Sql查询
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>        
        /// <returns></returns>
        public static void MutiSqlQuery(this DatabaseFacade facade, string sql,params DataTable[] tables)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn);

            var reader = command.ExecuteReader();
            var ds = new DataSet();
            ds.Load(reader,LoadOption.OverwriteChanges, tables);           
            conn.Close();
        }

        /// <summary>
        /// Sql查询(异步)
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>       
        /// <returns></returns>
        public static async Task<DataTable> SqlQueryAsync(this DatabaseFacade facade, string sql)
        {
            return await Task.Run(() => SqlQuery(facade, sql));
        }        

        /// <summary>
        /// Sql查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>        
        /// <returns></returns>
        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql) where T : class, new()
        {
            var dt = SqlQuery(facade, sql);
            return dt.ToList<T>();
        }       

        /// <summary>
        /// Sql查询(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="facade"></param>
        /// <param name="sql">Sql语句</param>       
        /// <returns></returns>
        public static async Task<List<T>> SqlQueryAsync<T>(this DatabaseFacade facade, string sql) where T : class, new()
        {
            var dt = await SqlQueryAsync(facade, sql);
            return dt.ToList<T>();
        }
        
        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            return list;
        }
    }
}
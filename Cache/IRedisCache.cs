using System.Threading.Tasks;

namespace DotNetCore.Repository.Cache
{
    public interface IRedisCache : ICache
    {
        /// <summary>
        /// 更新缓存
        /// </summary>
        Task UpdateCacheAsync();
    }
}
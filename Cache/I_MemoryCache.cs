namespace DotNetCore.Repository.Cache
{
    public interface I_MemoryCache : ICache
    {
        /// <summary>
        /// 更新缓存
        /// </summary>
        void UpdateCache();
    }
}

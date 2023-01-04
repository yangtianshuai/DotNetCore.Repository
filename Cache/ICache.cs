namespace DotNetCore.Repository.Cache
{
    public interface ICache
    {
        /// <summary>
        /// 是否已经缓存
        /// </summary>
        /// <param name="key">缓存码</param>
        /// <returns></returns>
        bool HasCache(string key);
        /// <summary>
        /// 获取缓存码
        /// </summary>
        /// <returns></returns>
        string GetCacheType();
    }
}
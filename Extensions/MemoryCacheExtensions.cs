using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace WeatherForecastAPI.Extension
{
    public static class MemoryCacheExtensions
    {
        public static IEnumerable<object> GetKeys(this IMemoryCache memoryCache)
        {
            if (memoryCache is MemoryCache memCache)
            {
                var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(memCache) as dynamic;
                List<object> cacheKeys = new List<object>();

                foreach (var cacheItem in cacheEntriesCollection)
                {
                    ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                    cacheKeys.Add(cacheItemValue.Key);
                }

                return cacheKeys;
            }
            throw new InvalidOperationException("Unable to retrieve cache keys.");
        }
    }
}

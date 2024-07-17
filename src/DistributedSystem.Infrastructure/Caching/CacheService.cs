using DistributedSystem.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace DistributedSystem.Infrastructure.Caching;

public class CacheService : ICacheService
{
    /**
     * Because we don't have any method to get all of the keys in redis
     * => Solution: Store key in memory at set value to redis
     * 
     * =>> Cache Service can be used concurrently, so we have to make sure that the data structure that we choose is thead safe
     * 
     * Khi mà làm với 1 cái gì đó mà có Key - Value mà với Cache như này mà mình set là async -> nó đang chạy trong việc 
     * concurrency tức là nhiều cái task chạy cùng với nhau 
     * => để mà quản lí theo kiểu ThreadSafe thì lúc đó phải dùng ConcurrentDictionary
     */
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = new ConcurrentDictionary<string, bool>(); // biến static vì mình lưu nó lại để xử lí
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) 
        where T : class
    {
        string? cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);

        if (cacheValue is null)
            return null;
        T? value = JsonConvert.DeserializeObject<T>(cacheValue);
        return value;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        string cacheValue = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);

        CacheKeys.TryAdd(key, false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
        CacheKeys.TryRemove(key, out bool _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        //foreach (string key in CacheKeys.Keys)
        //{
        //    if (key.StartsWith(prefixKey))
        //        await RemoveAsync(key, cancellationToken); // Call remove one by one
        //} // nó sẽ chỉ xóa key lần lượt theo vòng lặp trong khi đó mình đã xài asynchornous thì ko xài theo kiểu này

        // Để flexible hơn -> remove 1 phát 1 lần tất cả luôn -> giống kiểu publish đồng bộ rất nhiêu event 
        //-> chạy async rất là nhiều task cùng 1 lúc, thay vì chạy async từng task một 
        IEnumerable<Task> tasks = CacheKeys.Keys.Where(k => k.StartsWith(prefixKey))
            .Select(k => RemoveAsync(k, cancellationToken)); // -> chưa có await nên chưa remove -> đang tạo ra 1 danh sách mà các ptu của nó là Task function 

        await Task.WhenAll(tasks); // Execute in parallel
    }

    
}
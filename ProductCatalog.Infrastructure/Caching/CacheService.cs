using System;
using System.Collections.Generic;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Infrastructure.Caching
{

    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, CacheEntry> _cache = new();
        private readonly object _lock = new();

        public T? Get<T>(string key)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var entry))
                {
                    if (entry.Expiration == null || entry.Expiration > DateTime.UtcNow)
                    {
                        return (T?)entry.Value;
                    }
                    _cache.Remove(key);
                }
                return default;
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            lock (_lock)
            {
                var expirationTime = expiration.HasValue
                    ? DateTime.UtcNow.Add(expiration.Value)
                    : (DateTime?)null;

                _cache[key] = new CacheEntry(value, expirationTime);
            }
        }

        public void Remove(string key)
        {
            lock (_lock)
            {
                _cache.Remove(key);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        private record CacheEntry(object? Value, DateTime? Expiration);
    }
}
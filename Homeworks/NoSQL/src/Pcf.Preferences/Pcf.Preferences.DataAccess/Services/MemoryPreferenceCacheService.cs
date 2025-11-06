using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Pcf.Preferences.Core.Abstractions.Services;
using Pcf.Preferences.Core.Domain;

namespace Pcf.Preferences.DataAccess.Services
{
    /// <summary>
    /// Сервис кэширования предпочтений с использованием in-memory кэша
    /// </summary>
    public class MemoryPreferenceCacheService : IPreferenceCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private const string PREFERENCES_CACHE_KEY = "preferences:all";
        private const string PREFERENCE_CACHE_KEY_PREFIX = "preference:";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public MemoryPreferenceCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Preference>?> GetPreferencesFromCacheAsync()
        {
            return await Task.FromResult(_memoryCache.Get<IEnumerable<Preference>>(PREFERENCES_CACHE_KEY));
        }

        public async Task SetPreferencesToCacheAsync(IEnumerable<Preference> preferences)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };

            _memoryCache.Set(PREFERENCES_CACHE_KEY, preferences, options);
            await Task.CompletedTask;
        }

        public async Task<Preference?> GetPreferenceByIdFromCacheAsync(Guid id)
        {
            var cacheKey = $"{PREFERENCE_CACHE_KEY_PREFIX}{id}";
            return await Task.FromResult(_memoryCache.Get<Preference>(cacheKey));
        }

        public async Task ClearPreferencesCacheAsync()
        {
            _memoryCache.Remove(PREFERENCES_CACHE_KEY);
            await Task.CompletedTask;
        }
    }
}

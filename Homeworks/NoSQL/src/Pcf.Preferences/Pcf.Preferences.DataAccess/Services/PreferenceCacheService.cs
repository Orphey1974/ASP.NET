using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Pcf.Preferences.Core.Abstractions.Services;
using Pcf.Preferences.Core.Domain;

namespace Pcf.Preferences.DataAccess.Services
{
    /// <summary>
    /// Сервис кэширования предпочтений с использованием Redis
    /// </summary>
    public class PreferenceCacheService : IPreferenceCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private const string PREFERENCES_CACHE_KEY = "preferences:all";
        private const string PREFERENCE_CACHE_KEY_PREFIX = "preference:";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public PreferenceCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IEnumerable<Preference>?> GetPreferencesFromCacheAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(PREFERENCES_CACHE_KEY);

            if (string.IsNullOrEmpty(cachedData))
                return null;

            try
            {
                return JsonSerializer.Deserialize<IEnumerable<Preference>>(cachedData);
            }
            catch (JsonException)
            {
                // Если данные повреждены, очищаем кэш
                await _distributedCache.RemoveAsync(PREFERENCES_CACHE_KEY);
                return null;
            }
        }

        public async Task SetPreferencesToCacheAsync(IEnumerable<Preference> preferences)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };

            var jsonData = JsonSerializer.Serialize(preferences);
            await _distributedCache.SetStringAsync(PREFERENCES_CACHE_KEY, jsonData, options);
        }

        public async Task<Preference?> GetPreferenceByIdFromCacheAsync(Guid id)
        {
            var cacheKey = $"{PREFERENCE_CACHE_KEY_PREFIX}{id}";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedData))
                return null;

            try
            {
                return JsonSerializer.Deserialize<Preference>(cachedData);
            }
            catch (JsonException)
            {
                // Если данные повреждены, очищаем кэш
                await _distributedCache.RemoveAsync(cacheKey);
                return null;
            }
        }

        public async Task ClearPreferencesCacheAsync()
        {
            await _distributedCache.RemoveAsync(PREFERENCES_CACHE_KEY);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.Preferences.Core.Domain;

namespace Pcf.Preferences.Core.Abstractions.Services
{
    /// <summary>
    /// Сервис кэширования предпочтений
    /// </summary>
    public interface IPreferenceCacheService
    {
        /// <summary>
        /// Получить все предпочтения из кэша
        /// </summary>
        /// <returns>Список предпочтений</returns>
        Task<IEnumerable<Preference>?> GetPreferencesFromCacheAsync();

        /// <summary>
        /// Сохранить предпочтения в кэш
        /// </summary>
        /// <param name="preferences">Список предпочтений</param>
        /// <returns>Задача</returns>
        Task SetPreferencesToCacheAsync(IEnumerable<Preference> preferences);

        /// <summary>
        /// Получить предпочтение по идентификатору из кэша
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Предпочтение</returns>
        Task<Preference?> GetPreferenceByIdFromCacheAsync(Guid id);

        /// <summary>
        /// Очистить кэш предпочтений
        /// </summary>
        /// <returns>Задача</returns>
        Task ClearPreferencesCacheAsync();
    }
}

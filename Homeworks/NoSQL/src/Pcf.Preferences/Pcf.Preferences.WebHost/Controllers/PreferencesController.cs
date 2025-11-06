using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.Preferences.Core.Abstractions.Repositories;
using Pcf.Preferences.Core.Abstractions.Services;
using Pcf.Preferences.Core.Domain;
using Pcf.Preferences.WebHost.Models;

namespace Pcf.Preferences.WebHost.Controllers
{
    /// <summary>
    /// Контроллер для работы с предпочтениями
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IPreferenceCacheService _cacheService;

        public PreferencesController(
            IRepository<Preference> preferencesRepository,
            IPreferenceCacheService cacheService)
        {
            _preferencesRepository = preferencesRepository;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Получить все предпочтения (с кэшированием)
        /// </summary>
        /// <returns>Список предпочтений</returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            // Сначала пытаемся получить из кэша
            var cachedPreferences = await _cacheService.GetPreferencesFromCacheAsync();

            if (cachedPreferences != null)
            {
                var cachedResponse = cachedPreferences.Select(x => new PreferenceResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

                return Ok(cachedResponse);
            }

            // Если в кэше нет, получаем из базы данных
            var preferences = await _preferencesRepository.GetAllAsync();

            // Сохраняем в кэш
            await _cacheService.SetPreferencesToCacheAsync(preferences);

            var response = preferences.Select(x => new PreferenceResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить предпочтение по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <returns>Предпочтение</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PreferenceResponse>> GetPreferenceByIdAsync(Guid id)
        {
            // Сначала пытаемся получить из кэша
            var cachedPreference = await _cacheService.GetPreferenceByIdFromCacheAsync(id);

            if (cachedPreference != null)
            {
                return Ok(new PreferenceResponse
                {
                    Id = cachedPreference.Id,
                    Name = cachedPreference.Name,
                    Description = cachedPreference.Description
                });
            }

            // Если в кэше нет, получаем из базы данных
            var preference = await _preferencesRepository.GetByIdAsync(id);

            if (preference == null)
                return NotFound();

            return Ok(new PreferenceResponse
            {
                Id = preference.Id,
                Name = preference.Name,
                Description = preference.Description
            });
        }

        /// <summary>
        /// Создать новое предпочтение
        /// </summary>
        /// <param name="request">Данные предпочтения</param>
        /// <returns>Созданное предпочтение</returns>
        [HttpPost]
        public async Task<ActionResult<PreferenceResponse>> CreatePreferenceAsync(CreatePreferenceRequest request)
        {
            var preference = new Preference
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            await _preferencesRepository.AddAsync(preference);

            // Очищаем кэш, так как данные изменились
            await _cacheService.ClearPreferencesCacheAsync();

            return Created($"/api/v1/preferences/{preference.Id}",
                new PreferenceResponse
                {
                    Id = preference.Id,
                    Name = preference.Name,
                    Description = preference.Description
                });
        }

        /// <summary>
        /// Обновить предпочтение
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <param name="request">Данные предпочтения</param>
        /// <returns>Обновленное предпочтение</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<PreferenceResponse>> UpdatePreferenceAsync(Guid id, UpdatePreferenceRequest request)
        {
            var preference = await _preferencesRepository.GetByIdAsync(id);

            if (preference == null)
                return NotFound();

            preference.Name = request.Name;
            preference.Description = request.Description;

            await _preferencesRepository.UpdateAsync(preference);

            // Очищаем кэш, так как данные изменились
            await _cacheService.ClearPreferencesCacheAsync();

            return Ok(new PreferenceResponse
            {
                Id = preference.Id,
                Name = preference.Name,
                Description = preference.Description
            });
        }

        /// <summary>
        /// Удалить предпочтение
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreferenceAsync(Guid id)
        {
            var preference = await _preferencesRepository.GetByIdAsync(id);

            if (preference == null)
                return NotFound();

            await _preferencesRepository.DeleteAsync(preference);

            // Очищаем кэш, так как данные изменились
            await _cacheService.ClearPreferencesCacheAsync();

            return NoContent();
        }
    }
}

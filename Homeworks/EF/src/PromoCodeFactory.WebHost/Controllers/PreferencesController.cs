using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Контроллер для работы с предпочтениями клиентов.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IRepository<Preference> _preferenceRepository;

        public PreferencesController(IRepository<Preference> preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить все предпочтения.
        /// </summary>
        /// <returns>Список предпочтений.</returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await _preferenceRepository.GetAllAsync();
            var result = preferences.Select(p => new PreferenceResponse
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Получить предпочтение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор предпочтения.</param>
        /// <returns>Предпочтение.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PreferenceResponse>> GetPreferenceByIdAsync(Guid id)
        {
            var preference = await _preferenceRepository.GetByIdAsync(id);
            if (preference == null)
            {
                return NotFound();
            }

            var result = new PreferenceResponse
            {
                Id = preference.Id,
                Name = preference.Name
            };

            return Ok(result);
        }

        /// <summary>
        /// Создает новое предпочтение
        /// </summary>
        /// <param name="preferenceRequest">Данные для создания предпочтения</param>
        /// <returns>Созданное предпочтение</returns>
        [HttpPost]
        public async Task<ActionResult<PreferenceResponse>> CreatePreferenceAsync(PreferenceRequest preferenceRequest)
        {
            // Проверяем, что имя предпочтения не пустое
            if (string.IsNullOrWhiteSpace(preferenceRequest.Name))
            {
                return BadRequest("Название предпочтения не может быть пустым.");
            }

            var preference = new Preference
            {
                Id = Guid.NewGuid(),
                Name = preferenceRequest.Name
            };

            await _preferenceRepository.AddAsync(preference);
            await _preferenceRepository.SaveChangesAsync();

            var preferenceResponse = new PreferenceResponse
            {
                Id = preference.Id,
                Name = preference.Name
            };

            return Ok(preferenceResponse);
        }
    }
}

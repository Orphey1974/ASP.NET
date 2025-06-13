using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Контроллер для работы с промокодами
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly PromoCodeFactoryDbContext _dbContext;

        public PromocodesController(
            IRepository<PromoCode> promoCodeRepository,
            PromoCodeFactoryDbContext dbContext)
        {
            _promoCodeRepository = promoCodeRepository;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получает все промокоды в указанном диапазоне дат
        /// </summary>
        /// <param name="beginDate">Дата начала в формате yyyy-MM-dd</param>
        /// <param name="endDate">Дата окончания в формате yyyy-MM-dd</param>
        /// <returns>Список промокодов</returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(
            [FromQuery] string beginDate = null,
            [FromQuery] string endDate = null)
        {
            var query = _dbContext.PromoCodes.AsQueryable();

            // Фильтруем по датам, если они указаны
            if (!string.IsNullOrEmpty(beginDate) && DateTime.TryParse(beginDate, out var begin))
            {
                query = query.Where(pc => pc.BeginDate >= begin);
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
            {
                query = query.Where(pc => pc.EndDate <= end);
            }

            var promoCodes = await query.ToListAsync();

            var result = promoCodes.Select(pc => new PromoCodeShortResponse
            {
                Id = pc.Id,
                Code = pc.Code,
                ServiceInfo = pc.ServiceInfo,
                BeginDate = pc.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = pc.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = pc.PartnerName
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Создает промокод и выдает его клиентам с указанным предпочтением
        /// </summary>
        /// <param name="request">Данные для создания промокода</param>
        /// <returns>Результат создания промокода</returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            // Находим предпочтение по имени
            var preference = await _dbContext.Preferences
                .FirstOrDefaultAsync(p => p.Name == request.Preference);

            if (preference == null)
            {
                return BadRequest($"Предпочтение '{request.Preference}' не найдено");
            }

            // Находим клиентов с указанным предпочтением
            var customersWithPreference = await _dbContext.Customers
                .Include(c => c.CustomerPreferences)
                .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == preference.Id))
                .ToListAsync();

            if (!customersWithPreference.Any())
            {
                return BadRequest($"Клиенты с предпочтением '{request.Preference}' не найдены");
            }

            // Создаем промокоды для каждого клиента
            var promoCodes = new List<PromoCode>();
            foreach (var customer in customersWithPreference)
            {
                var promoCode = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1), // Промокод действителен месяц
                    PreferenceId = preference.Id,
                    CustomerId = customer.Id
                };

                promoCodes.Add(promoCode);
            }

            // Сохраняем промокоды в базу данных
            foreach (var promoCode in promoCodes)
            {
                await _promoCodeRepository.AddAsync(promoCode);
            }
            await _promoCodeRepository.SaveChangesAsync();

            return Ok(new {
                message = $"Создано {promoCodes.Count} промокодов для клиентов с предпочтением '{request.Preference}'",
                promoCodesCount = promoCodes.Count
            });
        }
    }
}
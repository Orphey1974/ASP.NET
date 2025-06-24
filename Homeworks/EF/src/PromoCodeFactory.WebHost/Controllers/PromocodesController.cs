using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;

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
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly PromoCodeFactoryDbContext _context;

        public PromocodesController(
            IRepository<PromoCode> promoCodeRepository,
            IRepository<Preference> preferenceRepository,
            IRepository<Employee> employeeRepository,
            PromoCodeFactoryDbContext context)
        {
            _promoCodeRepository = promoCodeRepository;
            _preferenceRepository = preferenceRepository;
            _employeeRepository = employeeRepository;
            _context = context;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <param name="beginDate">Дата начала в формате строки (yyyy-MM-dd)</param>
        /// <param name="endDate">Дата окончания в формате строки (yyyy-MM-dd)</param>
        /// <returns>Список промокодов</returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(
            [FromQuery] string beginDate = null,
            [FromQuery] string endDate = null)
        {
            var promoCodes = await _context.PromoCodes
                .Include(pc => pc.Preference)
                .Include(pc => pc.PartnerManager)
                .Include(pc => pc.Customer)
                .ToListAsync();

            // Фильтрация по датам, если указаны
            if (!string.IsNullOrEmpty(beginDate) && DateTime.TryParse(beginDate, out var begin))
            {
                promoCodes = promoCodes.Where(pc => pc.BeginDate >= begin).ToList();
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
            {
                promoCodes = promoCodes.Where(pc => pc.EndDate <= end).ToList();
            }

            var result = promoCodes.Select(pc => new PromoCodeShortResponse
            {
                Id = pc.Id,
                Code = pc.Code,
                ServiceInfo = pc.ServiceInfo,
                BeginDate = pc.BeginDate,
                EndDate = pc.EndDate,
                PartnerName = pc.PartnerName,
                PreferenceName = pc.Preference?.Name,
                CustomerName = pc.Customer != null ? $"{pc.Customer.FirstName} {pc.Customer.LastName}" : null
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <param name="request">Данные для создания промокода</param>
        /// <returns>Результат создания и выдачи промокода</returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Находим предпочтение по названию
            var preference = await _context.Preferences
                .FirstOrDefaultAsync(p => p.Name.Equals(request.Preference, StringComparison.OrdinalIgnoreCase));

            if (preference == null)
            {
                return BadRequest($"Предпочтение '{request.Preference}' не найдено");
            }

            // Находим клиентов с данным предпочтением
            var customersWithPreference = await _context.Customers
                .Include(c => c.CustomerPreferences)
                .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == preference.Id))
                .ToListAsync();

            if (!customersWithPreference.Any())
            {
                return BadRequest($"Клиенты с предпочтением '{request.Preference}' не найдены");
            }

            // Находим сотрудника-менеджера (берем первого PartnerManager)
            var partnerManager = await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Role.Name == "PartnerManager");

            if (partnerManager == null)
            {
                return BadRequest("Не найден партнерский менеджер");
            }

            // Создаем промокод
            var promoCode = new PromoCode
            {
                Id = Guid.NewGuid(),
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1), // По умолчанию на месяц
                PreferenceId = preference.Id,
                PartnerManagerId = partnerManager.Id
            };

            await _promoCodeRepository.AddAsync(promoCode);

            // Выдаем промокод первому клиенту с данным предпочтением
            var firstCustomer = customersWithPreference.First();
            promoCode.CustomerId = firstCustomer.Id;
            await _promoCodeRepository.UpdateAsync(promoCode);

            return Ok(new
            {
                Message = $"Промокод '{request.PromoCode}' создан и выдан клиенту {firstCustomer.FullName}",
                PromoCodeId = promoCode.Id,
                CustomerId = firstCustomer.Id,
                CustomerName = firstCustomer.FullName
            });
        }
    }
}
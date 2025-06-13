using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Контроллер для работы с клиентами
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly PromoCodeFactoryDbContext _dbContext;

        public CustomersController(
            IRepository<Customer> customerRepository,
            IRepository<PromoCode> promoCodeRepository,
            PromoCodeFactoryDbContext dbContext)
        {
            _customerRepository = customerRepository;
            _promoCodeRepository = promoCodeRepository;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получает список всех клиентов
        /// </summary>
        /// <returns>Список клиентов с краткой информацией</returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();

            var result = customers.Select(c => new CustomerShortResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Получает клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Информация о клиенте с предпочтениями и промокодами</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _dbContext.Customers
                .Include(c => c.CustomerPreferences)
                    .ThenInclude(cp => cp.Preference)
                .Include(c => c.PromoCodes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomerPreferences?
                    .Select(cp => new PreferenceResponse
                    {
                        Id = cp.Preference.Id,
                        Name = cp.Preference.Name
                    }).ToList() ?? new List<PreferenceResponse>(),
                PromoCodes = customer.PromoCodes?.Select(pc => new PromoCodeShortResponse
                {
                    Id = pc.Id,
                    Code = pc.Code,
                    ServiceInfo = pc.ServiceInfo,
                    BeginDate = pc.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = pc.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = pc.PartnerName
                }).ToList() ?? new List<PromoCodeShortResponse>()
            };

            return Ok(response);
        }

        /// <summary>
        /// Создает нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <returns>Результат создания клиента</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = request.PreferenceIds?.Select(pid => new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(), // Будет установлено после создания клиента
                    PreferenceId = pid
                }).ToList() ?? new List<CustomerPreference>()
            };

            // Устанавливаем правильный CustomerId для предпочтений
            foreach (var preference in customer.CustomerPreferences)
            {
                preference.CustomerId = customer.Id;
            }

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return Ok(new { id = customer.Id, message = "Клиент успешно создан" });
        }

        /// <summary>
        /// Обновляет информацию о клиенте
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Данные для обновления клиента</param>
        /// <returns>Результат обновления клиента</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _dbContext.Customers
                .Include(c => c.CustomerPreferences)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;

            // Удаляем существующие предпочтения
            _dbContext.CustomerPreferences.RemoveRange(customer.CustomerPreferences);

            // Добавляем новые предпочтения
            if (request.PreferenceIds != null && request.PreferenceIds.Any())
            {
                customer.CustomerPreferences = request.PreferenceIds
                    .Select(pid => new CustomerPreference
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        PreferenceId = pid
                    }).ToList();
            }

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Удаляет клиента и все связанные с ним промокоды
        /// </summary>
        /// <param name="id">Идентификатор клиента для удаления</param>
        /// <returns>Результат удаления клиента</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            // Получаем все промокоды клиента
            var customerPromoCodes = await _dbContext.PromoCodes
                .Where(pc => pc.CustomerId == id)
                .ToListAsync();

            // Удаляем промокоды клиента
            foreach (var promoCode in customerPromoCodes)
            {
                await _promoCodeRepository.DeleteAsync(promoCode.Id);
            }

            // Удаляем клиента
            await _customerRepository.DeleteAsync(id);
            await _customerRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}

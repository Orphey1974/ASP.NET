using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;
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
        private readonly IRepository<CustomerPreference> _customerPreferenceRepository;
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly PromoCodeFactoryDbContext _context;

        public CustomersController(
            IRepository<Customer> customerRepository,
            IRepository<CustomerPreference> customerPreferenceRepository,
            IRepository<PromoCode> promoCodeRepository,
            PromoCodeFactoryDbContext context)
        {
            _customerRepository = customerRepository;
            _customerPreferenceRepository = customerPreferenceRepository;
            _promoCodeRepository = promoCodeRepository;
            _context = context;
        }

        /// <summary>
        /// Получить список всех клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
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
        /// Получить клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Данные клиента с предпочтениями и промокодами</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerPreferences)
                .ThenInclude(cp => cp.Preference)
                .Include(c => c.PromoCodes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var response = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomerPreferences.Select(cp => new PreferenceResponse
                {
                    Id = cp.Preference.Id,
                    Name = cp.Preference.Name
                }).ToList(),
                PromoCodes = customer.PromoCodes.Select(pc => new PromoCodeShortResponse
                {
                    Id = pc.Id,
                    Code = pc.Code,
                    ServiceInfo = pc.ServiceInfo,
                    BeginDate = pc.BeginDate,
                    EndDate = pc.EndDate
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <returns>Результат создания</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            await _customerRepository.AddAsync(customer);

            // Добавляем предпочтения клиента
            if (request.PreferenceIds != null && request.PreferenceIds.Any())
            {
                foreach (var preferenceId in request.PreferenceIds)
                {
                    var customerPreference = new CustomerPreference
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        PreferenceId = preferenceId
                    };
                    await _customerPreferenceRepository.AddAsync(customerPreference);
                }
            }

            var response = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = new List<PreferenceResponse>(),
                PromoCodes = new List<PromoCodeShortResponse>()
            };

            // Возвращаем результат создания с явным указанием контроллера (в нижнем регистре)
            return CreatedAtAction(
                nameof(GetCustomerAsync),
                "customers", // контроллер с маленькой буквы
                new { id = customer.Id },
                response
            );
        }

        /// <summary>
        /// Обновить данные клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Данные для обновления</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;

            await _customerRepository.UpdateAsync(customer);

            // Удаляем старые предпочтения
            var existingPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == id)
                .ToListAsync();

            foreach (var preference in existingPreferences)
            {
                await _customerPreferenceRepository.DeleteAsync(preference);
            }

            // Добавляем новые предпочтения
            if (request.PreferenceIds != null && request.PreferenceIds.Any())
            {
                foreach (var preferenceId in request.PreferenceIds)
                {
                    var customerPreference = new CustomerPreference
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        PreferenceId = preferenceId
                    };
                    await _customerPreferenceRepository.AddAsync(customerPreference);
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Удаляем промокоды клиента
            var promoCodes = await _context.PromoCodes
                .Where(pc => pc.CustomerId == id)
                .ToListAsync();

            foreach (var promoCode in promoCodes)
            {
                promoCode.CustomerId = null; // Отвязываем от клиента
                await _promoCodeRepository.UpdateAsync(promoCode);
            }

            // Удаляем связи с предпочтениями
            var customerPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == id)
                .ToListAsync();

            foreach (var preference in customerPreferences)
            {
                await _customerPreferenceRepository.DeleteAsync(preference);
            }

            // Удаляем клиента
            await _customerRepository.DeleteAsync(customer);

            return NoContent();
        }
    }
}
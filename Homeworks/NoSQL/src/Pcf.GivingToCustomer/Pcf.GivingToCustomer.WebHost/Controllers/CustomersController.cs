using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;
using Pcf.GivingToCustomer.WebHost.Hubs;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IPreferencesGateway _preferencesGateway;
        private readonly IHubContext<CustomersHub> _hubContext;

        public CustomersController(
            IRepository<Customer> customerRepository,
            IPreferencesGateway preferencesGateway,
            IHubContext<CustomersHub> hubContext)
        {
            _customerRepository = customerRepository;
            _preferencesGateway = preferencesGateway;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers =  await _customerRepository.GetAllAsync();

            var response = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить клиента по id
        /// </summary>
        /// <param name="id">Id клиента, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            // Получаем предпочтения через шлюз
            var preferenceIds = customer.Preferences?.Select(p => p.PreferenceId).ToList() ?? new List<Guid>();
            var preferences = preferenceIds.Any()
                ? await _preferencesGateway.GetPreferencesByIdsAsync(preferenceIds)
                : new List<Preference>();

            var response = new CustomerResponse(customer, preferences);

            return Ok(response);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            //Получаем предпочтения через шлюз
            var preferences = await _preferencesGateway
                .GetPreferencesByIdsAsync(request.PreferenceIds);

            Customer customer = CustomerMapper.MapFromModel(request, preferences);

            await _customerRepository.AddAsync(customer);

            // Получаем полную информацию о клиенте для отправки через SignalR
            var customerResponse = new CustomerResponse(customer, preferences);

            // Отправляем уведомление через SignalR
            await _hubContext.Clients.All.SendAsync("CustomerCreated", customerResponse);

            return CreatedAtAction(nameof(GetCustomerAsync), new {id = customer.Id}, customer.Id);
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        /// <param name="id">Id клиента, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        /// <param name="request">Данные запроса></param>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            var preferences = await _preferencesGateway.GetPreferencesByIdsAsync(request.PreferenceIds);

            CustomerMapper.MapFromModel(request, preferences, customer);

            await _customerRepository.UpdateAsync(customer);

            // Получаем обновленную информацию о клиенте для отправки через SignalR
            var customerResponse = new CustomerResponse(customer, preferences);

            // Отправляем уведомление через SignalR
            await _hubContext.Clients.All.SendAsync("CustomerUpdated", customerResponse);

            return NoContent();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Id клиента, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(customer);

            // Отправляем уведомление через SignalR
            await _hubContext.Clients.All.SendAsync("CustomerDeleted", id);

            return NoContent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;
using Pcf.GivingToCustomer.WebHost.Protos;
using Google.Protobuf.WellKnownTypes;

namespace Pcf.GivingToCustomer.WebHost.Services
{
    /// <summary>
    /// gRPC сервис для работы с клиентами
    /// </summary>
    public class CustomersGrpcService : CustomersService.CustomersServiceBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IPreferencesGateway _preferencesGateway;
        private readonly ILogger<CustomersGrpcService> _logger;

        public CustomersGrpcService(
            IRepository<Customer> customerRepository,
            IPreferencesGateway preferencesGateway,
            ILogger<CustomersGrpcService> logger)
        {
            _customerRepository = customerRepository;
            _preferencesGateway = preferencesGateway;
            _logger = logger;
        }

        /// <summary>
        /// Получить список всех клиентов
        /// </summary>
        public override async Task<GetCustomersResponse> GetCustomers(
            Empty request,
            ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetCustomers вызван через gRPC");

                var customers = await _customerRepository.GetAllAsync();
                _logger.LogInformation("Получено {Count} клиентов из базы данных", customers.Count());

                var response = new GetCustomersResponse();
                response.Customers.AddRange(customers.Select(c => new CustomerShort
                {
                    Id = c.Id.ToString(),
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email
                }));

                _logger.LogInformation("GetCustomers успешно завершен, возвращено {Count} клиентов", response.Customers.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка клиентов");
                throw new RpcException(new Status(StatusCode.Internal, "Ошибка при получении списка клиентов"));
            }
        }

        /// <summary>
        /// Получить клиента по ID
        /// </summary>
        public override async Task<Protos.CustomerResponse> GetCustomer(
            GetCustomerRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Неверный формат ID клиента"));
                }

                var customer = await _customerRepository.GetByIdAsync(customerId);

                if (customer == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Клиент с ID {request.CustomerId} не найден"));
                }

                // Получаем предпочтения через шлюз
                var preferenceIds = customer.Preferences?.Select(p => p.PreferenceId).ToList() ?? new List<Guid>();
                var preferences = preferenceIds.Any()
                    ? await _preferencesGateway.GetPreferencesByIdsAsync(preferenceIds)
                    : new List<Preference>();

                return MapToGrpcCustomerResponse(customer, preferences);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиента по ID {CustomerId}", request.CustomerId);
                throw new RpcException(new Status(StatusCode.Internal, "Ошибка при получении клиента"));
            }
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        public override async Task<Protos.CustomerResponse> CreateCustomer(
            CreateCustomerRequest request,
            ServerCallContext context)
        {
            try
            {
                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Имя клиента не может быть пустым"));
                }

                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Фамилия клиента не может быть пустой"));
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Email клиента не может быть пустым"));
                }

                // Преобразуем строковые ID предпочтений в Guid
                var preferenceIds = new List<Guid>();
                foreach (var prefIdStr in request.PreferenceIds)
                {
                    if (Guid.TryParse(prefIdStr, out var prefId))
                    {
                        preferenceIds.Add(prefId);
                    }
                    else
                    {
                        _logger.LogWarning("Неверный формат ID предпочтения: {PreferenceId}", prefIdStr);
                    }
                }

                // Получаем предпочтения через шлюз
                var preferences = preferenceIds.Any()
                    ? await _preferencesGateway.GetPreferencesByIdsAsync(preferenceIds)
                    : new List<Preference>();

                // Создаем модель запроса для маппера
                var createRequest = new CreateOrEditCustomerRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PreferenceIds = preferenceIds
                };

                // Создаем клиента
                var customer = CustomerMapper.MapFromModel(createRequest, preferences);
                await _customerRepository.AddAsync(customer);

                // Возвращаем созданного клиента
                return MapToGrpcCustomerResponse(customer, preferences);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании клиента");
                throw new RpcException(new Status(StatusCode.Internal, "Ошибка при создании клиента"));
            }
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        public override async Task<Empty> UpdateCustomer(
            UpdateCustomerRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Неверный формат ID клиента"));
                }

                var customer = await _customerRepository.GetByIdAsync(customerId);

                if (customer == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Клиент с ID {request.CustomerId} не найден"));
                }

                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Имя клиента не может быть пустым"));
                }

                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Фамилия клиента не может быть пустой"));
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Email клиента не может быть пустым"));
                }

                // Преобразуем строковые ID предпочтений в Guid
                var preferenceIds = new List<Guid>();
                foreach (var prefIdStr in request.PreferenceIds)
                {
                    if (Guid.TryParse(prefIdStr, out var prefId))
                    {
                        preferenceIds.Add(prefId);
                    }
                    else
                    {
                        _logger.LogWarning("Неверный формат ID предпочтения: {PreferenceId}", prefIdStr);
                    }
                }

                // Получаем предпочтения через шлюз
                var preferences = preferenceIds.Any()
                    ? await _preferencesGateway.GetPreferencesByIdsAsync(preferenceIds)
                    : new List<Preference>();

                // Создаем модель запроса для маппера
                var updateRequest = new CreateOrEditCustomerRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PreferenceIds = preferenceIds
                };

                // Обновляем клиента
                CustomerMapper.MapFromModel(updateRequest, preferences, customer);
                await _customerRepository.UpdateAsync(customer);

                return new Empty();
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении клиента с ID {CustomerId}", request.CustomerId);
                throw new RpcException(new Status(StatusCode.Internal, "Ошибка при обновлении клиента"));
            }
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        public override async Task<Empty> DeleteCustomer(
            DeleteCustomerRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Неверный формат ID клиента"));
                }

                var customer = await _customerRepository.GetByIdAsync(customerId);

                if (customer == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Клиент с ID {request.CustomerId} не найден"));
                }

                await _customerRepository.DeleteAsync(customer);

                return new Empty();
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении клиента с ID {CustomerId}", request.CustomerId);
                throw new RpcException(new Status(StatusCode.Internal, "Ошибка при удалении клиента"));
            }
        }

        /// <summary>
        /// Преобразование доменной модели Customer в gRPC CustomerResponse
        /// </summary>
        private Protos.CustomerResponse MapToGrpcCustomerResponse(Customer customer, IEnumerable<Preference> preferences)
        {
            var response = new Protos.CustomerResponse
            {
                Id = customer.Id.ToString(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };

            // Маппинг предпочтений
            if (customer.Preferences != null && customer.Preferences.Any())
            {
                var preferencesDict = preferences.ToDictionary(p => p.Id, p => p.Name);
                foreach (var customerPreference in customer.Preferences)
                {
                    var preferenceInfo = new PreferenceInfo
                    {
                        Id = customerPreference.PreferenceId.ToString()
                    };

                    if (preferencesDict.ContainsKey(customerPreference.PreferenceId))
                    {
                        preferenceInfo.Name = preferencesDict[customerPreference.PreferenceId];
                    }

                    response.Preferences.Add(preferenceInfo);
                }
            }

            // Маппинг промокодов
            if (customer.PromoCodes != null && customer.PromoCodes.Any())
            {
                foreach (var promoCodeCustomer in customer.PromoCodes)
                {
                    var promoCode = promoCodeCustomer.PromoCode;
                    if (promoCode != null)
                    {
                        response.PromoCodes.Add(new PromoCodeInfo
                        {
                            Id = promoCode.Id.ToString(),
                            Code = promoCode.Code,
                            ServiceInfo = promoCode.ServiceInfo,
                            BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                            EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                            PartnerId = promoCode.PartnerId.ToString()
                        });
                    }
                }
            }

            return response;
        }
    }
}


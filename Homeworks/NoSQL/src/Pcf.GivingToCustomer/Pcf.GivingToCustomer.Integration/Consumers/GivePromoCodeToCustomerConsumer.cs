using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Messages;

namespace Pcf.GivingToCustomer.Integration.Consumers;

/// <summary>
/// Consumer для обработки события выдачи промокода клиентам
/// </summary>
public class GivePromoCodeToCustomerConsumer : IConsumer<GivePromoCodeToCustomerEvent>
{
    private readonly ILogger<GivePromoCodeToCustomerConsumer> _logger;
    private readonly IRepository<PromoCode> _promoCodesRepository;
    private readonly IRepository<Customer> _customersRepository;
    private readonly Pcf.GivingToCustomer.Core.Abstractions.Gateways.IPreferencesGateway _preferencesGateway;

    public GivePromoCodeToCustomerConsumer(
        ILogger<GivePromoCodeToCustomerConsumer> logger,
        IRepository<PromoCode> promoCodesRepository,
        IRepository<Customer> customersRepository,
        Pcf.GivingToCustomer.Core.Abstractions.Gateways.IPreferencesGateway preferencesGateway)
    {
        _logger = logger;
        _promoCodesRepository = promoCodesRepository;
        _customersRepository = customersRepository;
        _preferencesGateway = preferencesGateway;
    }

    public async Task Consume(ConsumeContext<GivePromoCodeToCustomerEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Получено событие выдачи промокода: PromoCodeId={PromoCodeId}, Code={Code}, PartnerId={PartnerId}",
            message.PromoCodeId,
            message.PromoCode,
            message.PartnerId);

        try
        {
            // Получаем предпочтение по ID
            var preference = await _preferencesGateway.GetPreferenceByIdAsync(message.PreferenceId);
            if (preference == null)
            {
                _logger.LogWarning("Предпочтение {PreferenceId} не найдено", message.PreferenceId);
                return;
            }

            // Получаем клиентов с этим предпочтением
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x => x.PreferenceId == preference.Id));

            // Создаем промокод напрямую
            var promoCode = new PromoCode
            {
                Id = message.PromoCodeId,
                PartnerId = message.PartnerId,
                Code = message.PromoCode,
                ServiceInfo = message.ServiceInfo,
                BeginDate = DateTime.SpecifyKind(DateTime.Parse(message.BeginDate), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(DateTime.Parse(message.EndDate), DateTimeKind.Utc),
                PreferenceId = preference.Id,
                Customers = new List<PromoCodeCustomer>()
            };

            // Добавляем клиентов к промокоду
            foreach (var customer in customers)
            {
                promoCode.Customers.Add(new PromoCodeCustomer
                {
                    CustomerId = customer.Id,
                    Customer = customer,
                    PromoCodeId = promoCode.Id,
                    PromoCode = promoCode
                });
            }

            // Сохраняем промокод
            await _promoCodesRepository.AddAsync(promoCode);

            _logger.LogInformation(
                "Промокод {PromoCodeId} успешно выдан {CustomerCount} клиентам",
                message.PromoCodeId,
                customers.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке события выдачи промокода: PromoCodeId={PromoCodeId}",
                message.PromoCodeId);
            throw;
        }
    }
}


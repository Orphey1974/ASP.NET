using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.ReceivingFromPartner.Core.Messages;

namespace Pcf.Administration.DataAccess.Consumers;

/// <summary>
/// Consumer для обработки события получения промокода от партнера
/// </summary>
public class PromoCodeReceivedFromPartnerConsumer : IConsumer<PromoCodeReceivedFromPartnerEvent>
{
    private readonly ILogger<PromoCodeReceivedFromPartnerConsumer> _logger;
    private readonly IMongoRepository<Employee> _employeeRepository;

    public PromoCodeReceivedFromPartnerConsumer(
        ILogger<PromoCodeReceivedFromPartnerConsumer> logger,
        IMongoRepository<Employee> employeeRepository)
    {
        _logger = logger;
        _employeeRepository = employeeRepository;
    }

    public async Task Consume(ConsumeContext<PromoCodeReceivedFromPartnerEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Получено событие получения промокода от партнера: PromoCodeId={PromoCodeId}, PartnerManagerId={PartnerManagerId}",
            message.PromoCodeId,
            message.PartnerManagerId);

        try
        {
            // Обновляем счетчик выданных промокодов только если указан менеджер партнера
            if (message.PartnerManagerId.HasValue)
            {
                var employee = await _employeeRepository.GetByIdAsync(message.PartnerManagerId.Value);

                if (employee == null)
                {
                    _logger.LogWarning("Сотрудник {PartnerManagerId} не найден", message.PartnerManagerId);
                    return;
                }

                employee.AppliedPromocodesCount++;

                await _employeeRepository.UpdateAsync(employee);

                _logger.LogInformation(
                    "Счетчик выданных промокодов для сотрудника {PartnerManagerId} обновлен. Новое значение: {Count}",
                    message.PartnerManagerId,
                    employee.AppliedPromocodesCount);
            }
            else
            {
                _logger.LogInformation(
                    "Менеджер партнера не указан для промокода {PromoCodeId}, счетчик не обновляется",
                    message.PromoCodeId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке события получения промокода от партнера: PromoCodeId={PromoCodeId}",
                message.PromoCodeId);
            throw;
        }
    }
}


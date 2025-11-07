using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Messages;

namespace Pcf.Administration.DataAccess.Consumers;

/// <summary>
/// Consumer для обработки события уведомления администрации о выдаче промокода менеджером партнера
/// </summary>
public class NotifyAdminAboutPartnerManagerPromoCodeConsumer : IConsumer<NotifyAdminAboutPartnerManagerPromoCodeEvent>
{
    private readonly ILogger<NotifyAdminAboutPartnerManagerPromoCodeConsumer> _logger;
    private readonly IMongoRepository<Employee> _employeeRepository;

    public NotifyAdminAboutPartnerManagerPromoCodeConsumer(
        ILogger<NotifyAdminAboutPartnerManagerPromoCodeConsumer> logger,
        IMongoRepository<Employee> employeeRepository)
    {
        _logger = logger;
        _employeeRepository = employeeRepository;
    }

    public async Task Consume(ConsumeContext<NotifyAdminAboutPartnerManagerPromoCodeEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Получено событие уведомления о выдаче промокода менеджером партнера: PartnerManagerId={PartnerManagerId}",
            message.PartnerManagerId);

        try
        {
            var employee = await _employeeRepository.GetByIdAsync(message.PartnerManagerId);

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
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке события уведомления о выдаче промокода: PartnerManagerId={PartnerManagerId}",
                message.PartnerManagerId);
            throw;
        }
    }
}


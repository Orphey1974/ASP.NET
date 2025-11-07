using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Messages;

namespace Pcf.GivingToCustomer.Integration.Consumers;

/// <summary>
/// Consumer для обработки событий уведомления партнера
/// </summary>
public class PartnerNotificationConsumer : IConsumer<PartnerNotificationEvent>
{
    private readonly ILogger<PartnerNotificationConsumer> _logger;

    public PartnerNotificationConsumer(ILogger<PartnerNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PartnerNotificationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Получено уведомление для партнера {PartnerId}: {Message}",
            message.PartnerId,
            message.Message);

        // Здесь можно добавить логику обработки уведомления
        // Например, отправка email, SMS, push-уведомления и т.д.

        await Task.CompletedTask;
    }
}


using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Messages;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class NotificationGateway : INotificationGateway
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public NotificationGateway(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendNotificationToPartnerAsync(Guid partnerId, string message)
        {
            var notificationEvent = new PartnerNotificationEvent
            {
                PartnerId = partnerId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(notificationEvent);
        }
    }
}
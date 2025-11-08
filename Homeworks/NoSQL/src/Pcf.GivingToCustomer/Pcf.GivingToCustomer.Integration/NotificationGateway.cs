using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Messages;

namespace Pcf.GivingToCustomer.Integration
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
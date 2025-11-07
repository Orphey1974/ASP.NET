using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.Administration.Core.Messages;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class AdministrationGateway : IAdministrationGateway
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public AdministrationGateway(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            var @event = new NotifyAdminAboutPartnerManagerPromoCodeEvent
            {
                PartnerManagerId = partnerManagerId,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(@event);
        }
    }
}
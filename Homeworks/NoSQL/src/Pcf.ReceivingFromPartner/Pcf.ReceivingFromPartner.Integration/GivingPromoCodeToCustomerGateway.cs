using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.GivingToCustomer.Core.Messages;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGateway : IGivingPromoCodeToCustomerGateway
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public GivingPromoCodeToCustomerGateway(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var @event = new GivePromoCodeToCustomerEvent
            {
                PromoCodeId = promoCode.Id,
                PartnerId = promoCode.Partner.Id,
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId,
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(@event);
        }
    }
}
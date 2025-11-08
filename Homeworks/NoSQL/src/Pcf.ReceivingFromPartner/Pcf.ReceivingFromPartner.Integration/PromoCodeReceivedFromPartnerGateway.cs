using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.Messages;

namespace Pcf.ReceivingFromPartner.Integration;

/// <summary>
/// Gateway для отправки события получения промокода от партнера
/// </summary>
public class PromoCodeReceivedFromPartnerGateway
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PromoCodeReceivedFromPartnerGateway(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPromoCodeReceivedEvent(PromoCode promoCode)
    {
        var @event = new PromoCodeReceivedFromPartnerEvent
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


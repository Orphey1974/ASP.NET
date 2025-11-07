using System;
using System.Linq;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions.Services;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration;

namespace Pcf.ReceivingFromPartner.DataAccess.Services;

/// <summary>
/// Сервис для работы с промокодами от партнеров
/// </summary>
public class PromoCodeService : IPromoCodeService
{
    private readonly IRepository<Partner> _partnersRepository;
    private readonly IPreferencesGateway _preferencesGateway;
    private readonly PromoCodeReceivedFromPartnerGateway _promoCodeReceivedGateway;

    public PromoCodeService(
        IRepository<Partner> partnersRepository,
        IPreferencesGateway preferencesGateway,
        PromoCodeReceivedFromPartnerGateway promoCodeReceivedGateway)
    {
        _partnersRepository = partnersRepository;
        _preferencesGateway = preferencesGateway;
        _promoCodeReceivedGateway = promoCodeReceivedGateway;
    }

    public async Task<PromoCode> ReceivePromoCodeFromPartnerWithPreferenceAsync(
        Guid partnerId,
        string promoCode,
        Guid preferenceId,
        DateTime beginDate,
        DateTime endDate,
        string serviceInfo,
        Guid? partnerManagerId = null)
    {
        var partner = await _partnersRepository.GetByIdAsync(partnerId);

        if (partner == null)
        {
            throw new InvalidOperationException("Партнер не найден");
        }

        var activeLimit = partner.PartnerLimits.FirstOrDefault(x
            => !x.CancelDate.HasValue && x.EndDate > DateTime.Now);

        if (activeLimit == null)
        {
            throw new InvalidOperationException("Нет доступного лимита на предоставление промокодов");
        }

        if (partner.NumberIssuedPromoCodes + 1 > activeLimit.Limit)
        {
            throw new InvalidOperationException("Лимит на выдачу промокодов превышен");
        }

        if (partner.PromoCodes.Any(x => x.Code == promoCode))
        {
            throw new InvalidOperationException("Данный промокод уже был выдан ранее");
        }

        // Получаем предпочтение по ID через шлюз
        var preference = await _preferencesGateway.GetPreferenceByIdAsync(preferenceId);

        if (preference == null)
        {
            throw new InvalidOperationException("Предпочтение не найдено");
        }

        // Создаем промокод
        var promoCodeEntity = new PromoCode
        {
            PartnerId = partner.Id,
            Partner = partner,
            Code = promoCode,
            ServiceInfo = serviceInfo,
            BeginDate = beginDate,
            EndDate = endDate,
            Preference = preference,
            PreferenceId = preference.Id,
            PartnerManagerId = partnerManagerId
        };

        partner.PromoCodes.Add(promoCodeEntity);
        partner.NumberIssuedPromoCodes++;

        await _partnersRepository.UpdateAsync(partner);

        // Отправка единого события в RabbitMQ для обработки микросервисами GivingToCustomer и Administration
        await _promoCodeReceivedGateway.PublishPromoCodeReceivedEvent(promoCodeEntity);

        return promoCodeEntity;
    }
}


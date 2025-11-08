using System;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Services;

/// <summary>
/// Сервис для работы с промокодами от партнеров
/// </summary>
public interface IPromoCodeService
{
    /// <summary>
    /// Получить промокод от партнера с предпочтением
    /// </summary>
    /// <param name="partnerId">Идентификатор партнера</param>
    /// <param name="promoCode">Код промокода</param>
    /// <param name="preferenceId">Идентификатор предпочтения</param>
    /// <param name="beginDate">Дата начала действия промокода</param>
    /// <param name="endDate">Дата окончания действия промокода</param>
    /// <param name="serviceInfo">Информация о сервисе</param>
    /// <param name="partnerManagerId">Идентификатор менеджера партнера (опционально)</param>
    /// <returns>Созданный промокод</returns>
    Task<PromoCode> ReceivePromoCodeFromPartnerWithPreferenceAsync(
        Guid partnerId,
        string promoCode,
        Guid preferenceId,
        DateTime beginDate,
        DateTime endDate,
        string serviceInfo,
        Guid? partnerManagerId = null);
}


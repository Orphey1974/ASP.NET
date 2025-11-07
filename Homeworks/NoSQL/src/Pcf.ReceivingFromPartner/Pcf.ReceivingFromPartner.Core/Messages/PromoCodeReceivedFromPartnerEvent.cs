namespace Pcf.ReceivingFromPartner.Core.Messages;

/// <summary>
/// Событие получения промокода от партнера
/// </summary>
public class PromoCodeReceivedFromPartnerEvent
{
    /// <summary>
    /// Идентификатор промокода
    /// </summary>
    public Guid PromoCodeId { get; set; }

    /// <summary>
    /// Код промокода
    /// </summary>
    public string PromoCode { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор партнера
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Идентификатор предпочтения
    /// </summary>
    public Guid PreferenceId { get; set; }

    /// <summary>
    /// Дата начала действия промокода
    /// </summary>
    public string BeginDate { get; set; } = string.Empty;

    /// <summary>
    /// Дата окончания действия промокода
    /// </summary>
    public string EndDate { get; set; } = string.Empty;

    /// <summary>
    /// Информация о сервисе
    /// </summary>
    public string ServiceInfo { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор менеджера партнера (опционально)
    /// </summary>
    public Guid? PartnerManagerId { get; set; }

    /// <summary>
    /// Дата и время создания события
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


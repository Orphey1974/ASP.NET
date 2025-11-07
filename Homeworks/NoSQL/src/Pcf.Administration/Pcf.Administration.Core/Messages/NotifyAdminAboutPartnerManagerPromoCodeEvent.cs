namespace Pcf.Administration.Core.Messages;

/// <summary>
/// Событие уведомления администрации о выдаче промокода менеджером партнера
/// </summary>
public class NotifyAdminAboutPartnerManagerPromoCodeEvent
{
    /// <summary>
    /// Идентификатор менеджера партнера
    /// </summary>
    public Guid PartnerManagerId { get; set; }

    /// <summary>
    /// Дата и время создания события
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


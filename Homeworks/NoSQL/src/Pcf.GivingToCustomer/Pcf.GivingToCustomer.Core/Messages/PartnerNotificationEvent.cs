namespace Pcf.GivingToCustomer.Core.Messages;

/// <summary>
/// Событие уведомления партнера
/// </summary>
public class PartnerNotificationEvent
{
    /// <summary>
    /// Идентификатор партнера
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Сообщение для партнера
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время создания события
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


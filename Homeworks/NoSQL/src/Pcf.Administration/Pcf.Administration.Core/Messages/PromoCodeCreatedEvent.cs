namespace Pcf.Administration.Core.Messages;

/// <summary>
/// Событие создания промокода
/// </summary>
public class PromoCodeCreatedEvent
{
    /// <summary>
    /// Идентификатор промокода
    /// </summary>
    public Guid PromoCodeId { get; set; }

    /// <summary>
    /// Код промокода
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор партнера
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Дата и время создания события
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


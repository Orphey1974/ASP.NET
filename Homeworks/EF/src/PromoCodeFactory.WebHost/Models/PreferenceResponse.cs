using System;

namespace PromoCodeFactory.WebHost.Models
{
    /// <summary>
    /// Модель ответа для предпочтения
    /// </summary>
    public class PreferenceResponse
    {
        /// <summary>
        /// Идентификатор предпочтения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название предпочтения
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
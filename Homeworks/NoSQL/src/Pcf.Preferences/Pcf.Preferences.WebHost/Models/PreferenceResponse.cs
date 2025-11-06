using System;

namespace Pcf.Preferences.WebHost.Models
{
    /// <summary>
    /// Модель ответа для предпочтения
    /// </summary>
    public class PreferenceResponse
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название предпочтения
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание предпочтения
        /// </summary>
        public string? Description { get; set; }
    }
}

using System;

namespace Pcf.Preferences.Core.Domain
{
    /// <summary>
    /// Предпочтение клиента
    /// </summary>
    public class Preference : BaseEntity
    {
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

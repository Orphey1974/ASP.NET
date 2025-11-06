using System;

namespace Pcf.Preferences.Core.Domain
{
    /// <summary>
    /// Базовая сущность
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }
    }
}

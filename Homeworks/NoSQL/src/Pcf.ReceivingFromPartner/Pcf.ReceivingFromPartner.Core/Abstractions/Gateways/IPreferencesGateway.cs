#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    /// <summary>
    /// Интерфейс шлюза для работы с микросервисом предпочтений
    /// </summary>
    public interface IPreferencesGateway
    {
        /// <summary>
        /// Получить все предпочтения
        /// </summary>
        /// <returns>Список предпочтений</returns>
        Task<IEnumerable<Preference>> GetPreferencesAsync();

        /// <summary>
        /// Получить предпочтение по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <returns>Предпочтение</returns>
        Task<Preference?> GetPreferenceByIdAsync(Guid id);
    }
}

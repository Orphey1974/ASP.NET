using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
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

        /// <summary>
        /// Получить предпочтения по списку идентификаторов
        /// </summary>
        /// <param name="ids">Список идентификаторов предпочтений</param>
        /// <returns>Список предпочтений</returns>
        Task<IEnumerable<Preference>> GetPreferencesByIdsAsync(IEnumerable<Guid> ids);
    }
}

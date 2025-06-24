using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    /// <summary>
    /// Модель ответа для клиента
    /// </summary>
    public class CustomerResponse
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email клиента
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Список предпочтений клиента
        /// </summary>
        public List<PreferenceResponse> Preferences { get; set; } = new List<PreferenceResponse>();

        /// <summary>
        /// Список промокодов клиента
        /// </summary>
        public List<PromoCodeShortResponse> PromoCodes { get; set; } = new List<PromoCodeShortResponse>();
    }
}
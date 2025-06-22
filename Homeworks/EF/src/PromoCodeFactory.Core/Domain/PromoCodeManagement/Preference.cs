using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Связь Many-to-Many с Customer через CustomerPreference
        /// </summary>
        public ICollection<CustomerPreference> CustomerPreferences { get; set; } = new List<CustomerPreference>();

        /// <summary>
        /// Связь One-to-Many с PromoCode
        /// </summary>
        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
    }
}
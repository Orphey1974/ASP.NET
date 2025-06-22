using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer
        : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Связь Many-to-Many с Preference через CustomerPreference
        /// </summary>
        public ICollection<CustomerPreference> CustomerPreferences { get; set; } = new List<CustomerPreference>();

        /// <summary>
        /// Связь One-to-Many с PromoCode
        /// </summary>
        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();

    }
}
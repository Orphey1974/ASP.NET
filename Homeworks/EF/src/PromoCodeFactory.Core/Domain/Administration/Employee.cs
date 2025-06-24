using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee
        : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Связь с Role
        /// </summary>
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public int AppliedPromocodesCount { get; set; }

        /// <summary>
        /// Связь One-to-Many с PromoCode
        /// </summary>
        public ICollection<PromoCodeFactory.Core.Domain.PromoCodeManagement.PromoCode> PromoCodes { get; set; } = new List<PromoCodeFactory.Core.Domain.PromoCodeManagement.PromoCode>();

        /// <summary>
        /// Связь One-to-Many с Partner
        /// </summary>
        public ICollection<Partner> ManagedPartners { get; set; } = new List<Partner>();
    }
}
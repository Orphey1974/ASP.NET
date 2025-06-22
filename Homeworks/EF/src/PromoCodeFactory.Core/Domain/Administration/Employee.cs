using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        /// <summary>
        /// Связь с Role
        /// </summary>
        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        public int AppliedPromocodesCount { get; set; }

        /// <summary>
        /// Связь One-to-Many с PromoCode
        /// </summary>
        public ICollection<PromoCodeFactory.Core.Domain.PromoCodeManagement.PromoCode> PromoCodes { get; set; } = new List<PromoCodeFactory.Core.Domain.PromoCodeManagement.PromoCode>();
    }
}
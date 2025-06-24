using System;
using System.Runtime;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode
        : BaseEntity
    {
        public string Code { get; set; } = string.Empty;

        public string? ServiceInfo { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? PartnerName { get; set; }

        /// <summary>
        /// Связь с Employee (PartnerManager)
        /// </summary>
        public Guid PartnerManagerId { get; set; }
        public Employee PartnerManager { get; set; } = null!;

        /// <summary>
        /// Связь с Partner
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner? Partner { get; set; }

        /// <summary>
        /// Связь с Preference
        /// </summary>
        public Guid PreferenceId { get; set; }
        public Preference Preference { get; set; } = null!;

        /// <summary>
        /// Связь с Customer (One-to-Many)
        /// </summary>
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PartnerManagement
{
    public class Partner : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ContactEmail { get; set; }

        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Связь с Employee (PartnerManager)
        /// </summary>
        public Guid PartnerManagerId { get; set; }
        public Employee PartnerManager { get; set; } = null!;

        /// <summary>
        /// Связь One-to-Many с PromoCode
        /// </summary>
        public ICollection<PromoCodeFactory.Core.Domain.PromoCodeManagement.PromoCode> PromoCodes { get; set; } = new List<PromoCodeFactory.Core.Domain.PromoCodeManagement.PromoCode>();

        /// <summary>
        /// Связь One-to-Many с PartnerLimit
        /// </summary>
        public ICollection<PartnerLimit> PartnerLimits { get; set; } = new List<PartnerLimit>();
    }
}
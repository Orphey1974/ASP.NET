using System;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Domain.PartnerManagement
{
    public class PartnerLimit : BaseEntity
    {
        public int Limit { get; set; }

        public int CurrentCount { get; set; } = 0;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Связь с Partner
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; } = null!;

        /// <summary>
        /// Проверяет, можно ли выдать промокод
        /// </summary>
        public bool CanIssuePromoCode()
        {
            return IsActive &&
                   CurrentCount < Limit &&
                   DateTime.UtcNow >= StartDate &&
                   DateTime.UtcNow <= EndDate;
        }

        /// <summary>
        /// Увеличивает счетчик выданных промокодов
        /// </summary>
        public void IncrementCount()
        {
            if (CanIssuePromoCode())
            {
                CurrentCount++;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Сбрасывает счетчик
        /// </summary>
        public void ResetCount()
        {
            CurrentCount = 0;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
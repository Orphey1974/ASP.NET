using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.Tests.Builders
{
    public class PartnerLimitBuilder
    {
        private Guid _id = Guid.NewGuid();
        private int _limit = 100;
        private int _currentCount = 0;
        private DateTime _startDate = DateTime.UtcNow;
        private DateTime _endDate = DateTime.UtcNow.AddDays(30);
        private bool _isActive = true;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _updatedAt = null;
        private Guid _partnerId = Guid.NewGuid();
        private Partner? _partner = null;

        public PartnerLimitBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public PartnerLimitBuilder WithLimit(int limit)
        {
            _limit = limit;
            return this;
        }

        public PartnerLimitBuilder WithCurrentCount(int currentCount)
        {
            _currentCount = currentCount;
            return this;
        }

        public PartnerLimitBuilder WithStartDate(DateTime startDate)
        {
            _startDate = startDate;
            return this;
        }

        public PartnerLimitBuilder WithEndDate(DateTime endDate)
        {
            _endDate = endDate;
            return this;
        }

        public PartnerLimitBuilder WithIsActive(bool isActive)
        {
            _isActive = isActive;
            return this;
        }

        public PartnerLimitBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public PartnerLimitBuilder WithUpdatedAt(DateTime? updatedAt)
        {
            _updatedAt = updatedAt;
            return this;
        }

        public PartnerLimitBuilder WithPartnerId(Guid partnerId)
        {
            _partnerId = partnerId;
            return this;
        }

        public PartnerLimitBuilder WithPartner(Partner partner)
        {
            _partner = partner;
            return this;
        }

        public PartnerLimit Build()
        {
            return new PartnerLimit
            {
                Id = _id,
                Limit = _limit,
                CurrentCount = _currentCount,
                StartDate = _startDate,
                EndDate = _endDate,
                IsActive = _isActive,
                CreatedAt = _createdAt,
                UpdatedAt = _updatedAt,
                PartnerId = _partnerId,
                Partner = _partner ?? new Partner { Id = _partnerId }
            };
        }

        public static PartnerLimitBuilder Create()
        {
            return new PartnerLimitBuilder();
        }

        public static PartnerLimitBuilder CreateActive()
        {
            return new PartnerLimitBuilder().WithIsActive(true);
        }

        public static PartnerLimitBuilder CreateInactive()
        {
            return new PartnerLimitBuilder().WithIsActive(false);
        }

        public static PartnerLimitBuilder CreateWithValidDates()
        {
            return new PartnerLimitBuilder()
                .WithStartDate(DateTime.UtcNow)
                .WithEndDate(DateTime.UtcNow.AddDays(30));
        }

        public static PartnerLimitBuilder CreateWithInvalidDates()
        {
            return new PartnerLimitBuilder()
                .WithStartDate(DateTime.UtcNow.AddDays(30))
                .WithEndDate(DateTime.UtcNow);
        }
    }
}
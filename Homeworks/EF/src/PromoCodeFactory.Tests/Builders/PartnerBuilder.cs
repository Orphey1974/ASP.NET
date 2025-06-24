using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.Tests.Builders
{
    public class PartnerBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "Test Partner";
        private string? _description = "Test Description";
        private string? _contactEmail = "test@partner.com";
        private string? _contactPhone = "+7 (999) 123-45-67";
        private bool _isActive = true;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _updatedAt = null;
        private Guid _partnerManagerId = Guid.NewGuid();
        private Employee? _partnerManager = null;

        public PartnerBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public PartnerBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public PartnerBuilder WithDescription(string? description)
        {
            _description = description;
            return this;
        }

        public PartnerBuilder WithContactEmail(string? contactEmail)
        {
            _contactEmail = contactEmail;
            return this;
        }

        public PartnerBuilder WithContactPhone(string? contactPhone)
        {
            _contactPhone = contactPhone;
            return this;
        }

        public PartnerBuilder WithIsActive(bool isActive)
        {
            _isActive = isActive;
            return this;
        }

        public PartnerBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public PartnerBuilder WithUpdatedAt(DateTime? updatedAt)
        {
            _updatedAt = updatedAt;
            return this;
        }

        public PartnerBuilder WithPartnerManagerId(Guid partnerManagerId)
        {
            _partnerManagerId = partnerManagerId;
            return this;
        }

        public PartnerBuilder WithPartnerManager(Employee partnerManager)
        {
            _partnerManager = partnerManager;
            return this;
        }

        public Partner Build()
        {
            return new Partner
            {
                Id = _id,
                Name = _name,
                Description = _description,
                ContactEmail = _contactEmail,
                ContactPhone = _contactPhone,
                IsActive = _isActive,
                CreatedAt = _createdAt,
                UpdatedAt = _updatedAt,
                PartnerManagerId = _partnerManagerId,
                PartnerManager = _partnerManager ?? new Employee { Id = _partnerManagerId }
            };
        }

        public static PartnerBuilder Create()
        {
            return new PartnerBuilder();
        }

        public static PartnerBuilder CreateActive()
        {
            return new PartnerBuilder().WithIsActive(true);
        }

        public static PartnerBuilder CreateInactive()
        {
            return new PartnerBuilder().WithIsActive(false);
        }
    }
}
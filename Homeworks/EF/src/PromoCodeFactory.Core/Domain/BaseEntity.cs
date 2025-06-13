using System;

namespace PromoCodeFactory.Core.Domain
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}
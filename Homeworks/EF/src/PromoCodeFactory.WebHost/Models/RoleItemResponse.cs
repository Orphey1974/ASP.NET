using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class RoleItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
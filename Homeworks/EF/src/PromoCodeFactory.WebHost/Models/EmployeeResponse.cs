using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleItemResponse? Role { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }
}
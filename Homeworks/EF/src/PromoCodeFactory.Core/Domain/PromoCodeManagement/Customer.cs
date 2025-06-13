using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer : BaseEntity
    {
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }

        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
        public ICollection<PromoCode> PromoCodes { get; set; }

        public Customer()
        {
            CustomerPreferences = new List<CustomerPreference>();
            PromoCodes = new List<PromoCode>();
        }
    }
}

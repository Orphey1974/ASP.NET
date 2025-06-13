using System.Collections.Generic;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class Preference : BaseEntity
{
    public string Name { get; set; }

    public ICollection<CustomerPreference> CustomerPreferences { get; set; }
}

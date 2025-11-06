using System;
using System.Collections.Generic;
using System.Linq;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.WebHost.Models
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<PreferenceResponse> Preferences { get; set; }
        public List<PromoCodeShortResponse> PromoCodes { get; set; }

        public CustomerResponse()
        {
            Preferences = new List<PreferenceResponse>();
            PromoCodes = new List<PromoCodeShortResponse>();
        }

        public CustomerResponse(Customer customer, IEnumerable<Preference> preferences = null)
        {
            Id = customer.Id;
            Email = customer.Email;
            FirstName = customer.FirstName;
            LastName = customer.LastName;

            // Получаем предпочтения из переданного списка или используем только ID
            if (preferences != null)
            {
                var preferencesDict = preferences.ToDictionary(p => p.Id, p => p.Name);
                Preferences = customer.Preferences?.Select(x => new PreferenceResponse()
                {
                    Id = x.PreferenceId,
                    Name = preferencesDict.ContainsKey(x.PreferenceId)
                        ? preferencesDict[x.PreferenceId]
                        : "Unknown"
                }).ToList() ?? new List<PreferenceResponse>();
            }
            else
            {
                // Если предпочтения не переданы, используем только ID
                Preferences = customer.Preferences?.Select(x => new PreferenceResponse()
                {
                    Id = x.PreferenceId,
                    Name = null
                }).ToList() ?? new List<PreferenceResponse>();
            }

            PromoCodes = customer.PromoCodes?.Select(x => new PromoCodeShortResponse()
                {
                    Id = x.PromoCode.Id,
                    Code = x.PromoCode.Code,
                    BeginDate = x.PromoCode.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = x.PromoCode.EndDate.ToString("yyyy-MM-dd"),
                    PartnerId = x.PromoCode.PartnerId,
                    ServiceInfo = x.PromoCode.ServiceInfo
                }).ToList() ?? new List<PromoCodeShortResponse>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public static class FakeDataFactory
    {

        // Предпочтения теперь получаются из микросервиса предпочтений

        public static List<Customer> Customers
        {
            get
            {
                var customerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
                var customers = new List<Customer>()
                {
                    new Customer()
                    {
                        Id = customerId,
                        Email = "ivan_sergeev@mail.ru",
                        FirstName = "Иван",
                        LastName = "Петров",
                        Preferences = new List<CustomerPreference>()
                        {
                            new CustomerPreference()
                            {
                                CustomerId = customerId,
                                PreferenceId = Guid.Parse("11111111-1111-1111-1111-111111111111") // Электроника
                            },
                            new CustomerPreference()
                            {
                                CustomerId = customerId,
                                PreferenceId = Guid.Parse("22222222-2222-2222-2222-222222222222") // Одежда
                            }
                        }
                    }
                };

                return customers;
            }
        }

        public static List<PromoCode> PromoCodes
        {
            get
            {
                var customerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
                var promoCodeId = Guid.Parse("b6c8c6b1-4349-45b0-ab31-244740aaf0f0");

                var promoCodes = new List<PromoCode>()
                {
                    new PromoCode()
                    {
                        Id = promoCodeId,
                        Code = "PROMO2024",
                        ServiceInfo = "Тестовый промокод для электроники",
                        BeginDate = DateTime.UtcNow.AddDays(-30),
                        EndDate = DateTime.UtcNow.AddDays(30),
                        PartnerId = Guid.Parse("7d994823-86a1-4d65-bda5-8b7924218a19"),
                        PreferenceId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Электроника
                        Customers = new List<PromoCodeCustomer>()
                        {
                            new PromoCodeCustomer()
                            {
                                Id = Guid.NewGuid(),
                                PromoCodeId = promoCodeId,
                                CustomerId = customerId
                            }
                        }
                    }
                };

                return promoCodes;
            }
        }
    }
}
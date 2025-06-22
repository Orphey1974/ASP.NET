using System;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static IEnumerable<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                Name = "Admin",
                Description = "Администратор",
            },
            new Role()
            {
                Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };

        public static IEnumerable<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"), // Admin
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                RoleId = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"), // PartnerManager
                AppliedPromocodesCount = 10
            },
        };

        public static IEnumerable<Preference> Preferences => new List<Preference>()
        {
            new Preference()
            {
                Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                Name = "Театр",
            },
            new Preference()
            {
                Id = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                Name = "Семья",
            },
            new Preference()
            {
                Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                Name = "Дети",
            }
        };

        public static IEnumerable<Customer> Customers
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
                    }
                };

                return customers;
            }
        }

        public static IEnumerable<CustomerPreference> CustomerPreferences
        {
            get
            {
                var customerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
                var preferences = Preferences.ToList();

                return new List<CustomerPreference>()
                {
                    new CustomerPreference()
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId,
                        PreferenceId = preferences[0].Id, // Театр
                    },
                    new CustomerPreference()
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId,
                        PreferenceId = preferences[1].Id, // Семья
                    }
                };
            }
        }

        public static IEnumerable<Partner> Partners
        {
            get
            {
                var partnerManagerId = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"); // Петр Андреев

                return new List<Partner>()
                {
                    new Partner()
                    {
                        Id = Guid.Parse("8f7b3c2a-1e4d-4f6a-9b8c-7d5e3f2a1b9c"),
                        Name = "Большой театр",
                        Description = "Государственный академический Большой театр России",
                        ContactEmail = "info@bolshoi.ru",
                        ContactPhone = "+7 (495) 250-73-17",
                        IsActive = true,
                        PartnerManagerId = partnerManagerId,
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new Partner()
                    {
                        Id = Guid.Parse("9e8c4d3b-2f5e-5g7b-0c9d-8e6f4g3b2c0d"),
                        Name = "Московский зоопарк",
                        Description = "Московский зоологический парк",
                        ContactEmail = "info@moscowzoo.ru",
                        ContactPhone = "+7 (499) 252-35-80",
                        IsActive = true,
                        PartnerManagerId = partnerManagerId,
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Partner()
                    {
                        Id = Guid.Parse("0f9d5e4c-3g6f-6h8c-1d0e-9f7g4h3c1d0e"),
                        Name = "Неактивный партнер",
                        Description = "Партнер для тестирования неактивных состояний",
                        ContactEmail = "inactive@test.ru",
                        ContactPhone = "+7 (000) 000-00-00",
                        IsActive = false,
                        PartnerManagerId = partnerManagerId,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }
                };
            }
        }

        public static IEnumerable<PartnerLimit> PartnerLimits
        {
            get
            {
                var bigTheaterId = Guid.Parse("8f7b3c2a-1e4d-4f6a-9b8c-7d5e3f2a1b9c");
                var zooId = Guid.Parse("9e8c4d3b-2f5e-5g7b-0c9d-8e6f4g3b2c0d");

                return new List<PartnerLimit>()
                {
                    new PartnerLimit()
                    {
                        Id = Guid.NewGuid(),
                        PartnerId = bigTheaterId,
                        Limit = 100,
                        CurrentCount = 25,
                        StartDate = DateTime.UtcNow.AddDays(-30),
                        EndDate = DateTime.UtcNow.AddDays(30),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new PartnerLimit()
                    {
                        Id = Guid.NewGuid(),
                        PartnerId = zooId,
                        Limit = 50,
                        CurrentCount = 10,
                        StartDate = DateTime.UtcNow.AddDays(-20),
                        EndDate = DateTime.UtcNow.AddDays(40),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    }
                };
            }
        }

        public static IEnumerable<PromoCode> PromoCodes
        {
            get
            {
                var employeeId = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"); // Петр Андреев
                var preferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"); // Театр
                var customerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"); // Иван Петров
                var partnerId = Guid.Parse("8f7b3c2a-1e4d-4f6a-9b8c-7d5e3f2a1b9c"); // Большой театр

                return new List<PromoCode>()
                {
                    new PromoCode()
                    {
                        Id = Guid.NewGuid(),
                        Code = "THEATER2024",
                        ServiceInfo = "Скидка 20% на билеты в театр",
                        BeginDate = DateTime.Now.AddDays(-30),
                        EndDate = DateTime.Now.AddDays(30),
                        PartnerName = "Большой театр",
                        PartnerManagerId = employeeId,
                        PartnerId = partnerId,
                        PreferenceId = preferenceId,
                        CustomerId = customerId
                    }
                };
            }
        }
    }
}
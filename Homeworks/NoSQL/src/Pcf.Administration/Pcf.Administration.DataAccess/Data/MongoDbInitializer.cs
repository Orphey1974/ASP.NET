using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Domain.MongoDb;
using Pcf.Administration.Core.Mappers;

namespace Pcf.Administration.DataAccess.Data
{
    /// <summary>
    /// Инициализатор данных для MongoDB
    /// </summary>
    public class MongoDbInitializer
    {
        private readonly MongoDbContext _mongoContext;

        public MongoDbInitializer(MongoDbContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        public async Task InitializeAsync()
        {
            await InitializeRolesAsync();
            await InitializeEmployeesAsync();
        }

        private async Task InitializeRolesAsync()
        {
            var rolesCollection = _mongoContext.Roles;
            var existingRoles = await rolesCollection.Find(_ => true).ToListAsync();

            if (existingRoles.Count == 0)
            {
                var roleDocuments = new List<RoleDocument>
                {
                    new RoleDocument
                    {
                        Id = Guid.Parse("53729686-a368-4eeb-8baa-cc69b6050d02"),
                        Name = "Manager",
                        Description = "Менеджер по работе с клиентами"
                    },
                    new RoleDocument
                    {
                        Id = Guid.Parse("53729686-a368-4eeb-8baa-cc69b6050d03"),
                        Name = "Administrator",
                        Description = "Администратор системы"
                    },
                    new RoleDocument
                    {
                        Id = Guid.Parse("53729686-a368-4eeb-8baa-cc69b6050d04"),
                        Name = "Support",
                        Description = "Служба поддержки"
                    }
                };

                foreach (var roleDocument in roleDocuments)
                {
                    await rolesCollection.InsertOneAsync(roleDocument);
                }
            }
        }

        private async Task InitializeEmployeesAsync()
        {
            var employeesCollection = _mongoContext.Employees;
            var rolesCollection = _mongoContext.Roles;

            var existingEmployees = await employeesCollection.Find(_ => true).ToListAsync();

            if (existingEmployees.Count == 0)
            {
                // Получаем роли для создания сотрудников
                var roles = await rolesCollection.Find(_ => true).ToListAsync();
                var managerRole = roles.Find(r => r.Name == "Manager");
                var adminRole = roles.Find(r => r.Name == "Administrator");

                if (managerRole != null && adminRole != null)
                {
                    var employeeDocuments = new List<EmployeeDocument>
                    {
                        new EmployeeDocument
                        {
                            Id = Guid.Parse("53729686-a000-4eeb-8baa-cc69b6050d01"),
                            FirstName = "Иван",
                            LastName = "Иванов",
                            Email = "ivan.ivanov@example.com",
                            RoleId = managerRole.Id,
                            AppliedPromocodesCount = 5
                        },
                        new EmployeeDocument
                        {
                            Id = Guid.Parse("53729686-a000-4eeb-8baa-cc69b6050d02"),
                            FirstName = "Петр",
                            LastName = "Петров",
                            Email = "petr.petrov@example.com",
                            RoleId = adminRole.Id,
                            AppliedPromocodesCount = 10
                        }
                    };

                    foreach (var employeeDocument in employeeDocuments)
                    {
                        await employeesCollection.InsertOneAsync(employeeDocument);
                    }
                }
            }
        }
    }
}

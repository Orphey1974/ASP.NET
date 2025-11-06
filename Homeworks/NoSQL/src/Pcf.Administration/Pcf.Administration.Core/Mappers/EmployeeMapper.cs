using System;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Domain.MongoDb;

namespace Pcf.Administration.Core.Mappers
{
    /// <summary>
    /// Маппер для преобразования между Employee и EmployeeDocument
    /// </summary>
    public static class EmployeeMapper
    {
        /// <summary>
        /// Преобразование из Employee в EmployeeDocument
        /// </summary>
        public static EmployeeDocument ToDocument(this Employee employee)
        {
            if (employee == null) return null;

            return new EmployeeDocument
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                RoleId = employee.RoleId,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
        }

        /// <summary>
        /// Преобразование из EmployeeDocument в Employee
        /// </summary>
        public static Employee ToEntity(this EmployeeDocument document, Role role = null)
        {
            if (document == null) return null;

            return new Employee
            {
                Id = document.Id,
                FirstName = document.FirstName,
                LastName = document.LastName,
                Email = document.Email,
                RoleId = role?.Id ?? Guid.Empty,
                Role = role,
                AppliedPromocodesCount = document.AppliedPromocodesCount
            };
        }
    }
}

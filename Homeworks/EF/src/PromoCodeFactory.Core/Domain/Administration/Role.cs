using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Связь One-to-Many с Employee
        /// </summary>
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
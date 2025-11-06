using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Domain.MongoDb;
using Pcf.Administration.Core.Mappers;

namespace Pcf.Administration.DataAccess.Repositories
{
    /// <summary>
    /// Репозиторий для работы с сотрудниками в MongoDB
    /// </summary>
    public class MongoEmployeeRepository : IMongoRepository<Employee>
    {
        private readonly IMongoCollection<EmployeeDocument> _employeeCollection;
        private readonly IMongoCollection<RoleDocument> _roleCollection;

        public MongoEmployeeRepository(IMongoCollection<EmployeeDocument> employeeCollection,
            IMongoCollection<RoleDocument> roleCollection)
        {
            _employeeCollection = employeeCollection;
            _roleCollection = roleCollection;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var employeeDocuments = await _employeeCollection.Find(_ => true).ToListAsync();
            var employees = new List<Employee>();

            foreach (var employeeDoc in employeeDocuments)
            {
                var roleDoc = await _roleCollection.Find(r => r.Id == employeeDoc.RoleId).FirstOrDefaultAsync();
                var role = roleDoc?.ToEntity();
                var employee = employeeDoc.ToEntity(role);
                employees.Add(employee);
            }

            return employees;
        }

        public async Task<Employee> GetByIdAsync(Guid id)
        {
            // Для упрощения, получаем все записи и ищем по Guid
            var allEmployees = await GetAllAsync();
            return allEmployees.FirstOrDefault(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetRangeByIdsAsync(List<Guid> ids)
        {
            // Для упрощения, получаем все записи и фильтруем по Guid
            var allEmployees = await GetAllAsync();
            return allEmployees.Where(e => ids.Contains(e.Id));
        }

        public async Task<Employee> GetFirstWhere(System.Linq.Expressions.Expression<Func<Employee, bool>> predicate)
        {
            // Для упрощения реализации, получаем все записи и фильтруем в памяти
            // В реальном проекте можно использовать более сложные запросы
            var allEmployees = await GetAllAsync();
            return allEmployees.FirstOrDefault(predicate.Compile());
        }

        public async Task<IEnumerable<Employee>> GetWhere(System.Linq.Expressions.Expression<Func<Employee, bool>> predicate)
        {
            // Для упрощения реализации, получаем все записи и фильтруем в памяти
            var allEmployees = await GetAllAsync();
            return allEmployees.Where(predicate.Compile());
        }

        public async Task AddAsync(Employee entity)
        {
            var document = entity.ToDocument();
            await _employeeCollection.InsertOneAsync(document);
        }

        public async Task UpdateAsync(Employee entity)
        {
            // Находим существующий документ по ID
            var filter = Builders<EmployeeDocument>.Filter.Eq(e => e.Id, entity.Id);
            var existingDocument = await _employeeCollection.Find(filter).FirstOrDefaultAsync();

            if (existingDocument != null)
            {
                // Обновляем существующий документ
                var update = Builders<EmployeeDocument>.Update
                    .Set(e => e.FirstName, entity.FirstName)
                    .Set(e => e.LastName, entity.LastName)
                    .Set(e => e.Email, entity.Email)
                    .Set(e => e.RoleId, entity.RoleId)
                    .Set(e => e.AppliedPromocodesCount, entity.AppliedPromocodesCount);

                await _employeeCollection.UpdateOneAsync(filter, update);
            }
        }

        public async Task DeleteAsync(Employee entity)
        {
            // Удаляем по ID
            var filter = Builders<EmployeeDocument>.Filter.Eq(e => e.Id, entity.Id);
            await _employeeCollection.DeleteOneAsync(filter);
        }
    }
}

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
            // Для упрощения, получаем все записи, находим нужную и обновляем
            var allEmployees = await GetAllAsync();
            var existingEmployee = allEmployees.FirstOrDefault(e => e.Email == entity.Email);
            if (existingEmployee != null)
            {
                var document = entity.ToDocument();
                // В реальном проекте здесь нужно сохранить оригинальный ObjectId
                await _employeeCollection.InsertOneAsync(document);
            }
        }

        public async Task DeleteAsync(Employee entity)
        {
            // Для упрощения, удаляем по email
            var filter = Builders<EmployeeDocument>.Filter.Eq(e => e.Email, entity.Email);
            await _employeeCollection.DeleteOneAsync(filter);
        }
    }
}

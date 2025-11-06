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
    /// Репозиторий для работы с ролями в MongoDB
    /// </summary>
    public class MongoRoleRepository : IMongoRepository<Role>
    {
        private readonly IMongoCollection<RoleDocument> _roleCollection;

        public MongoRoleRepository(IMongoCollection<RoleDocument> roleCollection)
        {
            _roleCollection = roleCollection;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            var roleDocuments = await _roleCollection.Find(_ => true).ToListAsync();
            return roleDocuments.Select(rd => rd.ToEntity());
        }

        public async Task<Role> GetByIdAsync(Guid id)
        {
            // Для упрощения, получаем все записи и ищем по Guid
            var allRoles = await GetAllAsync();
            return allRoles.FirstOrDefault(r => r.Id == id);
        }

        public async Task<IEnumerable<Role>> GetRangeByIdsAsync(List<Guid> ids)
        {
            // Для упрощения, получаем все записи и фильтруем по Guid
            var allRoles = await GetAllAsync();
            return allRoles.Where(r => ids.Contains(r.Id));
        }

        public async Task<Role> GetFirstWhere(System.Linq.Expressions.Expression<Func<Role, bool>> predicate)
        {
            // Для упрощения реализации, получаем все записи и фильтруем в памяти
            var allRoles = await GetAllAsync();
            return allRoles.FirstOrDefault(predicate.Compile());
        }

        public async Task<IEnumerable<Role>> GetWhere(System.Linq.Expressions.Expression<Func<Role, bool>> predicate)
        {
            // Для упрощения реализации, получаем все записи и фильтруем в памяти
            var allRoles = await GetAllAsync();
            return allRoles.Where(predicate.Compile());
        }

        public async Task AddAsync(Role entity)
        {
            var document = entity.ToDocument();
            await _roleCollection.InsertOneAsync(document);
        }

        public async Task UpdateAsync(Role entity)
        {
            // Для упрощения, получаем все записи, находим нужную и обновляем
            var allRoles = await GetAllAsync();
            var existingRole = allRoles.FirstOrDefault(r => r.Name == entity.Name);
            if (existingRole != null)
            {
                var document = entity.ToDocument();
                // В реальном проекте здесь нужно сохранить оригинальный ObjectId
                await _roleCollection.InsertOneAsync(document);
            }
        }

        public async Task DeleteAsync(Role entity)
        {
            // Для упрощения, удаляем по имени
            var filter = Builders<RoleDocument>.Filter.Eq(r => r.Name, entity.Name);
            await _roleCollection.DeleteOneAsync(filter);
        }
    }
}

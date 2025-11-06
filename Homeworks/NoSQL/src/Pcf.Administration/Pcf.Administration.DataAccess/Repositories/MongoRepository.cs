using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Domain.MongoDb;
using Pcf.Administration.Core.Mappers;

namespace Pcf.Administration.DataAccess.Repositories
{
    /// <summary>
    /// Базовый репозиторий для работы с MongoDB
    /// </summary>
    public class MongoRepository<T> : IMongoRepository<T>
        where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var documents = await _collection.Find(_ => true).ToListAsync();
            return documents;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            return document;
        }

        public virtual async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var filter = Builders<T>.Filter.In("Id", ids);
            var documents = await _collection.Find(filter).ToListAsync();
            return documents;
        }

        public virtual async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        {
            var documents = await _collection.Find(predicate).FirstOrDefaultAsync();
            return documents;
        }

        public virtual async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            var documents = await _collection.Find(predicate).ToListAsync();
            return documents;
        }

        public virtual async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("Id", entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("Id", entity.Id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}

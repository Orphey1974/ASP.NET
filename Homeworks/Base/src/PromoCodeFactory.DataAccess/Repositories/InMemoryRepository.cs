using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        private readonly ConcurrentDictionary<Guid, T> _data = new();

        public InMemoryRepository(IEnumerable<T> data = null)
        {
            _data = new ConcurrentDictionary<Guid, T>();

            if (data == null) return;

            foreach (var item in data)
            {
                if (item.Id == Guid.Empty)
                    item.Id = Guid.NewGuid();

                _data.TryAdd(item.Id, item);
            }
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            _data.TryGetValue(id, out var entity);
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_data.Values.AsEnumerable());
        }

        public Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

            return _data.TryAdd(entity.Id, entity)
                ? Task.CompletedTask
                : Task.FromException(new InvalidOperationException("Entity with same ID already exists"));
        }

        public Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.Id == Guid.Empty) throw new ArgumentException("Entity ID cannot be empty");

            _data[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _data.TryRemove(entity.Id, out _);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            return Task.FromResult(_data.ContainsKey(id));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Передан null вместо объекта.");
            }

            var dataList = Data.ToList();
            dataList.Add(entity);
            Data = dataList;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Передан null вместо объекта.");
            }

            var dataList = Data.ToList();
            var existingEntity = dataList.FirstOrDefault(x => x.Id == entity.Id);
            if (existingEntity != null)
            {
                var index = dataList.IndexOf(existingEntity);
                dataList[index] = entity;
                Data = dataList;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var dataList = Data.ToList();
            var entity = dataList.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                dataList.Remove(entity);
                Data = dataList;
            }

            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            // Для InMemory репозитория изменения сохраняются сразу
            return Task.CompletedTask;
        }
    }
}

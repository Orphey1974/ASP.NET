using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// Реализация репозитория в памяти для тестирования
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        private readonly List<T> _entities = new List<T>();

        public InMemoryRepository(IEnumerable<T> data)
        {
            _entities.AddRange(data);
        }

        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(_entities.AsEnumerable());
        }

        /// <summary>
        /// Получить сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность или null, если не найдена</returns>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(_entities.FirstOrDefault(e => e.Id == id));
        }

        /// <summary>
        /// Добавить новую сущность
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        public async Task AddAsync(T entity)
        {
            _entities.Add(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Обновить существующую сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        public async Task UpdateAsync(T entity)
        {
            var existingEntity = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existingEntity != null)
            {
                var index = _entities.IndexOf(existingEntity);
                _entities[index] = entity;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="entity">Сущность для удаления</param>
        public async Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Удалить сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }
    }
}
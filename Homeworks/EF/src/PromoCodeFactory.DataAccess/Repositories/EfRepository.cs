using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// Реализация репозитория на основе Entity Framework Core
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly PromoCodeFactoryDbContext _dataPromoCodeFactoryDbContext;
        private readonly DbSet<T> _dbSet;

        public EfRepository(PromoCodeFactoryDbContext dataPromoCodeFactoryDbContext)
        {
            _dataPromoCodeFactoryDbContext = dataPromoCodeFactoryDbContext;
            _dbSet = dataPromoCodeFactoryDbContext.Set<T>();
        }

        /// <summary>
        /// Получает все сущности из базы данных
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Получает сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Найденная сущность или null</returns>
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Добавляет новую сущность в базу данных
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        public async Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Передан null вместо объекта.");
            }

            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Обновляет существующую сущность в базе данных
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Передан null вместо объекта.");
            }

            _dbSet.Update(entity);
        }

        /// <summary>
        /// Удаляет сущность из базы данных
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        /// <summary>
        /// Сохраняет все изменения в базе данных
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _dataPromoCodeFactoryDbContext.SaveChangesAsync();
        }
    }
}

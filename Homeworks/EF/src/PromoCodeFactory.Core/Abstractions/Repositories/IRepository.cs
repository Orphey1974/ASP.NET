using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    /// <summary>
    /// Базовый интерфейс репозитория для работы с сущностями
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IRepository<T>
        where T : BaseEntity
    {
        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получить сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность или null, если не найдена</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Добавить новую сущность
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Обновить существующую сущность
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="entity">Сущность для удаления</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Удалить сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        Task DeleteAsync(Guid id);
    }
}
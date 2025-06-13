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
        /// Добавляет новую сущность в базу данных
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Получает все сущности из базы данных
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получает сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Найденная сущность или null</returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Обновляет существующую сущность в базе данных
        /// </summary>
        /// <param name="entity">Сущность для обновления</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Удаляет сущность из базы данных
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Сохраняет все изменения в базе данных
        /// </summary>
        Task SaveChangesAsync();
    }
}

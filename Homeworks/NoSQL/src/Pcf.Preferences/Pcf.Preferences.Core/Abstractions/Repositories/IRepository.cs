using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pcf.Preferences.Core.Domain;

namespace Pcf.Preferences.Core.Abstractions.Repositories
{
    /// <summary>
    /// Базовый интерфейс репозитория
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <returns>Список сущностей</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Получить сущность по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Сущность</returns>
        Task<TEntity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить сущности по списку идентификаторов
        /// </summary>
        /// <param name="ids">Список идентификаторов</param>
        /// <returns>Список сущностей</returns>
        Task<IEnumerable<TEntity>> GetRangeByIdsAsync(List<Guid> ids);

        /// <summary>
        /// Получить первую сущность по условию
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Сущность</returns>
        Task<TEntity?> GetFirstWhere(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Получить сущности по условию
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Список сущностей</returns>
        Task<IEnumerable<TEntity>> GetWhere(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Добавить сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Задача</returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Задача</returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Задача</returns>
        Task DeleteAsync(TEntity entity);
    }
}

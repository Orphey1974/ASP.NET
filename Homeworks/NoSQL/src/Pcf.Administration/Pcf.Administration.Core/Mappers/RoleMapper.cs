using System;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Domain.MongoDb;

namespace Pcf.Administration.Core.Mappers
{
    /// <summary>
    /// Маппер для преобразования между Role и RoleDocument
    /// </summary>
    public static class RoleMapper
    {
        /// <summary>
        /// Преобразование из Role в RoleDocument
        /// </summary>
        public static RoleDocument ToDocument(this Role role)
        {
            if (role == null) return null;

            return new RoleDocument
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }

        /// <summary>
        /// Преобразование из RoleDocument в Role
        /// </summary>
        public static Role ToEntity(this RoleDocument document)
        {
            if (document == null) return null;

            return new Role
            {
                Id = Guid.NewGuid(), // Генерируем новый Guid для совместимости с EF
                Name = document.Name,
                Description = document.Description
            };
        }
    }
}

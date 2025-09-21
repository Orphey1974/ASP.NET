using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pcf.Administration.Core.Domain.MongoDb
{
    /// <summary>
    /// Документ роли для MongoDB
    /// </summary>
    public class RoleDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
    }
}

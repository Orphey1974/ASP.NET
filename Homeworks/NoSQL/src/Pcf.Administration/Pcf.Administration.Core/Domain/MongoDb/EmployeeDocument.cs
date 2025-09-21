using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pcf.Administration.Core.Domain.MongoDb
{
    /// <summary>
    /// Документ сотрудника для MongoDB
    /// </summary>
    public class EmployeeDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("roleId")]
        public Guid RoleId { get; set; }

        [BsonElement("appliedPromocodesCount")]
        public int AppliedPromocodesCount { get; set; }

        [BsonElement("fullName")]
        public string FullName => $"{LastName} {FirstName}";
    }
}

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.Administration.Core.Domain.MongoDb;

namespace Pcf.Administration.DataAccess.Data
{
    /// <summary>
    /// Контекст для работы с MongoDB
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<EmployeeDocument> Employees =>
            _database.GetCollection<EmployeeDocument>("employees");

        public IMongoCollection<RoleDocument> Roles =>
            _database.GetCollection<RoleDocument>("roles");
    }
}

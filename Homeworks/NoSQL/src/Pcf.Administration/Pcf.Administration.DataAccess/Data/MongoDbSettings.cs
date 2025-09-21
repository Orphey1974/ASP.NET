namespace Pcf.Administration.DataAccess.Data
{
    /// <summary>
    /// Настройки подключения к MongoDB
    /// </summary>
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}

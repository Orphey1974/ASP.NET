using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public class EfDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public EfDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void InitializeDb()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            // Предпочтения больше не инициализируем локально - они получаются из микросервиса
            // Временно отключаем инициализацию тестовых данных для отладки
            // _dataContext.AddRange(FakeDataFactory.Customers);
            // _dataContext.SaveChanges();
        }
    }
}
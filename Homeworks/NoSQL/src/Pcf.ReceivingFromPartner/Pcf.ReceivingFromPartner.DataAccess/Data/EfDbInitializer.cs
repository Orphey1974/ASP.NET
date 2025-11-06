namespace Pcf.ReceivingFromPartner.DataAccess.Data
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

            _dataContext.AddRange(FakeDataFactory.Partners);
            _dataContext.SaveChanges();
        }
    }
}
namespace PromoCodeFactory.Tests.Builders
{
    public class SetPartnerLimitRequestBuilder
    {
        private int _limit = 100;
        private DateTime _startDate = DateTime.UtcNow.AddDays(1);
        private DateTime _endDate = DateTime.UtcNow.AddDays(31);

        public SetPartnerLimitRequestBuilder WithLimit(int limit)
        {
            _limit = limit;
            return this;
        }

        public SetPartnerLimitRequestBuilder WithStartDate(DateTime startDate)
        {
            _startDate = startDate;
            return this;
        }

        public SetPartnerLimitRequestBuilder WithEndDate(DateTime endDate)
        {
            _endDate = endDate;
            return this;
        }

        public PromoCodeFactory.WebHost.Controllers.SetPartnerLimitRequest Build()
        {
            return new PromoCodeFactory.WebHost.Controllers.SetPartnerLimitRequest
            {
                Limit = _limit,
                StartDate = _startDate,
                EndDate = _endDate
            };
        }

        public static SetPartnerLimitRequestBuilder Create()
        {
            return new SetPartnerLimitRequestBuilder();
        }

        public static SetPartnerLimitRequestBuilder CreateValid()
        {
            return new SetPartnerLimitRequestBuilder()
                .WithLimit(100)
                .WithStartDate(DateTime.UtcNow.AddDays(1))
                .WithEndDate(DateTime.UtcNow.AddDays(31));
        }

        public static SetPartnerLimitRequestBuilder CreateWithInvalidLimit()
        {
            return new SetPartnerLimitRequestBuilder()
                .WithLimit(0)
                .WithStartDate(DateTime.UtcNow.AddDays(1))
                .WithEndDate(DateTime.UtcNow.AddDays(31));
        }

        public static SetPartnerLimitRequestBuilder CreateWithInvalidDates()
        {
            return new SetPartnerLimitRequestBuilder()
                .WithLimit(100)
                .WithStartDate(DateTime.UtcNow.AddDays(31))
                .WithEndDate(DateTime.UtcNow.AddDays(1));
        }

        public static SetPartnerLimitRequestBuilder CreateWithPastStartDate()
        {
            return new SetPartnerLimitRequestBuilder()
                .WithLimit(100)
                .WithStartDate(DateTime.UtcNow.AddDays(-1))
                .WithEndDate(DateTime.UtcNow.AddDays(30));
        }
    }
}
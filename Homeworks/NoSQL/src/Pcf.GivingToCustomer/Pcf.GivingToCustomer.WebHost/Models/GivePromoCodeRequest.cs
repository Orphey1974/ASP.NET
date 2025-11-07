using System;

namespace Pcf.GivingToCustomer.WebHost.Models
{
    /// <example>
    /// {
    ///     "serviceInfo": "Билеты на лучший спектакль сезона",
    ///     "partnerId": "7d994823-86a1-4d65-bda5-8b7924218a19",
    ///     "promoCodeId": "a6c8c6b1-4349-45b0-ab31-244740aaf0f0",
    ///     "promoCode": "H123124",
    ///     "preferenceId": "ef7f299f-92d7-459f-896e-078ed53ef99c",
    ///     "beginDate": "2024-01-01",
    ///     "endDate": "2024-12-31"
    /// }
    /// </example>
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PromoCodeId { get; set; }

        public string PromoCode { get; set; }

        public Guid PreferenceId { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }
    }
}
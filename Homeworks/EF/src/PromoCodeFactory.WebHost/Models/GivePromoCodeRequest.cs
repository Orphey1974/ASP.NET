namespace PromoCodeFactory.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; } = string.Empty;

        public string PartnerName { get; set; } = string.Empty;

        public string PromoCode { get; set; } = string.Empty;

        public string Preference { get; set; } = string.Empty;
    }
}
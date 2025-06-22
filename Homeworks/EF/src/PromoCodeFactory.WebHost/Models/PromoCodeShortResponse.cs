using System;

namespace PromoCodeFactory.WebHost.Models
{
    /// <summary>
    /// Краткая модель ответа для промокода
    /// </summary>
    public class PromoCodeShortResponse
    {
        /// <summary>
        /// Идентификатор промокода
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Код промокода
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Информация об услуге
        /// </summary>
        public string ServiceInfo { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Название партнера
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// Название предпочтения
        /// </summary>
        public string PreferenceName { get; set; }

        /// <summary>
        /// Имя клиента, которому выдан промокод
        /// </summary>
        public string CustomerName { get; set; }
    }
}
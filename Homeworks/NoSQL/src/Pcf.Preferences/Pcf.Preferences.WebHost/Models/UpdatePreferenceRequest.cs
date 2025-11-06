using System.ComponentModel.DataAnnotations;

namespace Pcf.Preferences.WebHost.Models
{
    /// <summary>
    /// Модель запроса для обновления предпочтения
    /// </summary>
    public class UpdatePreferenceRequest
    {
        /// <summary>
        /// Название предпочтения
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание предпочтения
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }
    }
}

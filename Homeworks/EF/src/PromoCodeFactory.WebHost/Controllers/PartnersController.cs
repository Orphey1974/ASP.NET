using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnersController : ControllerBase
    {
        private readonly IRepository<Partner> _partnerRepository;
        private readonly IRepository<PartnerLimit> _partnerLimitRepository;

        public PartnersController(
            IRepository<Partner> partnerRepository,
            IRepository<PartnerLimit> partnerLimitRepository)
        {
            _partnerRepository = partnerRepository;
            _partnerLimitRepository = partnerLimitRepository;
        }

        [HttpPost("{partnerId}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(
            Guid partnerId,
            [FromBody] SetPartnerLimitRequest request)
        {
            // Проверка наличия партнера
            var partner = await _partnerRepository.GetByIdAsync(partnerId);
            if (partner == null)
            {
                return NotFound($"Partner with ID {partnerId} not found");
            }

            // Проверка активности партнера
            if (!partner.IsActive)
            {
                return BadRequest($"Partner {partner.Name} is not active");
            }

            // Валидация лимита
            if (request.Limit <= 0)
            {
                return BadRequest("Limit must be greater than 0");
            }

            if (request.StartDate >= request.EndDate)
            {
                return BadRequest("StartDate must be before EndDate");
            }

            if (request.StartDate < DateTime.UtcNow)
            {
                return BadRequest("StartDate cannot be in the past");
            }

            // Отключение предыдущего активного лимита
            var existingActiveLimit = (await _partnerLimitRepository.GetAllAsync())
                .FirstOrDefault(pl => pl.PartnerId == partnerId && pl.IsActive);

            if (existingActiveLimit != null)
            {
                existingActiveLimit.IsActive = false;
                existingActiveLimit.UpdatedAt = DateTime.UtcNow;
                await _partnerLimitRepository.UpdateAsync(existingActiveLimit);
            }

            // Создание нового лимита
            var newLimit = new PartnerLimit
            {
                PartnerId = partnerId,
                Limit = request.Limit,
                CurrentCount = 0, // Обнуление счетчика
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _partnerLimitRepository.AddAsync(newLimit);

            return Ok(new PartnerLimitResponse
            {
                Id = newLimit.Id,
                PartnerId = newLimit.PartnerId,
                Limit = newLimit.Limit,
                CurrentCount = newLimit.CurrentCount,
                StartDate = newLimit.StartDate,
                EndDate = newLimit.EndDate,
                IsActive = newLimit.IsActive
            });
        }

        /// <summary>
        /// Получить список всех партнёров
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPartnersAsync()
        {
            // Получаем всех партнёров
            var partners = await _partnerRepository.GetAllAsync();
            return Ok(partners);
        }

        /// <summary>
        /// Получить партнёра по идентификатору
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPartnerByIdAsync(Guid id)
        {
            // Получаем партнёра по id
            var partner = await _partnerRepository.GetByIdAsync(id);
            if (partner == null)
                return NotFound();
            return Ok(partner);
        }

        /// <summary>
        /// Добавить нового партнёра
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddPartnerAsync([FromBody] Partner partner)
        {
            // Генерируем новый идентификатор и дату создания
            partner.Id = Guid.NewGuid();
            partner.CreatedAt = DateTime.UtcNow;
            await _partnerRepository.AddAsync(partner);
            // Возвращаем результат с маршрутом для получения созданного партнёра
            return CreatedAtAction(nameof(GetPartnerByIdAsync), new { id = partner.Id }, partner);
        }

        /// <summary>
        /// Удалить партнёра по идентификатору
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePartnerAsync(Guid id)
        {
            // Получаем партнёра по id
            var partner = await _partnerRepository.GetByIdAsync(id);
            if (partner == null)
                return NotFound();
            await _partnerRepository.DeleteAsync(partner);
            return NoContent();
        }
    }

    public class SetPartnerLimitRequest
    {
        public int Limit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class PartnerLimitResponse
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public int Limit { get; set; }
        public int CurrentCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
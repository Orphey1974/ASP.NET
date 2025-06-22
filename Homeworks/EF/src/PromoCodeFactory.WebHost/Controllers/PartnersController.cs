using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PartnerManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnersController : ControllerBase
    {
        private readonly IRepository<Partner> _partnerRepository;
        private readonly IRepository<PartnerLimit> _partnerLimitRepository;
        private readonly PromoCodeFactoryDbContext _context;

        public PartnersController(
            IRepository<Partner> partnerRepository,
            IRepository<PartnerLimit> partnerLimitRepository,
            PromoCodeFactoryDbContext context)
        {
            _partnerRepository = partnerRepository;
            _partnerLimitRepository = partnerLimitRepository;
            _context = context;
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
            var existingActiveLimit = await _context.PartnerLimits
                .FirstOrDefaultAsync(pl => pl.PartnerId == partnerId && pl.IsActive);

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
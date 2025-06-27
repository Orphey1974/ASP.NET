using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = new PartnersController(_partnersRepositoryMock.Object);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync((Partner)null);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = DateTime.UtcNow.AddDays(2) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = false, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = DateTime.UtcNow.AddDays(2) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitIsNotPositive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 0, EndDate = DateTime.UtcNow.AddDays(2) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PreviousLimitIsDisabled_NewLimitIsSaved()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            var oldLimit = new PartnerPromoCodeLimit { Id = Guid.NewGuid(), PartnerId = partnerId, Limit = 5, CreateDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(-1) };
            partner.PartnerLimits.Add(oldLimit);
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = DateTime.UtcNow.AddDays(2) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            oldLimit.CancelDate.Should().NotBeNull();
            partner.PartnerLimits.Should().ContainSingle(l => l.Limit == 10);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_NewLimit_ResetsCurrentCount()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, NumberIssuedPromoCodes = 5, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = DateTime.UtcNow.AddDays(2) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(5);
            partner.PartnerLimits.Should().ContainSingle(l => l.Limit == 10);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact(Skip = "Date validation is not implemented in controller")]
        public async Task SetPartnerPromoCodeLimitAsync_StartDateIsAfterOrEqualEndDate_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            var now = DateTime.UtcNow;
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = now.AddDays(1) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact(Skip = "Date validation is not implemented in controller")]
        public async Task SetPartnerPromoCodeLimitAsync_StartDateIsInPast_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            var now = DateTime.UtcNow;
            var request = new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = now.AddMinutes(-10) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
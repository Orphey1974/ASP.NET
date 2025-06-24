using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PartnerManagement;
using PromoCodeFactory.Tests.Builders;
using PromoCodeFactory.WebHost.Controllers;
using Xunit;

namespace PromoCodeFactory.Tests.Controllers
{
    public class PartnersControllerTests : TestBase
    {
        private readonly PartnersController _controller;

        public PartnersControllerTests()
        {
            _controller = new PartnersController(
                MockPartnerRepository.Object,
                MockPartnerLimitRepository.Object);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenPartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var request = SetPartnerLimitRequestBuilder.CreateValid().Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync((Partner?)null);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            AssertNotFoundResult(result, $"Partner with ID {partnerId} not found");
            MockPartnerRepository.Verify(x => x.GetByIdAsync(partnerId), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenPartnerIsInactive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateInactive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.CreateValid().Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            AssertBadRequestResult(result, $"Partner {partner.Name} is not active");
            MockPartnerRepository.Verify(x => x.GetByIdAsync(partnerId), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenLimitIsZero_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.CreateWithInvalidLimit().Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            AssertBadRequestResult(result, "Limit must be greater than 0");
            MockPartnerRepository.Verify(x => x.GetByIdAsync(partnerId), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenLimitIsNegative_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.Create().WithLimit(-10).Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            AssertBadRequestResult(result, "Limit must be greater than 0");
            MockPartnerRepository.Verify(x => x.GetByIdAsync(partnerId), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenStartDateIsAfterEndDate_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.CreateWithInvalidDates().Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            AssertBadRequestResult(result, "StartDate must be before EndDate");
            MockPartnerRepository.Verify(x => x.GetByIdAsync(partnerId), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenStartDateIsInPast_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.CreateWithPastStartDate().Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            AssertBadRequestResult(result, "StartDate cannot be in the past");
            MockPartnerRepository.Verify(x => x.GetByIdAsync(partnerId), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenNoExistingActiveLimit_CreatesNewLimit()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.CreateValid().Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            MockPartnerLimitRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<PartnerLimit>());

            MockPartnerLimitRepository
                .Setup(x => x.AddAsync(It.IsAny<PartnerLimit>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as PromoCodeFactory.WebHost.Controllers.PartnerLimitResponse;

            response.Should().NotBeNull();
            response!.PartnerId.Should().Be(partnerId);
            response.Limit.Should().Be(request.Limit);
            response.CurrentCount.Should().Be(0);
            response.StartDate.Should().Be(request.StartDate);
            response.EndDate.Should().Be(request.EndDate);
            response.IsActive.Should().BeTrue();

            MockPartnerLimitRepository.Verify(x => x.AddAsync(It.Is<PartnerLimit>(pl =>
                pl.PartnerId == partnerId &&
                pl.Limit == request.Limit &&
                pl.CurrentCount == 0 &&
                pl.IsActive)), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenExistingActiveLimit_DeactivatesOldAndCreatesNew()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.CreateValid().Build();

            var existingLimit = PartnerLimitBuilder.CreateActive()
                .WithPartnerId(partnerId)
                .WithLimit(50)
                .WithCurrentCount(25)
                .Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            MockPartnerLimitRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<PartnerLimit> { existingLimit });

            MockPartnerLimitRepository
                .Setup(x => x.UpdateAsync(It.IsAny<PartnerLimit>()))
                .Returns(Task.CompletedTask);

            MockPartnerLimitRepository
                .Setup(x => x.AddAsync(It.IsAny<PartnerLimit>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            // Verify that existing limit was deactivated
            MockPartnerLimitRepository.Verify(x => x.UpdateAsync(It.Is<PartnerLimit>(pl =>
                pl.Id == existingLimit.Id &&
                !pl.IsActive)), Times.Once);

            // Verify that new limit was created
            MockPartnerLimitRepository.Verify(x => x.AddAsync(It.Is<PartnerLimit>(pl =>
                pl.PartnerId == partnerId &&
                pl.Limit == request.Limit &&
                pl.CurrentCount == 0 &&
                pl.IsActive)), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task SetPartnerPromoCodeLimitAsync_WithValidLimit_CreatesLimitWithCorrectValue(int limit)
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = PartnerBuilder.CreateActive().WithId(partnerId).Build();
            var request = SetPartnerLimitRequestBuilder.Create().WithLimit(limit).Build();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            MockPartnerLimitRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<PartnerLimit>());

            MockPartnerLimitRepository
                .Setup(x => x.AddAsync(It.IsAny<PartnerLimit>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as PromoCodeFactory.WebHost.Controllers.PartnerLimitResponse;

            response!.Limit.Should().Be(limit);

            MockPartnerLimitRepository.Verify(x => x.AddAsync(It.Is<PartnerLimit>(pl =>
                pl.Limit == limit)), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WithAutoFixture_WorksCorrectly()
        {
            // Arrange
            var partnerId = Fixture.Create<Guid>();
            var partner = Fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .Create();

            var request = Fixture.Build<PromoCodeFactory.WebHost.Controllers.SetPartnerLimitRequest>()
                .With(r => r.Limit, 100)
                .With(r => r.StartDate, DateTime.UtcNow.AddDays(1))
                .With(r => r.EndDate, DateTime.UtcNow.AddDays(31))
                .Create();

            MockPartnerRepository
                .Setup(x => x.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            MockPartnerLimitRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<PartnerLimit>());

            MockPartnerLimitRepository
                .Setup(x => x.AddAsync(It.IsAny<PartnerLimit>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as PromoCodeFactory.WebHost.Controllers.PartnerLimitResponse;

            response.Should().NotBeNull();
            response!.PartnerId.Should().Be(partnerId);
            response.Limit.Should().Be(request.Limit);
            response.CurrentCount.Should().Be(0);
            response.IsActive.Should().BeTrue();
        }
    }
}
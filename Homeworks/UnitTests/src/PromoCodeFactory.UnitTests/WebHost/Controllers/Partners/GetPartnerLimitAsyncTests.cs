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
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class GetPartnerLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public GetPartnerLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        private Partner CreateBasePartner()
        {
            return new Partner
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "Суперигрушки",
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2020, 07, 9),
                        EndDate = new DateTime(2020, 10, 9),
                        Limit = 100
                    }
                }
            };
        }

        [Fact]
        public async Task GetPartnerLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            Partner partner = null;

            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var result = await _partnersController.GetPartnerLimitAsync(partnerId, Guid.NewGuid());

            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetPartnerLimitAsync_LimitIsNotFound_ReturnsNotFound()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();

            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var result = await _partnersController.GetPartnerLimitAsync(partnerId, Guid.NewGuid());

            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}

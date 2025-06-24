using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PartnerManagement;

namespace PromoCodeFactory.Tests
{
    public abstract class TestBase
    {
        protected readonly IFixture Fixture;
        protected readonly Mock<IRepository<Partner>> MockPartnerRepository;
        protected readonly Mock<IRepository<PartnerLimit>> MockPartnerLimitRepository;

        protected TestBase()
        {
            Fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            // Настройка AutoFixture для обработки циклических ссылок
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            MockPartnerRepository = new Mock<IRepository<Partner>>();
            MockPartnerLimitRepository = new Mock<IRepository<PartnerLimit>>();
        }

        protected void AssertNotFoundResult(IActionResult result, string expectedMessage)
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().Be(expectedMessage);
        }

        protected void AssertBadRequestResult(IActionResult result, string expectedMessage)
        {
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(expectedMessage);
        }

        protected void AssertOkResult<T>(IActionResult result, T expectedValue)
        {
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedValue);
        }
    }
}
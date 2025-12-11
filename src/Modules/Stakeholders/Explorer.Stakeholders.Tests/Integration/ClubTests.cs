using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class ClubTests : BaseStakeholdersIntegrationTest
    {
        public ClubTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_club()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var newClub = new ClubDto
            {
                Name = "Novi Test Klub",
                Description = "Opis novog kluba.",
                ImageUris = new List<string> { "slika3.png" },
                OwnerId = -21
            };

            // Act
            var result = ((ObjectResult)controller.Create(newClub).Result)?.Value as ClubDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(newClub.Name);
            result.OwnerId.ShouldBe(-21);

            var storedClub = dbContext.Clubs.FirstOrDefault(c => c.Name == newClub.Name);
            storedClub.ShouldNotBeNull();
            storedClub.Id.ShouldBe(result.Id);
            storedClub.OwnerId.ShouldBe(-21);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var invalidClub = new ClubDto
            {
                Name = "",
                Description = "Opis.",
                ImageUris = new List<string> { "slika.png" }
            };

            Should.Throw<ArgumentException>(() => controller.Create(invalidClub));
        }

        [Fact]
        public void Retrieves_all_clubs()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<ClubDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
        }

        [Fact]
        public void Updates_club()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubToCreate = new ClubDto
            {
                Name = "Klub za azuriranje",
                Description = "Originalni opis.",
                ImageUris = new List<string> { "orig.png" }
            };

            var createdClub = ((ObjectResult)controller.Create(clubToCreate).Result)?.Value as ClubDto;

            var clubToUpdate = new ClubDto
            {
                Id = createdClub.Id,
                Name = "AZURIRANO IME",
                Description = createdClub.Description,
                ImageUris = createdClub.ImageUris,
                OwnerId = createdClub.OwnerId
            };

            var result = ((ObjectResult)controller.Update(clubToUpdate).Result)?.Value as ClubDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdClub.Id);
            result.Name.ShouldBe("AZURIRANO IME");

            var entityInDb = dbContext.Clubs.Find(createdClub.Id);
            entityInDb.ShouldNotBeNull();
            entityInDb.Name.ShouldBe("AZURIRANO IME");
        }

        [Fact]
        public void Deletes_club()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubToCreate = new ClubDto
            {
                Name = "Klub za brisanje",
                Description = "Opis.",
                ImageUris = new List<string> { "del.png" }
            };

            var createdClub = ((ObjectResult)controller.Create(clubToCreate).Result)?.Value as ClubDto;

            var deleteResult = (OkResult)controller.Delete(createdClub.Id);

            deleteResult.StatusCode.ShouldBe(200);

            var entityInDb = dbContext.Clubs.Find(createdClub.Id);
            entityInDb.ShouldBeNull();
        }

        private static ClubController CreateController(IServiceScope scope)
        {
            var controller = new ClubController(scope.ServiceProvider.GetRequiredService<IClubService>());


            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", "-21"),
                new Claim("personId", "-21")
            }, "test"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }
    }
}
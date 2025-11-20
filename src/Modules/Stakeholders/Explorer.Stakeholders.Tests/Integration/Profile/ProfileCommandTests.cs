using System.IO;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using AuthorProfileController = Explorer.API.Controllers.Author.ProfileController;
using TouristProfileController = Explorer.API.Controllers.Tourist.ProfileController;

namespace Explorer.Stakeholders.Tests.Integration.Profile;

[Collection("Sequential")]
public class ProfileCommandTests : BaseStakeholdersIntegrationTest
{
    private const long PrimaryTouristPersonId = -21;
    private const long SecondaryTouristPersonId = -22;
    private const long PrimaryAuthorPersonId = -11;
    private const long SecondaryAuthorPersonId = -12;

    public ProfileCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_existing_profile_for_tourist()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateTouristController(scope);

        var result = (OkObjectResult)controller.Get().Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe("Pera");
        dto.Surname.ShouldBe("Peric");
        dto.Biography.ShouldBe("Planinar i ljubitelj prirode.");
        dto.Motto.ShouldBe("Carpe diem");
        dto.ProfilePictureUrl.ShouldBe("http://example.com/pera.jpg");
    }

    [Fact]
    public void Gets_existing_profile_for_author()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateAuthorController(scope);

        var result = (OkObjectResult)controller.Get().Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe("Ana");
        dto.Surname.ShouldBe("Anic");
        dto.Biography.ShouldBe("Autorka avanturistickih tura.");
        dto.Motto.ShouldBe("Pisem dok lutam.");
        dto.ProfilePictureUrl.ShouldBeNull(); // frontend falls back to default avatar
    }

    [Fact]
    public void Updates_profile_for_tourist()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateTouristController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var updated = BuildValidProfileDto("Petar", "Petrovic");
        updated.Biography = "Volim planinarenje i nove izazove.";
        updated.Motto = "Uvek spreman";
        updated.ProfilePictureUrl = "http://example.com/petar.jpg";

        var result = (OkObjectResult)controller.Update(updated).Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe(updated.Name);
        dto.Surname.ShouldBe(updated.Surname);
        dto.Biography.ShouldBe(updated.Biography);
        dto.Motto.ShouldBe(updated.Motto);
        dto.ProfilePictureUrl.ShouldBe(updated.ProfilePictureUrl);

        var stored = dbContext.Profiles.First(p => p.PersonId == PrimaryTouristPersonId);
        stored.Name.ShouldBe(updated.Name);
        stored.Surname.ShouldBe(updated.Surname);
        stored.Biography.ShouldBe(updated.Biography);
        stored.Motto.ShouldBe(updated.Motto);
        stored.ProfilePictureUrl.ShouldBe(updated.ProfilePictureUrl);
    }

    [Fact]
    public void Updates_profile_for_author()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateAuthorController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var updated = BuildValidProfileDto("Ana", "Anic");
        updated.Biography = "Pisanje i putovanja su moj posao.";
        updated.Motto = "Inspiriši i vodi.";
        updated.ProfilePictureUrl = "http://example.com/ana.jpg";

        var result = (OkObjectResult)controller.Update(updated).Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe(updated.Name);
        dto.Surname.ShouldBe(updated.Surname);
        dto.Biography.ShouldBe(updated.Biography);
        dto.Motto.ShouldBe(updated.Motto);
        dto.ProfilePictureUrl.ShouldBe(updated.ProfilePictureUrl);

        var stored = dbContext.Profiles.First(p => p.PersonId == PrimaryAuthorPersonId);
        stored.Name.ShouldBe(updated.Name);
        stored.Surname.ShouldBe(updated.Surname);
        stored.Biography.ShouldBe(updated.Biography);
        stored.Motto.ShouldBe(updated.Motto);
        stored.ProfilePictureUrl.ShouldBe(updated.ProfilePictureUrl);
    }

    [Fact]
    public void Creates_profile_for_author_if_missing()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateAuthorController(scope, SecondaryAuthorPersonId);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var result = (OkObjectResult)controller.Get().Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe("Lena");
        dto.Surname.ShouldBe("Lenic");

        var stored = dbContext.Profiles.FirstOrDefault(p => p.PersonId == SecondaryAuthorPersonId);
        stored.ShouldNotBeNull();
        stored!.Name.ShouldBe("Lena");
        stored.Surname.ShouldBe("Lenic");
    }

    [Fact]
    public void Tourist_profile_without_picture_uses_default_avatar()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateTouristController(scope, SecondaryTouristPersonId);

        var result = (OkObjectResult)controller.Get().Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.ProfilePictureUrl.ShouldBeNull();
    }

    [Fact]
    public void Author_profile_without_picture_uses_default_avatar()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateAuthorController(scope);

        var result = (OkObjectResult)controller.Get().Result!;
        var dto = result.Value as ProfileDto;

        dto.ShouldNotBeNull();
        dto.ProfilePictureUrl.ShouldBeNull();
    }

    [Theory]
    [InlineData("", "Tester", true)]
    [InlineData("Tester", "", true)]
    [InlineData("", "Tester", false)]
    [InlineData("Tester", "", false)]
    public void Update_profile_fails_for_missing_required_fields(string name, string surname, bool forTourist)
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var dto = BuildValidProfileDto(name, surname);

        if (forTourist)
        {
            var controller = CreateTouristController(scope);
            Should.Throw<EntityValidationException>(() => controller.Update(dto));
        }
        else
        {
            var controller = CreateAuthorController(scope);
            Should.Throw<EntityValidationException>(() => controller.Update(dto));
        }
    }

    [Fact]
    public void Update_profile_fails_when_biography_is_too_long()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateTouristController(scope);
        var dto = BuildValidProfileDto();
        dto.Biography = new string('b', 251);

        Should.Throw<EntityValidationException>(() => controller.Update(dto));
    }

    [Fact]
    public void Update_author_profile_fails_when_motto_is_too_long()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateAuthorController(scope);
        var dto = BuildValidProfileDto();
        dto.Motto = new string('m', 251);

        Should.Throw<EntityValidationException>(() => controller.Update(dto));
    }

    [Fact]
    public void Tourist_can_only_update_his_profile()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateTouristController(scope, SecondaryTouristPersonId);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var untouchedBefore = dbContext.Profiles.First(p => p.PersonId == PrimaryTouristPersonId);

        var dto = BuildValidProfileDto("Milan", "Milic");
        dto.Biography = "Podesavanje samo za mene.";
        controller.Update(dto);

        var own = dbContext.Profiles.First(p => p.PersonId == SecondaryTouristPersonId);
        own.Name.ShouldBe(dto.Name);
        own.Biography.ShouldBe(dto.Biography);

        var stillOriginal = dbContext.Profiles.First(p => p.PersonId == PrimaryTouristPersonId);
        stillOriginal.Name.ShouldBe(untouchedBefore.Name);
        stillOriginal.Biography.ShouldBe(untouchedBefore.Biography);
    }

    [Fact]
    public void Author_can_only_update_his_profile()
    {
        using var scope = Factory.Services.CreateScope();
        ResetDatabase(scope);
        var controller = CreateAuthorController(scope, SecondaryAuthorPersonId);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var untouchedBefore = dbContext.Profiles.First(p => p.PersonId == PrimaryAuthorPersonId);

        var dto = BuildValidProfileDto("Lena", "Lenic");
        dto.Biography = "Moj licni opis.";
        controller.Update(dto);

        var own = dbContext.Profiles.First(p => p.PersonId == SecondaryAuthorPersonId);
        own.Name.ShouldBe(dto.Name);
        own.Biography.ShouldBe(dto.Biography);

        var stillOriginal = dbContext.Profiles.First(p => p.PersonId == PrimaryAuthorPersonId);
        stillOriginal.Name.ShouldBe(untouchedBefore.Name);
        stillOriginal.Biography.ShouldBe(untouchedBefore.Biography);
    }

    private static ProfileDto BuildValidProfileDto(string name = "Test", string surname = "Tester")
        => new ProfileDto
        {
            Name = name,
            Surname = surname,
            Biography = "Kratak opis.",
            Motto = "Carpe diem",
            ProfilePictureUrl = "http://example.com/default.jpg"
        };

    private static TouristProfileController CreateTouristController(IServiceScope scope, long personId = PrimaryTouristPersonId)
    {
        return new TouristProfileController(scope.ServiceProvider.GetRequiredService<IProfileService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }

    private static AuthorProfileController CreateAuthorController(IServiceScope scope, long personId = PrimaryAuthorPersonId)
    {
        return new AuthorProfileController(scope.ServiceProvider.GetRequiredService<IProfileService>())
        {
            ControllerContext = BuildContext(personId.ToString())
        };
    }

    private static void ResetDatabase(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var path = Path.Combine(".", "..", "..", "..", "TestData");
        var scriptFiles = Directory.GetFiles(path);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        dbContext.Database.ExecuteSqlRaw(script);
    }
}

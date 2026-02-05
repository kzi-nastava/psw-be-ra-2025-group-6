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
    public class MembershipRequestTests : BaseStakeholdersIntegrationTest
    {
        public MembershipRequestTests(StakeholdersTestFactory factory) : base(factory)
        {
            using var scope = factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            dbContext.ClubMembershipRequests.RemoveRange(dbContext.ClubMembershipRequests);
            dbContext.SaveChanges();
        }

        [Fact]
        public void Tourist_sends_request_to_active_club()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateRequestController(scope, -23); // Turista koji nije vlasnik
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Act - Šalje zahtev aktivnom klubu -21
            var result = ((ObjectResult)controller.SendRequest(-21).Result)?.Value as ClubMembershipRequestDto;

            // Assert
            result.ShouldNotBeNull();
            result.ClubId.ShouldBe(-21);
            result.TouristId.ShouldBe(-23);
            result.Status.ShouldBe(0); // Processing

            var storedRequest = dbContext.ClubMembershipRequests.FirstOrDefault(r => r.Id == result.Id);
            storedRequest.ShouldNotBeNull();
        }

        [Fact]
        public void Tourist_cannot_send_request_to_closed_club()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateRequestController(scope, -21);

            // Act & Assert - Klub -2 je Closed (status 1) u seed-u
            Should.Throw<InvalidOperationException>(() => controller.SendRequest(-2));
        }

        [Fact]
        public void Tourist_withdraws_request()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateRequestController(scope, -23);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Prvo posaljemo zahtev
            var request = controller.SendRequest(-21).Result as ObjectResult;
            var requestId = ((ClubMembershipRequestDto)request.Value).Id;

            // Act
            var result = controller.WithdrawRequest(requestId);

            // Assert
            result.ShouldBeOfType<OkResult>();
            dbContext.ClubMembershipRequests.Find(requestId).ShouldBeNull();
        }

        [Fact]
        public void Owner_accepts_request_creates_member_and_notifies()
        {
            using var scope = Factory.Services.CreateScope();
            var touristController = CreateRequestController(scope, -23);
            var ownerController = CreateRequestController(scope, -21); // Vlasnik kluba -21 je korisnik -21
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Turista šalje zahtev
            var requestDto = ((ObjectResult)touristController.SendRequest(-21).Result).Value as ClubMembershipRequestDto;

            // Act - Vlasnik prihvata
            var result = ownerController.AcceptRequest(requestDto.Id);

            // Assert
            result.ShouldBeOfType<OkResult>();

            // 1. Provera člana (integracija sa ClubMember)
            var member = dbContext.ClubMembers.FirstOrDefault(m => m.ClubId == -21 && m.UserId == -23);
            member.ShouldNotBeNull();

            // 2. Zahtev obrisan
            dbContext.ClubMembershipRequests.Find(requestDto.Id).ShouldBeNull();

            // 3. Notifikacija
            var notification = dbContext.Notifications.FirstOrDefault(n => n.RecipientId == -23 && n.Content.Contains("accepted"));
            notification.ShouldNotBeNull();
        }

        [Fact]
        public void Owner_rejects_request_and_notifies()
        {
            using var scope = Factory.Services.CreateScope();
            var touristController = CreateRequestController(scope, -23);
            var ownerController = CreateRequestController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var requestDto = ((ObjectResult)touristController.SendRequest(-21).Result).Value as ClubMembershipRequestDto;

            // Act
            var result = ownerController.RejectRequest(requestDto.Id);

            // Assert
            result.ShouldBeOfType<OkResult>();
            dbContext.ClubMembershipRequests.Find(requestDto.Id).ShouldBeNull();

            var notification = dbContext.Notifications.FirstOrDefault(n => n.RecipientId == -23 && n.Content.Contains("rejected"));
            notification.ShouldNotBeNull();
        }

        private static MembershipRequestController CreateRequestController(IServiceScope scope, long userId)
        {
            var controller = new MembershipRequestController(scope.ServiceProvider.GetRequiredService<IMembershipRequestService>());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", userId.ToString()),
            }, "test"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }
    }
}
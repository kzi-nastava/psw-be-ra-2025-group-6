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
    public class ClubOwnerTests : BaseStakeholdersIntegrationTest
    {
        public ClubOwnerTests(StakeholdersTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void Owner_changes_club_status_to_closed()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Create a club
            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var changeStatusDto = new ChangeClubStatusDto { Status = "Closed" };

            // Act
            var result = ((ObjectResult)controller.ChangeStatus(club.Id, changeStatusDto).Result)?.Value as ClubDto;

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe("Closed");

            var updatedClub = dbContext.Clubs.Find(club.Id);
            updatedClub.ShouldNotBeNull();
            updatedClub.Status.ToString().ShouldBe("Closed");
        }

        [Fact]
        public void Owner_changes_club_status_to_active()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Closed Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Closed"
            });

            var changeStatusDto = new ChangeClubStatusDto { Status = "Active" };

            // Act
            var result = ((ObjectResult)controller.ChangeStatus(club.Id, changeStatusDto).Result)?.Value as ClubDto;

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe("Active");
        }

        [Fact]
        public void Non_owner_cannot_change_club_status()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateOwnerController(scope, -21);
            var nonOwnerController = CreateOwnerController(scope, -22);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Owner Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var changeStatusDto = new ChangeClubStatusDto { Status = "Closed" };

            // Act & Assert
            var result = nonOwnerController.ChangeStatus(club.Id, changeStatusDto).Result;
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Owner_invites_member_to_active_club()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Active Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var inviteDto = new InviteToClubDto { Username = "turista2@gmail.com" };

            // Act
            var result = ((ObjectResult)controller.InviteMember(club.Id, inviteDto).Result)?.Value as ClubMemberDto;

            // Assert
            result.ShouldNotBeNull();
            result.ClubId.ShouldBe(club.Id);
            result.Status.ShouldBe("Active");

            // Verify member exists in database
            var member = dbContext.ClubMembers.FirstOrDefault(m => m.ClubId == club.Id && m.UserId == -22);
            member.ShouldNotBeNull();
        }

        [Fact]
        public void Owner_cannot_invite_member_to_closed_club()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Closed Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Closed"
            });

            var inviteDto = new InviteToClubDto { Username = "turista2@gmail.com" };

            // Act & Assert
            var result = controller.InviteMember(club.Id, inviteDto).Result;
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Owner_cannot_invite_already_existing_member()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var inviteDto = new InviteToClubDto { Username = "turista2@gmail.com" };

            // Invite once
            controller.InviteMember(club.Id, inviteDto);

            // Act & Assert - Try to invite again
            var result = controller.InviteMember(club.Id, inviteDto).Result;
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Owner_cannot_invite_non_existent_user()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var inviteDto = new InviteToClubDto { Username = "nonexistent@gmail.com" };

            // Act & Assert
            var result = controller.InviteMember(club.Id, inviteDto).Result;
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Non_owner_cannot_invite_members()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateOwnerController(scope, -21);
            var nonOwnerController = CreateOwnerController(scope, -22);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Owner Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var inviteDto = new InviteToClubDto { Username = "turista3@gmail.com" };

            // Act & Assert
            var result = nonOwnerController.InviteMember(club.Id, inviteDto).Result;
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Owner_gets_all_club_members()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Invite two members
            clubService.InviteMember(club.Id, "turista2@gmail.com", -21);
            clubService.InviteMember(club.Id, "turista3@gmail.com", -21);

            // Act
            var result = ((ObjectResult)controller.GetMembers(club.Id).Result)?.Value as List<ClubMemberDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldAllBe(m => m.Status == "Active");
        }

        [Fact]
        public void Non_owner_cannot_get_club_members()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateOwnerController(scope, -21);
            var nonOwnerController = CreateOwnerController(scope, -22);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Owner Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Act & Assert
            var result = nonOwnerController.GetMembers(club.Id).Result;
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Owner_removes_member_from_active_club()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Invite member
            var member = clubService.InviteMember(club.Id, "turista2@gmail.com", -21);

            // Act
            var result = controller.RemoveMember(club.Id, member.Id);

            // Assert
            result.ShouldBeOfType<NoContentResult>();

            // Verify member is removed from database
            var removedMember = dbContext.ClubMembers.Find(member.Id);
            removedMember.ShouldBeNull();
        }

        [Fact]
        public void Owner_cannot_remove_member_from_closed_club()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Invite member
            var member = clubService.InviteMember(club.Id, "turista2@gmail.com", -21);

            // Close the club
            clubService.ChangeStatus(club.Id, "Closed", -21);

            // Act & Assert
            var result = controller.RemoveMember(club.Id, member.Id);
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Non_owner_cannot_remove_members()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateOwnerController(scope, -21);
            var nonOwnerController = CreateOwnerController(scope, -22);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Owner Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Invite member
            var member = clubService.InviteMember(club.Id, "turista3@gmail.com", -21);

            // Act & Assert
            var result = nonOwnerController.RemoveMember(club.Id, member.Id);
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Member_receives_notification_when_invited()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Notification Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var inviteDto = new InviteToClubDto { Username = "turista2@gmail.com" };

            // Act
            controller.InviteMember(club.Id, inviteDto);

            // Assert - Check notification was created
            var notification = dbContext.Notifications
                .FirstOrDefault(n => n.RecipientId == -22 && 
                                    n.SenderId == -21 && 
                                    n.ReferenceId == club.Id);

            notification.ShouldNotBeNull();
            notification.Content.ShouldContain("invited to join the club");
            notification.Content.ShouldContain(club.Name);
            notification.Status.ToString().ShouldBe("Unread");
        }

        [Fact]
        public void Member_receives_notification_when_removed()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Removal Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Invite and then remove member
            var member = clubService.InviteMember(club.Id, "turista2@gmail.com", -21);

            // Clear previous notifications
            var existingNotifications = dbContext.Notifications.Where(n => n.RecipientId == -22).ToList();
            dbContext.Notifications.RemoveRange(existingNotifications);
            dbContext.SaveChanges();

            // Act
            controller.RemoveMember(club.Id, member.Id);

            // Assert - Check notification was created
            var notification = dbContext.Notifications
                .FirstOrDefault(n => n.RecipientId == -22 && 
                                    n.SenderId == -21 && 
                                    n.ReferenceId == club.Id);

            notification.ShouldNotBeNull();
            notification.Content.ShouldContain("removed from the club");
            notification.Content.ShouldContain(club.Name);
            notification.Status.ToString().ShouldBe("Unread");
        }

        [Fact]
        public void Owner_cannot_remove_non_existent_member()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Act & Assert
            var result = controller.RemoveMember(club.Id, 99999);
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Invited_member_appears_in_members_list()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            var inviteDto = new InviteToClubDto { Username = "turista2@gmail.com" };
            controller.InviteMember(club.Id, inviteDto);

            // Act
            var result = ((ObjectResult)controller.GetMembers(club.Id).Result)?.Value as List<ClubMemberDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].UserEmail.ShouldContain("turista2");
        }

        [Fact]
        public void Removed_member_does_not_appear_in_members_list()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateOwnerController(scope, -21);

            var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();
            var club = clubService.Create(new ClubDto
            {
                Name = "Test Club",
                Description = "Test Description",
                ImageUris = new List<string> { "image.jpg" },
                OwnerId = -21,
                Status = "Active"
            });

            // Invite two members
            clubService.InviteMember(club.Id, "turista2@gmail.com", -21);
            var member3 = clubService.InviteMember(club.Id, "turista3@gmail.com", -21);

            // Remove one member
            controller.RemoveMember(club.Id, member3.Id);

            // Act
            var result = ((ObjectResult)controller.GetMembers(club.Id).Result)?.Value as List<ClubMemberDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.ShouldNotContain(m => m.UserEmail.Contains("turista3"));
        }

        private static ClubOwnerController CreateOwnerController(IServiceScope scope, long personId)
        {
            var controller = new ClubOwnerController(scope.ServiceProvider.GetRequiredService<IClubService>());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", personId.ToString()),
                new Claim("personId", personId.ToString())
            }, "test"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }
    }
}

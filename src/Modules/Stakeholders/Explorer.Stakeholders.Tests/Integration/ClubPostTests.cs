using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class ClubPostTests : BaseStakeholdersIntegrationTest
    {
        public ClubPostTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_post()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var newPost = new ClubPostDto
            {
                ClubId = -1,
                Text = "New post in club",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = ((ObjectResult)controller.Create(newPost.ClubId, newPost).Result)?.Value as ClubPostDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Text.ShouldBe(newPost.Text);
            result.AuthorId.ShouldBe(-21);

            var storedPost = dbContext.ClubPosts.FirstOrDefault(p => p.Id == result.Id);
            storedPost.ShouldNotBeNull();
        }

        [Fact]
        public void Updates_post()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var initialPost = new Core.Domain.ClubPost(-21, -1, "Original text", null, null, DateTime.UtcNow, null);
            dbContext.ClubPosts.Add(initialPost);
            dbContext.SaveChanges();

            var postToUpdate = new ClubPostDto
            {
                Id = initialPost.Id,
                ClubId = -1,
                Text = "Updated text",
                UpdatedAt = DateTime.UtcNow,
                AuthorId = -21,
            };

            // Act
            var result = ((ObjectResult)controller.Update(postToUpdate.ClubId, postToUpdate.Id, postToUpdate).Result)?.Value as ClubPostDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(initialPost.Id);
            result.Text.ShouldBe("Updated text");

            var storedPost = dbContext.ClubPosts.FirstOrDefault(p => p.Id == initialPost.Id);
            storedPost.ShouldNotBeNull();
            storedPost.Text.ShouldBe("Updated text");
        }

        [Fact]
        public void Fails_to_update_post_if_not_author()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22"); // Different user
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var initialPost = new Core.Domain.ClubPost(-21, -1, "Original text", null, null, DateTime.UtcNow, null);
            dbContext.ClubPosts.Add(initialPost);
            dbContext.SaveChanges();
            
            var postToUpdate = new ClubPostDto
            {
                Id = initialPost.Id,
                ClubId = -1,
                Text = "Updated text",
                UpdatedAt = DateTime.UtcNow,
                AuthorId = -21,
            };

            // Act & Assert
            Should.Throw<UnauthorizedAccessException>(() => controller.Update(postToUpdate.ClubId, postToUpdate.Id, postToUpdate));
        }

        [Fact]
        public void Deletes_post()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21"); // Club owner
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var club = new Core.Domain.Club(
                "Test club",
                "Test description",
                new List<string> { "img.jpg" },
                -21 
            );
            dbContext.Clubs.Add(club);
            dbContext.SaveChanges();

            var initialPost = new Core.Domain.ClubPost(
                -22,        
                club.Id,    
                "Post to delete",
                null,
                null,
                DateTime.UtcNow,
                null
            );
            dbContext.ClubPosts.Add(initialPost);
            dbContext.SaveChanges();

            // Act
            var result = (OkResult)controller.Delete(club.Id, initialPost.Id);

            // Assert
            result.StatusCode.ShouldBe(200);

            var storedPost = dbContext.ClubPosts.FirstOrDefault(p => p.Id == initialPost.Id);
            storedPost.ShouldBeNull();
        }

        [Fact]
        public void Gets_all_posts_for_club()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            dbContext.ClubPosts.RemoveRange(dbContext.ClubPosts.Where(p => p.ClubId == -2));
            dbContext.SaveChanges();
            dbContext.ClubPosts.Add(new Core.Domain.ClubPost(-21, -2, "Post 1", null, null, DateTime.UtcNow, null));
            dbContext.ClubPosts.Add(new Core.Domain.ClubPost(-22, -2, "Post 2", null, null, DateTime.UtcNow, null));
            dbContext.SaveChanges();

            // Act
            var result = ((ObjectResult)controller.GetForClub(-2).Result)?.Value as List<ClubPostDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        private static ClubPostController CreateController(IServiceScope scope, string userId)
        {
            var controller = new ClubPostController(scope.ServiceProvider.GetRequiredService<IClubPostService>());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", userId),
                new Claim("personId", userId)
            }, "test"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }
    }
}

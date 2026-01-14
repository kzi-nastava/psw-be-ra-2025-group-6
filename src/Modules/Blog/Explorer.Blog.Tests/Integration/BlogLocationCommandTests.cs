using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Blog.Tests.Integration
{
    [Collection("Sequential")]
    public class BlogLocationCommandTests : BaseBlogIntegrationTest
    {
        public BlogLocationCommandTests(BlogTestFactory factory) : base(factory) { }

        private static ClaimsPrincipal CreateUser()
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "-11"),
            new Claim(ClaimTypes.Role, "Author")
        };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        [Fact]
        public void AddLocation_ToNewBlog_ShouldSetLocation()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var blog = new BlogPost(-11, "Blog sa lokacijom", "Opis", new List<string>(), BlogStatus.DRAFT);
            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();

            var location = new BlogLocation("Belgrade", "Serbia", 44.8176, 20.4569, "Central");
            blog.SetLocation(location);
            dbContext.SaveChanges();

            var storedBlog = dbContext.Blogs.Find(blog.Id);
            storedBlog.Location.ShouldNotBeNull();
            storedBlog.Location.City.ShouldBe("Belgrade");
        }

        [Fact]
        public void UpdateLocation_OnExistingBlog_ShouldChangeLocation()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var blog = new BlogPost(-11, "Blog sa lokacijom", "Opis", new List<string>(), BlogStatus.DRAFT);
            blog.SetLocation(new BlogLocation("Belgrade", "Serbia", 44.8176, 20.4569, "Central"));
            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();

            blog.UpdateLocation("Novi Sad", "Serbia", 45.2671, 19.8335, "Vojvodina");
            dbContext.SaveChanges();

            var storedBlog = dbContext.Blogs.Find(blog.Id);
            storedBlog.Location.City.ShouldBe("Novi Sad");
            storedBlog.Location.Region.ShouldBe("Vojvodina");
        }

        [Fact]
        public void AddLocation_InvalidData_ShouldThrow()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var blog = new BlogPost(-11, "Blog sa lokacijom", "Opis", new List<string>(), BlogStatus.DRAFT);
            dbContext.Blogs.Add(blog);
            dbContext.SaveChanges();

            Should.Throw<ArgumentException>(() =>
            {
                var invalidLocation = new BlogLocation("", "Serbia", 44, 20);
                blog.SetLocation(invalidLocation);
            });
        }
    }
}

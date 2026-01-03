namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class CommentCommandTests : BaseBlogIntegrationTest
{
    public CommentCommandTests(BlogTestFactory factory) : base(factory)
    {
    }
}

using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.Integration;

public class SmokeTests
{
    [Fact]
    public void Blog_tests_discoverable()
    {
        true.ShouldBeTrue();
    }
}

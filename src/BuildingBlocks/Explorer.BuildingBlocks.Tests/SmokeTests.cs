using Shouldly;
using Xunit;

namespace Explorer.BuildingBlocks.Tests;

public class SmokeTests
{
    [Fact]
    public void BuildingBlocks_tests_discoverable()
    {
        true.ShouldBeTrue();
    }
}

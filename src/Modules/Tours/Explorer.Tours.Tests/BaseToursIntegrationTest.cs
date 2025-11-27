using Explorer.BuildingBlocks.Tests;

namespace Explorer.Tours.Tests;

public class BaseToursIntegrationTest : BaseWebIntegrationTest<ToursTestFactory>
{
    public BaseToursIntegrationTest(ToursTestFactory factory) : base(factory) { }
}
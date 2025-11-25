using Explorer.BuildingBlocks.Tests;

namespace Explorer.Stakeholders.Tests;

public class BaseStakeholdersIntegrationTest : BaseWebIntegrationTest<StakeholdersTestFactory>
{
    protected HttpClient Client { get; }
    public BaseStakeholdersIntegrationTest(StakeholdersTestFactory factory): base(factory) { Client = factory.CreateClient(); }
}
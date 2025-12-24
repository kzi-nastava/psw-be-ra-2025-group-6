using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Explorer.Architecture.Tests;

public class ModulesTests : BaseArchitecturalTests
{
    [Theory]
    [MemberData(nameof(GetModules))]
    public void API_projects_should_only_reference_themselves_and_core_building_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Explorer.{moduleName}.API");
        var forbiddenTypes = GetForbiddenTypes("Explorer.BuildingBlocks.Core", $"Explorer.{moduleName}.API");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Core_projects_should_only_reference_themselves_API_projects_and_core_building_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Explorer.{moduleName}.Core");
        var forbiddenTypes = GetForbiddenTypes("Explorer.BuildingBlocks.Core", "Explorer\\..+\\.API", $"Explorer.{moduleName}.Core");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Infra_projects_should_only_reference_themselves_their_API_and_core_projects_and_building_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Explorer.{moduleName}.Infrastructure");
        var forbiddenTypes = GetForbiddenTypes("Explorer.BuildingBlocks.", $"Explorer.{moduleName}.");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes);

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Domain_namespaces_should_only_reference_themselves_and_core_building_blocks(string moduleName)
    {
        var allTypesFromCoreAssembly = GetExaminedTypes($"Explorer.{moduleName}.Core").ToList();
        var domainTypes = allTypesFromCoreAssembly.Where(x => x.FullName.Contains(".Domain.")).ToList();
        var nonDomainTypes = allTypesFromCoreAssembly.Where(x => !x.FullName.Contains(".Domain."));
        var typesFromOtherAssemblies = GetForbiddenTypes("Explorer.BuildingBlocks.Core", $"Explorer.{moduleName}.Core");

        var otherAssemblyRule = Types().That().Are(domainTypes).Should().NotDependOnAny(typesFromOtherAssemblies).WithoutRequiringPositiveResults();
        var sameAssemblyRule = Types().That().Are(domainTypes).Should().NotDependOnAny(nonDomainTypes).WithoutRequiringPositiveResults();

        otherAssemblyRule.Check(Architecture);
        sameAssemblyRule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Services_should_not_reference_public_APIs_of_other_modules(string moduleName)
    {
        var allTypesFromCoreAssembly = GetExaminedTypes($"Explorer.{moduleName}.Core").ToList();
        var useCaseTypes = allTypesFromCoreAssembly.Where(x => x.FullName.Contains(".UseCases.")).ToList();
        var typesFromOtherAssemblies = GetForbiddenTypes("Explorer.API", $"Explorer.{moduleName}.API");
        var publicApiTypesFromOtherAssemblies = typesFromOtherAssemblies.Where(x => x.FullName.Contains("API.Public"));

        var rule = Types().That().Are(useCaseTypes).Should().NotDependOnAny(publicApiTypesFromOtherAssemblies).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Web_API_should_not_reference_internal_APIs_of_modules()
    {
        var apiTypes = GetExaminedTypes("Explorer.API").ToList();
        var typesFromOtherAssemblies = GetForbiddenTypes("Explorer.API");
        var internalApiTypes = typesFromOtherAssemblies.Where(x => x.FullName.Contains("API.Internal"));

        var rule = Types().That().Are(apiTypes).Should().NotDependOnAny(internalApiTypes);

        rule.Check(Architecture);
    }

    public static IEnumerable<object[]> GetModules() => new List<object[]>
    {
        new object[]
        {
            "Stakeholders"
        },
        new object[]
        {
            "Blog"
        },
        new object[]
        {
            "Tours"
        },
        new object[]
        {
            "Encounters"
        }
    };
}
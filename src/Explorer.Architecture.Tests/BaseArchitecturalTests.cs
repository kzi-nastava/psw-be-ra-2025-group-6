using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using System.Text.RegularExpressions;
using Assembly = System.Reflection.Assembly;

namespace Explorer.Architecture.Tests;

public class BaseArchitecturalTests
{
    protected ArchUnitNET.Domain.Architecture Architecture;

    public BaseArchitecturalTests()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        foreach (var dll in Directory.GetFiles(path, "Explorer.*.dll"))
        {
            Assembly.LoadFile(dll);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        Architecture = new ArchLoader().LoadAssemblies(assemblies
            .Where(a => a.FullName.StartsWith("Explorer"))
            .Select(a => Assembly.Load(a.FullName))
            .ToArray()
        ).Build();
    }

    protected IEnumerable<IType> GetExaminedTypes(string assemblyName)
    {
        return Architecture.Assemblies
            .Where(a => Regex.IsMatch(a.FullName, assemblyName))
            .SelectMany(a => Architecture.Types.Where(c => c.Assembly.Equals(a)));
    }

    protected IEnumerable<IType> GetForbiddenTypes(params string[] exemptAssemblyNames)
    {
        return Architecture.Assemblies
            .Where(a => exemptAssemblyNames.All(n => !Regex.IsMatch(a.FullName, n)))
            .SelectMany(a => Architecture.Types.Where(c => c.Assembly.Equals(a)));
    }
}
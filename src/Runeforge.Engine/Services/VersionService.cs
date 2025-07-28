using System.Reflection;
using Runeforge.Engine.Data.Version;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Services;

public class VersionService : IVersionService
{
    public VersionService()
    {
        _ = GetVersionInfo();
    }

    public VersionInfoData GetVersionInfo()
    {
        var version = typeof(VersionService).Assembly.GetName().Version;

        var codename = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "Codename")
            ?.Value;

        return new VersionInfoData(
            "Runeforge",
            codename,
            version.ToString(),
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch,
            ThisAssembly.Git.CommitDate
        );
    }

    public void Dispose()
    {
    }
}

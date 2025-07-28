using Runeforge.Engine.Data.Version;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IVersionService : IRuneforgeService
{
    VersionInfoData GetVersionInfo();
}

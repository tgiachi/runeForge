using GoRogue.GameFramework;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapService : IRuneforgeStartableService
{
    Map CurrentMap { get; set; }


}

using Runeforge.Engine.Interfaces.Services.Base;
using SadRogue.Primitives;

namespace Runeforge.Engine.Interfaces.Services;

public interface IColorService : IRuneforgeService
{
    void AddColor(string colorSet, string colorName, Color color);

    Color GetColor(string colorName);

    void SetDefaultColorSet(string colorSet);
}

using Runeforge.Data.Colors;
using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Common;

public class JsonColorData : BaseJsonEntityData
{
    public Dictionary<string, ColorDef> Colors { get; set; }
}

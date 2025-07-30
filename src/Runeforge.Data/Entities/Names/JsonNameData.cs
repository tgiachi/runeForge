using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Names;

public class JsonNameData : BaseJsonEntityData
{
    public string Gender { get; set; }

    public string[] Names { get; set; }
}

using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Keysets;

public class JsonKeysetData : BaseJsonEntityData
{
    public string Context { get; set; }
    public string ActionName { get; set; }

    public string Key { get; set; }
}

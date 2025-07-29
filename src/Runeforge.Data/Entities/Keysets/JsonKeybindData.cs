using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Keysets;

public class JsonKeybindData : BaseJsonEntityData
{
    public string Context { get; set; }
    public string ActionName { get; set; }
    public string Key { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int Priority { get; set; } = 0;
}

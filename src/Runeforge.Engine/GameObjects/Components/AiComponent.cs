using Runeforge.Engine.Contexts;
using SadRogue.Integration.Components;

namespace Runeforge.Engine.GameObjects.Components;

public class AiComponent : RogueLikeComponentBase<NpcGameObject>
{
    public string BrainName { get; set; }

    public AiContext AiContext { get; set; }

    public AiComponent(string brainName) : base(false, false, false, false)
    {
        BrainName = brainName;
    }
}

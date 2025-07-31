using SadConsole;
using SadRogue.Integration;
using SadRogue.Integration.Components;

namespace Runeforge.Engine.GameObjects.Components;

public class TimedRemoveComponent : RogueLikeComponentBase<RogueLikeEntity>
{
    private TimeSpan _timeToLive;


    public TimedRemoveComponent(TimeSpan timeToLive) : base(true, false, false, false)
    {
        _timeToLive = timeToLive;
    }


    public override void Update(IScreenObject host, TimeSpan delta)
    {
        _timeToLive -= delta;

        if (_timeToLive <= TimeSpan.Zero)
        {
            Parent.CurrentMap.RemoveEntity(Parent);
        }

        base.Update(host, delta);
    }
}

using Runeforge.Engine.Instance;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.TickActions;
using SadRogue.Primitives;

namespace Runeforge.Engine.Actions;

public static class DefaultActions
{
    public static void RegisterDefaultActions(IActionService actionService, ITickSystemService tickSystemService)
    {
        actionService.AddAction("execute_tick", (parameter) => { tickSystemService.ExecuteTick(); });


        actionService.AddAction(
            "move_up",
            (object parameter) =>
            {
                var player = RuneforgeInstances.GetService<IPlayerService>().Player;

                tickSystemService.EnqueueAction(new MoveAction(player, Direction.Up));

                actionService.ExecuteAction("execute_tick", parameter);
            }
        );

        actionService.AddAction(
            "move_down",
            (object parameter) =>
            {
                var player = RuneforgeInstances.GetService<IPlayerService>().Player;

                tickSystemService.EnqueueAction(new MoveAction(player, Direction.Down));

                actionService.ExecuteAction("execute_tick", parameter);
            }
        );

        actionService.AddAction(
            "move_left",
            (object parameter) =>
            {
                var player = RuneforgeInstances.GetService<IPlayerService>().Player;

                tickSystemService.EnqueueAction(new MoveAction(player, Direction.Left));

                actionService.ExecuteAction("execute_tick", parameter);
            }
        );

        actionService.AddAction(
            "move_right",
            (object parameter) =>
            {
                var player = RuneforgeInstances.GetService<IPlayerService>().Player;

                tickSystemService.EnqueueAction(new MoveAction(player, Direction.Right));

                actionService.ExecuteAction("execute_tick", parameter);
            }
        );
    }
}

using Runeforge.Engine.Attributes.Scripts;
using GoRogue.DiceNotation;

namespace Runeforge.Engine.Modules;

[ScriptModule("random")]
public class RandomModule
{
    /// <summary>
    /// Generates a random number between the specified minimum and maximum values.
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <returns>A random integer between min and max.</returns>
    [ScriptFunction("Get random integer between min and max")]
    public int Int(int min, int max)
    {
        return Random.Shared.Next(min, max);
    }

    [ScriptFunction("Get random boolean value")]
    public bool Bool()
    {
        return Random.Shared.Next(0, 2) == 1;
    }

    [ScriptFunction("Roll a dice expression")]
    public int Roll(string dice)
    {
        return Dice.DiceParser.Parse(dice).Roll();
    }
}

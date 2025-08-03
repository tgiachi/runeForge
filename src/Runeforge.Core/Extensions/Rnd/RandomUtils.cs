namespace Runeforge.Core.Extensions.Rnd;

/// <summary>
/// Static utility class for random item selection from collections
/// </summary>
public static class RandomUtils
{
    private static readonly Random _random = new();

    /// <summary>
    /// Get a random item from an enumerable collection
    /// </summary>
    /// <typeparam name="T">Type of items in collection</typeparam>
    /// <param name="source">Source collection</param>
    /// <returns>Random item from collection or default if empty</returns>
    public static T? RandomItem<T>(this IEnumerable<T> source)
    {
        if (source == null)
        {
            return default;
        }

        var list = source as IList<T> ?? source.ToList();

        if (list.Count == 0)
        {
            return default;
        }

        var randomIndex = _random.Next(list.Count);
        return list[randomIndex];
    }

    /// <summary>
    /// Get multiple random items from an enumerable collection
    /// </summary>
    /// <typeparam name="T">Type of items in collection</typeparam>
    /// <param name="source">Source collection</param>
    /// <param name="count">Number of items to select</param>
    /// <param name="allowDuplicates">Allow duplicate items in result</param>
    /// <returns>List of random items from collection</returns>
    public static List<T> RandomItems<T>(this IEnumerable<T> source, int count, bool allowDuplicates = true)
    {
        if (source == null || count <= 0)
            return new List<T>();

        var list = source as IList<T> ?? source.ToList();

        if (list.Count == 0)
            return new List<T>();

        var result = new List<T>();

        if (!allowDuplicates && count >= list.Count)
        {
            // Return all items if requesting more than available without duplicates
            return list.ToList();
        }

        if (allowDuplicates)
        {
            // Simple random selection with duplicates allowed
            for (int i = 0; i < count; i++)
            {
                var randomIndex = _random.Next(list.Count);
                result.Add(list[randomIndex]);
            }
        }
        else
        {
            // Random selection without duplicates using Fisher-Yates shuffle approach
            var availableIndices = Enumerable.Range(0, list.Count).ToList();

            for (int i = 0; i < count && availableIndices.Count > 0; i++)
            {
                var randomIndex = _random.Next(availableIndices.Count);
                var selectedIndex = availableIndices[randomIndex];
                result.Add(list[selectedIndex]);
                availableIndices.RemoveAt(randomIndex);
            }
        }

        return result;
    }

    /// <summary>
    /// Get a random item with weighted selection
    /// </summary>
    /// <typeparam name="T">Type of items in collection</typeparam>
    /// <param name="source">Source collection with weights</param>
    /// <returns>Random weighted item or default if empty</returns>
    public static T? RandomWeightedItem<T>(this IEnumerable<(T item, float weight)> source)
    {
        if (source == null)
            return default;

        var list = source.ToList();

        if (list.Count == 0)
            return default;

        var totalWeight = list.Sum(x => x.weight);

        if (totalWeight <= 0)
            return default;

        var randomValue = (float)(_random.NextDouble() * totalWeight);
        var currentWeight = 0f;

        foreach (var (item, weight) in list)
        {
            currentWeight += weight;
            if (randomValue <= currentWeight)
            {
                return item;
            }
        }

        // Fallback to last item
        return list.Last().item;
    }
}

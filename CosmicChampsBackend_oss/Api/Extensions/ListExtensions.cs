namespace CosmicChamps.Api.Extensions;

public static class ListExtensions
{
    private static readonly Random _random = new();

    public static bool GetWeightedRandom<T> (this IList<T> collection, Func<T, int> getWeight, out T random)
    {
        random = default!;
        if (collection.Count == 0)
            return false;

        var weightedCollection = collection.Select (x => (value: x, weight: getWeight (x))).Where (x => x.weight > 0).ToArray ();
        var totalWeight = weightedCollection.Sum (x => x.weight);
        var randomValue = _random.Next (0, totalWeight);
        var cumulative = 0;

        foreach (var item in weightedCollection)
        {
            cumulative += item.weight;
            if (randomValue >= cumulative)
                continue;

            random = item.value;
            return true;
        }

        random = collection.Last ();
        return true;
    }
}
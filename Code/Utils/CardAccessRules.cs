namespace CraftTheSpire.Utils;

/// <summary>Pure rules for one starter replacement and relative card-selection weights.</summary>
public static class CardAccessRules
{
    public static List<T> ReplaceOneStarter<T>(IEnumerable<T> source, T replacement,
        Func<T, bool> isStrike, Func<T, bool> isBasicAttack)
    {
        var deck = source.ToList();
        var index = deck.FindIndex(card => isStrike(card));
        if (index < 0) index = deck.FindIndex(card => isBasicAttack(card));
        if (index >= 0) deck[index] = replacement;
        return deck;
    }

    public static int TotalWeight(IReadOnlyList<int> weights)
    {
        var total = 0;
        foreach (var weight in weights)
        {
            if (weight <= 0) throw new ArgumentOutOfRangeException(nameof(weights));
            total = checked(total + weight);
        }
        return total;
    }

    public static int IndexFromRoll(IReadOnlyList<int> weights, int roll)
    {
        if (roll < 0 || roll >= TotalWeight(weights))
            throw new ArgumentOutOfRangeException(nameof(roll));
        for (var i = 0; i < weights.Count; i++)
        {
            if (roll < weights[i]) return i;
            roll -= weights[i];
        }
        throw new InvalidOperationException("Weighted roll did not resolve a candidate.");
    }
}

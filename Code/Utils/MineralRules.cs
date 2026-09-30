namespace CraftTheSpire.Utils;

public enum MineralKind { Coal, IronIngot, GoldIngot, Diamond }

/// <summary>Frozen V1 mapping. No game dependencies, clocks, or local RNG.</summary>
public static class MineralRules
{
    public static MineralKind FromRoll(int roll) => roll switch
    {
        >= 0 and < 40 => MineralKind.Coal,
        >= 40 and < 75 => MineralKind.IronIngot,
        >= 75 and < 95 => MineralKind.GoldIngot,
        >= 95 and < 100 => MineralKind.Diamond,
        _ => throw new ArgumentOutOfRangeException(nameof(roll), "Expected an integer in [0,99].")
    };

    public static int LavaHpLoss(int maxHp, int currentHp)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxHp);
        ArgumentOutOfRangeException.ThrowIfNegative(currentHp);
        var desired = ((long)maxHp * 15 + 99) / 100;
        return (int)Math.Min(desired, Math.Max(0, currentHp - 1));
    }

    public static bool IsDiamondRoll(int roll) => roll switch
    {
        >= 0 and < 50 => true,
        >= 50 and < 100 => false,
        _ => throw new ArgumentOutOfRangeException(nameof(roll))
    };
}

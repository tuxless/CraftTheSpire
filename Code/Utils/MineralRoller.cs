using CraftTheSpire.Cards.Minerals;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Random;

namespace CraftTheSpire.Utils;

public static class MineralRoller
{
    public static MineralKind Roll(Rng sharedRng) => MineralRules.FromRoll(sharedRng.NextInt(100));

    public static CardModel Canonical(MineralKind kind) => kind switch
    {
        MineralKind.Coal => ModelDb.Card<Coal>(),
        MineralKind.IronIngot => ModelDb.Card<IronIngot>(),
        MineralKind.GoldIngot => ModelDb.Card<GoldIngot>(),
        MineralKind.Diamond => ModelDb.Card<Diamond>(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    public static async Task AddRandomToHand(Player owner, ICombatState? combat, int count)
    {
        // Match vanilla generated-card semantics after a fatal attack: no cards after combat ends.
        if (CombatManager.Instance.IsOverOrEnding) return;
        ArgumentNullException.ThrowIfNull(combat);
        var cards = new List<CardModel>(count);
        for (var i = 0; i < count; i++)
        {
            var mineral = Roll(owner.RunState.Rng.CombatCardGeneration);
            cards.Add(combat.CreateCard(Canonical(mineral), owner));
        }
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, owner);
    }

    public static async Task AddToHand(Player owner, ICombatState? combat, MineralKind kind)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;
        ArgumentNullException.ThrowIfNull(combat);
        await CardPileCmd.AddGeneratedCardToCombat(combat.CreateCard(Canonical(kind), owner), PileType.Hand, owner);
    }
}

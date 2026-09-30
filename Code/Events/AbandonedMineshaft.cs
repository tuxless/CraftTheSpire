using CraftTheSpire.Cards;
using CraftTheSpire.Relics;
using CraftTheSpire.Utils;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Runs;

namespace CraftTheSpire.Events;

// Shared here means shared across Acts/characters. Native player-choice actions own execution.
[RegisterSharedEvent]
public sealed class AbandonedMineshaft : ModEventTemplate
{
    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://CraftTheSpire/images/events/abandoned_mineshaft.png");

    public override bool IsAllowed(IRunState runState)
        => runState is RunState activeRun && !activeRun.VisitedEventIds.Contains(Id);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MaxHpCost", 5m),
        new DynamicVar("FallbackGold", 75m),
        new DynamicVar("LavaLoss", 0m)
    ];

    public override void CalculateVars()
    {
        DynamicVars["LavaLoss"].BaseValue = MineralRules.LavaHpLoss(Owner!.Creature.MaxHp, Owner.Creature.CurrentHp);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var deeperKey = InitialOptionKey("GO_DEEPER");
        var deeperDescription = L10NLookup(deeperKey + (AvailableMineRelics().Count == 0 ? ".goldDescription" : ".description"));
        deeperDescription.Add("MaxHpCost", 5m);
        deeperDescription.Add("FallbackGold", 75m);
        var deeper = new EventOption(this, GoDeeper, L10NLookup(deeperKey + ".title"), deeperDescription, deeperKey, []);
        if (AvailableMineRelics().Count > 0) deeper.ThatDecreasesMaxHp(5m);
        var hasDiamond = Owns(ModelDb.Relic<DiamondPickaxe>());
        var digKey = InitialOptionKey("DIG_STRAIGHT_DOWN");
        var digDescription = L10NLookup(digKey + (hasDiamond ? ".lockedDescription" : ".description"));
        digDescription.Add("LavaLoss", DynamicVars["LavaLoss"].BaseValue);
        var dig = new EventOption(this, hasDiamond ? null : DigStraightDown,
            L10NLookup(digKey + ".title"), digDescription, digKey, []);
        return
        [
            new EventOption(this, TakeWood, InitialOptionKey("TAKE_WOOD"), HoverTipFactory.FromCard<WoodenPickaxe>()),
            deeper,
            dig,
            new EventOption(this, Leave, InitialOptionKey("LEAVE"))
        ];
    }

    private bool Owns(RelicModel canonical) => Owner!.Relics.Any(r => r.Id == canonical.Id);

    private List<RelicModel> AvailableMineRelics()
    {
        // The stable list order is part of the deterministic RNG mapping.
        RelicModel[] candidates = [ModelDb.Relic<Furnace>(), ModelDb.Relic<CraftingTable>(), ModelDb.Relic<EnderChest>()];
        return candidates.Where(r => !Owns(r)).ToList();
    }

    private async Task TakeWood()
    {
        var owner = Owner ?? throw new InvalidOperationException("Abandoned Mineshaft has no active owner.");
        var card = owner.RunState.CreateCard(ModelDb.Card<WoodenPickaxe>(), owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        SetEventFinished(PageDescription("REWARD"));
    }

    private async Task GoDeeper()
    {
        var owner = Owner ?? throw new InvalidOperationException("Abandoned Mineshaft has no active owner.");
        var candidates = AvailableMineRelics();
        if (candidates.Count == 0)
        {
            await PlayerCmd.GainGold(75m, owner);
            SetEventFinished(PageDescription("GOLD"));
            return;
        }
        // Native event RNG is derived from the run seed and owner NetId on every peer.
        // A global run RNG inside concurrent player callbacks would depend on message arrival order.
        var relic = candidates[Rng.NextInt(candidates.Count)];
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), owner.Creature, 5m, false);
        if (!owner.Creature.IsDead) await RelicCmd.Obtain(relic.ToMutable(), owner);
        SetEventFinished(PageDescription("REWARD"));
    }

    private async Task DigStraightDown()
    {
        var owner = Owner ?? throw new InvalidOperationException("Abandoned Mineshaft has no active owner.");
        // The UI is locked when owned; the callback also guards against duplicate grants.
        if (Owns(ModelDb.Relic<DiamondPickaxe>())) return;
        if (MineralRules.IsDiamondRoll(Rng.NextInt(100)))
        {
            await RelicCmd.Obtain(ModelDb.Relic<DiamondPickaxe>().ToMutable(), owner);
            SetEventFinished(PageDescription("REWARD"));
        }
        else
        {
            var loss = MineralRules.LavaHpLoss(owner.Creature.MaxHp, owner.Creature.CurrentHp);
            // SetCurrentHp is a native command. It makes this exact nonlethal HP loss independent of Block/damage modifiers.
            if (loss > 0) await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - loss);
            SetEventFinished(PageDescription("LAVA"));
        }
    }

    private Task Leave()
    {
        SetEventFinished(PageDescription("LEAVE"));
        return Task.CompletedTask;
    }
}

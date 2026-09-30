using CraftTheSpire.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace CraftTheSpire.Relics;

[RegisterRelic(typeof(SharedRelicPool), StableEntryStem = "crafting_table")]
public sealed class CraftingTable : CraftRelicBase
{
    private int _mineralProgress;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    public override int DisplayAmount => MineralProgress;

    [SavedProperty]
    public int MineralProgress
    {
        get => _mineralProgress;
        set
        {
            AssertMutable();
            if (value is < 0 or > 2) throw new ArgumentOutOfRangeException(nameof(value));
            _mineralProgress = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task BeforeCombatStart()
    {
        MineralProgress = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not MineralCardBase) return;
        if (!CombatManager.Instance.IsInProgress) return;
        var completesSet = MineralProgress == 2;
        MineralProgress = (MineralProgress + 1) % 3;
        if (!completesSet) return;
        Flash();
        await CardPileCmd.Draw(context, 1m, Owner);
    }
}

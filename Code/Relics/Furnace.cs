using CraftTheSpire.Cards.Minerals;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace CraftTheSpire.Relics;

[RegisterRelic(typeof(SharedRelicPool), StableEntryStem = "furnace")]
public sealed class Furnace : CraftRelicBase
{
    private bool _usedThisCombat;
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override bool IncludeEnergyHoverTip => true;

    [SavedProperty]
    public bool UsedThisCombat
    {
        get => _usedThisCombat;
        set { AssertMutable(); _usedThisCombat = value; }
    }

    public override Task BeforeCombatStart()
    {
        UsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not Coal || UsedThisCombat) return;
        if (!CombatManager.Instance.IsInProgress) return;
        UsedThisCombat = true;
        Flash();
        await PlayerCmd.GainEnergy(1m, Owner);
    }
}

using CraftTheSpire.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace CraftTheSpire.Relics;

[RegisterRelic(typeof(SharedRelicPool), StableEntryStem = "diamond_pickaxe")]
public sealed class DiamondPickaxe : CraftRelicBase
{
    private bool _openingMineralAdded;
    public override RelicRarity Rarity => RelicRarity.Rare;

    [SavedProperty]
    public bool OpeningMineralAdded
    {
        get => _openingMineralAdded;
        set { AssertMutable(); _openingMineralAdded = value; }
    }

    public override Task BeforeCombatStart()
    {
        OpeningMineralAdded = false;
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1 || OpeningMineralAdded) return;
        OpeningMineralAdded = true;
        Flash();
        await MineralRoller.AddToHand(Owner, combatState, MineralKind.Diamond);
    }
}

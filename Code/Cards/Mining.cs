using CraftTheSpire.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "mining")]
public sealed class Mining() : CraftCardBase(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        => MineralRoller.AddRandomToHand(Owner, CombatState, 2);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

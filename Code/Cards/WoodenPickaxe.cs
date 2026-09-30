using CraftTheSpire.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "wooden_pickaxe")]
public sealed class WoodenPickaxe() : CraftCardBase(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        await MineralRoller.AddRandomToHand(Owner, CombatState, 1);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

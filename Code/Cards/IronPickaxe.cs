using CraftTheSpire.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "iron_pickaxe")]
public sealed class IronPickaxe() : CraftCardBase(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        await MineralRoller.AddToHand(Owner, CombatState, MineralKind.IronIngot);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "tnt")]
public sealed class Tnt() : CraftCardBase(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move), new HpLossVar(4m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combat = CombatState;
        ArgumentNullException.ThrowIfNull(combat);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(combat).Execute(choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}

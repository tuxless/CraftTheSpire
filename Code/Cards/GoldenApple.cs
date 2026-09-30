using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace CraftTheSpire.Cards;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "golden_apple")]
public sealed class GoldenApple() : CraftCardBase(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

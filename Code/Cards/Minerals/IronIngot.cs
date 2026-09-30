using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards.Minerals;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "iron_ingot")]
public sealed class IronIngot : MineralCardBase
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
}

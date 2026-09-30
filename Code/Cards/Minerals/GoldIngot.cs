using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards.Minerals;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "gold_ingot")]
public sealed class GoldIngot : MineralCardBase
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, 1m, Owner);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}

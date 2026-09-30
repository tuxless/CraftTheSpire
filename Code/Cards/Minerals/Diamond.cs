using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards.Minerals;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "diamond")]
public sealed class Diamond : MineralCardBase
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(1m, Owner);
        await CardPileCmd.Draw(choiceContext, 1m, Owner);
    }
}

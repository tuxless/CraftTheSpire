using MegaCrit.Sts2.Core.Models.CardPools;

namespace CraftTheSpire.Cards.Minerals;

[RegisterCard(typeof(ColorlessCardPool), StableEntryStem = "coal")]
public sealed class Coal : MineralCardBase
{
    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => PlayerCmd.GainEnergy(1m, Owner);
}

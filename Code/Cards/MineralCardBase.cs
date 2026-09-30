namespace CraftTheSpire.Cards;

public abstract class MineralCardBase() : CraftCardBase(0, CardType.Skill, CardRarity.Event, TargetType.Self, false)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;
}

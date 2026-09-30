namespace CraftTheSpire.Cards;

public abstract class CraftCardBase(int cost, CardType type, CardRarity rarity, TargetType target, bool showInLibrary = true)
    : ModCardTemplate(cost, type, rarity, target, showInLibrary)
{
    public override CardAssetProfile AssetProfile => new(PortraitPath: $"res://CraftTheSpire/images/cards/{GetType().Name}.png");
}

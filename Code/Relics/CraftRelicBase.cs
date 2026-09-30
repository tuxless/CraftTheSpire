namespace CraftTheSpire.Relics;

public abstract class CraftRelicBase : ModRelicTemplate
{
    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://CraftTheSpire/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://CraftTheSpire/images/relics/{GetType().Name}_outline.png",
        BigIconPath: $"res://CraftTheSpire/images/relics/{GetType().Name}_big.png");
}

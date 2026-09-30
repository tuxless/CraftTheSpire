using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using CraftTheSpire.Utils;

namespace CraftTheSpire;

[ModInitializer(nameof(Init))]
public static class Entry
{
    public const string ModId = "CraftTheSpire";
    public static Logger Logger { get; private set; } = null!;

    public static void Init()
    {
        Logger = RitsuLibFramework.CreateLogger(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        LibraryDiscovery.Initialize();
        CardAccessHooks.Initialize();
        Logger.Info($"Craft the Spire {assembly.GetName().Version}: assembly registered; expected 9 cards, 4 relics, 1 event. Target Beta 0.111.0 / RitsuLib 0.6.2. In-game validation required.");
    }
}

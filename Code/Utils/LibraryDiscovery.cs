using CraftTheSpire.Cards;
using Godot;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves;
using STS2RitsuLib;
using STS2RitsuLib.Utils.Persistence;

namespace CraftTheSpire.Utils;

/// <summary>Reveal the five collectible cards after models and the active profile are ready.</summary>
public static class LibraryDiscovery
{
    private static readonly List<IDisposable> Subscriptions = [];
    private static bool _initialized;
    private static bool _modelIdsReady;
    private static bool _mainMenuReady;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        Subscriptions.Add(RitsuLibFramework.SubscribeLifecycle<ModelIdsInitializedEvent>(_ =>
        {
            _modelIdsReady = true;
            RevealFormalCards();
        }));
        Subscriptions.Add(RitsuLibFramework.SubscribeLifecycle<ProfileDataReadyEvent>(_ =>
        {
            // Profile changes can happen inside native save initialization. Run after that stack completes.
            if (_mainMenuReady) Callable.From(RevealFormalCards).CallDeferred();
        }));
        Subscriptions.Add(RitsuLibFramework.SubscribeLifecycle<MainMenuReadyEvent>(_ =>
        {
            _mainMenuReady = true;
            RevealFormalCards();
        }));
    }

    private static void RevealFormalCards()
    {
        if (!_modelIdsReady || !_mainMenuReady) return;
        var saves = SaveManager.Instance;
        if (!saves.IsProfileInitialized) return;

        CardModel[] cards =
        [
            ModelDb.Card<WoodenPickaxe>(),
            ModelDb.Card<IronPickaxe>(),
            ModelDb.Card<Mining>(),
            ModelDb.Card<Tnt>(),
            ModelDb.Card<GoldenApple>()
        ];
        var progress = saves.Progress;
        var newlyRevealed = 0;
        foreach (var card in cards)
            if (progress.MarkCardAsSeen(card.Id)) newlyRevealed++;

        // Native discovery records are idempotent. Save once only when this profile gained new entries.
        if (newlyRevealed > 0) saves.SaveProgressFile();

        var poolIds = ModelDb.CardPool<ColorlessCardPool>().AllCardIds.ToHashSet();
        var inPool = cards.Count(card => poolIds.Contains(card.Id));
        Entry.Logger.Info($"Library initialization: formal cards={cards.Length}; colorless pool={inPool}/5; newly revealed={newlyRevealed}; profile={saves.CurrentProfileId}.");
        foreach (var card in cards)
            Entry.Logger.Info($"Library card {card.Id}: rarity={card.Rarity}, show={card.ShouldShowInCardLibrary}, discovered={progress.DiscoveredCards.Contains(card.Id)}.");
        if (inPool != cards.Length)
            Entry.Logger.Warn("CraftTheSpire formal-card pool membership is incomplete. Check RitsuLib registration diagnostics.");
    }
}

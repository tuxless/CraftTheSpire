using System.Reflection;
using System.Reflection.Emit;
using CraftTheSpire.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Builders;
using STS2RitsuLib.Patching.Core;

namespace CraftTheSpire.Utils;

/// <summary>Ritsu-managed native integration for the requested starting deck and acquisition changes.</summary>
public static class CardAccessHooks
{
    private static IDisposable? _subscription;
    private static ModPatcher? _patcher;
    private static bool _attempted;
    private static readonly MethodInfo WeightedSelection = typeof(CardAccessHooks)
        .GetMethod(nameof(PickWeightedCard), BindingFlags.Static | BindingFlags.NonPublic)!;

    public static void Initialize()
    {
        _subscription ??= RitsuLibFramework.SubscribeLifecycle<ModelIdsInitializedEvent>(_ =>
        {
            if (_attempted) return;
            _attempted = true;
            try { ApplyHooks(); }
            catch (Exception error)
            {
                Entry.Logger.Warn($"CraftTheSpire card-access integration failed: {error}");
            }
        });
    }

    private static void ApplyHooks()
    {
        var builder = new DynamicPatchBuilder("CraftTheSpire.card_access");
        var starterPatch = DynamicPatchBuilder.FromMethod(typeof(CardAccessHooks), nameof(StartingDeckPostfix));
        var selectionPatch = DynamicPatchBuilder.FromMethod(typeof(CardAccessHooks), nameof(CardSelectionTranspiler));
        var starterGetters = new HashSet<MethodInfo>();
        foreach (var character in ModelDb.AllCharacters)
        {
            var getter = character.GetType().GetProperty(nameof(CharacterModel.StartingDeck),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetMethod;
            if (getter is null || getter.IsAbstract)
                throw new MissingMethodException(character.GetType().FullName, "get_StartingDeck");
            if (starterGetters.Add(getter))
                builder.Add(getter, postfix: starterPatch, description: "Replace one starting Strike with Wooden Pickaxe.");
        }
        if (starterGetters.Count == 0) throw new InvalidOperationException("No character starting decks found.");

        // Replace only the final native CardModel draw. Native rarity, color, eligibility and blacklist filters remain upstream.
        (string Name, Type[] Parameters)[] selectionTargets =
        [
            (nameof(CardFactory.CreateForMerchant), [typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardType)]),
            (nameof(CardFactory.CreateForMerchant), [typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardRarity)]),
            (nameof(CardFactory.CreateForReward), [typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardCreationOptions)]),
            (nameof(CardFactory.GetForCombat), [typeof(Player), typeof(IEnumerable<CardModel>), typeof(int), typeof(Rng)]),
            (nameof(CardFactory.CreateRandomCardForTransform), [typeof(CardModel), typeof(bool), typeof(Rng)]),
            (nameof(CardFactory.CreateRandomCardForTransform), [typeof(CardModel), typeof(IEnumerable<CardModel>), typeof(bool), typeof(Rng)])
        ];
        foreach (var (name, parameters) in selectionTargets)
        {
            var method = typeof(CardFactory).GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null, types: parameters, modifiers: null)
                ?? throw new MissingMethodException(typeof(CardFactory).FullName, name);
            builder.Add(method, transpiler: selectionPatch, description: "Double relative weight of Iron Pickaxe, Mining, TNT and Golden Apple.");
        }

        _patcher = RitsuLibFramework.CreatePatcher(Entry.ModId, "card_access", "Craft the Spire card access");
        var success = _patcher.ApplyDynamic(builder, rollbackOnCriticalFailure: true);
        Entry.Logger.Info($"Card-access patches: starting getters={starterGetters.Count}; selection methods={selectionTargets.Length}; registered={_patcher.RegisteredDynamicPatchCount}; applied={_patcher.AppliedPatchCount}/{builder.Patches.Count}; success={success}.");
        if (!success || _patcher.AppliedPatchCount != builder.Patches.Count)
            throw new InvalidOperationException("Card-access patches were not all applied. Check RitsuLib dynamic patch diagnostics.");
    }

    private static void StartingDeckPostfix(ref IEnumerable<CardModel> __result)
    {
        // Return canonical models before native Player.PopulateStartingDeck creates the owned instances.
        __result = CardAccessRules.ReplaceOneStarter(__result, ModelDb.Card<WoodenPickaxe>(),
            card => card.Tags.Contains(CardTag.Strike),
            card => card.Rarity == CardRarity.Basic && card.Type == CardType.Attack);
    }

    private static IEnumerable<CodeInstruction> CardSelectionTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase __originalMethod)
    {
        var result = instructions.ToList();
        var replacements = 0;
        foreach (var instruction in result)
        {
            if ((instruction.opcode != OpCodes.Call && instruction.opcode != OpCodes.Callvirt)
                || instruction.operand is not MethodInfo method
                || method.DeclaringType != typeof(Rng) || method.Name != nameof(Rng.NextItem)
                || !method.IsGenericMethod || method.GetGenericArguments().Length != 1
                || method.GetGenericArguments()[0] != typeof(CardModel)) continue;
            // Instance Rng.NextItem<CardModel>(choices) and static PickWeightedCard(rng, choices) have the same stack inputs.
            // Mutate the instruction so its branch labels and exception blocks are retained.
            instruction.opcode = OpCodes.Call;
            instruction.operand = WeightedSelection;
            replacements++;
        }
        if (replacements != 1)
            throw new InvalidOperationException($"Expected one card draw in {__originalMethod}, found {replacements}.");
        Entry.Logger.Info($"Card weight integration: {__originalMethod}; native card draws replaced={replacements}.");
        return result;
    }

    private static CardModel PickWeightedCard(Rng rng, IEnumerable<CardModel> choices)
    {
        var cards = choices.ToArray();
        if (!cards.Any(IsBoosted)) return rng.NextItem(cards)!;
        var weights = cards.Select(card => IsBoosted(card) ? 2 : 1).ToArray();
        var roll = rng.NextInt(CardAccessRules.TotalWeight(weights));
        return cards[CardAccessRules.IndexFromRoll(weights, roll)];
    }

    private static bool IsBoosted(CardModel card) => card is IronPickaxe or Mining or Tnt or GoldenApple;
}

using CraftTheSpire.Utils;

var checks = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    checks++;
}
var weights = Enum.GetValues<MineralKind>().ToDictionary(k => k, _ => 0);
for (var roll = 0; roll < 100; roll++) weights[MineralRules.FromRoll(roll)]++;
Check(weights[MineralKind.Coal] == 40 && weights[MineralKind.IronIngot] == 35 && weights[MineralKind.GoldIngot] == 20 && weights[MineralKind.Diamond] == 5, "Frozen weight mapping changed.");
Check(Enumerable.Range(0, 100).Count(MineralRules.IsDiamondRoll) == 50, "Event diamond chance must be 50 percent.");
foreach (var roll in new[] {-1, 100})
{
    try { MineralRules.FromRoll(roll); throw new Exception("Invalid roll accepted."); }
    catch (ArgumentOutOfRangeException) { checks++; }
}
for (var max = 1; max <= 200; max++)
for (var hp = 1; hp <= max; hp++)
{
    var loss = MineralRules.LavaHpLoss(max, hp);
    Check(loss == Math.Min((int)Math.Ceiling(max * .15m), hp - 1), $"Rounding mismatch: {max}/{hp}");
    Check(hp - loss >= 1, "Lava must never kill.");
}
Check(MineralRules.LavaHpLoss(int.MaxValue, int.MaxValue) == 322122548, "Large HP must not overflow.");

// Exhaust every possible roll; boosted candidates must own exactly twice as many outcomes.
foreach (var candidateWeights in new[] { new[] { 1 }, new[] { 2 }, new[] { 2, 1, 2, 1 }, new[] { 1, 1, 1, 1 }, new[] { 2, 2, 2, 2, 1 } })
{
    var outcomes = new int[candidateWeights.Length];
    for (var roll = 0; roll < CardAccessRules.TotalWeight(candidateWeights); roll++)
        outcomes[CardAccessRules.IndexFromRoll(candidateWeights, roll)]++;
    Check(outcomes.SequenceEqual(candidateWeights), "Weighted selection does not match the relative weights.");
    foreach (var invalid in new[] { -1, candidateWeights.Sum() })
    {
        try { CardAccessRules.IndexFromRoll(candidateWeights, invalid); throw new Exception("Invalid weighted roll accepted."); }
        catch (ArgumentOutOfRangeException) { checks++; }
    }
}
foreach (var invalidWeights in new[] { Array.Empty<int>(), new[] { 0 }, new[] { -1, 2 } })
{
    try { CardAccessRules.IndexFromRoll(invalidWeights, 0); throw new Exception("Invalid candidate weights accepted."); }
    catch (ArgumentOutOfRangeException) { checks++; }
}

// Different character layouts, a custom character without Strike tags, and an attack-free deck.
foreach (var original in new[]
{
    new[] { "Strike", "Strike", "Defend", "Unique" },
    new[] { "Defend", "Unique", "Strike", "Strike" },
    new[] { "Unique", "Defend", "Strike", "Defend", "Strike" },
    new[] { "BasicAttack", "Defend", "Strike", "Unique" },
    new[] { "Unique", "Strike", "Defend" },
    new[] { "Unique", "BasicAttack", "BasicAttack", "Defend" },
    new[] { "Unique", "Defend" }
})
{
    var snapshot = original.ToArray();
    var replaced = CardAccessRules.ReplaceOneStarter(original, "WoodenPickaxe", card => card == "Strike", card => card == "BasicAttack");
    var expectedIndex = Array.IndexOf(original, "Strike");
    if (expectedIndex < 0) expectedIndex = Array.IndexOf(original, "BasicAttack");
    Check(replaced.Count == original.Length, "Starting deck size changed.");
    Check(original.SequenceEqual(snapshot), "Canonical starting deck was mutated.");
    Check(replaced.Count(card => card == "WoodenPickaxe") == (expectedIndex >= 0 ? 1 : 0), "Wrong number of starting pickaxes.");
    for (var index = 0; index < original.Length; index++)
        Check(replaced[index] == (index == expectedIndex ? "WoodenPickaxe" : original[index]), "A different starting card changed.");
}
Console.WriteLine($"PASS: {checks} deterministic rule checks. Game/API/multiplayer tests are separate.");

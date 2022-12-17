using Content.Shared.Storage;
using Robust.Shared.Random;

namespace Content.Server._00Citadel.Worldgen.Tools;

/// <summary>
/// A faster version of EntitySpawnCollection that requires caching to work.
/// </summary>
public sealed class EntitySpawnCollectionCache
{
    private sealed class OrGroup
    {
        public List<EntitySpawnEntry> Entries { get; set; } = new();
        public float CumulativeProbability { get; set; } = 0f;
    }

    private Dictionary<string, OrGroup> _orGroups = new();

    public EntitySpawnCollectionCache(IEnumerable<EntitySpawnEntry> entries)
    {
        // collect groups together, create singular items that pass probability
        foreach (var entry in entries)
        {
            if (!_orGroups.TryGetValue(entry.GroupId ?? string.Empty, out var orGroup))
            {
                orGroup = new();
                _orGroups.Add(entry.GroupId ?? string.Empty, orGroup);
            }

            orGroup.Entries.Add(entry);
            orGroup.CumulativeProbability += entry.SpawnProbability;
            continue;
        }
    }

    /// <summary>
    ///     Using a collection of entity spawn entries, picks a random list of entity prototypes to spawn from that collection.
    /// </summary>
    /// <remarks>
    ///     This does not spawn the entities. The caller is responsible for doing so, since it may want to do something
    ///     special to those entities (offset them, insert them into storage, etc)
    /// </remarks>
    /// <param name="random">Resolve param.</param>
    /// <param name="spawned">List that spawned entities are inserted into.</param>
    /// <returns>A list of entity prototypes that should be spawned.</returns>
    /// <remarks>This is primarily useful if you're calling it many times over, as it lets you reuse the list repeatedly.</remarks>
    public void GetSpawns(IRobustRandom random, ref List<string?> spawned)
    {
        // handle orgroup spawns
        foreach (var spawnValue in _orGroups.Values)
        {
            // For each group use the added cumulative probability to roll a double in that range
            double diceRoll = random.NextDouble() * spawnValue.CumulativeProbability;
            // Add the entry's spawn probability to this value, if equals or lower, spawn item, otherwise continue to next item.
            var cumulative = 0.0;
            foreach (var entry in spawnValue.Entries)
            {
                cumulative += entry.SpawnProbability;
                if (diceRoll > cumulative)
                    continue;
                // Dice roll succeeded, add item and break loop

                var amount = entry.Amount;

                if (entry.MaxAmount > amount)
                    amount = random.Next(amount, entry.MaxAmount);

                for (var index = 0; index < amount; index++)
                {
                    spawned.Add(entry.PrototypeId);
                }

                break;
            }
        }
    }
}

using Robust.Shared.Serialization;

namespace Content.Server._Citadel.Overmaps;

[Serializable, NetSerializable]
public sealed class OvermapSignal
{
    /// <summary>
    /// strength. by default, this is meters away a 1x sensor can pick it up at.
    /// </summary>
    public float strength = 50.0f;

    /// <summary>
    /// encryption string. a receiver must recognize this to get the string tags contained within.
    /// </summary>
    public string? encryption = null;

    /// <summary>
    /// string tags.
    ///
    /// backend wise, this can be anything (must be plaintext, will not have special rendering).
    /// players (and mappers) should generally be forced to set all caps with dashes and spaces though, for fluff
    /// reasons.
    ///
    /// TODO: i don't know how chat message system works but is there a way to have this
    /// use richer text for adminbus reasons?
    ///
    /// </summary>
    public List<string> tags = new List<string>();
}

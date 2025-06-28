using Robust.Shared.Serialization;

namespace Content.Shared._Citadel.Overmaps
{
    /// <summary>
    /// Overmap IDs. This is a string to support persistence later, as unlike MapIds, we expect
    /// overmap references to remain usable throughout rounds if it's actually persistent.
    ///
    /// This also means that there's no 'invalid' type, the field just becomes nullable on referencing
    /// entities.
    /// </summary>
    [Serializable, NetSerializable]
    public readonly struct OvermapId : IEquatable<OvermapId>
    {
        internal readonly string Value;

        public OvermapId(string value)
        {
            this.Value = value;
        }

        public bool Equals(OvermapId other) { return Value.Equals(other.Value); }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is OvermapId id && Equals(id);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(OvermapId a, OvermapId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(OvermapId a, OvermapId b)
        {
            return !a.Equals(b);
        }
    }
}

using System.Diagnostics.Contracts;

namespace Content.Server._00OuterRim.Worldgen2;

public static class WorldGen
{
    /// <summary>
    /// Be very, very careful about the value of this.
    /// </summary>
    public const int ChunkSize = 128;

    [Pure]
    public static Vector2i WorldToChunkCoords(Vector2i inp)
    {
        return inp / ChunkSize;
    }

    [Pure]
    public static Vector2 WorldToChunkCoords(Vector2 inp)
    {
        return inp / ChunkSize;
    }

    [Pure]
    public static Vector2 ChunkToWorldCoords(Vector2i inp)
    {
        return inp * ChunkSize;
    }

    [Pure]
    public static Vector2 ChunkToWorldCoordsCentered(Vector2i inp)
    {
        return inp * ChunkSize + Vector2i.One * (ChunkSize / 2);
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Content.Server._00OuterRim.Worldgen2;

public struct GridPointsNearEnumerator
{
    private int _radius;
    private Vector2i _center;
    private int _x;
    private int _y;

    public GridPointsNearEnumerator(Vector2i center, int radius)
    {
        _radius = radius;
        _center = center;
        _x = -_radius;
        _y = -_radius;
    }

    [Pure]
    public bool MoveNext([NotNullWhen(true)] out Vector2i? chunk)
    {
        while (!(_x * _x + _y * _y <= _radius * _radius))
        {
            if (_y > _radius)
            {
                chunk = null;
                return false;
            }
            if (_x > _radius)
            {
                _x = -_radius;
                _y++;
            }
            else
            {
                _x++;
            }
        }

        chunk = _center + new Vector2i(_x, _y);
        _x++;
        return true;
    }

    /*
    public IEnumerable<Vector2i> ChunksNear(EntityUid ent)
    {
        var offs = Transform(ent).WorldPosition.Floored() / ChunkSize;
        const int division = (WorldLoadRadius / ChunkSize) + 1;
        for (var x = -division; x <= division; x+=1)
        {
            for (var y = -division; y <= division; y+=1)
            {
                if (x * x + y * y <= division * division)
                {
                    yield return offs + (x, y);
                }
            }
        }
    }
    */
}

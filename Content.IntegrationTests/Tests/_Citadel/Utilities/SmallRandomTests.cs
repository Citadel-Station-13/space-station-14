using Content.Shared._Citadel.Utilities;
using Robust.Shared.Utility;

namespace Content.IntegrationTests.Tests._Citadel.Utilities;

[TestFixture]
public sealed class SmallRandomTests
{
    [Test]
    public void IsReproducable()
    {
        Assert.That(SmallRandom.TryFromStringAsSeed("awawa", out var myRandom));

        var stringified = myRandom.ToString();

        Assert.That(SmallRandom.TryFromStringAsSerialized(stringified, out var andBackAgain));

        var andBackAgainV = andBackAgain!.Value;

        Assert.That(myRandom!.Value.DebugCheckByteEquality(ref andBackAgainV));
    }

    // Ensure we're not just returning a consistent value.
    [Test]
    public void ReasonablyRandom()
    {
        Assert.That(SmallRandom.TryFromStringAsSeed("awawa", out var myRandomNullable));
        var myRandom = myRandomNullable!.Value;

        Assert.That(myRandom.Next() != myRandom.Next());
        Assert.That(myRandom.Next() != myRandom.Next());
        Assert.That(myRandom.Next() != myRandom.Next());
    }
}

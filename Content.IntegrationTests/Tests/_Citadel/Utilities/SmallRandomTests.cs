using System.IO;
using System.Linq;
using Content.Shared._Citadel.Utilities;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using YamlDotNet.RepresentationModel;

namespace Content.IntegrationTests.Tests._Citadel.Utilities;

[TestFixture]
public sealed class SmallRandomTests : CitadelGameTest
{
    [SidedDependency(Side.Server)]
    private readonly ISerializationManager _ser = default!;

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
        Assert.That(SmallRandom.TryFromStringAsSeed("gay!", out var myRandomNullable));
        var myRandom = myRandomNullable!.Value;

        Assert.That(myRandom.Next() != myRandom.Next());
        Assert.That(myRandom.Next() != myRandom.Next());
        Assert.That(myRandom.Next() != myRandom.Next());
    }

    [Test]
    public void Serialize()
    {
        Assert.That(SmallRandom.TryFromStringAsSeed("colon-three", out var myRandomNullable));
        var myRandom = myRandomNullable!.Value;

        var node = (MappingDataNode)_ser.WriteValue(new SmallRandomTestSer(myRandom));
        var document = new YamlStream {new(node.ToYaml())};
        var writer = new StringWriter();
        document.Save(writer);

        var reader = new StringReader(writer.ToString());

        var readDocument = DataNodeParser.ParseYamlStream(reader).First();

        var mapping = (MappingDataNode) readDocument.Root;

        var parsedMyRandom = _ser.Read<SmallRandomTestSer>(mapping).MyRandom;

        Assert.That(myRandom.DebugCheckByteEquality(ref parsedMyRandom));
    }
}


[DataDefinition]
public sealed partial class SmallRandomTestSer
{
    [DataField]
    public SmallRandom MyRandom;

    public SmallRandomTestSer(SmallRandom myRandom)
    {
        MyRandom = myRandom;
    }
}

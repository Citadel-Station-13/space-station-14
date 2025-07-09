namespace Content.IntegrationTests.Tests._Citadel.Worldgen;

[TestFixture]
public sealed class ChunkTests
{
    private const string TestChunkId = "TESTS_CitadelWorldgenChunksTestChunk";

    [TestPrototypes]
    public const string Prototypes = $"""
        - type: entity
          id: {TestChunkId}
          components:
            - type: CitadelWorldChunk
        """;

}

#nullable enable
using Content.Server._Citadel.Contracts;
using Content.Shared._Citadel.Contracts.Components;
using Content.Shared._Citadel.Contracts.Systems;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests._Citadel.Contracts;

[TestFixture]
public sealed class ContractsTest
{
    private const string TestContractId = "TESTS_CitadelTestContract";
    private const string TestSignerId = "ToyAmongPequeno"; //Suspicious contractors.

    [TestPrototypes]
    public const string Prototypes = $"""
        - type: entity
          id: {TestContractId}
          components:
            - type: CitadelContract
        """;

    public sealed class ContractsTestData : GameTestData
    {
        [System(Side.Server)]
        public readonly ContractSystem SharedContractSys = default!;
    }

    [GameTest<ContractsTestData>]
    public async Task Transitions(ContractsTestData data)
    {
        await data.Server.WaitAssertion(() =>
        {
            var contract = data.SEntity<CitadelContractComponent>(data.SSpawn(TestContractId));

            // Nobody has signed on, shouldn't be able to sign it.
            Assert.Multiple(() =>
            {
                Assert.That(data.SharedContractSys.TrySignContract(contract) is TENoPartyA or TENoPartyB);
                Assert.That(contract.Comp.State is ContractStateUnsigned);
            });

            // Should sign on fine.
            Assert.Multiple(() =>
            {
                Assert.That(data.SharedContractSys.TrySignOn(contract, data.SSpawn(TestSignerId), Party.PartyA));
                Assert.That(data.SharedContractSys.TrySignOn(contract, data.SSpawn(TestSignerId), Party.PartyB));
            });

            // And contract should be signable now.
            Assert.Multiple(() =>
            {
                Assert.That(data.SharedContractSys.TrySignContract(contract), Is.Null);
                Assert.That(contract.Comp.State is ContractStateSigned);
            });

            // Now we breach it, and blame party B. Poor party B, they're going to owe an amogillion dollars.
            Assert.Multiple(() =>
            {
                Assert.That(data.SharedContractSys.TryBreachContract(contract, Party.PartyB), Is.Null);
                Assert.That(contract.Comp.State is ContractStateBreached { BreachingParty: Party.PartyB });
            });
        });
    }
}

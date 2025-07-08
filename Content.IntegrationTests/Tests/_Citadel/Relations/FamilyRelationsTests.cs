// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using Content.Shared._Citadel.Relations;
using Content.Shared._Citadel.Relations.Testing;

namespace Content.IntegrationTests.Tests._Citadel.Relations;

[TestFixture]
public sealed class FamilyRelationsTests
{
    private const string TestFamilyMemberId = "TESTS_CitadelRelationsTestChild";
    [TestPrototypes]
    public const string Prototypes = $"""
        - type: entity
          id: {TestFamilyMemberId}
          components:
            - type: TestRelation
        """;
    public sealed class FamilyRelationData : GameTestData
    {
        [System(Side.Server)] public TestRelationSystem Relation = default!;
    }

    [GameTest<FamilyRelationData>(RunOnSide = Side.Server)]
    public void CreateFamilyRelations(FamilyRelationData data)
    {
        var grandchild = data.SSpawn(TestFamilyMemberId);
        var child = data.SSpawn(TestFamilyMemberId);
        var parent = data.SSpawn(TestFamilyMemberId);

        data.Relation.MakeRelated(child, parent);
        data.Relation.MakeRelated(grandchild, child);

        Assert.Multiple(() =>
        {
            Assert.That(data.Relation.GetParent(child), Is.EqualTo(parent));
            Assert.That(data.Relation.GetParent(grandchild), Is.EqualTo(child));
        });
    }

    [GameTest<FamilyRelationData>(RunOnSide = Side.Server)]
    public void DeletionHandled(FamilyRelationData data)
    {
        {
            var child = data.SSpawn(TestFamilyMemberId);
            var parent = data.SSpawn(TestFamilyMemberId);

            data.Relation.MakeRelated(child, parent);

            var childComp = data.SComp<TestRelationComponent>(child);

            data.SDeleteNow(parent);

            Assert.That(childComp.Parent, Is.EqualTo(null));

            data.SDeleteNow(child);
        }

        {
            var child = data.SSpawn(TestFamilyMemberId);
            var parent = data.SSpawn(TestFamilyMemberId);

            data.Relation.MakeRelated(child, parent);

            var parentComp = data.SComp<TestRelationComponent>(parent);

            data.SDeleteNow(child);

            Assert.That(parentComp.Children, Does.Not.Contain(child));

            data.SDeleteNow(parent);
        }
    }
}

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

#nullable enable
using System.Reflection;
using Content.IntegrationTests.Pair;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests._Citadel;

/// <summary>
///     A base class for data automatically injected by a game test.
///     Can also be used in lieu of a parent class if you don't need much.
/// </summary>
[Virtual]
public class GameTestData
{
    private bool _pairDirty = false;

    public TestPair Pair { get; private set; } = default!; // NULLABILITY: This is always set during test setup.
    public RobustIntegrationTest.ServerIntegrationInstance Server => Pair.Server;
    public RobustIntegrationTest.ClientIntegrationInstance Client => Pair.Client;
    public ICommonSession? Player => Pair.Player;

    public IEntityManager SEntMan => Server.EntMan;

    public void MarkDirty()
    {
        _pairDirty = true;
    }

    public async Task DoSetup()
    {
        _pairDirty = false;
        Pair = await PoolManager.GetServerClient(new PoolSettings {Connected = true});

        foreach (var field in GetType().GetAllFields())
        {
            if (field.GetCustomAttribute<SystemAttribute>() is {} sysAttrib)
            {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (sysAttrib.Side is Side.Server)
                {
                    field.SetValue(this, Server.EntMan.EntitySysManager.GetEntitySystem(field.FieldType));
                }
                else
                {
                    field.SetValue(this, Client.EntMan.EntitySysManager.GetEntitySystem(field.FieldType));
                }
            }
            else if (field.GetCustomAttribute<SidedDependencyAttribute>() is { } depAttrib)
            {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (depAttrib.Side is Side.Server)
                {
                    field.SetValue(this, Server.InstanceDependencyCollection.ResolveType(field.FieldType));
                }
                else
                {
                    field.SetValue(this, Client.InstanceDependencyCollection.ResolveType(field.FieldType));
                }
            }
        }
    }

    public async Task DoTeardown()
    {
        if (!_pairDirty)
            await Pair.CleanReturnAsync();
        else
            await Pair.DisposeAsync();
    }

    public EntityUid ToClientUid(EntityUid serverUid)
    {
        return Pair.ToClientUid(serverUid);
    }

    public EntityUid ToServerUid(EntityUid clientUid)
    {
        return Pair.ToServerUid(clientUid);
    }

    public T GetSysServer<T>()
        where T : EntitySystem
    {
        return Server.EntMan.System<T>();
    }

    public T SComp<T>(EntityUid target)
        where T : IComponent
    {
        return SEntMan.GetComponent<T>(target);
    }

    public Entity<T> SEntity<T>(EntityUid target)
        where T : IComponent
    {
        return new(target, SEntMan.GetComponent<T>(target));
    }

    public EntityUid Spawn(string id)
    {
        return SEntMan.Spawn(id);
    }
}

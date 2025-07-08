// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

#nullable enable
using System.Collections.Generic;
using System.Reflection;
using Content.IntegrationTests.Pair;
using JetBrains.Annotations;
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
[PublicAPI]
public class GameTestData
{
    private bool _pairDirty;

    private List<EntityUid> _serverEntitiesToClean = new();
    private List<EntityUid> _clientEntitiesToClean = new();

    /// <summary>
    ///     Settings for the client/server pair. By default, this gets you a client and server that have connected together.
    /// </summary>
    public virtual PoolSettings PoolSettings => new() { Connected = true };

    /// <summary>
    ///     The client and server pair.
    /// </summary>
    public TestPair Pair { get; private set; } = default!; // NULLABILITY: This is always set during test setup.
    /// <summary>
    ///     The game server instance.
    /// </summary>
    public RobustIntegrationTest.ServerIntegrationInstance Server => Pair.Server;
    /// <summary>
    ///     The game client instance.
    /// </summary>
    public RobustIntegrationTest.ClientIntegrationInstance Client => Pair.Client;

    /// <summary>
    ///     The test player, if any.
    /// </summary>
    public ICommonSession? Player => Pair.Player;

    /// <summary>
    ///     The server-side entity manager.
    /// </summary>
    public IEntityManager SEntMan => Server.EntMan;
    /// <summary>
    ///     The client-side entity manager.
    /// </summary>
    public IEntityManager CEntMan => Server.EntMan;

    /// <summary>
    ///     Marks the test pair as dirty, ensuring it is returned as such.
    /// </summary>
    public void MarkDirty()
    {
        _pairDirty = true;
    }

    /// <summary>
    ///     Internal function to do initial setup. Don't use this..
    /// </summary>
    public async Task DoSetup()
    {
        _pairDirty = false;
        Pair = await PoolManager.GetServerClient(PoolSettings);

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

    /// <summary>
    ///     Internal function to do post-test teardown. Don't use this..
    /// </summary>
    public async Task DoTeardown()
    {
        try
        {
            await Server.WaitAssertion(() =>
            {
                foreach (var junk in _serverEntitiesToClean)
                {
                    if (!SEntMan.Deleted(junk))
                        SEntMan.DeleteEntity(junk);
                }
            });


            await Client.WaitAssertion(() =>
            {
                foreach (var junk in _clientEntitiesToClean)
                {
                    if (!CEntMan.Deleted(junk))
                        CEntMan.DeleteEntity(junk);
                }
            });
        }
        catch (Exception e)
        {
            _pairDirty = true;
            throw;
        }
        finally
        {
            if (!_pairDirty)
                await Pair.CleanReturnAsync();
            else
                await Pair.DisposeAsync();
        }

    }

    /// <summary>
    ///     Converts a server EntityUid into the client-side equivalent entity.
    /// </summary>
    public EntityUid ToClientUid(EntityUid serverUid)
    {
        return Pair.ToClientUid(serverUid);
    }

    /// <summary>
    ///     Converts a client EntityUid into the server-side equivalent entity.
    /// </summary>
    public EntityUid ToServerUid(EntityUid clientUid)
    {
        return Pair.ToServerUid(clientUid);
    }

    /// <summary>
    ///     Retrieves the given entitysystem from the server.
    /// </summary>
    public T GetSysServer<T>()
        where T : EntitySystem
    {
        return Server.EntMan.System<T>();
    }

    /// <summary>
    ///     Retrieves the given entitysystem from the client.
    /// </summary>
    public T GetSysClient<T>()
        where T : EntitySystem
    {
        return Client.EntMan.System<T>();
    }

    /// <summary>
    ///     Retrieves the given component from an entity, from the server.
    /// </summary>
    public T SComp<T>(EntityUid target)
        where T : IComponent
    {
        return SEntMan.GetComponent<T>(target);
    }

    /// <summary>
    ///     Retrieves the given component from an entity, from the client.
    /// </summary>
    public T CComp<T>(EntityUid target)
        where T : IComponent
    {
        return CEntMan.GetComponent<T>(target);
    }

    /// <summary>
    ///     Pairs an EntityUid with the given component, from the server.
    /// </summary>
    public Entity<T> SEntity<T>(EntityUid target)
        where T : IComponent
    {
        return new(target, SEntMan.GetComponent<T>(target));
    }

    /// <summary>
    ///     Pairs an EntityUid with the given component, from the client.
    /// </summary>
    public Entity<T> CEntity<T>(EntityUid target)
        where T : IComponent
    {
        return new(target, CEntMan.GetComponent<T>(target));
    }

    /// <summary>
    ///     Spawns an entity on the server.
    /// </summary>
    /// <remarks>This tracks the entity for post-test cleanup.</remarks>
    public EntityUid SSpawn(string? id)
    {
        var res = SEntMan.Spawn(id);
        _serverEntitiesToClean.Add(res);
        return res;
    }
    /// <summary>
    ///     Spawns an entity on the client.
    /// </summary>
    /// <remarks>This tracks the entity for post-test cleanup.</remarks>
    public EntityUid CSpawn(string? id)
    {
        var res = CEntMan.Spawn(id);
        _clientEntitiesToClean.Add(res);
        return res;
    }

    /// <summary>
    ///     Deletes an entity on the server immediately.
    /// </summary>
    public void SDeleteNow(EntityUid id)
    {
        SEntMan.DeleteEntity(id);
    }

    /// <summary>
    ///     Deletes an entity on the client immediately.
    /// </summary>
    public void CDeleteNow(EntityUid id)
    {
        CEntMan.DeleteEntity(id);
    }

}

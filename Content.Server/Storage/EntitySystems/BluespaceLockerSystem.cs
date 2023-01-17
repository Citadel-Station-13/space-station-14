using System.Linq;
using Content.Server.Lock;
using Content.Server.Mind.Components;
using Content.Server.Resist;
using Content.Server.Station.Components;
using Content.Server.Storage.Components;
using Content.Server.Tools.Systems;
using Content.Shared.Coordinates;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Storage.EntitySystems;

public sealed class BluespaceLockerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityStorageSystem _entityStorage = default!;
    [Dependency] private readonly WeldableSystem _weldableSystem = default!;
    [Dependency] private readonly LockSystem _lockSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BluespaceLockerComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<BluespaceLockerComponent, StorageBeforeOpenEvent>(PreOpen);
        SubscribeLocalEvent<BluespaceLockerComponent, StorageAfterCloseEvent>(PostClose);
    }

    private void OnStartup(EntityUid uid, BluespaceLockerComponent component, ComponentStartup args)
    {
<<<<<<< HEAD
        GetTargetStorage(component);
=======
        GetTarget(uid, component);

        if (component.BehaviorProperties.BluespaceEffectOnInit)
            BluespaceEffect(uid, component, component, true);
    }

    public void BluespaceEffect(EntityUid effectTargetUid, BluespaceLockerComponent effectSourceComponent, BluespaceLockerComponent? effectTargetComponent, bool bypassLimit = false)
    {
        if (!bypassLimit && Resolve(effectTargetUid, ref effectTargetComponent, false))
            if (effectTargetComponent!.BehaviorProperties.BluespaceEffectMinInterval > 0)
            {
                var curTimeTicks = _timing.CurTick.Value;
                if (curTimeTicks < effectTargetComponent.BluespaceEffectNextTime)
                    return;

                effectTargetComponent.BluespaceEffectNextTime = curTimeTicks + (uint) (_timing.TickRate * effectTargetComponent.BehaviorProperties.BluespaceEffectMinInterval);
            }

        Spawn(effectSourceComponent.BehaviorProperties.BluespaceEffectPrototype, effectTargetUid.ToCoordinates());
>>>>>>> 445622983 (Bluespace lockers update (#13469))
    }

    private void PreOpen(EntityUid uid, BluespaceLockerComponent component, StorageBeforeOpenEvent args)
    {
        EntityStorageComponent? entityStorageComponent = null;
        int transportedEntities = 0;

        if (!Resolve(uid, ref entityStorageComponent))
            return;

<<<<<<< HEAD
=======
        if (component.CancelToken != null)
        {
            component.CancelToken.Cancel();
            component.CancelToken = null;
            return;
        }

        if (!component.BehaviorProperties.ActOnOpen)
            return;

>>>>>>> 445622983 (Bluespace lockers update (#13469))
        // Select target
        var targetContainerStorageComponent = GetTargetStorage(component);
        if (targetContainerStorageComponent == null)
            return;
        BluespaceLockerComponent? targetContainerBluespaceComponent = null;

        // Close target if it is open
        if (targetContainerStorageComponent.Open)
            _entityStorage.CloseStorage(targetContainerStorageComponent.Owner, targetContainerStorageComponent);

        // Apply bluespace effects if target is not a bluespace locker, otherwise let it handle it
        if (!Resolve(targetContainerStorageComponent.Owner, ref targetContainerBluespaceComponent, false))
        {
            // Move contained items
            if (component.TransportEntities)
                foreach (var entity in targetContainerStorageComponent.Contents.ContainedEntities.ToArray())
                {
<<<<<<< HEAD
                    if (!component.AllowSentient && EntityManager.HasComponent<MindComponent>(entity))
                        continue;
                    entityStorageComponent.Contents.Insert(entity, EntityManager);
=======
                    if (EntityManager.HasComponent<MindComponent>(entity))
                    {
                        if (!component.BehaviorProperties.TransportSentient)
                            continue;

                        entityStorageComponent.Contents.Insert(entity, EntityManager);
                        transportedEntities++;
                    }
                    else if (component.BehaviorProperties.TransportEntities)
                    {
                        entityStorageComponent.Contents.Insert(entity, EntityManager);
                        transportedEntities++;
                    }
>>>>>>> 445622983 (Bluespace lockers update (#13469))
                }

            // Move contained air
            if (component.TransportGas)
            {
                entityStorageComponent.Air.CopyFromMutable(targetContainerStorageComponent.Air);
                targetContainerStorageComponent.Air.Clear();
            }
<<<<<<< HEAD
        }
    }

    private bool ValidLink(BluespaceLockerComponent component, EntityStorageComponent link)
    {
        return link.Owner.Valid && link.LifeStage != ComponentLifeStage.Deleted;
=======

            // Bluespace effects
            if (component.BehaviorProperties.BluespaceEffectOnTeleportSource)
                BluespaceEffect(target.Value.uid, component, target.Value.bluespaceLockerComponent);
            if (component.BehaviorProperties.BluespaceEffectOnTeleportTarget)
                BluespaceEffect(uid, component, component);
        }

        DestroyAfterLimit(uid, component, transportedEntities);
    }

    private bool ValidLink(EntityUid locker, EntityUid link, BluespaceLockerComponent lockerComponent, bool intendToLink = false)
    {
        if (!link.Valid ||
            !TryComp<EntityStorageComponent>(link, out var linkStorage) ||
            linkStorage.LifeStage == ComponentLifeStage.Deleted ||
            link == locker)
            return false;

        if (lockerComponent.BehaviorProperties.InvalidateOneWayLinks &&
            !(intendToLink && lockerComponent.AutoLinksBidirectional) &&
            !(HasComp<BluespaceLockerComponent>(link) && Comp<BluespaceLockerComponent>(link).BluespaceLinks.Contains(locker)))
            return false;

        return true;
>>>>>>> 445622983 (Bluespace lockers update (#13469))
    }

    private bool ValidAutolink(BluespaceLockerComponent component, EntityStorageComponent link)
    {
<<<<<<< HEAD
        if (!ValidLink(component, link))
            return false;

        if (component.PickLinksFromSameMap &&
            link.Owner.ToCoordinates().GetMapId(_entityManager) == component.Owner.ToCoordinates().GetMapId(_entityManager))
            return false;

        if (component.PickLinksFromStationGrids &&
            !_entityManager.HasComponent<StationMemberComponent>(link.Owner.ToCoordinates().GetGridUid(_entityManager)))
=======
        if ((a == null || a.Count == 0) && (b == null || b.Count == 0))
            return true;
        if (a != null && a.Any(aSet => aSet.Count == 0))
            return true;
        if (b != null && b.Any(bSet => bSet.Count == 0))
            return true;

        if (a != null && b != null)
            return a.Any(aSet => b.Any(aSet.SetEquals));
        return false;
    }

    private bool ValidAutolink(EntityUid locker, EntityUid link, BluespaceLockerComponent lockerComponent)
    {
        if (!ValidLink(locker, link, lockerComponent, true))
            return false;

        if (lockerComponent.PickLinksFromSameMap &&
            link.ToCoordinates().GetMapId(EntityManager) != locker.ToCoordinates().GetMapId(EntityManager))
            return false;

        if (lockerComponent.PickLinksFromStationGrids &&
            !HasComp<StationMemberComponent>(link.ToCoordinates().GetGridUid(EntityManager)))
>>>>>>> 445622983 (Bluespace lockers update (#13469))
            return false;

        if (component.PickLinksFromResistLockers &&
            !_entityManager.HasComponent<ResistLockerComponent>(link.Owner))
            return false;

        return true;
    }

<<<<<<< HEAD
    private EntityStorageComponent? GetTargetStorage(BluespaceLockerComponent component)
=======
    public (EntityUid uid, EntityStorageComponent storageComponent, BluespaceLockerComponent? bluespaceLockerComponent)? GetTarget(EntityUid lockerUid, BluespaceLockerComponent component)
>>>>>>> 445622983 (Bluespace lockers update (#13469))
    {
        while (true)
        {
            // Ensure MinBluespaceLinks
            if (component.BluespaceLinks.Count < component.MinBluespaceLinks)
            {
                // Get an shuffle the list of all EntityStorages
                var storages = _entityManager.EntityQuery<EntityStorageComponent>().ToArray();
                _robustRandom.Shuffle(storages);

                // Add valid candidates till MinBluespaceLinks is met
                foreach (var storage in storages)
                {
                    if (!ValidAutolink(component, storage))
                        continue;

                    component.BluespaceLinks.Add(storage);
                    if (component.AutoLinksBidirectional)
                    {
<<<<<<< HEAD
                        _entityManager.EnsureComponent<BluespaceLockerComponent>(storage.Owner, out var targetBluespaceComponent);
                        targetBluespaceComponent.BluespaceLinks.Add(_entityManager.GetComponent<EntityStorageComponent>(component.Owner));
=======
                        var targetBluespaceComponent = CompOrNull<BluespaceLockerComponent>(potentialLink);

                        if (targetBluespaceComponent == null)
                        {
                            using var compInitializeHandle =
                                EntityManager.AddComponentUninitialized<BluespaceLockerComponent>(potentialLink);
                            targetBluespaceComponent = compInitializeHandle.Comp;

                            if (component.AutoLinksBidirectional)
                                targetBluespaceComponent.BluespaceLinks.Add(lockerUid);

                            if (component.AutoLinksUseProperties)
                                targetBluespaceComponent.BehaviorProperties = component.AutoLinkProperties with {};

                            compInitializeHandle.Dispose();
                        }
                        else if (component.AutoLinksBidirectional)
                        {
                            targetBluespaceComponent.BluespaceLinks.Add(lockerUid);
                        }
>>>>>>> 445622983 (Bluespace lockers update (#13469))
                    }
                    if (component.BluespaceLinks.Count >= component.MinBluespaceLinks)
                        break;
                }
            }

            // If there are no possible link targets and no links, return null
            if (component.BluespaceLinks.Count == 0)
            {
                if (component.MinBluespaceLinks == 0)
                    RemComp<BluespaceLockerComponent>(lockerUid);

                return null;
            }

            // Attempt to select, validate, and return a link
            var links = component.BluespaceLinks.ToArray();
            var link = links[_robustRandom.Next(0, component.BluespaceLinks.Count)];
            if (ValidLink(component, link))
                return link;
            component.BluespaceLinks.Remove(link);
        }
    }


    private void PostClose(EntityUid uid, BluespaceLockerComponent component, StorageAfterCloseEvent args)
    {
        EntityStorageComponent? entityStorageComponent = null;
        int transportedEntities = 0;

        if (!Resolve(uid, ref entityStorageComponent))
            return;

<<<<<<< HEAD
=======
        component.CancelToken?.Cancel();

        if (!component.BehaviorProperties.ActOnClose)
            return;

        // Do delay
        if (doDelay && component.BehaviorProperties.Delay > 0)
        {
            EnsureComp<DoAfterComponent>(uid);
            component.CancelToken = new CancellationTokenSource();

            _doAfterSystem.DoAfter(
                new DoAfterEventArgs(uid, component.BehaviorProperties.Delay, component.CancelToken.Token)
                {
                    UserFinishedEvent = new BluespaceLockerTeleportDelayComplete()
                });
            return;
        }

>>>>>>> 445622983 (Bluespace lockers update (#13469))
        // Select target
        var targetContainerStorageComponent = GetTargetStorage(component);
        if (targetContainerStorageComponent == null)
            return;

        // Move contained items
        if (component.TransportEntities)
            foreach (var entity in entityStorageComponent.Contents.ContainedEntities.ToArray())
            {
<<<<<<< HEAD
                if (!component.AllowSentient && EntityManager.HasComponent<MindComponent>(entity))
                    continue;
                targetContainerStorageComponent.Contents.Insert(entity, EntityManager);
=======
                if (EntityManager.HasComponent<MindComponent>(entity))
                {
                    if (!component.BehaviorProperties.TransportSentient)
                        continue;

                    target.Value.storageComponent.Contents.Insert(entity, EntityManager);
                    transportedEntities++;
                }
                else if (component.BehaviorProperties.TransportEntities)
                {
                    target.Value.storageComponent.Contents.Insert(entity, EntityManager);
                    transportedEntities++;
                }
>>>>>>> 445622983 (Bluespace lockers update (#13469))
            }

        // Move contained air
        if (component.TransportGas)
        {
            targetContainerStorageComponent.Air.CopyFromMutable(entityStorageComponent.Air);
            entityStorageComponent.Air.Clear();
        }

        // Open and empty target
        if (targetContainerStorageComponent.Open)
        {
            _entityStorage.EmptyContents(targetContainerStorageComponent.Owner, targetContainerStorageComponent);
            _entityStorage.ReleaseGas(targetContainerStorageComponent.Owner, targetContainerStorageComponent);
        }
        else
        {
            if (targetContainerStorageComponent.IsWeldedShut)
            {
                // It gets bluespaced open...
                _weldableSystem.ForceWeldedState(targetContainerStorageComponent.Owner, false);
                if (targetContainerStorageComponent.IsWeldedShut)
                    targetContainerStorageComponent.IsWeldedShut = false;
            }
            LockComponent? lockComponent = null;
            if (Resolve(targetContainerStorageComponent.Owner, ref lockComponent, false) && lockComponent.Locked)
                _lockSystem.Unlock(lockComponent.Owner, lockComponent.Owner, lockComponent);

            _entityStorage.OpenStorage(targetContainerStorageComponent.Owner, targetContainerStorageComponent);
        }
<<<<<<< HEAD
=======

        // Bluespace effects
        if (component.BehaviorProperties.BluespaceEffectOnTeleportSource)
            BluespaceEffect(uid, component, component);
        if (component.BehaviorProperties.BluespaceEffectOnTeleportTarget)
            BluespaceEffect(target.Value.uid, component, target.Value.bluespaceLockerComponent);

        DestroyAfterLimit(uid, component, transportedEntities);
    }

    private void DestroyAfterLimit(EntityUid uid, BluespaceLockerComponent component, int transportedEntities)
    {
        if (component.BehaviorProperties.DestroyAfterUsesMinItemsToCountUse > transportedEntities)
            return;

        if (component.BehaviorProperties.ClearLinksEvery != -1)
        {
            component.UsesSinceLinkClear++;
            if (component.BehaviorProperties.ClearLinksEvery <= component.UsesSinceLinkClear)
            {
                if (component.BehaviorProperties.ClearLinksDebluespaces)
                    foreach (var link in component.BluespaceLinks)
                        RemComp<BluespaceLockerComponent>(link);

                component.BluespaceLinks.Clear();
                component.UsesSinceLinkClear = 0;
            }
        }

        if (component.BehaviorProperties.DestroyAfterUses == -1)
            return;

        component.BehaviorProperties.DestroyAfterUses--;
        if (component.BehaviorProperties.DestroyAfterUses > 0)
            return;

        switch (component.BehaviorProperties.DestroyType)
        {
            case BluespaceLockerDestroyType.Explode:
                _explosionSystem.QueueExplosion(uid.ToCoordinates().ToMap(EntityManager),
                    ExplosionSystem.DefaultExplosionPrototypeId, 4, 1, 2, maxTileBreak: 0);
                goto case BluespaceLockerDestroyType.Delete;
            case BluespaceLockerDestroyType.Delete:
                QueueDel(uid);
                break;
            default:
            case BluespaceLockerDestroyType.DeleteComponent:
                RemComp<BluespaceLockerComponent>(uid);
                break;
        }
    }

    private sealed class BluespaceLockerTeleportDelayComplete : EntityEventArgs
    {
>>>>>>> 445622983 (Bluespace lockers update (#13469))
    }
}

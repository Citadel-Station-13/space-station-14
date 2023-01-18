using System.Linq;
using Content.Server.Lock;
using Content.Server.Mind.Components;
using Content.Server.Resist;
using Content.Server.Station.Components;
using Content.Server.Storage.Components;
using Content.Server.Tools.Systems;
using Content.Shared.Coordinates;
using Robust.Shared.Random;

namespace Content.Server.Storage.EntitySystems;

public sealed class BluespaceLockerSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
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
        GetTarget(uid, component, true);

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
>>>>>>> 5f2bccd1b (Bluespace lockers fix (#13575))
    }

    private void PreOpen(EntityUid uid, BluespaceLockerComponent component, StorageBeforeOpenEvent args)
    {
        EntityStorageComponent? entityStorageComponent = null;

        if (!Resolve(uid, ref entityStorageComponent))
            return;

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
                    if (!component.AllowSentient && EntityManager.HasComponent<MindComponent>(entity))
                        continue;
                    entityStorageComponent.Contents.Insert(entity, EntityManager);
                }

            // Move contained air
            if (component.TransportGas)
            {
                entityStorageComponent.Air.CopyFromMutable(targetContainerStorageComponent.Air);
                targetContainerStorageComponent.Air.Clear();
            }
        }
    }

    private bool ValidLink(BluespaceLockerComponent component, EntityStorageComponent link)
    {
        return link.Owner.Valid && link.LifeStage != ComponentLifeStage.Deleted;
    }

    private bool ValidAutolink(BluespaceLockerComponent component, EntityStorageComponent link)
    {
        if (!ValidLink(component, link))
            return false;

        if (component.PickLinksFromSameMap &&
            link.Owner.ToCoordinates().GetMapId(_entityManager) == component.Owner.ToCoordinates().GetMapId(_entityManager))
            return false;

        if (component.PickLinksFromStationGrids &&
            !_entityManager.HasComponent<StationMemberComponent>(link.Owner.ToCoordinates().GetGridUid(_entityManager)))
            return false;

        if (component.PickLinksFromResistLockers &&
            !_entityManager.HasComponent<ResistLockerComponent>(link.Owner))
            return false;

        return true;
    }

<<<<<<< HEAD
    private EntityStorageComponent? GetTargetStorage(BluespaceLockerComponent component)
=======
    /// <returns>True if any HashSet in <paramref name="a"/> would grant access to <paramref name="b"/></returns>
    private bool AccessMatch(IReadOnlyCollection<HashSet<string>>? a, IReadOnlyCollection<HashSet<string>>? b)
    {
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
            return false;

        if (lockerComponent.PickLinksFromResistLockers &&
            !HasComp<ResistLockerComponent>(link))
            return false;

        if (lockerComponent.PickLinksFromSameAccess)
        {
            TryComp<AccessReaderComponent>(locker, out var sourceAccess);
            TryComp<AccessReaderComponent>(link, out var targetAccess);
            if (!AccessMatch(sourceAccess?.AccessLists, targetAccess?.AccessLists))
                return false;
        }

        if (HasComp<BluespaceLockerComponent>(link))
        {
            if (lockerComponent.PickLinksFromNonBluespaceLockers)
                return false;
        }
        else
        {
            if (lockerComponent.PickLinksFromBluespaceLockers)
                return false;
        }

        return true;
    }

    public (EntityUid uid, EntityStorageComponent storageComponent, BluespaceLockerComponent? bluespaceLockerComponent)? GetTarget(EntityUid lockerUid, BluespaceLockerComponent component, bool init = false)
>>>>>>> 5f2bccd1b (Bluespace lockers fix (#13575))
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
                            targetBluespaceComponent = AddComp<BluespaceLockerComponent>(potentialLink);

                            if (component.AutoLinksBidirectional)
                                targetBluespaceComponent.BluespaceLinks.Add(lockerUid);

                            if (component.AutoLinksUseProperties)
                                targetBluespaceComponent.BehaviorProperties = component.AutoLinkProperties with {};

                            GetTarget(potentialLink, targetBluespaceComponent, true);
                            BluespaceEffect(potentialLink, targetBluespaceComponent, targetBluespaceComponent, true);
                        }
                        else if (component.AutoLinksBidirectional)
                        {
                            targetBluespaceComponent.BluespaceLinks.Add(lockerUid);
                        }
>>>>>>> 5f2bccd1b (Bluespace lockers fix (#13575))
                    }
                    if (component.BluespaceLinks.Count >= component.MinBluespaceLinks)
                        break;
                }
            }

            // If there are no possible link targets and no links, return null
            if (component.BluespaceLinks.Count == 0)
<<<<<<< HEAD
=======
            {
                if (component.MinBluespaceLinks == 0 && init)
                    RemComp<BluespaceLockerComponent>(lockerUid);

>>>>>>> 5f2bccd1b (Bluespace lockers fix (#13575))
                return null;

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

        if (!Resolve(uid, ref entityStorageComponent))
            return;

        // Select target
        var targetContainerStorageComponent = GetTargetStorage(component);
        if (targetContainerStorageComponent == null)
            return;

        // Move contained items
        if (component.TransportEntities)
            foreach (var entity in entityStorageComponent.Contents.ContainedEntities.ToArray())
            {
                if (!component.AllowSentient && EntityManager.HasComponent<MindComponent>(entity))
                    continue;
                targetContainerStorageComponent.Contents.Insert(entity, EntityManager);
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
    }
}

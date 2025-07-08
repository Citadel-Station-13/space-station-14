// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System.Collections;

namespace Content.Shared._Citadel.Relations;

public abstract partial class FamilyEntitySystem<TChild, TParent>
{
    public struct ParentsEnumerable(EntityQuery<TChild> childQuery, EntityUid start) : IEnumerable<EntityUid>
    {
        public EntityQuery<TChild> ChildQuery = childQuery;
        public EntityUid Start = start;

        public struct ParentsEnumerator : IEnumerator<EntityUid>
        {
            public EntityUid CurrentPointer;
            public EntityUid Start;
            public EntityQuery<TChild> ChildQuery;

            public bool MoveNext()
            {
                if (!ChildQuery.TryComp(CurrentPointer, out var parent))
                    return false;

                if (parent.Parent is not { } p)
                    return false;

                CurrentPointer = p;
                return true;
            }

            public void Reset()
            {
                CurrentPointer = Start;
            }

            EntityUid IEnumerator<EntityUid>.Current => CurrentPointer;

            object? IEnumerator.Current => CurrentPointer;

            public void Dispose()
            {
                // nothin
            }

            public ParentsEnumerator(EntityUid start, EntityQuery<TChild> childQuery)
            {
                Start = start;
                CurrentPointer = Start;
                ChildQuery = childQuery;
            }
        }

        public ParentsEnumerator GetEnumerator()
        {
            return new ParentsEnumerator(Start, ChildQuery);
        }

        IEnumerator<EntityUid> IEnumerable<EntityUid>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

// This Source Code Form is subject to the terms of the Mozilla Public
//     License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared._Citadel.Utilities;

[TypeSerializer]
public sealed class SmallRandomSerializer : ITypeSerializer<SmallRandom, ValueDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        return SmallRandom.TryFromStringAsSerialized(node.Value, out _) ? new ValidatedValueNode(node) : new ErrorNode(node, $"Invalid serialized SmallRandom. Failed to parse {node.Value}");
    }

    public SmallRandom Read(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<SmallRandom>? instanceProvider = null)
    {
        SmallRandom.TryFromStringAsSerialized(node.Value, out var rng);
        return rng!.Value;
    }

    public DataNode Write(ISerializationManager serializationManager,
        SmallRandom value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false,
        ISerializationContext? context = null)
    {
        return new ValueDataNode(value.ToString());
    }
}

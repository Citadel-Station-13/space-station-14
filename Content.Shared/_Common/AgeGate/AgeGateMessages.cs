// SPDX-FileCopyrightText: Copyright (c) 2025 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Common.AgeGate;

/// <summary>
/// Sent by the server when the client should show the Age Gate.
/// </summary>
public sealed class ShowAgeGateMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
    }
}

/// <summary>
/// Sent by the client when it clicks the Age Gate's submit button.
/// </summary>
public sealed class AgeGateSubmittedMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public bool IsAboveRequiredAge { get; set; }

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        IsAboveRequiredAge = buffer.ReadBoolean();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(IsAboveRequiredAge);
    }
}

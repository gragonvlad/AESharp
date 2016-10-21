﻿using AESharp.Core.Interfaces;

namespace AESharp.Router.Routing.Packets
{
    internal sealed class InitiateHandshakePacket : IPacket
    {
        public ushort ProtocolVersion { get; private set; }
    }
}
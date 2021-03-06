﻿using AESharp.Interop.Extensions;
using AESharp.Networking.Data.Packets;

namespace AESharp.Interop.Protocol
{
    public abstract class RoutingPacket : Packet
    {
        public RoutingPacketId PacketId { get; }

        public RoutingPacket(RoutingPacketId packetId)
        {
            PacketId = packetId;
            this.WritePacketId(packetId);
        }

        public RoutingPacket(byte[] data)
            : base(data)
        {
            PacketId = this.ReadPacketId();
        }
    }
}
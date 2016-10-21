﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AESharp.Core.Interfaces;

namespace AESharp.Networking.Packets
{
    public struct PacketHandlerResult
    {
        public bool DisconnectClient { get; }
        public IPacket ResponsePacket { get; }

        public PacketHandlerResult( bool disconnect, IPacket response )
        {
            this.DisconnectClient = disconnect;
            this.ResponsePacket = response;
        }
    }
}
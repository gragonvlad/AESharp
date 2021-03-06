﻿//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Threading.Tasks;
//using AESharp.Core.Extensions;
//using AESharp.Interop.Protocol;
//using AESharp.Networking.Data;
//using AESharp.Networking.Exceptions;

//namespace AESharp.Interop
//{
//    public sealed class RoutingRemoteClient : RemoteClient
//    {
//        public const ushort RoutingPort = 10695;
//        public const ushort ProtocolVersion = 1;

//        private readonly bool OutboundMode;
//        private Guid LastKeepAliveGuid;
//        private DateTime LastKeepAliveTime;

//        public RoutingRemoteClient( TcpClient rawClient, CancellationTokenSource tokenSource )
//            : this( rawClient, tokenSource, false )
//        {
//        }

//        private RoutingRemoteClient( TcpClient rawClient, CancellationTokenSource tokenSource, bool outboundMode )
//            : base( rawClient, tokenSource )
//        {
//            this.LastKeepAliveTime = DateTime.MinValue;
//            this.LastKeepAliveGuid = Guid.Empty;
//            this.OutboundMode = outboundMode;
//            if ( !this.OutboundMode )
//            {
//                this.KeepAliveLoopAsync();
//            }
//        }

//        public static async Task<RoutingRemoteClient> ConnectToMasterRouter()
//        {
//            TcpClient client = new TcpClient();
//            await client.ConnectAsync( IPAddress.Loopback, RoutingPort );

//            RoutingRemoteClient remoteClient = new RoutingRemoteClient( client, new CancellationTokenSource(), true );
//            InitiateHandshakePacket initiateHandshake = new InitiateHandshakePacket( ProtocolVersion );

//            await remoteClient.SendDataAsync( initiateHandshake.FinalizePacket() );
//            remoteClient.ListenForDataTask( remoteClient.CancellationToken ).RunAsync();

//            return remoteClient;
//        }

//        public override async Task HandleDataAsync( byte[] data, CancellationToken token )
//        {
//            if ( token.IsCancellationRequested )
//            {
//                return;
//            }

//            RoutingPacketId id = (RoutingPacketId) data[0];
//            Console.WriteLine(
//                $"Received {Enum.GetName( typeof( RoutingPacketId ), id )} packet (opcode 0x{(byte) id:X2})" );
//            switch ( id )
//            {
//                case RoutingPacketId.InitiateHandshake:
//                {
//                    InitiateHandshakePacket packet = new InitiateHandshakePacket( data );
//                    if ( packet.ProtocolVersion != ProtocolVersion )
//                    {
//                        await this.Kick( $"Requested protocol version ({packet.ProtocolVersion}) " +
//                                         $"is different than the expected version ({ProtocolVersion})", token );
//                        break;
//                    }
//                    Console.WriteLine( $"Received handshake for protocol version {packet.ProtocolVersion}" );
//                    break;
//                }

//                case RoutingPacketId.KeepAlive:
//                {
//                    KeepAlivePacket keepAlive = new KeepAlivePacket( data );
//                    if ( this.OutboundMode )
//                    {
//                        // Send it back with the current timestamp
//                        Console.WriteLine( "  - Responding to keep alive" );
//                        await this.SendDataAsync( keepAlive.WithDateTime( DateTime.UtcNow )., token );
//                    }
//                    else
//                    {
//                        Console.WriteLine( "  - Validating keep alive" );
//                        if ( keepAlive.Guid != this.LastKeepAliveGuid )
//                        {
//                            await this.Kick( "Keep alive response had the wrong GUID", token );
//                            break;
//                        }

//                        this.LastKeepAliveTime = keepAlive.TimeSent;
//                    }
//                    break;
//                }

//                case RoutingPacketId.Disconnect:
//                {
//                    DisconnectPacket disconnect = new DisconnectPacket( data );
//                    Console.WriteLine( $"Client kicked. Reason: {disconnect.Reason}" );

//                    if ( !this.Connected )
//                    {
//                        await this.Disconnect( TimeSpan.FromMilliseconds( 100 ) );
//                    }

//                    break;
//                }

//                default:
//                    throw new InvalidPacketException(
//                        $"Received unknown or unimplemented packet (opcode: 0x{(byte) id:X2})" );
//            }
//        }

//        private async void KeepAliveLoopAsync()
//        {
//            TimeSpan timeout = TimeSpan.FromSeconds( 15 );
//            TimeSpan gracePeriod = TimeSpan.FromSeconds( 1 ); // 1 second grace period to allow for latency
//            while ( this.Connected && !this.CancellationToken.IsCancellationRequested )
//            {
//                if ( ( this.LastKeepAliveTime != DateTime.MinValue ) &&
//                     ( this.LastKeepAliveTime + timeout + gracePeriod < DateTime.UtcNow ) )
//                {
//                    Console.WriteLine( this.LastKeepAliveTime );
//                    Console.WriteLine( this.LastKeepAliveTime + timeout );
//                    Console.WriteLine( DateTime.UtcNow );
//                    await this.Kick( "Timed out", this.CancellationToken );
//                    break;
//                }

//                this.LastKeepAliveTime = DateTime.UtcNow;
//                this.LastKeepAliveGuid = Guid.NewGuid();
//                KeepAlivePacket packet = new KeepAlivePacket( this.LastKeepAliveTime, this.LastKeepAliveGuid );

//                await this.SendDataAsync( packet, this.CancellationToken );
//                await Task.Delay( timeout );
//            }
//        }

//        private async Task Kick( string reason, CancellationToken token )
//        {
//            DisconnectPacket packet = new DisconnectPacket( reason );
//            await this.SendDataAsync( packet, token );
//            await this.Disconnect( TimeSpan.FromMilliseconds( 100 ) );
//        }
//    }
//}


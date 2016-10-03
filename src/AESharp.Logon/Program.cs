﻿using System;
using System.Collections.Generic;
using System.Net;
using AESharp.Core.Interfaces;
using AESharp.Core.Interfaces.Networking;
using AESharp.Logon.Networking;
using AESharp.Networking;
using AESharp.Networking.Events;
using AESharp.Networking.Packets;
using AESharp.Networking.Packets.Serialization;
using SimpleInjector;

namespace AESharp.Logon
{
    public static class Program
    {
        private static readonly Dictionary<PacketId, Type> PacketTypes;
        static Container _container;

        static Program()
        {
            PacketTypes = new Dictionary<PacketId, Type>
            {
                [PacketId.Logon] = typeof( LogonPacket )
            };
        }

        public static void Main( string[] args )
        {
            _container = new Container();

            // Build (de)serializers before they're needed to speed up packet reads/writes
            PacketSerialization packetSerializer = new PacketSerialization();
            packetSerializer.CacheObjects( typeof( LogonPacket) );

            _container.RegisterSingleton( typeof(IPacketSerializer), packetSerializer );
            _container.RegisterSingleton<INetworkEngine, LogonNetworkEngine>();

            _container.Verify();

            AETcpServer server = _container.GetInstance<AETcpServer>();
            server.StartListening( IPAddress.Any, 3724 );

            server.ReceiveData += ServerOnReceiveData;

            Console.WriteLine( "Listening..." );
            Console.ReadLine();
        }

        private static void ServerOnReceiveData( object sender, NetworkEventArgs networkEventArgs )
        {
            IPacketSerializer serializer = _container.GetInstance<IPacketSerializer>();

            PacketId packetId = (PacketId) networkEventArgs.DataStream.ReadByte();
            Type type;
            if ( !PacketTypes.TryGetValue( packetId, out type ) )
            {
                Console.Error.WriteLine( "Unknown packet 0x{0:X2}", (int) packetId );
                networkEventArgs.DisconnectClient = true;
                return;
            }

            LogonPacket packet = serializer.DeserializePacket<LogonPacket>( networkEventArgs.DataStream, null );

            Console.WriteLine( "Received logon packet:" );
            Console.WriteLine( $"\tOpcode:\t\t\t{packetId}" );
            Console.WriteLine( $"\tError:\t\t\t{packet.Error}" );
            Console.WriteLine( $"\tLength:\t\t\t{packet.Length}" );
            Console.WriteLine( $"\tGame:\t\t\t{packet.Game}" );
            Console.WriteLine( $"\tBuild:\t\t\t{packet.Build}" );
            Console.WriteLine( $"\tPlatform:\t\t{packet.Platform}" );
            Console.WriteLine( $"\tOS:\t\t\t{packet.OS}" );
            Console.WriteLine( $"\tCountry:\t\t{packet.Country}" );
            Console.WriteLine( $"\tTimezone Bias:\t\t{packet.TimezoneBias}" );
            Console.WriteLine( $"\tIP:\t\t\t{packet.IPAddress}" );
            Console.WriteLine( $"\tAccount Name:\t\t{packet.AccountName}" );

            // Nothing else to do at this stage in development
            networkEventArgs.DisconnectClient = true;

            //NetworkPacket packet = new NetworkPacket(networkEventArgs.DataStream);

            //Console.WriteLine($"Reading 4 byte header");

            //int opcode = packet.ReadByte();
            //int error = packet.ReadByte();
            //int length = packet.ReadShort();
            //string game = packet.ReadFixedString( 4 ).Flip();
            //string build = $"{packet.ReadByte()}.{packet.ReadByte()}.{packet.ReadByte()} {packet.ReadShort()}";
            //string platform = packet.ReadFixedString( 4 ).Flip();
            //string os = packet.ReadFixedString( 4 ).Flip();
            //string country = packet.ReadFixedString( 4 ).Flip();
            //uint timezoneBias = packet.ReadUInt();
            //string ip = $"{packet.ReadByte()}.{packet.ReadByte()}.{packet.ReadByte()}.{packet.ReadByte()}";
            //byte accountNameLength = packet.ReadByte();
            //string accountName = packet.ReadFixedString( accountNameLength );

            //Console.WriteLine( $"Received logon packet:" );
            //Console.WriteLine( $"\tOpcode:\t\t\t{opcode}" );
            //Console.WriteLine( $"\tError:\t\t\t{error}" );
            //Console.WriteLine( $"\tLength:\t\t\t{length}" );
            //Console.WriteLine( $"\tGame:\t\t\t{game}" );
            //Console.WriteLine( $"\tBuild:\t\t\t{build}" );
            //Console.WriteLine( $"\tPlatform:\t\t{platform}" );
            //Console.WriteLine( $"\tOS:\t\t\t{os}" );
            //Console.WriteLine( $"\tCountry:\t\t{country}" );
            //Console.WriteLine( $"\tTimezone Bias:\t\t{timezoneBias}" );
            //Console.WriteLine( $"\tIP:\t\t\t{ip}" );
            //Console.WriteLine( $"\tAccount Name Length:\t{accountNameLength}" );
            //Console.WriteLine( $"\tAccount Name:\t\t{accountName}" );
        }
    }
}
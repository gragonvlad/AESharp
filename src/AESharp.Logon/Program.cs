﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AESharp.Logon.Universal.Networking;
using AESharp.Networking;
using AESharp.Networking.Data;
using SimpleInjector;

namespace AESharp.Logon
{
    public static class Program
    {
        private static Container _container;

        private static readonly RemoteClientRepository ClientRepository = new RemoteClientRepository();

        public static void Main( string[] args )
        {
            _container = new Container();

            _container.Verify();

            RealTcpServer server = new RealTcpServer( new IPEndPoint( IPAddress.Loopback, 3724 ) );
            server.Start( AcceptClientAction );

            Console.WriteLine( "Listening..." );
            Console.ReadLine();
        }

        private static async void AcceptClientAction( TcpClient rawClient )
        {
            Console.WriteLine( "Accepting client" );
            LogonRemoteClient client = new LogonRemoteClient( rawClient, new CancellationTokenSource() );

            Guid clientGuid = Guid.Empty;
            try
            {
                clientGuid = ClientRepository.AddClient( client );
                await client.ListenForDataTask( client.CancellationToken );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Unhandled exception in AcceptClientAction: {ex}" );
            }
            finally
            {
                if ( clientGuid != Guid.Empty )
                {
                    ClientRepository.RemoveClient( clientGuid );
                }
            }
        }
    }
}
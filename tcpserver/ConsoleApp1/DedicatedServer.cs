using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApp1
{
    class DedicatedServer
    {
        public static List<PlayerClient> pendingPlayers = new List<PlayerClient>();
        public static List<Room> roomList = new List<Room>();
        public static TcpListener server;
        public static void Start(int port)
        {
            server = new TcpListener(IPAddress.Any, port);

            server.Start();
            Console.WriteLine("Server started on port " + port.ToString());
            server.BeginAcceptTcpClient(ClientConnectCallback, 0);

        }
        static async void ClientConnectCallback(IAsyncResult result)
        {

            var incomingConnection = server.EndAcceptTcpClient(result);
            var client = new PlayerClient(incomingConnection);
            Console.WriteLine($"Incoming connection from {incomingConnection.Client.RemoteEndPoint}");

            byte[] buffer = new byte[4096];
            int dataLength = await incomingConnection.GetStream().ReadAsync(buffer, 0, 4096);
            var data = new byte[dataLength];
            Array.Copy(buffer, data, dataLength);
            server.BeginAcceptTcpClient(ClientConnectCallback, 0);
            try
            {
                var msg = Encoding.ASCII.GetString(data);
                Console.WriteLine(msg);
                var split = msg.Split();
                client.name = split[0];
                var roomId = int.Parse(split[1]);
                if (roomId != -1)
                {
                    var playerId = int.Parse(split[2]);
                    if (roomList[roomId].GetPlayer(playerId).name == client.name)
                    {
                        roomList[roomId].SetDisconnectedPlayer(playerId, client);
                        return;
                    }

                }
            }
            catch
            {

            }



            if (pendingPlayers.Count > 0)
            {
                Room room = new Room(pendingPlayers[0], client, roomList.Count);
                roomList.Add(room);
                pendingPlayers.RemoveAt(0);
            }
            else
            {
                pendingPlayers.Add(client);
            }
            //ConsoleCommand();
        }
        public static void ConsoleCommand()
        {
            string cmd = Console.ReadLine();
            if (cmd == "client")
            {
                pendingPlayers.ForEach(n =>
                {
                    Console.Write(n.tcpSocket.Connected);
                });
            }
            if (cmd == "send")
            {
                pendingPlayers.ForEach(n =>
                {
                    n.WriteData("hello");
                });
            }
            ConsoleCommand();
        }
    }
}

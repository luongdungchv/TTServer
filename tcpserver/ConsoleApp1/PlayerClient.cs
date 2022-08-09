using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ConsoleApp1
{
    class PlayerClient
    {
        public TcpClient tcpSocket;
        public bool isConnected;
        public Room currentRoom;
        public string name;

        NetworkStream tcpStream;
        int dataBufferSize = 4096;
        byte[] receiveBuffer;
        //public AsyncCallback ReceiveCallback;

        public PlayerClient partner;
        
        public PlayerClient(TcpClient socket)
        {

            tcpSocket = socket;
            tcpStream = socket.GetStream();
            receiveBuffer = new byte[dataBufferSize];
        }

        public void BeginCommunicate(PlayerClient _partner)
        {
            partner = _partner;
            try
            {
                tcpStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, 0);
            }
            catch
            {
                Disconnect();
                //partner.Disconnect();
                if(currentRoom != null) currentRoom.Abandon();
            }
        }
        void ReceiveCallback(IAsyncResult result)
        {
            //if (!CheckConnection(tcpSocket.Client))
            //{
            //    isConnected = false;
            //    return;
            //}
            isConnected = true;
            int dataLength = 0;          
            try
            {
                dataLength = tcpStream.EndRead(result);
                if(dataLength <= 0)
                {                  
                    Disconnect();
                    partner.Disconnect();
                    currentRoom.Abandon();
                    return;
                }
                byte[] data = new byte[dataLength];
                Array.Copy(receiveBuffer, data, dataLength);
                receiveBuffer = new byte[dataBufferSize];

                string msg = Encoding.ASCII.GetString(data);
                Console.WriteLine($"ReceiveData: {name}: {msg}, length: {msg.Length}");
                tcpStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, 0);
                WriteData("pr");
                partner.WriteData(msg);
            }
            catch
            {
                Disconnect();
                //partner.Disconnect();
                //currentRoom.Abandon();
                return;
            }
           
        }
        public async void WriteData(string msg)
        {
            if (!CheckConnection(tcpSocket.Client))
            {
                Console.WriteLine("Not Connected");
                isConnected = false;
                return;
            }
            isConnected = true;
            byte[] data = Encoding.ASCII.GetBytes(msg);
            try
            {
                await tcpStream.WriteAsync(data);
                Console.WriteLine($"SendData: {name}: {msg}");
            }
            catch
            {
                Console.WriteLine("Disconnected");
            }
        }
        public void Disconnect()
        {
            try
            {
                isConnected = false;
                tcpSocket.Close();
                //tcpSocket = null;
                Console.WriteLine("Disconnected");
            }
            catch(NullReferenceException)
            {
                Console.WriteLine("Disconnected");
            }
        }
        public bool CheckConnection(Socket socket)
        {
            if(socket == null)
            {
                return false;
            }
            if (socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0)
            {
                return false;
            }
            return true;
        }
    }
}

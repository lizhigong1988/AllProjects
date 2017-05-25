using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ProjectsManageServer.Connect
{
    class Connection
    {
        internal static bool InitConnect(IPAddress ip, int portNO)
        {
            //默认地址池
            int bocklog = 10;
            ///获取本地的IP地址
            IPAddress AddressIP = ip;
            if (null == AddressIP)
            {
                foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        AddressIP = _IPAddress;
                        break;
                    }
                }
            }
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(AddressIP, portNO));
            listener.Listen(bocklog);
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
            Console.Write("\n已建立服务器监听" + AddressIP + ":" + portNO.ToString() + "\n");
            int i = 0;
            while (true)
            {
                i++;
                Thread.Sleep(5000);
                Console.Write(".");
                if (i % 10 == 0)
                {
                    Console.Write("\n");
                }
                if (i == 100)
                {
                    i = 0;
                    Console.Clear();
                    Console.Write("\n已建立服务器监听" + AddressIP + ":" + portNO.ToString() + "\n");
                    Console.Write("已连接[" + connectedEndPoint.Count + "]:\n");
                    foreach (string connect in connectedEndPoint)
                    {
                        Console.Write(connect + "\n");
                    }
                }
            }
            //return true;
        }

        static List<string> connectedEndPoint = new List<string>();
        private static void OnConnectRequest(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket sock = listener.EndAccept(ar);
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
            Console.Write("\n连接:" + sock.RemoteEndPoint.ToString() + "\n");
            if (!connectedEndPoint.Contains(sock.RemoteEndPoint.ToString()))
            {
                connectedEndPoint.Add(sock.RemoteEndPoint.ToString());
            }
            byte[] temp = new byte[0];
            sock.BeginReceive(temp, 0, 0, SocketFlags.None, new AsyncCallback(OnRecievedData), sock);
        }


        private static void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int RecievedSize = sock.Available;
            if (RecievedSize == 0)
            {
                Console.Write("\n退出:" + sock.RemoteEndPoint.ToString() + "\n");
                if (connectedEndPoint.Contains(sock.RemoteEndPoint.ToString()))
                {
                    connectedEndPoint.Remove(sock.RemoteEndPoint.ToString());
                }
                sock.Close();
                return;
            }
            byte[] RecievedData = new byte[RecievedSize];
            sock.Receive(RecievedData);
            byte[] SendData = new byte[0];
            ConnectService.Services(sock.RemoteEndPoint.ToString(), RecievedData, ref SendData);
            List<byte> listSendData = new List<byte>(Encoding.Default.GetBytes("[BIGEN]"));
            listSendData.AddRange(SendData);
            listSendData.AddRange(Encoding.Default.GetBytes("[END]"));
            if (SendData.Length != 0) sock.BeginSend(listSendData.ToArray(), 0, listSendData.Count, SocketFlags.None, null, sock);
            RecievedData = new byte[0];
            sock.BeginReceive(RecievedData, 0, 0, SocketFlags.None, new AsyncCallback(OnRecievedData), sock);
        }
    }
}

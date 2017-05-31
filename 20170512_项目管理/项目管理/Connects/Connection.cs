using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace 项目管理.Connect
{
    class Connection
    {
        /// <summary>  
        /// Socket 
        /// </summary>  
        static Socket socket = null;
        /// <summary>  
        /// 接受数据  
        /// </summary>  
        public delegate void ReadByte(Byte[] ReadByte);
        public static event ReadByte ReadBytes;
        /// <summary>  
        /// 初始化连接  
        /// </summary>  
        public static bool AppConnectInit(string ipAddr)
        {
            string[] ipElem = ipAddr.Split(':');
            if(ipElem.Length < 2)
            {
                return false;
            }
            string serverIp = ipElem[0];
            string readPort = ipElem[1];
            try
            {
                IPAddress _SERVERIP = null;
                if (!IPAddress.TryParse(serverIp, out _SERVERIP))
                {
                    _SERVERIP = Dns.GetHostAddresses(serverIp)[0];
                }
                socket = new Socket(_SERVERIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  //建立SOCKET  
                ipendpoint = new IPEndPoint(_SERVERIP, int.Parse(readPort));  //建立IP目标和端口  
                socket.Connect(ipendpoint);//连接 
            }
            catch
            {
                return false;
            }
            byte[] temp = new byte[1]; //建立个临时的发送数据  
            socket.BeginReceive(temp, 0, 0, SocketFlags.None, new AsyncCallback(Read), socket);  //建立异步读取  
            return true;
        }

        internal static void CloseConnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            //socket.Close();
            socket = null;
            ipendpoint = null;
            ReadBytes = null;
        }
        static IPEndPoint ipendpoint;
        /// <summary>  
        /// 发送数据  
        /// </summary>  
        public static void SendMessage(Byte[] SendByte)
        {
            List<byte> listSendData = new List<byte>(Encoding.Default.GetBytes("[BIGEN]"));
            listSendData.AddRange(SendByte);
            listSendData.AddRange(Encoding.Default.GetBytes("[END]"));
            if (!socket.Connected)
            {
                socket.Connect(ipendpoint);//连接 
            }
            socket.BeginSend(listSendData.ToArray(), 0, listSendData.Count, SocketFlags.None, null, socket); //发送数据  
        }

        private static void Read(IAsyncResult iar)
        {
            Socket socket = (Socket)iar.AsyncState; //获取Socket对象  
            int ReadCount = socket.Available;  //获得包大小  
            if (ReadCount == 0) return;  //如果包是0 就直接返回  
            byte[] Byte = new byte[ReadCount];   //建立要接收的对象信息  
            socket.BeginReceive(Byte, 0, ReadCount, SocketFlags.None, null, socket); //开始读取  
            if (ReadBytes != null) ReadBytes(Byte);  //执行委托 
            byte[] temp = new byte[1]; //建立个临时的发送数据  
            socket.BeginReceive(temp, 0, 0, SocketFlags.None, new AsyncCallback(Read), socket);  //建立异步读取     
        }

        public static void AddFunc(ReadByte func)
        {
            if (ReadBytes == null)
            {
                ReadBytes += func;
            }
        }
    }
}

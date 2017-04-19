using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.Net;
using System.Net.Sockets;

namespace ClientCommunication.Connects
{
    public class Connect
    {
        /// <summary>
        /// 尝试连接服务端
        /// </summary>
        /// <returns></returns>
        internal static CommonDef.COM_RET TryConnect()
        {
            string serverAddr = ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.SERVER_ADDR);
            if ("" != serverAddr)
            {
                return CommonDef.COM_RET.CONNECT_ERROR;
            }
            try
            {
                string[] ipAddr = serverAddr.Split(':');
                int port = int.Parse(ipAddr[1]);
                IPAddress serverIp = IPAddress.Parse(ipAddr[0]);
                Socket socket = new Socket(serverIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  //建立SOCKET  
                IPEndPoint ipendpoint = new IPEndPoint(serverIp, port);  //建立IP目标和端口  
                socket.Connect(ipendpoint);//连接 
                socket.Close();
            }
            catch
            {
                return CommonDef.COM_RET.CONNECT_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }
    }
}

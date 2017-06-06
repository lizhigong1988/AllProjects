using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
using System.Threading;
using System.Data;
using System.Windows.Interop;

namespace 项目管理.Connect
{
    class CommunicationHelper
    {
        //阻塞进程
        static AutoResetEvent resetEvent = new AutoResetEvent(false);
        //回复判断
        static bool isRecived = true;
        //回复消息
        static List<Byte> retBuf = new List<byte>();

        public static bool IsConnected = false;

        /// <summary>
        /// 约定报文长度，此值发生变化需要同时修改“CommonLib.CommonDef.cs”中对应的定义
        /// </summary>
        public static int MAX_MSG_LENGTH = 1024 * 50;

        /// <summary>
        /// 通信超时设置，此值发生变化需要同时修改“CommonLib.CommonDef.cs”中对应的定义
        /// </summary>
        public static int TIME_OUT = 10000;

        private static bool SendAndRcvWorker(string Msg, out string revMsg)
        {
            LoadingWorker loading = new LoadingWorker();
            loading.MsgData = Msg;
            loading.ShowDialog();
            revMsg = loading.RecvData;
            return loading.ret;
        }

        public static bool SendAndRcv(string sendData, out string recvData)
        {
            List<byte> listSendData = Encoding.Default.GetBytes(sendData).ToList();
            List<byte> sendOnce;
            int dataLength = listSendData.Count; 
            string recv = "";
            string[] recvElem = null;
            string msgId = Guid.NewGuid().ToString(); 
            recvData = "";
            //报文过长多次发送
            while (listSendData.Count > MAX_MSG_LENGTH)
            {
                sendOnce = Encoding.Default.GetBytes(msgId.ToString() + "\n" + dataLength.ToString() + "\n").ToList();
                sendOnce.AddRange(listSendData.GetRange(0, MAX_MSG_LENGTH));
                isRecived = false;
                retBuf.Clear();
                resetEvent = new AutoResetEvent(false);
                Connection.SendMessage(sendOnce.ToArray());
                resetEvent.WaitOne(TIME_OUT, true);//阻塞 等待服务端回复.
                if (!isRecived)
                {
                    return false;
                }
                recv = Encoding.Default.GetString(retBuf.ToArray());
                recvElem = recv.Split('\n');
                if (recvElem[2] != "0")//正常应答
                {
                    return false;
                }
                listSendData.RemoveRange(0, MAX_MSG_LENGTH);
            }
            //最后一次发送
            sendOnce = Encoding.Default.GetBytes(msgId.ToString() + "\n" + 0 + "\n").ToList();
            sendOnce.AddRange(listSendData);
            isRecived = false;
            retBuf.Clear();
            resetEvent = new AutoResetEvent(false);
            Connection.SendMessage(sendOnce.ToArray());
            resetEvent.WaitOne(TIME_OUT, true);//阻塞 等待服务端回复.
            if (!isRecived)
            {
                return false;
            }
            //收报文
            recv = Encoding.Default.GetString(retBuf.ToArray());
            recvElem = recv.Split('\n');
            int headerLength = recvElem[0].Length + recvElem[1].Length + 2 ;
            int recvLength = retBuf.Count - headerLength;
            recvData += recv.Substring(headerLength);
            //报文过长 多次收取
            while (recvLength < int.Parse(recvElem[1]))
            {
                sendOnce = Encoding.Default.GetBytes(msgId.ToString() + "\n-1").ToList();
                isRecived = false;
                retBuf.Clear();
                resetEvent = new AutoResetEvent(false);
                Connection.SendMessage(sendOnce.ToArray());
                resetEvent.WaitOne(TIME_OUT, true);//阻塞 等待服务端回复.
                if (!isRecived)
                {
                    return false;
                }
                recv = Encoding.Default.GetString(retBuf.ToArray());
                recvElem = recv.Split('\n');
                headerLength = recvElem[0].Length + recvElem[1].Length + 2;
                recvData += recv.Substring(headerLength);
                recvLength = retBuf.Count - headerLength;
            }

            return true;
        }

        public static void ReceiveMsg(Byte[] msgBuf)
        {
            if (isRecived)
            {
                return;
            }
            retBuf.AddRange(msgBuf);
            string check = Encoding.Default.GetString(retBuf.ToArray());
            if (check.StartsWith("[BIGEN]") && check.EndsWith("[END]"))
            {
                retBuf.RemoveRange(0, 7);
                retBuf.RemoveRange(retBuf.Count - 5, 5);
                check = Encoding.Default.GetString(retBuf.ToArray());
                isRecived = true;
                resetEvent.Set();//解阻塞
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        internal static bool AppConnectInit(string port)
        {
            if (IsConnected)
            {
                return true;
            }
            Connection.AddFunc(ReceiveMsg);
            IsConnected = Connection.AppConnectInit(port);
            return IsConnected;
        }

        internal static void CloseConnect()
        {
            IsConnected = false;
            Connection.CloseConnect();
        }

        internal static DataTable GetProgramFiles()
        {
            string Msg = "-1\n";//GetProgramFiles
            string revMsg = "";
            bool ret = SendAndRcvWorker(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return GetDataTable(revMsg.Split('\n')[1]);
        }

        /// <summary>
        /// 方法更新需要更改：项目管理.Connects.CommunicationHelper.cs中对应方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string str)
        {
            if (str == "")
            {
                return null;
            }
            DataTable retDt = new DataTable();
            string[] lines = str.Split('\r');
            string[] columns = lines[0].Split('\t');
            foreach (string colum in columns)
            {
                if (colum == "")
                {
                    continue;
                }
                retDt.Columns.Add(colum);
            }
            for (int i = 1; i < lines.Length; i++)
            {
                List<string> row = new List<string>(lines[i].Split('\t'));
                if (row.Count < retDt.Columns.Count)
                {
                    continue;
                }
                row.RemoveRange(retDt.Columns.Count, row.Count - retDt.Columns.Count);
                retDt.Rows.Add(row.ToArray());
            }
            return retDt;
        }

        internal static bool DownloadFile(string fileName, string tagetFileName)
        {
            string Msg = "25\n";//DOWNLOAD_FILE
            Msg += fileName + "\n";
            string revMsg = "";
            bool ret = SendAndRcvWorker(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            try
            {
                if (revMsg == "")
                {
                    return false;
                }
                byte[] fileData = Convert.FromBase64String(revMsg);
                File.WriteAllBytes(tagetFileName, fileData);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

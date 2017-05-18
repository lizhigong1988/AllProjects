using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
using System.Threading;
using CommonLib;
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


        private static bool SendAndRcv(string sendData, out string recvData)
        {
            List<byte> listSendData = Encoding.Default.GetBytes(sendData).ToList();
            List<byte> sendOnce;
            int dataLength = listSendData.Count; 
            string recv = "";
            string[] recvElem = null;
            string msgId = Guid.NewGuid().ToString(); 
            recvData = "";
            //报文过长多次发送
            while (listSendData.Count > CommonDef.MAX_MSG_LENGTH)
            {
                sendOnce = Encoding.Default.GetBytes(msgId.ToString() + "\n" + dataLength.ToString() + "\n").ToList();
                sendOnce.AddRange(listSendData.GetRange(0, CommonDef.MAX_MSG_LENGTH));
                isRecived = false;
                retBuf.Clear();
                resetEvent = new AutoResetEvent(false);
                Connection.SendMessage(sendOnce.ToArray());
                resetEvent.WaitOne(CommonDef.TIME_OUT, true);//阻塞 等待服务端回复.
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
                listSendData.RemoveRange(0, CommonDef.MAX_MSG_LENGTH);
            }
            //最后一次发送
            sendOnce = Encoding.Default.GetBytes(msgId.ToString() + "\n" + 0 + "\n").ToList();
            sendOnce.AddRange(listSendData);
            isRecived = false;
            retBuf.Clear();
            resetEvent = new AutoResetEvent(false);
            Connection.SendMessage(sendOnce.ToArray());
            resetEvent.WaitOne(CommonDef.TIME_OUT, true);//阻塞 等待服务端回复.
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
                resetEvent.WaitOne(CommonDef.TIME_OUT, true);//阻塞 等待服务端回复.
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


        internal static List<string> GetHisDeparts()
        {
            List<string> retList = new List<string>();
            string Msg = ((int)CommonDef.FUN_NO.GET_HIS_DEPARTS).ToString() + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return retList;
            }
            string[] departs = revMsg.Split('\n')[1].Split('\r');
            foreach (string depart in departs)
            {
                if (depart == "")
                {
                    continue;
                }
                retList.Add(depart);
            }
            return retList;
        }

        internal static bool AddNewProject(string demandName, string depart, string date, string expectDate,
            string kinds, string stage, string state, string note, string testPersion, string businessPersion,
            string remark, DataTable dtSysInfo)
        {
            string Msg = ((int)CommonDef.FUN_NO.ADD_NEW_PROJECT).ToString() + "\n";
            Msg += demandName + "\n";
            Msg += depart + "\n";
            Msg += date + "\n";
            Msg += expectDate + "\n";
            Msg += kinds + "\n";
            Msg += stage + "\n";
            Msg += state + "\n";
            Msg += note + "\n";
            Msg += testPersion + "\n";
            Msg += businessPersion + "\n";
            Msg += remark + "\n";
            Msg += CommonDef.GetDataTableStr(dtSysInfo) + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static Dictionary<string, string> GetCurProNames(string sysId, bool showAll)
        {
            Dictionary<string, string> retList = new Dictionary<string, string>();
            string Msg = ((int)CommonDef.FUN_NO.GET_CUR_PRO_NAMES).ToString() + "\n";
            Msg += sysId + "\n";
            Msg += showAll.ToString() + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return retList;
            }
            string[] departs = revMsg.Split('\n')[1].Split('\r');
            foreach (string depart in departs)
            {
                if (depart == "")
                {
                    continue;
                }
                retList.Add(depart.Split('\t')[0], depart.Split('\t')[1]);
            }
            return retList;
        }

        internal static DataTable GetProInfo(string select)
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_PRO_INFO).ToString() + "\n";
            Msg += select + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static DataTable GetTradesInfo(string curProId, string sysId)
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_TRADES_INFO).ToString() + "\n";
            Msg += curProId + "\n";
            Msg += sysId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static bool ModProject(string curProId, string sysId, string demandName, 
            string depart, string date, string expectDate,
            string kinds, string stage, string state, string finish, string note, string testPersion, string businessPersion,
            string remark, DataTable dtSysInfo, DataTable dtTrades)
        {
            string Msg = ((int)CommonDef.FUN_NO.MOD_PROJECT).ToString() + "\n";
            Msg += curProId + "\n";
            Msg += sysId + "\n";
            Msg += demandName + "\n";
            Msg += depart + "\n";
            Msg += date + "\n";
            Msg += expectDate + "\n";
            Msg += kinds + "\n";
            Msg += stage + "\n";
            Msg += state + "\n";
            Msg += finish + "\n";
            Msg += note + "\n";
            Msg += testPersion + "\n";
            Msg += businessPersion + "\n";
            Msg += remark + "\n";
            Msg += CommonDef.GetDataTableStr(dtSysInfo) + "\n";
            Msg += CommonDef.GetDataTableStr(dtTrades) + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static DataTable QueryProInfo(string stage, string state, string date, string sysId)
        {
            string Msg = ((int)CommonDef.FUN_NO.QUERY_PRO_INFO).ToString() + "\n";
            Msg += stage + "\n";
            Msg += state + "\n";
            Msg += date + "\n";
            Msg += sysId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static List<string> DiffTrade(string proID, string sysId, string tradeNo)
        {
            List<string> retList = new List<string>();
            string Msg = ((int)CommonDef.FUN_NO.DIFF_TRADE).ToString() + "\n";
            Msg += proID + "\n";
            Msg += sysId + "\n";
            Msg += tradeNo + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return retList;
            }
            string[] departs = revMsg.Split('\n')[1].Split('\r');
            foreach (string depart in departs)
            {
                if (depart == "")
                {
                    continue;
                }
                retList.Add(depart);
            }
            return retList;
        }

        internal static List<string> GetHisWorkers(string sysId)
        {
            List<string> retList = new List<string>();
            string Msg = ((int)CommonDef.FUN_NO.GET_HIS_WORKERS).ToString() + "\n";
            Msg += sysId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return retList;
            }
            string[] departs = revMsg.Split('\n')[1].Split('\r');
            foreach (string depart in departs)
            {
                if (depart == "")
                {
                    continue;
                }
                retList.Add(depart);
            }
            return retList;
        }

        internal static DataTable QueryProDaysInfo(string sysId, string worker, string yearMonth)
        {
            string Msg = ((int)CommonDef.FUN_NO.QUERY_PRO_DAYS_INFO).ToString() + "\n";
            Msg += sysId + "\n";
            Msg += worker + "\n";
            Msg += yearMonth + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static DataTable GetWorkDays(string sysId, string curQueryWorker, string demandId)
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_WORK_DAYS).ToString() + "\n";
            Msg += sysId + "\n";
            Msg += curQueryWorker + "\n";
            Msg += demandId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static bool SaveAdjustWorkDays(string demandId, string sysId, string worker, DataTable dataTable)
        {
            string Msg = ((int)CommonDef.FUN_NO.SAVE_ADJUST_WORK_DAYS).ToString() + "\n";
            Msg += demandId + "\n";
            Msg += sysId + "\n";
            Msg += worker + "\n";
            Msg += CommonDef.GetDataTableStr(dataTable) + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static string GetWorkerMonthDays(string curQueryWorker, string curQueryDate)
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_WORKER_MONTH_DAYS).ToString() + "\n";
            Msg += curQueryWorker + "\n";
            Msg += curQueryDate + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return "";
            }
            return revMsg.Split('\n')[1];
        }

        internal static Dictionary<string, string> GetAllSysDic()
        {
            Dictionary<string, string> retList = new Dictionary<string, string>();
            string Msg = ((int)CommonDef.FUN_NO.GET_ALL_SYS_DIC).ToString() + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return retList;
            }
            string[] departs = revMsg.Split('\n')[1].Split('\r');
            foreach (string depart in departs)
            {
                if (depart == "")
                {
                    continue;
                }
                retList.Add(depart.Split('\t')[0], depart.Split('\t')[1]);
            }
            return retList;
        }

        internal static bool AddNewSystem(string sysName, string manage1,
            string manage2, string remark)
        {
            string Msg = ((int)CommonDef.FUN_NO.ADD_NEW_SYSTEM).ToString() + "\n";
            Msg += sysName + "\n";
            Msg += manage1 + "\n";
            Msg += manage2 + "\n";
            Msg += remark + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static DataTable GetSystemInfo(string sysId = "")
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_SYSTEM_INFO).ToString() + "\n";
            Msg += sysId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static bool ModSystem(string sysId, string sysName, string user1, string user2, string remark)
        {
            string Msg = ((int)CommonDef.FUN_NO.MOD_SYSTEM).ToString() + "\n";
            Msg += sysId + "\n";
            Msg += sysName + "\n";
            Msg += user1 + "\n";
            Msg += user2 + "\n";
            Msg += remark + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static bool DelSystem(string sysId)
        {
            string Msg = ((int)CommonDef.FUN_NO.DEL_SYSTEM).ToString() + "\n";
            Msg += sysId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static DataTable GetUserInfo(string userName = "")
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_USER_INFO).ToString() + "\n";
            Msg += userName + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static bool ModPassword(string userName, string password)
        {
            string Msg = ((int)CommonDef.FUN_NO.MOD_PASSWORD).ToString() + "\n";
            Msg += userName + "\n";
            Msg += password + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static bool AddNewUser(string userName, string sysId, string role, string company, string remark)
        {
            string Msg = ((int)CommonDef.FUN_NO.ADD_NEW_USER).ToString() + "\n";
            Msg += userName + "\n";
            Msg += sysId + "\n";
            Msg += role + "\n";
            Msg += company + "\n";
            Msg += remark + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }


        internal static bool ModUserInfo(string userName, string orgSysId, string psw, string sysId, string role,
            string company, string remark)
        {
            string Msg = ((int)CommonDef.FUN_NO.MOD_USER_INFO).ToString() + "\n";
            Msg += userName + "\n";
            Msg += orgSysId + "\n";
            Msg += psw + "\n";
            Msg += sysId + "\n";
            Msg += role + "\n";
            Msg += company + "\n";
            Msg += remark + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static DataTable GetProSystemInfo(string curProId)
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_PRO_SYS_INFO).ToString() + "\n";
            Msg += curProId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static bool DelUserInfo(string userName, string sysId)
        {
            string Msg = ((int)CommonDef.FUN_NO.DEL_USER_INFO).ToString() + "\n";
            Msg += userName + "\n";
            Msg += sysId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static DataTable GetProFileInfo(string curProId)
        {
            string Msg = ((int)CommonDef.FUN_NO.GET_PRO_FILE_INFO).ToString() + "\n";
            Msg += curProId + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return null;
            }
            return CommonDef.GetDataTable(revMsg.Split('\n')[1]);
        }

        internal static bool DownloadFile(string fileName)
        {
            string Msg = ((int)CommonDef.FUN_NO.DOWNLOAD_FILE).ToString() + "\n";
            Msg += fileName + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
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
                File.WriteAllBytes(fileName, fileData);
            }
            catch
            {
                return false;
            }
            return true;
        }

        internal static bool UploadFile(string fileAllName)
        {

            byte[] fileData = File.ReadAllBytes(fileAllName);
            string strData = Convert.ToBase64String(fileData);
            string Msg = ((int)CommonDef.FUN_NO.UPLOAD_FILE).ToString() + "\n";
            Msg += fileAllName + "\n";
            Msg += strData + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }

        internal static bool DelFile(string fileName)
        {
            string Msg = ((int)CommonDef.FUN_NO.DEL_FILE).ToString() + "\n";
            Msg += fileName + "\n";
            string revMsg = "";
            bool ret = SendAndRcv(Msg, out revMsg);
            if (!ret)
            {
                return false;
            }
            return revMsg == "0";
        }
    }
}

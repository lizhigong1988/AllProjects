using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using ProjectsManageServer.DataBases;
using System.Data;
using System.IO;

namespace ProjectsManageServer.Connect
{
    class MiddleService
    {
        public static string LOG_PATH = "LOG";
        private static string curDate = DateTime.Now.ToString("yyyyMMdd");
        static int LOG_LENGH = 256;

        public static byte[] AnalysisFile(string connectFlag, string msgData)
        {
            string[] elem = msgData.Split('\n');
            string logPath = LOG_PATH + "\\" + curDate;
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            logPath += "\\" + connectFlag.Replace(".", "_").Split(':')[0];
            string log = "";
            if (msgData.Length > LOG_LENGH)
            {
                log += "[REV]" + DateTime.Now.ToString("HHmmss") + ":" + msgData.Substring(0, LOG_LENGH) + "\r\n";
            }
            else
            {
                log += "[REV]" + DateTime.Now.ToString("HHmmss") + ":" + msgData + "\r\n";
            }
            string ret = "";
            switch ((CommonDef.FUN_NO)int.Parse(elem[0]))
            {
                case CommonDef.FUN_NO.GET_HIS_DEPARTS:
                    ret = GetHisDeparts(elem);
                    break;
                case CommonDef.FUN_NO.ADD_NEW_PROJECT:
                    ret = AddNewProject(elem);
                    break;
                case CommonDef.FUN_NO.GET_CUR_PRO_NAMES:
                    ret = GetCurProNames(elem);
                    break;
                case CommonDef.FUN_NO.GET_PRO_INFO:
                    ret = GetProInfo(elem);
                    break;
                case CommonDef.FUN_NO.GET_TRADES_INFO:
                    ret = GetTradesInfo(elem);
                    break;
                case CommonDef.FUN_NO.MOD_PROJECT:
                    ret = ModProject(elem);
                    break;
                case CommonDef.FUN_NO.QUERY_PRO_INFO:
                    ret = QueryProInfo(elem);
                    break;
                case CommonDef.FUN_NO.DIFF_TRADE:
                    ret = DiffTrade(elem);
                    break;
                case CommonDef.FUN_NO.GET_HIS_WORKERS:
                    ret = GetHisWorkers(elem);
                    break;
                case CommonDef.FUN_NO.QUERY_PRO_DAYS_INFO:
                    ret = QueryProDaysInfo(elem);
                    break;
                case CommonDef.FUN_NO.GET_WORK_DAYS:
                    ret = GetWorkDays(elem);
                    break;
                case CommonDef.FUN_NO.SAVE_ADJUST_WORK_DAYS:
                    ret = SaveAdjustWorkDays(elem);
                    break;
                case CommonDef.FUN_NO.GET_WORKER_MONTH_DAYS:
                    ret = GetWorkerMonthDays(elem);
                    break;
                case CommonDef.FUN_NO.GET_ALL_SYS_DIC:
                    ret = GetAllSysDic(elem);
                    break;
                case CommonDef.FUN_NO.ADD_NEW_SYSTEM:
                    ret = AddNewSystem(elem);
                    break;
                case CommonDef.FUN_NO.GET_SYSTEM_INFO:
                    ret = GetSystemInfo(elem);
                    break;
                case CommonDef.FUN_NO.MOD_SYSTEM:
                    ret = ModSystem(elem);
                    break;
                case CommonDef.FUN_NO.DEL_SYSTEM:
                    ret = DelSystem(elem);
                    break;
                case CommonDef.FUN_NO.GET_USER_INFO:
                    ret = GetUserInfo(elem);
                    break;
                case CommonDef.FUN_NO.MOD_PASSWORD:
                    ret = ModPassword(elem);
                    break;
                case CommonDef.FUN_NO.ADD_NEW_USER:
                    ret = AddNewUser(elem);
                    break;
                case CommonDef.FUN_NO.MOD_USER_INFO:
                    ret = ModUserInfo(elem);
                    break;
                case CommonDef.FUN_NO.DEL_USER_INFO:
                    ret = DelUserInfo(elem);
                    break;
                case CommonDef.FUN_NO.GET_PRO_SYS_INFO:
                    ret = GetProSysInfo(elem);
                    break;
                case CommonDef.FUN_NO.GET_PRO_FILE_INFO:
                    ret = GetProFileInfo(elem);
                    break;
                case CommonDef.FUN_NO.DOWNLOAD_FILE:
                    ret = DownloadFile(elem);
                    break;
                case CommonDef.FUN_NO.UPLOAD_FILE:
                    ret = UploadFile(elem);
                    break;
                case CommonDef.FUN_NO.DEL_FILE:
                    ret = DelFile(elem);
                    break;
                case CommonDef.FUN_NO.GET_SERVER_VERSION:
                    ret = CommonDef.VERSION_NUM;
                    break;
            }
            if (ret.Length > LOG_LENGH)
            {
                log += "[SEND]" + DateTime.Now.ToString("HHmmss") + ":" + ret.Substring(0, LOG_LENGH) + "\r\n";
            }
            else
            {
                log += "[SEND]" + DateTime.Now.ToString("HHmmss") + ":" + ret + "\r\n";
            }
            File.AppendAllText(logPath, log);
            return Encoding.Default.GetBytes(ret);
        }

        private static string DelFile(string[] elem)
        {
            File.Delete(elem[1]);
            return "0";
        }

        private static string UploadFile(string[] elem)
        {
            byte[] fileData = Convert.FromBase64String(elem[2]);
            File.WriteAllBytes(elem[1], fileData);
            return "0";
        }

        private static string DownloadFile(string[] elem)
        {
            if (File.Exists(elem[1]))
            {
                byte[] fileData = File.ReadAllBytes(elem[1]);
                return Convert.ToBase64String(fileData);
            }
            else
            {
                return "";
            }
        }

        private static string GetProFileInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.GetProFileInfo(elem[1]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string DelUserInfo(string[] elem)
        {
            bool sec = DataBaseManager.DelUserInfo(elem[1], elem[2]);
            return sec ? "0" : "-1";
        }

        private static string GetProSysInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.GetProSystemInfo(elem[1]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string ModUserInfo(string[] elem)
        {
            bool sec = DataBaseManager.ModUserInfo(elem[1], elem[2],
                elem[3], elem[4], elem[5], elem[6], elem[7]
                );
            return sec ? "0" : "-1";
        }

        private static string AddNewUser(string[] elem)
        {
            bool sec = DataBaseManager.AddNewUser(elem[1], elem[2],
                elem[3], elem[4], elem[5]
                );
            return sec ? "0" : "-1";
        }

        private static string ModPassword(string[] elem)
        {
            bool sec = DataBaseManager.ModPassword(elem[1], elem[2]);
            return sec ? "0" : "-1";
        }

        private static string GetUserInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.GetUserInfo(elem[1]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string DelSystem(string[] elem)
        {
            bool sec = DataBaseManager.DelSystem(elem[1]);
            return sec ? "0" : "-1";
        }

        private static string ModSystem(string[] elem)
        {
            bool sec = DataBaseManager.ModSystem(elem[1], elem[2],
                elem[3], elem[4], elem[5]);
            return sec ? "0" : "-1";
        }

        private static string GetSystemInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.GetSystemInfo(elem[1]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string AddNewSystem(string[] elem)
        {
            bool sec = DataBaseManager.AddNewSystem(elem[1], elem[2],
                elem[3], elem[4]);
            return sec ? "0" : "-1";
        }

        private static string GetAllSysDic(string[] elem)
        {
            Dictionary<string, string> dicAll = DataBaseManager.GetAllSysDic();
            string ret = "0\n";
            foreach (var dic in dicAll)
            {
                ret += dic.Key + "\t" + dic.Value + "\r";
            }
            return ret;
        }

        private static string GetWorkerMonthDays(string[] elem)
        {
            return "0\n" + DataBaseManager.GetWorkerMonthDays(elem[1], elem[2]);
        }

        private static string SaveAdjustWorkDays(string[] elem)
        {
            DataTable dtSysInfo = CommonDef.GetDataTable(elem[4]);
            bool sec = DataBaseManager.SaveAdjustWorkDays(elem[1], elem[2], elem[3],
                dtSysInfo);
            return sec ? "0" : "-1";
        }

        private static string GetWorkDays(string[] elem)
        {
            DataTable dt = DataBaseManager.GetWorkDays(elem[1], elem[2], elem[3]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string QueryProDaysInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.QueryProDaysInfo(elem[1], elem[2], elem[3]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string GetHisWorkers(string[] elem)
        {
            List<string> list = DataBaseManager.GetHisWorkers(elem[1]);
            string ret = "0\n";
            foreach (string name in list)
            {
                ret += name + "\r";
            }
            return ret;
        }

        private static string DiffTrade(string[] elem)
        {
            List<string> list = DataBaseManager.DiffTrade(elem[1], elem[2], elem[3]);
            string ret = "0\n";
            foreach (string name in list)
            {
                ret += name + "\r";
            }
            return ret;
        }

        private static string QueryProInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.QueryProInfo(elem[1], elem[2], elem[3], elem[4]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string ModProject(string[] elem)
        {
            DataTable dtSysInfo = CommonDef.GetDataTable(elem[15]);
            DataTable dtTradesInfo = CommonDef.GetDataTable(elem[16]);
            bool sec = DataBaseManager.ModProject(elem[1], elem[2], elem[3], elem[4], elem[5],
                elem[6], elem[7], elem[8], elem[9], elem[10], elem[11], elem[12], elem[13], elem[14],
                dtSysInfo, dtTradesInfo);
            return sec ? "0" : "-1";
        }

        private static string GetTradesInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.GetTradesInfo(elem[1], elem[2]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string GetProInfo(string[] elem)
        {
            DataTable dt = DataBaseManager.GetProInfo(elem[1]);
            return "0\n" + CommonDef.GetDataTableStr(dt);
        }

        private static string GetCurProNames(string[] elem)
        {
            Dictionary<string, string> list = DataBaseManager.GetCurProNames(elem[1], bool.Parse(elem[2]));
            string ret = "0\n";
            foreach (var name in list)
            {
                ret += name.Key + "\t" + name.Value + "\r";
            }
            return ret;
        }

        private static string AddNewProject(string[] elem)
        {
            DataTable dt = CommonDef.GetDataTable(elem[12]);
            bool sec = DataBaseManager.AddNewProject(elem[1], elem[2], elem[3], elem[4], elem[5],
                elem[6], elem[7], elem[8], elem[9], elem[10], elem[11], dt);

            return sec ? "0" : "-1";
        }

        private static string GetHisDeparts(string[] elem)
        {
            List<string> list = DataBaseManager.GetHisDeparts();
            string ret = "0\n";
            foreach (string depart in list)
            {
                ret += depart + "\r";
            }
            return ret;
        }
    }
}

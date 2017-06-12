using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Cryptography;

namespace CommonLib
{
    public class CommonDef
    {
        /// <summary>
        /// 版本号，用于客户端与服务端对比 1级不一致不允许登录 2级版不一致可正常使用，仅提示
        /// 1.1增加版本号对比功能
        /// 2.1修改人员管理中人员与系统的对应方式，增加电子邮件字段
        /// </summary>
        public static string VERSION_NUM = "2.1";

        /// <summary>
        /// 约定报文长度，此值发生变化需要同时修改“项目管理.Connects.CommunicationHelper.cs”中对应的定义
        /// </summary>
        public static int MAX_MSG_LENGTH = 1024 * 50;

        /// <summary>
        /// 通信超时设置，此值发生变化需要同时修改“项目管理.Connects.CommunicationHelper.cs”中对应的定义
        /// </summary>
        public static int TIME_OUT = 10000;

        public static int ERROR = -1;

        public static int OK = 0;

        public enum FUN_NO
        {
            GET_PROGRAM_FILES = -1,

            GET_HIS_DEPARTS,//0
            ADD_NEW_PROJECT,
            GET_CUR_PRO_NAMES,
            GET_PRO_INFO,
            GET_TRADES_INFO,

            MOD_PROJECT,//5
            QUERY_PRO_INFO,
            DIFF_TRADE,
            GET_HIS_WORKERS,
            QUERY_PRO_DAYS_INFO,

            GET_WORK_DAYS,//10
            SAVE_ADJUST_WORK_DAYS,
            GET_WORKER_MONTH_DAYS,
            GET_ALL_SYS_DIC,
            ADD_NEW_SYSTEM,

            GET_SYSTEM_INFO,//15
            MOD_SYSTEM,
            DEL_SYSTEM,
            GET_USER_INFO,
            MOD_PASSWORD,
            
            ADD_NEW_USER,//20
            MOD_USER_INFO,
            DEL_USER_INFO,
            GET_PRO_SYS_INFO,
            GET_PRO_FILE_INFO,
            
            DOWNLOAD_FILE,//25
            UPLOAD_FILE,
            DEL_FILE,
            GET_USER_SYS_INFO,
            GET_PRO_RATE_INFO,

            ENTRY_PRO_RATE,//30
            SAVE_SYS_CONFIG,
            GET_SYS_CONFIG,
            TEST_EMAIL,
            DEL_PRO_INFO,

            MOD_DEVELOPMENT, //35
            GET_NOTICE_INFO,
            ADD_NEW_NOTICE,
            DEL_NOTICE,
        };


        public enum CONFIG_KEYS
        {
            SEND_EMAIL_NAME,
            SEND_EMAIL_ADDR,
            SEND_EMAIL_PASSWORD,
            SEND_EMAIL_HOST,//smtp.hkcts.com
            SEND_PM_TIME,
            SEND_ALL_TIME,
            LAST_SEND_PM,
            LAST_SEND_ALL,
            SEND_FLAG, // 发送标志 0不允许发送邮件 1 允许发送邮件
        };
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

        public static string GetDataTableStr(DataTable dt)
        {
            if (dt == null)
            {
                return "";
            }
            string ret = "";
            foreach (DataColumn dc in dt.Columns)
            {
                ret += dc.ColumnName + "\t";
            }
            ret += "\r";
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    ret += dr[dc.ColumnName].ToString() + "\t";
                }
                ret += "\r";
            }
            return ret;
        }
        #region md5加密
        //md5加密
        public static string MD5(String argString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(argString);
            byte[] result = md5.ComputeHash(data);
            String strReturn = String.Empty;
            for (int i = 0; i < result.Length; i++)
                strReturn += result[i].ToString("x").PadLeft(2, '0');
            return strReturn;
        }
        #endregion
    }
}

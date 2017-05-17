using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CommonLib
{
    public class CommonDef
    {
        public static int MAX_MSG_LENGTH = 2048;

        public static int TIME_OUT = 10000;

        public static int ERROR = -1;
        public static int OK = 0;

        public enum FUN_NO
        {
            GET_HIS_DEPARTS,
            ADD_NEW_PROJECT,
            GET_CUR_PRO_NAMES,
            GET_PRO_INFO_FROM_NAME,
            GET_TRADES_INFO,
            MOD_PROJECT,
            QUERY_PRO_INFO,
            DIFF_TRADE,
            GET_HIS_WORKERS,
            QUERY_PRO_DAYS_INFO,
            GET_WORK_DAYS,
            SAVE_ADJUST_WORK_DAYS,
            GET_WORKER_MONTH_DAYS,
            GET_ALL_SYS_DIC,
            ADD_NEW_SYSTEM,
            GET_SYSTEM_INFO,
            MOD_SYSTEM,
            DEL_SYSTEM,
            GET_USER_INFO,
            MOD_PASSWORD,
            ADD_NEW_USER,
            MOD_USER_INFO,
            DEL_USER_INFO,
            GET_PRO_SYS_INFO,
            GET_PRO_FILE_INFO,
            DOWNLOAD_FILE,
            UPLOAD_FILE,
            DEL_FILE,
        };


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
    }
}

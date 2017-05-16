using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace ProjectsManageServer.DataBases
{
    class DataBaseManager
    {
        /// <summary>
        /// 数据库工具
        /// </summary>
        static DataBaseTool_SQLite3 dataBaseTool = new DataBaseTool_SQLite3();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        internal static bool InitDataBases()
        {
            bool ret = dataBaseTool.InitDataBase();
            if (ret) ret = T_DAYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_USER_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_PRO_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_PRO_SYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_SYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_TRADE_INFO.InitTable(dataBaseTool);
            return ret;
        }


        internal static List<string> GetHisDeparts()
        {
            List<string> retList =new List<string>();
            string sql = "select distinct DEMAND_DEPART from T_PRO_INFO where DEMAND_DEPART != ''";
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    retList.Add(dr[0].ToString());
                }
            }
            return retList;
        }

        internal static bool AddNewProject(string demandName, string depart, string date, string expectDate,
            string kinds, string stage, string state, string note, string testPersion, string businessPersion, 
            string remark, DataTable dtSysInfo)
        {
            string sql = "";
            string proId = Guid.NewGuid().ToString();
            List<string> values = new List<string>()
            {
                proId, demandName, depart, date, expectDate, "", kinds, stage, state, note, 
                testPersion, businessPersion, remark,
                DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            if (!dataBaseTool.AddInfo(T_PRO_INFO.TABLE_NAME, T_PRO_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            foreach (DataRow dr in dtSysInfo.Rows)
            {
                values = new List<string>() 
                {
                    proId, dr["SYS_ID"].ToString(), dr["IS_MAIN"].ToString(),
                    dr["ESTIMATE_DAYS"].ToString(), dr["REMARK"].ToString()
                };
                if (!dataBaseTool.AddInfo(T_PRO_SYS_INFO.TABLE_NAME, T_PRO_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static List<string> GetCurProNames(string sysId, bool showAll)
        {
            List<string> retList = new List<string>();
            string sql = "select distinct DEMAND_NAME from T_PRO_INFO where 1=1";
            if (!showAll)
            {
                sql += " and PRO_STATE != '完成'";
            }
           
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    retList.Add(dr[0].ToString());
                }
            }
            return retList;
        }

        internal static DataTable GetProInfoFromName(string select)
        {
            string sql = "select * from T_PRO_INFO where DEMAND_NAME = '{0}'";
            sql = string.Format(sql, select);
            return dataBaseTool.SelectFunc(sql);
        }

        internal static DataTable GetTradesInfo(string curProId, string sysId)
        {
            string sql = "select * from T_TRADE_INFO where DEMAND_ID = '{0}'";
            sql = string.Format(sql, curProId);
            if (sysId != "")
            {
                sql += string.Format(" and SYS_ID = '{0}'", sysId);
            }
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool ModProject(string curProId, string sysId, string demandName, string depart, string date, string expectDate,
            string kinds, string stage, string state, string finish, string note, string testPersion, string businessPersion,
            string remark, DataTable dtSysInfo, DataTable dtTrades)
        {
            string sql = "";
            List<string> values = new List<string>()
            {
                curProId, demandName, depart, date, expectDate, finish, kinds, stage, state, note, 
                testPersion, businessPersion, remark, DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            if (!dataBaseTool.ModInfo(T_PRO_INFO.TABLE_NAME, T_PRO_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            sql += string.Format("delete from T_PRO_SYS_INFO where DEMAND_ID = '{0}';", curProId);
            foreach (DataRow dr in dtSysInfo.Rows)
            {
                values = new List<string>() 
                    {
                        curProId, dr["SYS_ID"].ToString(), dr["IS_MAIN"].ToString(), dr["ESTIMATE_DAYS"].ToString(),
                        dr["REMARK"].ToString()
                    };
                if (!dataBaseTool.AddInfo(T_PRO_SYS_INFO.TABLE_NAME, T_PRO_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            if (sysId != "")
            {
                sql += string.Format("delete from T_TRADE_INFO where DEMAND_ID = '{0}' and SYS_ID = '{1}';", curProId, sysId);
                foreach (DataRow dr in dtTrades.Rows)
                {
                    values = new List<string>() 
                    {
                        curProId, sysId, dr["TRADE_CODE"].ToString(), dr["TRADE_NAME"].ToString(),
                        dr["IS_NEW"].ToString(), dr["FILE_NAME"].ToString(),
                        dr["WORKER"].ToString(), dr["WORKLOAD"].ToString(),
                        dr["REMARK"].ToString()
                    };
                    if (!dataBaseTool.AddInfo(T_TRADE_INFO.TABLE_NAME, T_TRADE_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                }
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable QueryProInfo(string stage, string state, string date)
        {
            string sql = "select * from T_PRO_INFO where 1=1";
            if (stage != "全部")
            {
                sql += string.Format(" and PRO_STAGE = '{0}'", stage);
            }
            if (state != "全部")
            {
                if (state == "全部未完成")
                {
                    sql += " and PRO_STATE != '完成'";
                }
                else
                {
                    sql += string.Format(" and PRO_STATE = '{0}'", state);
                }
            }
            if (date != "")
            {
                sql += string.Format(" and DEMAND_DATE like '{0}%'", date);
            }
            return dataBaseTool.SelectFunc(sql);
        }

        internal static List<string> DiffTrade(string proID, string sysId, string tradeNo)
        {
            List<string> difDemandNames = new List<string>();
            string sql = "select DISTINCT DEMAND_ID from T_TRADE_INFO where DEMAND_ID in ";
            sql += string.Format("(select DEMAND_ID from T_PRO_INFO where DEMAND_ID != '{0}' and PRO_STATE != '完成')", proID);
            sql += string.Format(" and TRADE_CODE = '{0}' and SYS_ID = '{1}'", tradeNo, sysId);
            sql = "select DEMAND_NAME from T_PRO_INFO where DEMAND_ID in (" + sql + ")";
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return null;
            }
            foreach (DataRow dr in dt.Rows)
            {
                difDemandNames.Add(dr[0].ToString());
            }
            return difDemandNames;
        }

        internal static List<string> GetHisWorkers(string sysId)
        {
            List<string> retList = new List<string>();
            string sql = "select distinct WORKER from T_TRADE_INFO where WORKER != '' and SYS_ID = '{0}'";
            sql = string.Format(sql, sysId);
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    retList.Add(dr[0].ToString());
                }
            }
            return retList;
        }

        internal static DataTable QueryProDaysInfo(string worker, string yearMonth)
        {
            string sql = "select t1.DEMAND_ID, t1.DEMAND_NAME, t1.DEMAND_DATE, t1.EXPECT_DATE, t1.FINISH_DATE, ";
            sql += "t1.PRO_STATE, t1.ESTIMATE_DAYS, sum(t2.WORKLOAD) as CURRENT_DAYS, ";
            sql += "sum(case when t2.WORKER = '{0}' then t2.WORKLOAD else 0 end) as PERSON_DAYS, ";
            sql += "t1.REMARK from T_PRO_INFO t1, T_TRADE_INFO t2 where t1.DEMAND_ID = t2.DEMAND_ID ";
            sql += "and t1.DEMAND_DATE < '{1}' and (t1.FINISH_DATE > '{2}' or t1.FINISH_DATE = '') group by t1.DEMAND_ID";
            sql = string.Format(sql, worker, yearMonth + "99", yearMonth + "00");
            return dataBaseTool.SelectFunc(sql);
        }

        internal static DataTable GetWorkDays(string curQueryWorker, string demandId)
        {
            string sql = "select * from T_DAYS_INFO where DEMAND_ID = '{0}' and WORKER = '{1}'";
            sql = string.Format(sql, demandId, curQueryWorker);
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool SaveAdjustWorkDays(string demandId, string worker, DataTable dataTable)
        {
            string sql = string.Format("delete from T_DAYS_INFO where DEMAND_ID = '{0}' and WORKER = '{1}';", demandId, worker);
            foreach (DataRow dr in dataTable.Rows)
            {
                List<string> values = new List<string>() { demandId, worker, dr["MONTH"].ToString(), dr["WORKLOAD"].ToString() };
                if (!dataBaseTool.AddInfo("T_DAYS_INFO", T_DAYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(), values, ref sql))
                {
                    return false;
                }
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static string GetWorkerMonthDays(string curQueryWorker, string curQueryDate)
        {
            string sql = "select sum(WORKLOAD) from T_DAYS_INFO where WORKER = '{0}' and MONTH = '{1}' group by WORKER";
            sql = string.Format(sql, curQueryWorker, curQueryDate);
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            { 
                return "";
            }
            if (dt.Rows.Count == 0)
            {
                return "0";
            }
            return dt.Rows[0][0].ToString();
        }

        internal static Dictionary<string, string> GetAllSysDic()
        {
            Dictionary<string, string> dicAllSys = new Dictionary<string, string>();
            string sql = "select * from T_SYS_INFO";
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return dicAllSys;
            }
            foreach (DataRow dr in dt.Rows)
            {
                dicAllSys.Add(dr["SYS_ID"].ToString(), dr["SYS_NAME"].ToString());
            }
            return dicAllSys;
        }

        internal static bool AddNewSystem(string sysName, string manage1,
            string manage2, string remark)
        {
            string guid = Guid.NewGuid().ToString();
            List<string> values = new List<string>() { guid, sysName, manage1, manage2, remark };
            string sql = "";
            if (!dataBaseTool.AddInfo(T_SYS_INFO.TABLE_NAME, T_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            string selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'", manage1);
            DataTable dt = dataBaseTool.SelectFunc(selectSql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                values = new List<string>() { manage1, "111111", guid, "焦作中旅银行股份有限公司", "项目经理", "" };
                if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'", manage2);
            dt = dataBaseTool.SelectFunc(selectSql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                values = new List<string>() { manage1, "111111", guid, "焦作中旅银行股份有限公司", "项目经理", "" };
                if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable GetSystemInfo(string sysId = "")
        {
            string sql = "select * from T_SYS_INFO";
            if (sysId != "")
            {
                sql += string.Format(" where SYS_ID = '{0}'", sysId);
            }
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool ModSystem(string sysId, string sysName, string uesr1, string user2, string remark)
        {
            List<string> values = new List<string>() { sysId, sysName, uesr1, user2, remark };
            string sql = "";
            if (!dataBaseTool.ModInfo(T_SYS_INFO.TABLE_NAME, T_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool DelSystem(string sysId)
        {
            string countSql = string.Format("select count(*) from T_PRO_SYS_INFO where SYS_ID = '{0}'", sysId);
            DataTable dt = dataBaseTool.SelectFunc(countSql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return false;
            }
            countSql = string.Format("select count(*) from T_USER_INFO where SYS_ID = '{0}'", sysId);
            dt = dataBaseTool.SelectFunc(countSql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return false;
            }
            string sql = string.Format("delete from T_SYS_INFO where SYS_ID = '{0}'", sysId);
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable GetUserInfo(string userName = "")
        {
            string sql = "select t1.*, t2.SYS_NAME from T_USER_INFO t1, T_SYS_INFO t2 where t1.SYS_ID = t2.SYS_ID";
            if (userName != "")
            {
                sql += string.Format(" and t1.USER_NAME = '{0}'", userName);
            }
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool ModPassword(string userName, string password)
        {
            string sql = "update T_USER_INFO set USER_PSW = '{0}' where USER_NAME = '{1}'";
            sql = string.Format(sql, password, userName);
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool AddNewUser(string userName, string sysId, string role, string company, string remark)
        {
            List<string> values = new List<string>() { userName, "111111", sysId, company, role, remark };
            string sql = "";
            if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }


        internal static bool ModUserInfo(string userName, string psw, string sysId, string role,
            string company, string remark)
        {
            List<string> values = new List<string>() { userName, psw, sysId, company, role, remark };
            string sql = "";
            if (!dataBaseTool.ModInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable GetProSystemInfo(string curProId)
        {
            string sql = "select t1.*, t2.*, case when sum(t3.WORKLOAD) is null then 0 else sum(t3.WORKLOAD) end as USED_DAYS from T_PRO_SYS_INFO t1, T_SYS_INFO t2 ";
            sql += "LEFT JOIN T_TRADE_INFO t3 on t1.DEMAND_ID = t3.DEMAND_ID and t1.SYS_ID = t3.SYS_ID ";
            sql += "where t1.SYS_ID = t2.SYS_ID ";
            sql += string.Format("and t1.DEMAND_ID = '{0}' group by t1.DEMAND_ID, t1.SYS_ID", curProId);
            return dataBaseTool.SelectFunc(sql);
        }
    }
}

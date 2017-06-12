using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using CommonLib;

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
            if (ret) ret = T_CONFIG_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_DAYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_USER_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_USER_SYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_PRO_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_PRO_RATE.InitTable(dataBaseTool);
            if (ret) ret = T_PRO_SYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_SYS_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_TRADE_INFO.InitTable(dataBaseTool);
            if (ret) ret = T_NOTICE_RECORDS.InitTable(dataBaseTool);
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

        static string PRO_FILE_PATH = "projects/";

        internal static bool AddNewProject(string demandName, string depart, string date, string expectDate,
            string kinds, string stage, string state, string note, string finishDate, string testPersion, string businessPersion,
            string remark, DataTable dtSysInfo)
        {
            string sql = "";
            string proId = Guid.NewGuid().ToString();
            List<string> values = new List<string>()
            {
                proId, demandName, depart, date, expectDate, finishDate, kinds, stage, state, note, 
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
            string curFilePath = PRO_FILE_PATH + date + "_" + demandName;
            if (!Directory.Exists(curFilePath))
            {
                Directory.CreateDirectory(curFilePath);
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static Dictionary<string, string> GetCurProNames(string sysId, bool showAll)
        {
            Dictionary<string, string> retList = new Dictionary<string, string>();
            string sql = "select DEMAND_ID, DEMAND_NAME from T_PRO_INFO where 1=1 ";
            if (sysId != "")
            {
                sql += string.Format("and DEMAND_ID in (select distinct DEMAND_ID from T_PRO_SYS_INFO where SYS_ID = '{0}')", sysId);
            }
            if (!showAll)
            {
                sql += " and PRO_STATE != '完成'";
            }
           
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    retList.Add(dr[0].ToString(), dr[1].ToString());
                }
            }
            return retList;
        }

        internal static DataTable GetProInfo(string select)
        {
            string sql = "select * from T_PRO_INFO where DEMAND_ID = '{0}'";
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
            string remark, DataTable dtSysInfo)
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
            string curFilePath = PRO_FILE_PATH + date + "_" + demandName;
            if (!Directory.Exists(curFilePath))
            {
                Directory.CreateDirectory(curFilePath);
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable QueryProInfo(string stage, string state, string date, string sysId)
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
            if (sysId != "")
            {
                sql += string.Format(" and DEMAND_ID in (select distinct DEMAND_ID from T_PRO_SYS_INFO where SYS_ID = '{0}')", sysId);
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

        internal static DataTable QueryProDaysInfo(string sysId, string worker, string yearMonth)
        {
            string sql = "select t1.DEMAND_ID, t1.DEMAND_NAME, t1.DEMAND_DATE, t1.EXPECT_DATE, t1.FINISH_DATE, ";
            sql += "t1.PRO_STATE, t3.ESTIMATE_DAYS, sum(t2.WORKLOAD) as CURRENT_DAYS, ";
            sql += "sum(case when t2.WORKER = '{0}' then t2.WORKLOAD else 0 end) as PERSON_DAYS, ";
            sql += "t1.REMARK from T_PRO_INFO t1, T_TRADE_INFO t2, T_PRO_SYS_INFO t3 where ";
            sql += "t1.DEMAND_ID = t2.DEMAND_ID and t1.DEMAND_ID = t3.DEMAND_ID and t3.SYS_ID = t2.SYS_ID ";
            sql += "and t2.SYS_ID = '{1}' ";
            sql += "and t1.DEMAND_DATE < '{2}' and (t1.FINISH_DATE > '{3}' or t1.FINISH_DATE = '') group by t1.DEMAND_ID";
            sql = string.Format(sql, worker, sysId, yearMonth + "99", yearMonth + "00");
            return dataBaseTool.SelectFunc(sql);
        }

        internal static DataTable GetWorkDays(string sysId, string curQueryWorker, string demandId)
        {
            string sql = "select * from T_DAYS_INFO where SYS_ID = '{0}' and DEMAND_ID = '{1}' and WORKER = '{2}'";
            sql = string.Format(sql, sysId, demandId, curQueryWorker);
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool SaveAdjustWorkDays(string demandId, string sysId, string worker, DataTable dataTable)
        {
            string sql = string.Format("delete from T_DAYS_INFO where DEMAND_ID = '{0}' and SYS_ID = '{1}' and WORKER = '{2}';", 
                demandId, sysId, worker);
            foreach (DataRow dr in dataTable.Rows)
            {
                List<string> values = new List<string>() { demandId, sysId, worker, dr["MONTH"].ToString(), dr["WORKLOAD"].ToString() };
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
            if (manage1 != "")
            {
                string selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'",
                manage1);
                DataTable dt = dataBaseTool.SelectFunc(selectSql);
                if (dt == null)
                {
                    return false;
                }
                if (dt.Rows[0][0].ToString() == "0")
                {
                    values = new List<string>() { manage1, CommonDef.MD5("111111"), "", "焦作中旅银行股份有限公司", "项目经理", "" };
                    if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                }
                values = new List<string> { manage1, guid};
                if (!dataBaseTool.AddInfo(T_USER_SYS_INFO.TABLE_NAME, T_USER_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            if (manage2 != "")
            {
                string selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'",
                manage2);
                DataTable dt = dataBaseTool.SelectFunc(selectSql);
                if (dt == null)
                {
                    return false;
                }
                if (dt.Rows[0][0].ToString() == "0")
                {
                    values = new List<string>() { manage2, CommonDef.MD5("111111"), "", "焦作中旅银行股份有限公司", "项目经理", "" };
                    if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                }
                values = new List<string> { manage2, guid };
                if (!dataBaseTool.AddInfo(T_USER_SYS_INFO.TABLE_NAME, T_USER_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
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

        internal static bool ModSystem(string sysId, string sysName, string manage1, string manage2, string remark)
        {
            List<string> values = new List<string>() { sysId, sysName, manage1, manage2, remark };
            string sql = "";
            if (!dataBaseTool.ModInfo(T_SYS_INFO.TABLE_NAME, T_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            if (manage1 != "")
            {
                string selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'",
                manage1);
                DataTable dt = dataBaseTool.SelectFunc(selectSql);
                if (dt == null)
                {
                    return false;
                }
                if (dt.Rows[0][0].ToString() == "0")
                {
                    values = new List<string>() { manage1, CommonDef.MD5("111111"), "", "焦作中旅银行股份有限公司", "项目经理", "" };
                    if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                    values = new List<string> { manage1, sysId };
                    if (!dataBaseTool.AddInfo(T_USER_SYS_INFO.TABLE_NAME, T_USER_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                }
            }
            if (manage2 != "")
            {
                string selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'",
                    manage2);
                DataTable dt = dataBaseTool.SelectFunc(selectSql);
                if (dt == null)
                {
                    return false;
                }
                if (dt.Rows[0][0].ToString() == "0")
                {
                    values = new List<string>() { manage2, CommonDef.MD5("111111"), "", "焦作中旅银行股份有限公司", "项目经理", "" };
                    if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                    values = new List<string> { manage2, sysId };
                    if (!dataBaseTool.AddInfo(T_USER_SYS_INFO.TABLE_NAME, T_USER_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                }
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

        internal static DataTable GetUserInfo(string userName = "", string userSys = "")
        {
            string sql = "select * from T_USER_INFO where 1=1";
            if (userName != "")
            {
                sql += string.Format(" and USER_NAME = '{0}'", userName);
            }
            if (userSys != "" && userSys != "0")
            {
                if (userSys == "-1")
                {
                    sql += string.Format(" and USER_NAME not in (select distinct USER_NAME from T_USER_SYS_INFO)");
                }
                else
                {
                    sql += string.Format(" and USER_NAME in (select USER_NAME from T_USER_SYS_INFO where SYS_ID = '{0}')", userSys);
                }
            }
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool ModPassword(string userName, string password)
        {
            string sql = "update T_USER_INFO set USER_PSW = '{0}' where USER_NAME = '{1}'";
            sql = string.Format(sql, password, userName);
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool AddNewUser(string userName, string email, string role, string company, string remark, string sysInfo)
        {
            string selectSql = string.Format("select count(*) from T_USER_INFO where USER_NAME = '{0}'",
                userName);
            DataTable dt = dataBaseTool.SelectFunc(selectSql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return false;
            }
            List<string> values = new List<string>() { userName, CommonDef.MD5("111111"), email, company, role, remark };
            string sql = "";
            if (!dataBaseTool.AddInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            sql += string.Format("delete from T_USER_SYS_INFO where USER_NAME = '{0}';", userName);
            string[] sysInfos = sysInfo.Split('\r');
            foreach (string sys in sysInfos)
            {
                if (sys == "")
                {
                    continue;
                }
                values = new List<string>() { userName, sys };
                if (!dataBaseTool.AddInfo(T_USER_SYS_INFO.TABLE_NAME, T_USER_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            return dataBaseTool.ActionFunc(sql);
        }


        internal static bool ModUserInfo(string userName, string email, string psw, string role,
            string company, string remark, string sysInfo)
        {
            List<string> values = new List<string>() { userName, psw, email, company, role, remark };
            string sql = "";
            if (!dataBaseTool.ModInfo(T_USER_INFO.TABLE_NAME, T_USER_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            sql += string.Format("delete from T_USER_SYS_INFO where USER_NAME = '{0}';", userName);
            string[] sysInfos = sysInfo.Split('\r');
            foreach (string sys in sysInfos)
            {
                if (sys == "")
                {
                    continue;
                }
                values = new List<string>() { userName, sys };
                if (!dataBaseTool.AddInfo(T_USER_SYS_INFO.TABLE_NAME, T_USER_SYS_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
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

        internal static bool DelUserInfo(string userName)
        {
            string sql = string.Format("delete from T_USER_INFO where USER_NAME = '{0}';", userName);
            sql += string.Format("delete from T_USER_SYS_INFO where USER_NAME = '{0}'", userName);
            return dataBaseTool.ActionFunc(sql);
        }


        internal static DataTable GetProFileInfo(string proId)
        {
            string sql = "select * from T_PRO_INFO where DEMAND_ID = '{0}'";
            sql = string.Format(sql, proId);
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return null;
            }
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            DataTable dtRet = new DataTable();
            dtRet.Columns.Add("FILE_NAME");
            dtRet.Columns.Add("FILE_TIME");
            string curFilePath = PRO_FILE_PATH + dt.Rows[0]["DEMAND_DATE"].ToString() + "_" + dt.Rows[0]["DEMAND_NAME"].ToString();
            if (!Directory.Exists(curFilePath))
            {
                Directory.CreateDirectory(curFilePath);
            }
            else
            {
                string[] files = Directory.GetFiles(curFilePath);
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    string time = File.GetLastWriteTime(file).ToString("yyyyMMddHHmmss");
                    dtRet.Rows.Add(new string[]{fileName, time});
                }
            }
            return dtRet;
        }

        internal static DataTable GetUserSysInfo(string userName = "")
        {
            string sql = string.Format("select t1.*, t2.* from T_USER_SYS_INFO t1, T_SYS_INFO t2 where t1.SYS_ID = t2.SYS_ID");
            if (userName != "")
            {
                sql += string.Format(" and t1.USER_NAME = '{0}'", userName);
            }
            return dataBaseTool.SelectFunc(sql);
        }

        internal static DataTable GetProRateInfo(string proId)
        {
            string sql = string.Format("select t1.*,t2.* from T_PRO_RATE t1, T_SYS_INFO t2 where t1.SYS_ID = t2.SYS_ID and t1.DEMAND_ID = '{0}'", proId);
            sql += " order by t1.DATE desc";
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool EntryProRate(string proId, string sysId, string date, string note, string rate,
            string explain, string problem)
        {
            string selectSql = string.Format("select count(*) from T_PRO_RATE where DEMAND_ID = '{0}' and SYS_ID = '{1}' and DATE = '{2}'", 
                proId, sysId, date);
            DataTable selectDt = dataBaseTool.SelectFunc(selectSql);
            if (selectDt == null)
            {
                return false;
            }
            string sql = "";
            List<string> values = new List<string>() { proId, sysId, date, rate, explain, problem };
            if (selectDt.Rows[0][0].ToString() == "0")
            {
                if (!dataBaseTool.AddInfo(T_PRO_RATE.TABLE_NAME, T_PRO_RATE.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql))
                {
                    return false;
                }
            }
            else
            {
                if (!dataBaseTool.ModInfo(T_PRO_RATE.TABLE_NAME, T_PRO_RATE.DIC_TABLE_COLUMS.Keys.ToList(),
                    values, ref sql, new Dictionary<string, string>() 
                    {
                        {"DEMAND_ID", proId }, {"SYS_ID", sysId }, {"DATE", date }
                    }))
                {
                    return false;
                }
            }
            sql += string.Format("update T_PRO_INFO set PRO_NOTE = '{0}' where DEMAND_ID = '{1}';", note, proId);
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool SaveSysConfig(string senderName, string senderEmail, string senderPasword,
            string sendServer, string sendPMDate, string sendAllDate,
            string sendPMTime, string sendAllTime, string autoSendFlag)
        {
            string sql = "select * from T_CONFIG_INFO";
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return false;
            }
            sql = "";
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_NAME, senderName, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_ADDR, senderEmail, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_PASSWORD, senderPasword, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_HOST, sendServer, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_PM, sendPMDate, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_ALL, sendAllDate, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_PM_TIME, sendPMTime, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_ALL_TIME, sendAllTime, ref sql))
            {
                return false;
            }
            if (!T_CONFIG_INFO.SaveConfig(dt, CommonDef.CONFIG_KEYS.SEND_FLAG, autoSendFlag, ref sql))
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool SaveSysConfig(DataTable dt, CommonDef.CONFIG_KEYS key, string date)
        {
            string sql = "";
            if (!T_CONFIG_INFO.SaveConfig(dt, key, date, ref sql))
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable GetSysConfig()
        {
            string sql = "select * from T_CONFIG_INFO";
            return dataBaseTool.SelectFunc(sql);
        }


        internal static bool DelProject(string proId)
        {
            DataTable dt = GetProInfo(proId);
            if (dt == null)
            {
                return false;
            }
            string curFilePath = PRO_FILE_PATH + dt.Rows[0]["DEMAND_DATE"].ToString() + "_" + dt.Rows[0]["DEMAND_NAME"].ToString();
            if (Directory.Exists(curFilePath))
            {
                Directory.Delete(curFilePath, true);
            }
            string sql = string.Format("delete from T_PRO_INFO where DEMAND_ID = '{0}';", proId);
            sql += string.Format("delete from T_PRO_RATE where DEMAND_ID = '{0}';", proId);
            sql += string.Format("delete from T_PRO_SYS_INFO where DEMAND_ID = '{0}';", proId);
            sql += string.Format("delete from T_DAYS_INFO where DEMAND_ID = '{0}';", proId);
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool ModDevelopment(string curProId, string sysId, DataTable dtTrades)
        {
            string sql = string.Format("delete from T_TRADE_INFO where DEMAND_ID = '{0}' and SYS_ID = '{1}';",
                curProId, sysId);
            foreach (DataRow dr in dtTrades.Rows)
            {
                List<string> values = new List<string>() 
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
            return dataBaseTool.ActionFunc(sql);
        }
#if false
        /// <summary>
        /// 统计今年：新增数目、子单数目、上线、子单数目
        /// </summary>
        /// <returns></returns>
        internal static List<string> ProInfoCount(int year)
        {
            List<string> listData = new List<string>();
            string sql = string.Format("select count(*) from T_PRO_INFO where DEMAND_DATE like '{0}%'", year.ToString());
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return null;
            }
            listData.Add(dt.Rows[0][0].ToString()); //新增
            sql = "select count(*) from T_PRO_SYS_INFO where DEMAND_ID in ";
            sql += "(select DEMAND_ID from T_PRO_INFO where DEMAND_DATE like '{0}%')";
            sql = string.Format(sql, year.ToString());
            dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return null;
            }
            listData.Add(dt.Rows[0][0].ToString()); //新增子单数目
            sql = string.Format("select count(*) from T_PRO_INFO where FINISH_DATE like '{0}%'", year.ToString());
            dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return null;
            }
            listData.Add(dt.Rows[0][0].ToString()); //上线数目
            sql = "select count(*) from T_PRO_SYS_INFO where DEMAND_ID in ";
            sql += "(select DEMAND_ID from T_PRO_INFO where FINISH_DATE like '{0}%')";
            sql = string.Format(sql, year.ToString());
            dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return null;
            }
            listData.Add(dt.Rows[0][0].ToString()); //上线子单数目
            return listData;
        }
#endif
        /// <summary>
        /// 统计今年：系统名称、负责人、新增主单数、新增子单数目、上线主单数、上线子单数、在建主单数、在建子单数
        /// </summary>
        /// <returns></returns>
        internal static DataTable ProInfoCount(int year)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SYS_ID");
            dt.Columns.Add("SYS_NAME");
            dt.Columns.Add("MANAGERS");
            dt.Columns.Add("NEW_DEMAND", typeof(int));
            dt.Columns.Add("NEW_DEMAND_SYS", typeof(int));
            dt.Columns.Add("FINISH_DEMAND", typeof(int));
            dt.Columns.Add("FINISH_DEMAND_SYS", typeof(int));
            dt.Columns.Add("UNFINISH_DEMAND", typeof(int));
            dt.Columns.Add("UNFINISH_DEMAND_SYS", typeof(int));

            string sql = "select t1.*, SUM(case when t2.IS_MAIN = '是' then 1 else 0 end) as NEW_DEMAND, count(t2.DEMAND_ID) as NEW_DEMAND_SYS ";
            sql += "from T_SYS_INFO t1, T_PRO_SYS_INFO t2 ";
            sql += "where t1.SYS_ID = t2.SYS_ID and t2.DEMAND_ID in ";
            sql += "(select DEMAND_ID from T_PRO_INFO where DEMAND_DATE like '2017%') group by t1.SYS_ID";
            DataTable dtNew = dataBaseTool.SelectFunc(sql);
            if (dtNew == null)
            {
                return null;
            }
            sql = "select t1.*, SUM(case when t2.IS_MAIN = '是' then 1 else 0 end) as FINISH_DEMAND, count(t2.DEMAND_ID) as FINISH_DEMAND_SYS ";
            sql += "from T_SYS_INFO t1, T_PRO_SYS_INFO t2 ";
            sql += "where t1.SYS_ID = t2.SYS_ID and t2.DEMAND_ID in ";
            sql += "(select DEMAND_ID from T_PRO_INFO where FINISH_DATE like '2017%') group by t1.SYS_ID";
            DataTable dtFinish = dataBaseTool.SelectFunc(sql);
            if (dtFinish == null)
            {
                return null;
            }
            sql = "select t1.*, SUM(case when t2.IS_MAIN = '是' then 1 else 0 end) as UNFINISH_DEMAND, count(t2.DEMAND_ID) as UNFINISH_DEMAND_SYS ";
            sql += "from T_SYS_INFO t1, T_PRO_SYS_INFO t2 ";
            sql += "where t1.SYS_ID = t2.SYS_ID and t2.DEMAND_ID in ";
            sql += "(select DEMAND_ID from T_PRO_INFO where PRO_STATE != '完成') group by t1.SYS_ID";
            DataTable dtUnfinish = dataBaseTool.SelectFunc(sql);
            if (dtUnfinish == null)
            {
                return null;
            }
            List<string> addedSysId = new List<string>();
            foreach (DataRow dr in dtNew.Rows)
            {
                if (addedSysId.Contains(dr["SYS_ID"].ToString()))
                {
                    foreach (DataRow drAdded in dt.Rows)
                    {
                        if (dr["SYS_ID"].ToString() == drAdded["SYS_ID"].ToString())
                        {
                            drAdded["NEW_DEMAND"] = dr["NEW_DEMAND"].ToString();
                            drAdded["NEW_DEMAND_SYS"] = dr["NEW_DEMAND_SYS"].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    dt.Rows.Add(new string[] {
                        dr["SYS_ID"].ToString(), dr["SYS_NAME"].ToString(),
                        dr["USER_NAME1"].ToString() + "\r" + dr["USER_NAME2"].ToString(),
                        dr["NEW_DEMAND"].ToString(), dr["NEW_DEMAND_SYS"].ToString(),
                        "0","0","0","0"
                    });
                    addedSysId.Add(dr["SYS_ID"].ToString());
                }
            }
            foreach (DataRow dr in dtFinish.Rows)
            {
                if (addedSysId.Contains(dr["SYS_ID"].ToString()))
                {
                    foreach (DataRow drAdded in dt.Rows)
                    {
                        if (dr["SYS_ID"].ToString() == drAdded["SYS_ID"].ToString())
                        {
                            drAdded["FINISH_DEMAND"] = dr["FINISH_DEMAND"].ToString();
                            drAdded["FINISH_DEMAND_SYS"] = dr["FINISH_DEMAND_SYS"].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    dt.Rows.Add(new string[] {
                        dr["SYS_ID"].ToString(), dr["SYS_NAME"].ToString(),
                        dr["USER_NAME1"].ToString() + "\r" + dr["USER_NAME2"].ToString(), "0","0",
                        dr["FINISH_DEMAND"].ToString(), dr["FINISH_DEMAND_SYS"].ToString(),
                        "0","0"
                    });
                    addedSysId.Add(dr["SYS_ID"].ToString());
                }
            }
            foreach (DataRow dr in dtUnfinish.Rows)
            {
                if (addedSysId.Contains(dr["SYS_ID"].ToString()))
                {
                    foreach (DataRow drAdded in dt.Rows)
                    {
                        if (dr["SYS_ID"].ToString() == drAdded["SYS_ID"].ToString())
                        {
                            drAdded["UNFINISH_DEMAND"] = dr["UNFINISH_DEMAND"].ToString();
                            drAdded["UNFINISH_DEMAND_SYS"] = dr["UNFINISH_DEMAND_SYS"].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    dt.Rows.Add(new string[] {
                        dr["SYS_ID"].ToString(), dr["SYS_NAME"].ToString(),
                        dr["USER_NAME1"].ToString() + "\r" + dr["USER_NAME2"].ToString(),
                        "0","0","0","0",
                        dr["UNFINISH_DEMAND"].ToString(), dr["UNFINISH_DEMAND_SYS"].ToString()
                    });
                    addedSysId.Add(dr["SYS_ID"].ToString());
                }
            } 
            DataView dataView = dt.DefaultView;
            dataView.Sort = "FINISH_DEMAND_SYS desc";
            return dataView.ToTable();  
        }

        internal static DataTable QueryNoticeInfo(string startDate, string endDate)
        {
            string sql = "select * from T_NOTICE_RECORDS where NOTICE_DATE >= '{0}' and NOTICE_DATE <= '{1}'";
            sql += " order by LVL asc, NOTICE_DATE desc, NOTICE_TIME desc";
            sql = string.Format(sql, startDate, endDate);
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool AddNewNotice(string name, string title, string content, int lvl)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            List<string> values = new List<string>() { Guid.NewGuid().ToString(), date,
                time, name, title, content, lvl.ToString()};
            string sql = "";
            if (!dataBaseTool.AddInfo(T_NOTICE_RECORDS.TABLE_NAME, T_NOTICE_RECORDS.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static bool DelNotice(string id)
        {
            string sql = "delete from T_NOTICE_RECORDS where NOTICE_ID = '{0}'";
            sql = string.Format(sql, id);
            return dataBaseTool.ActionFunc(sql);
        }
    }
}

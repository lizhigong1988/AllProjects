using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace 项目管理.DataBases
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
            if (ret) ret = T_PRO_INFO.InitTable(dataBaseTool);
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

        internal static System.Collections.IEnumerable GetHisSystem()
        {
            List<string> retList = new List<string>();
            string sql = "select distinct SYSTEM from T_PRO_INFO where SYSTEM != ''";
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

        internal static DataTable GetNewTradeTable()
        {
            DataTable dt = new DataTable();
            foreach (var dic in T_TRADE_INFO.DIC_TABLE_COLUMS)
            {
                dt.Columns.Add(dic.Key);
            }
            return dt;
        }

        internal static bool AddNewProject(string demandName, string depart, string date, string expectDate,
            string kinds, string stage, string state, string days, string note, string sys, string relationSys, 
            string firstPersion, string secondPersion, string testPersion, string businessPersion, 
            string remark, DataTable dtTrades)
        {
            string sql = "";
            string proId = Guid.NewGuid().ToString();
            List<string> values = new List<string>()
            {
                proId, demandName, depart, date, expectDate, kinds, stage, state, days, note, 
                sys, relationSys, firstPersion, secondPersion, testPersion, businessPersion, remark
            };
            if (!dataBaseTool.AddInfo(T_PRO_INFO.TABLE_NAME, T_PRO_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            foreach (DataRow dr in dtTrades.Rows)
            {
                values = new List<string>() 
                {
                    proId, dr["TRADE_CODE"].ToString(), dr["TRADE_NAME"].ToString(),
                    dr["IS_NEW"].ToString(), dr["SERVER_NODE"].ToString(), dr["FILE_NAME"].ToString(),
                    dr["TRADE_MENU"].ToString(), dr["WORKER"].ToString(), dr["WORKLOAD"].ToString(),
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

        internal static List<string> GetCurProNames()
        {
            List<string> retList = new List<string>();
            string sql = "select distinct DEMAND_NAME from T_PRO_INFO where PRO_STATE != '完成'";
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

        internal static DataTable GetTradesInfo(string curProId)
        {
            string sql = "select * from T_TRADE_INFO where DEMAND_ID = '{0}'";
            sql = string.Format(sql, curProId);
            return dataBaseTool.SelectFunc(sql);
        }

        internal static bool ModProject(string curProId, string demandName, string depart, string date, string expectDate,
            string kinds, string stage, string state, string days, string note, string sys, string relationSys,
            string firstPersion, string secondPersion, string testPersion, string businessPersion,
            string remark, DataTable dtTrades)
        {
            string sql = "";
            List<string> values = new List<string>()
            {
                curProId, demandName, depart, date, expectDate, kinds, stage, state, days, note, 
                sys, relationSys, firstPersion, secondPersion, testPersion, businessPersion, remark
            };
            if (!dataBaseTool.ModInfo(T_PRO_INFO.TABLE_NAME, T_PRO_INFO.DIC_TABLE_COLUMS.Keys.ToList(),
                values, ref sql))
            {
                return false;
            }
            sql += string.Format("delete from T_TRADE_INFO where DEMAND_ID = '{0}';", curProId);
            foreach (DataRow dr in dtTrades.Rows)
            {
                values = new List<string>() 
                {
                    curProId, dr["TRADE_CODE"].ToString(), dr["TRADE_NAME"].ToString(),
                    dr["IS_NEW"].ToString(), dr["SERVER_NODE"].ToString(), dr["FILE_NAME"].ToString(),
                    dr["TRADE_MENU"].ToString(), dr["WORKER"].ToString(), dr["WORKLOAD"].ToString(),
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

        internal static bool DiffTrade(string proID, string tradeNo, ref bool res)
        {
            string sql = "select COUNT(*) from T_TRADE_INFO where DEMAND_ID in ";
            sql += string.Format("(select DEMAND_ID from T_PRO_INFO where DEMAND_ID != '{0}' and PRO_STATE != '完成')", proID);
            sql += string.Format(" and TRADE_CODE = '{0}'", tradeNo);
            DataTable dt = dataBaseTool.SelectFunc(sql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                res = false;
            }
            else
            {
                res = true;
            }
            return true;
        }
    }
}

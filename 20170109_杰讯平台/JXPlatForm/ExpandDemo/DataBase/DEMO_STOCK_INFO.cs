using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_STOCK_INFO
    {
        internal static string TABLE_NAME = "DEMO_STOCK_INFO";

        internal enum TABLE_COLUMS
        {
            STOCK_ID,           //订货单ID
            ORDER_ID,           //订单ID
            STOCK_DATE,         //订货日期
            TOTAL_AMT,          //总金额
            REMARK1,             //备注1
            REMARK2,             //备注2

            TOTAL_COUNT               
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(36) UNIQUE NOT NULL",              //订货单ID
            "VARCHAR(36)",              //订单ID
            "INTEGER",              //订货日期
            "DECIMAL",              //总金额
            "VARCHAR(64)",              //备注1
            "VARCHAR(64)",              //备注2
        };

        internal static List<string> LIST_COLUMS;


        internal static bool InitTable()
        {
            LIST_COLUMS = new List<string>();
            for (int i = 0; i < (int)TABLE_COLUMS.TOTAL_COUNT; i++)
            {
                LIST_COLUMS.Add(((TABLE_COLUMS)i).ToString());
            }
            if (!DataTableTool.TableExist(TABLE_NAME))
            {
                string sql = "";
                if (!DataTableTool.CreateDataTable(TABLE_NAME, LIST_COLUMS, ELEMNT_RULES, ref sql))
                {
                    return false;
                }
                if (!DataTableTool.ActionTableFunc(sql))
                {
                    return false;
                }
            }
            return true;
        }

        internal static CommonDef.COM_RET AddStockInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            List<string> values = new List<string>()
            {
                elems[0], elems[1], elems[4], elems[5], elems[6], elems[7]
            };
            if (values.Count < (int)TABLE_COLUMS.TOTAL_COUNT)
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            values.RemoveRange((int)TABLE_COLUMS.TOTAL_COUNT, values.Count - (int)TABLE_COLUMS.TOTAL_COUNT);
            string sql = "";
            if (!DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            if (!DEMO_STOCK_DETAIL.AddDetailInfo(elems[2], elems[3], ref sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET GetStockInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = "select * from DEMO_STOCK_INFO where 1=1";
            if (elems[0] != "")
            {
                sql += string.Format(" and STOCK_DATE >= {0}", elems[0]);
            }
            if (elems[1] != "")
            {
                sql += string.Format(" and STOCK_DATE <= {0}", elems[1]);
            }
            sql += " order by STOCK_DATE desc";
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET DelClientInfo(DataHelper dataHelper)
        {
            string clients = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            clients = clients.TrimEnd('\n').Replace("\n", "','");
            clients = "('" + clients + "');";
            string sql = "delete from DEMO_CLIENT_INFO where CLIENT_ID in " + clients;
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET DelStockInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = string.Format("delete from DEMO_STOCK_INFO where STOCK_ID = '{0}';", elems[0]);
            sql += string.Format("delete from DEMO_STOCK_DETAIL where STOCK_ID = '{0}';", elems[0]);
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_STOCK_PRICE_LIST
    {
        internal static string TABLE_NAME = "DEMO_STOCK_PRICE_LIST";

        internal enum TABLE_COLUMS
        {
            KIND,           //商品种类
            NAME,           //品名
            MODEL,          //型号
            PRICE,          //单价-分

            TOTAL_COUNT         
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(32)",              //品名
            "VARCHAR(32)",              //型号
            "VARCHAR(32)",              //尺寸
            "INTEGER",              //单价
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

        internal static CommonDef.COM_RET AddClientInfo(DataHelper dataHelper)
        {
            string select = "select Max(CLIENT_ID) from DEMO_CLIENT_INFO";
            DataTable selectDt = DataTableTool.SelectTableInfo(select);
            int index = 0;
            if (selectDt != null)
            {
                if (selectDt.Rows.Count > 0)
                {
                    if (selectDt.Rows[0][0].ToString() != "")
                    {
                        index = int.Parse(selectDt.Rows[0][0].ToString()) + 1;
                    }
                }
            }

            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');//0name,1tel
            List<string> values = new List<string>(elems);
            values.Insert(0, index.ToString());
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
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET ModClientInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');//0name,1tel
            List<string> values = new List<string>(elems);
            if (values.Count < (int)TABLE_COLUMS.TOTAL_COUNT)
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            values.RemoveRange((int)TABLE_COLUMS.TOTAL_COUNT, values.Count - (int)TABLE_COLUMS.TOTAL_COUNT);
            string sql = "";
            if (!DataTableTool.ModInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET GetClientInfo(DataHelper dataHelper)
        {
            string sql = "select * from DEMO_CLIENT_INFO order by CLIENT_ID desc";
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
    }
}

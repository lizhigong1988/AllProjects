using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_CLIENT_INFO
    {
        internal static string TABLE_NAME = "DEMO_CLIENT_INFO";

        internal enum TABLE_COLUMS
        {
            CLIENT_ID,
            CLIENT_NAME,
            CLIENT_TEL,
            CLIENT_ADDR,
            CLIENT_REMARK,
            TOTAL_COUNT
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "INTEGER UNIQUE NOT NULL",              //ID
            "VARCHAR(32) NOT NULL",              //客户名称
            "VARCHAR(20)",              //电话号码
            "VARCHAR(128)",              //地址
            "VARCHAR(128)",              //备注
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
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');//0name,1tel
            List<string> values = new List<string>(elems);
            string select = "select count(*) from DEMO_CLIENT_INFO where CLIENT_NAME = '{0}' and CLIENT_TEL = '{1}'";
            select = string.Format(select, elems[0], elems[1]);
            DataTable selectDt = DataTableTool.SelectTableInfo(select);
            if (selectDt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (selectDt.Rows[0][0].ToString() != "0")
            {
                return CommonDef.COM_RET.DATA_DUP;
            }

            select = "select Max(CLIENT_ID) from DEMO_CLIENT_INFO";
            selectDt = DataTableTool.SelectTableInfo(select);
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
            values.Insert(0, index.ToString());
            if (values.Count < (int)TABLE_COLUMS.TOTAL_COUNT)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            values.RemoveRange((int)TABLE_COLUMS.TOTAL_COUNT, values.Count - (int)TABLE_COLUMS.TOTAL_COUNT);
            string sql = "";
            if (!DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
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
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            string select = "select count(*) from DEMO_CLIENT_INFO where CLIENT_NAME = '{0}' and CLIENT_TEL = '{1}' and CLIENT_ID != '{2}'";
            select = string.Format(select, elems[1], elems[2], elems[0]);
            DataTable selectDt = DataTableTool.SelectTableInfo(select);
            if (selectDt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (selectDt.Rows[0][0].ToString() != "0")
            {
                return CommonDef.COM_RET.DATA_DUP;
            }
            values.RemoveRange((int)TABLE_COLUMS.TOTAL_COUNT, values.Count - (int)TABLE_COLUMS.TOTAL_COUNT);
            string sql = "";
            if (!DataTableTool.ModInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
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
            string select = "select count(*) from DEMO_ORDER_INFO where CLIENT_ID in {0}";
            select = string.Format(select, clients);
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return CommonDef.COM_RET.DATA_RELY;
            }
            string sql = "delete from DEMO_CLIENT_INFO where CLIENT_ID in " + clients;
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static bool AddClientSql(string name, string tel, string addr, ref string sql, out string clientId)
        {
            string select = "select Max(CLIENT_ID) from DEMO_CLIENT_INFO";
            DataTable selectDt = DataTableTool.SelectTableInfo(select);
            int index = 0;
            if (selectDt == null)
            {
                clientId = "0";
                return false;
            }
            if (selectDt.Rows.Count > 0)
            {
                if (selectDt.Rows[0][0].ToString() != "")
                {
                    index = int.Parse(selectDt.Rows[0][0].ToString()) + 1;
                }
            }
            clientId = index.ToString();
            List<string> values = new List<string>() {index.ToString(), name, tel, addr, "" };
            return DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql);
        }

        internal static bool ModClientSql(string name, string tel, string addr, ref string sql, string clientId)
        {
            List<string> values = new List<string>() { clientId, name, tel, addr};
            List<string> ModColums = new List<string>(LIST_COLUMS);
            ModColums.RemoveAt(ModColums.Count - 1);
            return DataTableTool.ModInfo(TABLE_NAME, ModColums, values, ref sql);
        }
    }
}

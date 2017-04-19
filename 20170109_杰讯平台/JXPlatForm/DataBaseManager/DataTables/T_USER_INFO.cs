using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.Data;

namespace DataBaseManager.DataTables
{
    class T_USER_INFO
    {
        internal static string TABLE_NAME = "T_USER_INFO";

        internal enum TABLE_COLUMS
        { 
            USER_CODE,
            USER_PSW,
            USER_NAME,
            USER_RULE,
            UESR_TEL,
            USER_ID,
            TOTAL_COUNT
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(32) NOT NULL PRIMARY KEY UNIQUE",  //用户名
            "VARCHAR(32) NOT NULL",              //用户密码
            "VARCHAR(32) NOT NULL UNIQUE",                     //姓名
            "INTEGER",                                  //角色ID
            "VARCHAR(16)",                              //电话号码
            "VARCHAR(20)",                              //身份证号
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

        internal static bool AddUserInfo(DataHelper dataHelper, ref string sql)
        {
            string defautPsw = CommonDef.DEFAUTL_PSW;
            List<string> values = new List<string>()
            {
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE),
                defautPsw,
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_NAME),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_TEL),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_ID),
            };
            if (!DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return false;
            }
            return true;
        }

        internal static bool ModUserInfo(DataHelper dataHelper, ref string sql)
        {
            string passwd = "";
            string select = "select USER_PSW from T_USER_INFO where USER_CODE = '{0}'";
            select = string.Format(select, dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE)); 
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            passwd = dt.Rows[0][0].ToString();
            List<string> values = new List<string>()
            {
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE),
                passwd,
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_NAME),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_TEL),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_ID),
            };
            if (!DataTableTool.ModInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return false;
            }
            return true;
        }

        internal static bool DelUserInfo(DataHelper dataHelper, ref string sql)
        {
            string users = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE).Replace("\t", "','");
            users = "('" + users + "');";
            sql += "delete from T_USER_INFO where USER_CODE in " + users;
            return true;
        }

        internal static bool ModUserPsw(DataHelper dataHelper, ref string sql)
        {
            List<string> column = new List<string>() { 
                TABLE_COLUMS.USER_CODE.ToString(),
                TABLE_COLUMS.USER_PSW.ToString() 
            };
            List<string> values = new List<string>() {
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_PSW)
            };
            if (!DataTableTool.ModInfo(TABLE_NAME, column, values, ref sql))
            {
                return false;
            }
            return true;
        }
    }
}

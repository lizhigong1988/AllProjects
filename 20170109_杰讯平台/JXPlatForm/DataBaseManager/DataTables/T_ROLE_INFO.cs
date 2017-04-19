using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.Data;

namespace DataBaseManager.DataTables
{
    class T_ROLE_INFO
    {
        internal static string TABLE_NAME = "T_ROLE_INFO";

        internal enum TABLE_COLUMS
        { 
            ROLE_ID,
            ROLE_NAME,
            REMARK,
            TOTAL_COUNT
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "INTEGER NOT NULL PRIMARY KEY UNIQUE",  //角色ID
            "VARCHAR(32) NOT NULL UNIQUE",              //角色名称
            "VARCHAR(64)",                              //备注
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

        internal static bool AddRoleInfo(DataHelper dataHelper, ref string sql)
        {
            List<string> values = new List<string>()
            {
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_NAME),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.REMARK),
            };
            if (!DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return false;
            }
            return true;
        }

        internal static bool DelRoleInfo(DataHelper dataHelper, ref string sql)
        {
            string roles = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID).Replace("\t", "','");
            roles = "('" + roles + "');";
            sql += "delete from T_ROLE_INFO where ROLE_ID in " + roles;
            return true;
        }

        internal static bool ModRoleInfo(DataHelper dataHelper, ref string sql)
        {
            List<string> values = new List<string>()
            {
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_NAME),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.REMARK),
            };
            if (!DataTableTool.ModInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return false;
            }
            return true;
        }
    }
}

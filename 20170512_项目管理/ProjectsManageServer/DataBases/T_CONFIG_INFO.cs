using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLib;

namespace ProjectsManageServer.DataBases
{
    class T_CONFIG_INFO
    {
        /// <summary>
        /// 定义表名
        /// </summary>
        internal static string TABLE_NAME = "T_CONFIG_INFO";

        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"KEY", "INTEGER"},
            {"VALUE", "VARCHAR(128)"},
        };

        static DataBaseTool_SQLite3 dataBaseTool;
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        internal static bool InitTable(DataBaseTool_SQLite3 dbTool)
        {
            dataBaseTool = dbTool;
            //不存在表则新建
            if (!dbTool.TableExist(TABLE_NAME))
            {
                string sql = "";
                if (!dbTool.CreateDataTable(TABLE_NAME, DIC_TABLE_COLUMS.Keys.ToList(), DIC_TABLE_COLUMS.Values.ToList(), ref sql))
                {
                    return false;
                }
                if (!dbTool.ActionFunc(sql))
                {
                    return false;
                }

            }
            return true;
        }

        internal static bool SaveConfig(DataTable dt, CommonDef.CONFIG_KEYS key, string value, ref string sql)
        {
            string keyIndex = ((int)key).ToString();
            foreach (DataRow dr in dt.Rows)
            {
                if (keyIndex == dr["KEY"].ToString())
                {
                    sql += string.Format("update T_CONFIG_INFO set VALUE = '{0}' where KEY = '{1}';", value, keyIndex);
                    return true;
                }
            }
            sql += string.Format("insert into T_CONFIG_INFO values('{0}','{1}');", keyIndex, value);
            return true;
        }
    }
}

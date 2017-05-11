using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 项目管理.DataBases
{
    class T_PRO_INFO
    { 
        /// <summary>
        /// 定义表名
        /// </summary>
        internal static string TABLE_NAME = "T_PRO_INFO";


        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"DEMAND_ID", "VARCHAR(36) UNIQUE"},
            {"DEMAND_NAME", "VARCHAR(64) UNIQUE"},
            {"DEMAND_DEPART", "VARCHAR(32)"},
            {"DEMAND_DATE", "VARCHAR(8)"},
            {"EXPECT_DATE", "VARCHAR(8)"},
            {"FINISH_DATE", "VARCHAR(8)"},
            {"PRO_KIND", "VARCHAR(16)"},
            {"PRO_STAGE", "VARCHAR(16)"},
            {"PRO_STATE", "VARCHAR(16)"},
            {"PRO_NOTE", "VARCHAR(64)"},
            {"TEST_PERSON", "VARCHAR(32)"},
            {"BUSINESS_PERSON", "VARCHAR(32)"},
            {"REMARK", "VARCHAR(64)"},
            {"LAST_MOD_TIME", "VARCHAR(14)"},
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


    }
}

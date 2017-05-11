using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 项目管理.DataBases
{
    class T_TRADE_INFO
    { 
        /// <summary>
        /// 定义表名
        /// </summary>
        internal static string TABLE_NAME = "T_TRADE_INFO";


        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"DEMAND_ID", "VARCHAR(36)"},
            {"SYS_ID", "VARCHAR(36)"},
            {"TRADE_CODE", "VARCHAR(32)"},
            {"TRADE_NAME", "VARCHAR(32)"},
            {"IS_NEW", "VARCHAR(8)"},
            {"FILE_NAME", "VARCHAR(32)"},
            {"WORKER", "VARCHAR(32)"},
            {"WORKLOAD", "DECIMAL"},
            {"REMARK", "VARCHAR(64)"},
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

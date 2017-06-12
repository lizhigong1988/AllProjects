using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectsManageServer.DataBases
{
    class T_PRO_RATE
    { 
        /// <summary>
        /// 定义表名
        /// 项目进度登记表
        /// </summary>
        internal static string TABLE_NAME = "T_PRO_RATE";


        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"DEMAND_ID", "VARCHAR(36)"},   //需求ID
            {"SYS_ID", "VARCHAR(36)"},      //系统ID
            {"DATE", "VARCHAR(8)"},         //录入日期
            {"RATE", "INTEGER"},            //进度比例
            {"EXPLAIN", "VARCHAR(64)"},     //说明
            {"PROBLEM", "VARCHAR(64)"},     //当前问题
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

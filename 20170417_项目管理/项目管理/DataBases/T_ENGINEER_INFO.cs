﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 项目管理.DataBases
{
    class T_ENGINEER_INFO
    { 
        /// <summary>
        /// 定义表名
        /// </summary>
        internal static string TABLE_NAME = "T_ENGINEER_INFO";

        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"SYS_ID", "VARCHAR(36) UNIQUE"},
            {"ENGINEER_NAME", "VARCHAR(32)"},
            {"COMPANY", "VARCHAR(64)"},
            {"REMARK", "VARCHAR(128)"},
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

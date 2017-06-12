﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectsManageServer.DataBases
{
    class T_USER_SYS_INFO
    { 
        /// <summary>
        /// 定义表名
        /// 用户与系统关系表
        /// </summary>
        internal static string TABLE_NAME = "T_USER_SYS_INFO";

        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"USER_NAME", "VARCHAR(32)"},
            {"SYS_ID", "VARCHAR(36)"},
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

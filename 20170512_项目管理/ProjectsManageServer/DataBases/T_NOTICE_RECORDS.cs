using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectsManageServer.DataBases
{
    class T_NOTICE_RECORDS
    { 
        /// <summary>
        /// 定义表名
        /// 公告记录表
        /// </summary>
        internal static string TABLE_NAME = "T_NOTICE_RECORDS";


        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"NOTICE_ID", "VARCHAR(36)"},   //公告ID
            {"NOTICE_DATE", "VARCHAR(8)"},      //公告日期
            {"NOTICE_TIME", "VARCHAR(8)"},         //公告时间
            {"NOTICE_MAN", "VARCHAR(32)"},            //公告人
            {"NOTICE_TITLE", "VARCHAR(64)"},     //标题
            {"NOTICE_CONTENT", "VARCHAR(256)"},     //公告内容
            {"LVL", "INTEGER"},     //公告级别 0 置顶公告 1 普通（暂定）
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

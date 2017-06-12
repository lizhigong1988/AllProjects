using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectsManageServer.DataBases
{
    class T_PRO_DAILY_DETAIL
    { 
        /// <summary>
        /// 定义表名
        /// 项目日报明细表
        /// </summary>
        internal static string TABLE_NAME = "T_PRO_DAILY_DETAIL";


        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"DAILY_ID", "VARCHAR(36)"},        //外键签到表ID
            {"DEMAND_ID", "VARCHAR(36)"},       //工作项目ID
            {"SYS_ID", "VARCHAR(36)"},          //子单系统ID
            {"WORK_TYPE", "VARCHAR(1)"},        //工作内容类型 1、开发、2、会议 3、文档整理 4、协助运维 Z、其他
            {"TRADE_CODE", "VARCHAR(32)"},      //开发交易信息（工作类型为1时使用）
            {"USER_HOURS", "DECIMAL"},          //用时（小时，可以为小数）
            {"REMARK", "VARCHAR(64)"},          //描述
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectsManageServer.DataBases
{
    class T_PRO_DAILY
    { 
        /// <summary>
        /// 定义表名
        /// 项目开发日报表
        /// </summary>
        internal static string TABLE_NAME = "T_PRO_DAILY";


        /// <summary>
        /// 定义列标题
        /// </summary>
        internal static Dictionary<string, string> DIC_TABLE_COLUMS = new Dictionary<string, string>()
        {
            {"DAILY_ID", "VARCHAR(36)"},        //唯一标识
            {"USER_NAME", "VARCHAR(32)"},       //填写人姓名
            {"DAILY_DATE", "VARCHAR(8)"},       //日期
            {"LOGIN_TIME", "VARCHAR(6)"},       //签到时间
            {"LOGOUT_TIME", "VARCHAR(6)"},      //签退时间
            {"HARD_ID", "VARCHAR(36)"},       //硬盘编码
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

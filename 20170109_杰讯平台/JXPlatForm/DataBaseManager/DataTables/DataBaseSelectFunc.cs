using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.Data;

namespace DataBaseManager.DataTables
{
    class DataBaseSelectFunc
    {
        /// <summary>
        /// 组合查询语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="items"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static string SelectSql(string tableName, string items, string limit = "")
        {
            string sql = "select " + items + " from " + tableName;
            if (limit != "")
            {
                sql += " where " + limit;
            }
            return sql;
        }

        /// <summary>
        /// 组合查询语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="items"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static string SelectSql(string tableName, List<string> items, string limit = "")
        {
            string sql = "select ";
            foreach (string item in items)
            {
                sql += item + ", ";
            }
            sql = sql.Remove(sql.Length - 2);
            sql += " from " + tableName;
            if (limit != "")
            {
                sql += " where " + limit;
            }
            return sql;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET GetUserInfo(DataHelper dataHelper)
        {
            string sql = SelectSql(T_USER_INFO.TABLE_NAME, T_USER_INFO.LIST_COLUMS, "USER_CODE = '" +
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE) + "'");
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }
        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET GetAllUserInfo(DataHelper dataHelper)
        {
            string sql = SelectSql(T_USER_INFO.TABLE_NAME, T_USER_INFO.LIST_COLUMS);
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 获取用户数
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET CountUserInfo(DataHelper dataHelper)
        {
            string sql = SelectSql(T_USER_INFO.TABLE_NAME, "COUNT(*)");
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 获取所有的角色信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET GetAllRoleInfo(DataHelper dataHelper)
        {
            string sql = SelectSql(T_ROLE_INFO.TABLE_NAME, T_ROLE_INFO.LIST_COLUMS);
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 获取所有的角色权限限制信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET GetRoleAuthInfo(DataHelper dataHelper)
        {
            string limit = "ROLE_ID = '" + dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID) + "'";
            string sql = SelectSql(T_ROLE_AUTH_INFO.TABLE_NAME, T_ROLE_AUTH_INFO.LIST_COLUMS, limit);
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET DelRoleInfo(DataHelper dataHelper)
        {
            string roles = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID).Replace("\t", "','");
            roles = "('" + roles + "');";
            string select = "select count(*) from T_USER_INFO where USER_RULE in " + roles;

            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return CommonDef.COM_RET.HAS_USER_ROLE;
            }

            string sql = "";
            if (!T_ROLE_INFO.DelRoleInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!T_ROLE_AUTH_INFO.DelRoleInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }
        /// <summary>
        /// 新增用户信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET AddUserInfo(DataHelper dataHelper)
        {
            //检查重复
            string select = "select count(*) from T_USER_INFO where USER_CODE = '{0}' or USER_NAME = '{1}'";
            select = string.Format(select, dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE),
                dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_NAME));
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return CommonDef.COM_RET.HAS_USER_INFO;
            }

            string sql = "";
            if (!T_USER_INFO.AddUserInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET ModUserPsw(DataHelper dataHelper)
        { 
            //检查存在
            string select = "select count(*) from T_USER_INFO where USER_CODE = '{0}'";
            select = string.Format(select, dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE));
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                return CommonDef.COM_RET.NO_USER_INFO;
            }

            string sql = "";
            if (!T_USER_INFO.ModUserPsw(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET ModUserInfo(DataHelper dataHelper)
        {
            //检查存在
            string select = "select count(*) from T_USER_INFO where USER_CODE = '{0}'";
            select = string.Format(select, dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE));
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                return CommonDef.COM_RET.NO_USER_INFO;
            }

            string sql = "";
            if (!T_USER_INFO.ModUserInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET DelUserInfo(DataHelper dataHelper)
        {
            string sql = "";
            if (!T_USER_INFO.DelUserInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 新增角色权限
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET AddRoleAuthInfo(DataHelper dataHelper)
        {
            //检查重复
            int newRoleID = 1;
            string select = "select count(*) from T_ROLE_INFO";
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                newRoleID = 1;
            }
            else
            {
                select = "select max(ROLE_ID) from T_ROLE_INFO";
                dt = DataTableTool.SelectTableInfo(select);
                if (dt == null)
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
                newRoleID = int.Parse(dt.Rows[0][0].ToString()) + 1;
            }
            if (!dataHelper.AddConfig(DataHelper.CONFIG_KEYS.ROLE_ID, newRoleID.ToString()))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }

            select = "select count(*) from T_ROLE_INFO where ROLE_NAME = '{0}'";
            select = string.Format(select, dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_NAME));
            dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() != "0")
            {
                return CommonDef.COM_RET.HAS_ROLE_NAME;
            }

            string sql = "";
            if (!T_ROLE_INFO.AddRoleInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!T_ROLE_AUTH_INFO.AddRoleAuthInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET ModRoleInfo(DataHelper dataHelper)
        {
            string select = "select count(*) from T_ROLE_INFO where ROLE_ID = '{0}'" ;
            select = string.Format(select, dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ROLE_ID));
            DataTable dt = DataTableTool.SelectTableInfo(select);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                return CommonDef.COM_RET.NO_ROLE_INFO;
            }
            string sql = "";
            if (!T_ROLE_INFO.ModRoleInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!T_ROLE_AUTH_INFO.ModRoleAuthInfo(dataHelper, ref  sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 重置用户的密码
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET ResetUserPsw(DataHelper dataHelper)
        {
            string[] users = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_CODE).Split('\t');
            string defautPsw = CommonDef.DEFAUTL_PSW;
            string sql = "";
            foreach (string user in users)
            {
                if (user == "")
                {
                    continue;
                }
                dataHelper.AddConfig(DataHelper.CONFIG_KEYS.USER_CODE, user);
                dataHelper.AddConfig(DataHelper.CONFIG_KEYS.USER_PSW, defautPsw);
                if (!T_USER_INFO.ModUserPsw(dataHelper, ref  sql))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 扩展通用接口
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        internal static CommonDef.COM_RET PlantfromCom(DataHelper dataHelper)
        {
            string nameSpace = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_NAME);
            ExpandBaseServer expandServer = null;
            try
            {
                string className = nameSpace + "." + nameSpace + "Server";
                Type tp = CommonDef.DicFileAss[nameSpace].GetType(className);
                Object obj = Activator.CreateInstance(tp);
                expandServer = obj as ExpandBaseServer;
            }
            catch
            {
                return CommonDef.COM_RET.SERVER_ERROR;
            }
            return expandServer.HandleData(dataHelper);
        }
    }
}

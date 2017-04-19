using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataBaseManager.DataTables.DataBaseTools;
using CommonLib;

namespace DataBaseManager
{
    interface IDataBaseTool
    { 
        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        bool InitDataBase();

        /// <summary>
        /// 执行数据库非查询类操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        bool ActionFunc(string sql);
        
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <returns></returns>
        void CloseDataBase();

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>true 存在 false 失败或者不存在</returns>
        bool TableExist(string tableName);
        
        /// <summary>
        /// 查询语句
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        DataTable SelectFunc(string selectSql);
    }



    public class DataTableTool
    {
        /// <summary>
        /// 数据库操作实例
        /// </summary>
        private static IDataBaseTool dataBaseTool = null;

        /// <summary>
        /// 增加语句
        /// </summary>
        /// <param name="TABLE_NAME"></param>
        /// <param name="LIST_INFO_ELEMENTS"></param>
        /// <param name="ELEMNT_RULES"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool AddInfo(string tableName, List<string> LIST_INFO_ELEMENTS, List<string> values, ref string sql)
        {
            if (LIST_INFO_ELEMENTS.Count != values.Count)
            {
                sql = "";
                return false;
            }
            sql += "INSERT INTO " + tableName + "(";
            for (int i = 0; i < LIST_INFO_ELEMENTS.Count; i++)
            {
                sql += LIST_INFO_ELEMENTS[i] + ", ";
            }
            sql = sql.Remove(sql.Length - 2);
            sql += ") VALUES (";
            for (int i = 0; i < values.Count; i++)
            {
                sql += "'" + values[i] + "', ";
            }
            sql = sql.Remove(sql.Length - 2);
            sql += ");";
            return true;
        }

        /// <summary>
        /// 修改语句，第一项为主键
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="LIST_INFO_ELEMENTS"></param>
        /// <param name="values"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool ModInfo(string tableName, List<string> LIST_INFO_ELEMENTS, List<string> values, ref string sql)
        {
            if (LIST_INFO_ELEMENTS.Count != values.Count)
            {
                sql = "";
                return false;
            }
            sql += "UPDATE " + tableName + " SET ";
            for (int i = 1; i < LIST_INFO_ELEMENTS.Count; i++)
            {
                sql += LIST_INFO_ELEMENTS[i] + " = '" + values[i] + "', ";
            }
            sql = sql.Remove(sql.Length - 2);
            sql += " where " + LIST_INFO_ELEMENTS[0] + " = '" + values[0] + "'";
            sql += ";";
            return true;
        }

        /// <summary>
        /// 写表格
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <param name="dt"></param>
        public static void SetDataTable(DataHelper dataHelper, DataTable dt)
        {
            string dataTable = "";
            foreach (DataColumn colum in dt.Columns)
            {
                dataTable += colum.ColumnName + "\t";
            }
            dataTable = dataTable.Remove(dataTable.Length - 1);
            dataTable += "\n";
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dataTable += dr[i].ToString() + "\t";
                }
                dataTable = dataTable.Remove(dataTable.Length - 1);
                dataTable += "\n";
            }
            dataHelper.AddConfig(DataHelper.CONFIG_KEYS.SELECT_TABLE, dataTable);
        }

        /// <summary>
        /// 创建datatable
        /// </summary>
        /// <param name="LIST_INFO_ELEMENTS"></param>
        /// <param name="ELEMNT_RULES"></param>
        /// <returns></returns>
        public static bool CreateDataTable(string tableName, List<string> LIST_INFO_ELEMENTS,
            List<string> ELEMNT_RULES, ref string sql)
        {
            if (LIST_INFO_ELEMENTS.Count != ELEMNT_RULES.Count)
            {
                return false;
            }
            sql += "CREATE TABLE " + tableName + "(";
            for (int i = 0; i < LIST_INFO_ELEMENTS.Count; i++)
            {
                sql += LIST_INFO_ELEMENTS[i] + " " + ELEMNT_RULES[i] + ", ";
            }
            sql = sql.Remove(sql.Length - 2);
            sql += ");";
            return true;
        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectTableInfo(string sql)
        {
            if (dataBaseTool == null)
            {
                InitDataBase();
            }
            if (null == dataBaseTool)
            {
                return null;
            }
            return dataBaseTool.SelectFunc(sql);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public static bool ActionTableFunc(string sql)
        {
            if (dataBaseTool == null)
            {
                InitDataBase();
            }
            if (null == dataBaseTool)
            {
                return false;
            }
            return dataBaseTool.ActionFunc(sql);
        }

        /// <summary>
        /// 判断表存在
        /// </summary>
        /// <returns></returns>
        public static bool TableExist(string tableName)
        {
            if (dataBaseTool == null)
            {
                InitDataBase();
            }
            if (null == dataBaseTool)
            {
                return false;
            }
            return dataBaseTool.TableExist(tableName);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static CommonDef.COM_RET InitDataBase()
        {
            string databaseType = ServerConfigHeper.ReadConfig(ServerConfigHeper.CONFIG_KEYS.DATABASE_TYPE);
            switch (databaseType)
            {
                case "SQLITE3":
                    dataBaseTool = new DataBaseTool_SQLite3();
                    dataBaseTool.InitDataBase();
                    break;
                default:
                    return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }
    }
}

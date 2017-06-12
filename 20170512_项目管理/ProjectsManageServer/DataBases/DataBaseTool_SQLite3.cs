using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
using System.Data.SQLite;

namespace ProjectsManageServer.DataBases
{
    public class DataBaseTool_SQLite3
    {
        /// <summary>
        /// 定义数据库文件名
        /// </summary>
        public static string DATABASE_FILE = "DataBase.db";

        /// <summary>
        /// DataBase连接、全局使用
        /// </summary>
        private SQLiteConnection _conn_ = null;

        /// <summary>
        /// DataBase执行指令、全局使用
        /// </summary>
        private SQLiteCommand _command_ = null;


        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        public bool InitDataBase()
        {
            try
            {
                //创建数据库文件,FailIfMissing为Flase丢失情况下新建
                string init = "Data Source=.\\" + DATABASE_FILE + ";Pooling=true;FailIfMissing=false";
                _conn_ = new SQLiteConnection(init);
                _command_ = new SQLiteCommand(_conn_);
                _conn_.Open();
                //_command_.CommandText = "PRAGMA foreign_keys = ON;";
                //_command_.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 执行数据库非查询类操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>影响行数,失败返回-1</returns>
        public bool ActionFunc(string sql)
        {
            string actSql = "begin;\n" + sql + ";\ncommit;";
            _command_.CommandText = actSql;
            int ret = 0;
            try
            {
                ret = _command_.ExecuteNonQuery();
            }
            catch
            {
                try
                {
                    _command_.CommandText = "rollback;";
                    _command_.ExecuteNonQuery();
                }
                catch
                {

                }
                return false;
            }
            return ret >= 0;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <returns></returns>
        public void CloseDataBase()
        {
            _conn_.Close();
            SQLiteConnection.ClearAllPools();
        }

        /// <summary>
        /// 查询语句
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public DataTable SelectFunc(string selectSql)
        {
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter();
                _command_.CommandText = selectSql;
                da.SelectCommand = _command_;
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableExist(string tableName)
        {
            string sql = "SELECT COUNT(*) FROM sqlite_master where type='table' and name='" + tableName + "'";
            DataTable dt = SelectFunc(sql);
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows[0][0].ToString() == "0")
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 创建datatable
        /// </summary>
        /// <param name="LIST_INFO_ELEMENTS"></param>
        /// <param name="ELEMNT_RULES"></param>
        /// <returns></returns>
        internal bool CreateDataTable(string tableName, List<string> LIST_INFO_ELEMENTS,
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
        /// 增加语句
        /// </summary>
        /// <param name="TABLE_NAME"></param>
        /// <param name="LIST_INFO_ELEMENTS"></param>
        /// <param name="ELEMNT_RULES"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal bool AddInfo(string tableName, List<string> LIST_INFO_ELEMENTS, List<string> values, ref string sql)
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
                sql += "'" + values[i].Replace('\'', '\"') + "', ";
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
        internal bool ModInfo(string tableName, List<string> LIST_INFO_ELEMENTS,
            List<string> values, ref string sql, Dictionary<string, string> dicMainKey = null)
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
            if (dicMainKey == null)
            {
                sql += " where " + LIST_INFO_ELEMENTS[0] + " = '" + values[0] + "'";
            }
            else
            {
                sql += " where 1 = 1";
                for (int i = 0; i < dicMainKey.Count; i++)
                {
                    sql += string.Format(" and {0} = '{1}'", 
                        dicMainKey.ElementAt(i).Key, dicMainKey.ElementAt(i).Value);
                }
            }
            sql += ";";
            return true;
        }
    }
}

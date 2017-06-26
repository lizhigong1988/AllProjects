using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace 图片识别.DataBases
{
    class DataBaseManager
    {
        /// <summary>
        /// 数据库工具
        /// </summary>
        static DataBaseTool_SQLite3 dataBaseTool = new DataBaseTool_SQLite3();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        internal static bool InitDataBases()
        {
            bool ret = dataBaseTool.InitDataBase();
            if (ret) ret = T_IMAGE_TAG.InitTable(dataBaseTool);
            return ret;
        }


        internal static bool ImportImages(DataTable dtImport)
        {
            string sql = "";
            string selectSql = "select * from T_IMAGE_TAG";
            DataTable dtSelect = dataBaseTool.SelectFunc(selectSql);
            if (dtSelect == null)
            {
                return false;
            }
            foreach (DataRow dr in dtImport.Rows)
            {
                List<string> values = new List<string>() { dr["IMAGE_NAME"].ToString(), 
                    dr["IMAGE_TAG"].ToString(), dr["REMARK"].ToString() };
                bool has = false;
                foreach (DataRow drSelect in dtSelect.Rows)
                {
                    if (drSelect["IMAGE_NAME"].ToString() == dr["IMAGE_NAME"].ToString())
                    {
                        has = true;
                        if (!dataBaseTool.ModInfo(T_IMAGE_TAG.TABLE_NAME, T_IMAGE_TAG.DIC_TABLE_COLUMS.Keys.ToList(),
                            values, ref sql))
                        {
                            return false;
                        }
                    }
                }
                if (!has)
                {
                    if (!dataBaseTool.AddInfo(T_IMAGE_TAG.TABLE_NAME, T_IMAGE_TAG.DIC_TABLE_COLUMS.Keys.ToList(),
                        values, ref sql))
                    {
                        return false;
                    }
                }
            }
            return dataBaseTool.ActionFunc(sql);
        }

        internal static DataTable GetImages()
        {
            string selectSql = "select * from T_IMAGE_TAG";
            return dataBaseTool.SelectFunc(selectSql);
        }

        internal static bool DelImages(string name)
        {
            string sql = string.Format("delete from T_IMAGE_TAG where IMAGE_NAME = '{0}'", name);
            return dataBaseTool.ActionFunc(sql);
        }
    }
}

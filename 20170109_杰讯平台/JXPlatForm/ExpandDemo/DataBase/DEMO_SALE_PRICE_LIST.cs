using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_SALE_PRICE_LIST
    {
        internal static string TABLE_NAME = "DEMO_SALE_PRICE_LIST";

        internal enum TABLE_COLUMS
        {
            KIND,           //商品种类
            NAME,           //品名
            MODEL,          //型号
            PRICE,          //单价

            TOTAL_COUNT         
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(32)",              //品名
            "VARCHAR(32)",              //型号
            "VARCHAR(32)",              //尺寸
            "DECIMAL",              //单价
        };

        internal static List<string> LIST_COLUMS;


        internal static bool InitTable()
        {
            LIST_COLUMS = new List<string>();
            for (int i = 0; i < (int)TABLE_COLUMS.TOTAL_COUNT; i++)
            {
                LIST_COLUMS.Add(((TABLE_COLUMS)i).ToString());
            }
            if (!DataTableTool.TableExist(TABLE_NAME))
            {
                string sql = "";
                if (!DataTableTool.CreateDataTable(TABLE_NAME, LIST_COLUMS, ELEMNT_RULES, ref sql))
                {
                    return false;
                }
                if (!DataTableTool.ActionTableFunc(sql))
                {
                    return false;
                }
            }
            return true;
        }
        
        internal static CommonDef.COM_RET DelPriceInfo(DataHelper dataHelper)
        {
            string delInfo = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elemns = delInfo.Split('\n');
            string sql = string.Format("delete from DEMO_SALE_PRICE_LIST where KIND = '{0}' and  NAME = '{1}' and  MODEL = '{2}';", elemns[0], elemns[1], elemns[2]);
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET AddModPriceInfo(DataHelper dataHelper)
        {
            string delInfo = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elemns = delInfo.Split('\n');
            string sql = string.Format("delete from DEMO_SALE_PRICE_LIST where KIND = '{0}' and  NAME = '{1}' and  MODEL = '{2}';", elemns[0], elemns[1], elemns[2]);
            List<string> values = new List<string>() { elemns[0], elemns[1], elemns[2], elemns[3] };
            if (!DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET GelPriceInfo(DataHelper dataHelper)
        {
            string sql = "select * from DEMO_SALE_PRICE_LIST";
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }
    }
}

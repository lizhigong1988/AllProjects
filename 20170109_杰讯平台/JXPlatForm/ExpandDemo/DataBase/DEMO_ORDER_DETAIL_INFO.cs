using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_ORDER_DETAIL_INFO
    {
        internal static string TABLE_NAME = "DEMO_ORDER_DETAIL_INFO";

        internal enum TABLE_COLUMS
        {
            ORDER_ID,       //订单ID
            KIND,           //商品种类
            NAME,           //品名
            MODEL,          //型号
            SIZE,           //尺寸

            STYLE_KIND,     //套线类型
            COLOR,          //颜色
            COUNT,          //数量
            PRICE,          //单价
            REMARK,         //备注

            TOTAL_COUNT         
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(36)",              //订单ID
            "VARCHAR(16)",              //商品种类
            "VARCHAR(32)",              //品名
            "VARCHAR(32)",              //型号
            "VARCHAR(32)",              //尺寸
            
            "VARCHAR(16)",              //套线类型
            "VARCHAR(16)",              //颜色
            "DECIMAL",              //数量
            "DECIMAL",              //单价
            "VARCHAR(64)",              //备注
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

        internal static CommonDef.COM_RET GetOrderDetail(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = "";
            sql = string.Format("select * from DEMO_ORDER_DETAIL_INFO where ORDER_ID = '{0}'", elems[0]);
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

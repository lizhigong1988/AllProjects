using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_STOCK_DETAIL
    {
        internal static string TABLE_NAME = "DEMO_STOCK_DETAIL";

        internal enum TABLE_COLUMS
        {
            STOCK_ID,           //订货单ID
            ORDER_ID,           //订单ID
            COUNT_INDEX,         //序号
            MODEL,              //型号
            TEXTURE,        //材质

            LINE,           //线条
            PLATE,              //套板
            COLOR,              //颜色
            SIZE,               //尺寸
            COUNT,        //数量

            PRICE,         //单价
            REMARK,             //备注
            NAME,               //品名
            DOOR_COUNT,         //扇数
            SHUTTER,            //百叶

            SUSPEND,            //吊脚
            GLASS_MODEL,           //玻璃型号
            FORWORD,            //开向
            KIND,               //0木门、窗户2合金门

            TOTAL_COUNT               
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(36)",              //订单ID
            "VARCHAR(36)",              //订单ID
            "INTEGER",              //序号
            "VARCHAR(32)",              //型号
            "VARCHAR(32)",              //材质
            
            "VARCHAR(32)",              //线条
            "VARCHAR(32)",              //套板
            "VARCHAR(32)",              //颜色
            "VARCHAR(32)",              //尺寸
            "VARCHAR(32)",              //数量
            
            
            "DECIMAL",              //单价
            "VARCHAR(64)",              //备注
            "VARCHAR(32)",              //品名
            "INTEGER",              //扇数
            "VARCHAR(32)",              //百叶
            
            "VARCHAR(32)",              //吊脚
            "VARCHAR(32)",              //玻璃型号
            "VARCHAR(32)",              //开向
            "INTEGER",              //0木门、窗户1合金门
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

        internal static bool AddDetailInfo(string woodInfo, string alloyInfo, ref string sql)
        {
            List<string> lines = new List<string>(woodInfo.Split('\r'));
            lines.AddRange(alloyInfo.Split('\r'));
            foreach (string line in lines)
            {
                if (line == "")
                {
                    continue;
                }
                List<string> values = new List<string>(line.Split('\t'));
                if (!DataTableTool.AddInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
                {
                    return false;
                }
            }
            return true;
        }

        internal static CommonDef.COM_RET GetStockDetail(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = string.Format("select * from DEMO_STOCK_DETAIL where STOCK_ID = '{0}' and ORDER_ID = '{1}'", elems[0], elems[1]);
            sql += " order by COUNT_INDEX";
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

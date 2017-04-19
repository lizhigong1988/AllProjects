using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseManager;
using CommonLib;
using System.Data;

namespace ExpandDemo.DataBase
{
    class DEMO_ORDER_INFO
    {
        internal static string TABLE_NAME = "DEMO_ORDER_INFO";

        internal enum TABLE_COLUMS
        {
            ORDER_ID,           //订单ID
            CLIENT_ID,          //客户ID
            ORDER_DATE,         //订单日期
            PRO_END_DATE,       //安装完成日期
            ALL_END_DATE,       //完结日期

            EXPECT_DAYS,        //预计日期
            ORDER_AMT,        //订单金额
            DEPOSIT_AMT,        //已付定金
            ORDER_STAT,         //0正常、1关闭
            WORKER,             //工人
            REMARK,             //备注

            TOTAL_COUNT         
        }
        /// <summary>
        /// 字段属性
        /// </summary>
        public static List<string> ELEMNT_RULES = new List<string>()
        {
            "VARCHAR(36) UNIQUE NOT NULL",              //订单ID
            "INTEGER",              //客户ID
            "VARCHAR(8)",              //订单日期
            "VARCHAR(8)",              //安装完成日期
            "VARCHAR(8)",              //完成日期

            "VARCHAR(8)",              //预计完成日期
            "DECIMAL",              //订单金额
            "DECIMAL",              //已付定金，单位分
            "INTEGER",              //状态
            "VARCHAR(32)",              //安装工人

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

        internal static CommonDef.COM_RET AddOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            //客户名称、电话、地址、商品信息（类型、名称、型号、尺寸、套线类型、颜色、数量、单价、备注）
            //日期、预计结束日期、订单金额、定金金额、备注
            string select = "select CLIENT_ID from DEMO_CLIENT_INFO where CLIENT_NAME = '{0}' and CLIENT_TEL = '{1}'";
            select = string.Format(select, elems[0], elems[1]);
            string sql = "";
            DataTable selectDt = DataTableTool.SelectTableInfo(select);
            string clientID = "";
            if (selectDt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (selectDt.Rows.Count == 0)
            {
                if (!DEMO_CLIENT_INFO.AddClientSql(elems[0], elems[1], elems[2], ref sql, out clientID))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }
            else
            {
                clientID = selectDt.Rows[0][0].ToString();
                if (!DEMO_CLIENT_INFO.ModClientSql(elems[0], elems[1], elems[2], ref sql, clientID))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }
            string orderId = Guid.NewGuid().ToString();
            string[] goodsInfo = elems[3].Split('\r');
            foreach (string goods in goodsInfo)
            {
                if (goods == "")
                {
                    continue;
                }
                List<string> newGoodsInfo = new List<string>(goods.Split('\t'));
                newGoodsInfo.Insert(0, orderId);
                if (!DataTableTool.AddInfo(DEMO_ORDER_DETAIL_INFO.TABLE_NAME, DEMO_ORDER_DETAIL_INFO.LIST_COLUMS, newGoodsInfo, ref sql))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }

            //客户名称、电话、地址、商品信息（类型、名称、型号、尺寸、套线类型、颜色、数量、单价、备注）
            //4日期、5预计结束日期、6订单金额、7定金金额、8备注
            List<string> values = new List<string>()
            { 
                orderId, clientID, elems[4], "", "", 
                elems[5], elems[6], elems[7], "0", "", elems[8]
            };
            values.RemoveRange((int)TABLE_COLUMS.TOTAL_COUNT, values.Count - (int)TABLE_COLUMS.TOTAL_COUNT);
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

        internal static CommonDef.COM_RET ModOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            //客户名称、电话、地址、商品信息（类型、名称、型号、尺寸、套线类型、颜色、数量、单价、备注）
            //日期、预计结束日期、订单金额、定金金额、备注
            string select = "select CLIENT_ID from DEMO_CLIENT_INFO where CLIENT_NAME = '{0}' and CLIENT_TEL = '{1}'";
            select = string.Format(select, elems[0], elems[1]);
            string sql = "";
            DataTable selectDt = DataTableTool.SelectTableInfo(select);
            string clientID = "";
            if (selectDt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (selectDt.Rows.Count == 0)
            {
                if (!DEMO_CLIENT_INFO.AddClientSql(elems[0], elems[1], elems[2], ref sql, out clientID))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }
            else
            {
                clientID = selectDt.Rows[0][0].ToString();
                if (!DEMO_CLIENT_INFO.ModClientSql(elems[0], elems[1], elems[2], ref sql, clientID))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }
            string orderId = elems[4];
            string[] goodsInfo = elems[3].Split('\r');
            sql += string.Format("delete from DEMO_ORDER_DETAIL_INFO where ORDER_ID = '{0}';", orderId);
            foreach (string goods in goodsInfo)
            {
                if (goods == "")
                {
                    continue;
                }
                List<string> newGoodsInfo = new List<string>(goods.Split('\t'));
                newGoodsInfo.Insert(0, orderId);
                if (!DataTableTool.AddInfo(DEMO_ORDER_DETAIL_INFO.TABLE_NAME, DEMO_ORDER_DETAIL_INFO.LIST_COLUMS, newGoodsInfo, ref sql))
                {
                    return CommonDef.COM_RET.DATABASE_ERROR;
                }
            }
            List<string> values = new List<string>()
            { 
                orderId, clientID, elems[5], elems[6], elems[7], 
                elems[8], elems[9], elems[10], "0", elems[11], elems[12]
            };
            values.RemoveRange((int)TABLE_COLUMS.TOTAL_COUNT, values.Count - (int)TABLE_COLUMS.TOTAL_COUNT);
            if (!DataTableTool.ModInfo(TABLE_NAME, LIST_COLUMS, values, ref sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET GetOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = "";
            //0查未完成。 1、查已完成。 2、查全部加查询条件
            switch(elems[0])
            {
                case "0":
                    sql = "select t1.*, t2.* from DEMO_CLIENT_INFO t1, DEMO_ORDER_INFO t2 where ";
                    sql += "t1.CLIENT_ID = t2.CLIENT_ID and t2.ALL_END_DATE = '' ";
                    sql += "and t2.ORDER_STAT = '0' order by EXPECT_DAYS desc";
                    break;
                case "1":
                    sql = "select t1.*, t2.* from DEMO_CLIENT_INFO t1, DEMO_ORDER_INFO t2 where ";
                    sql += string.Format("t1.CLIENT_ID = t2.CLIENT_ID and (t2.ALL_END_DATE >= '{0}' and t2.ALL_END_DATE <= '{1}' ",
                        elems[1], elems[2]);
                    if (elems[3] == "1")
                    {
                        sql += "or t2.ORDER_STAT = '1'";
                    }
                    sql += ") order by EXPECT_DAYS desc";
                    break;
                case "2":
                    sql = "select t1.*, t2.* from DEMO_CLIENT_INFO t1, DEMO_ORDER_INFO t2 where t1.CLIENT_ID = t2.CLIENT_ID";
                    if (elems[1] != "")
                    {
                        sql += string.Format(" and t1.CLIENT_NAME = '{0}'", elems[1]);
                    }
                    if (elems[2] != "")
                    {
                        sql += string.Format(" and t2.WORKER = '{0}'", elems[2]);
                    }
                    switch (elems[3])
                    {
                        case "1"://未安装
                            sql += string.Format(" and t2.PRO_END_DATE = ''");
                            elems[7] = "";//安装日期1
                            elems[8] = "";//安装日期2
                            elems[9] = "";//结束日期1
                            elems[10] = "";//结束日期2
                            break;
                        case "2"://已安装（未完成）
                            sql += string.Format(" and t2.PRO_END_DATE != '' and t2.ALL_END_DATE = ''");
                            elems[9] = "";//结束日期1
                            elems[10] = "";//结束日期2
                            break;
                        case "3"://全部未完成
                            sql += string.Format(" and t2.ALL_END_DATE = ''");
                            elems[9] = "";//结束日期1
                            elems[10] = "";//结束日期2
                            break;
                        case "4"://已完成
                            sql += string.Format(" and t2.ALL_END_DATE != ''");
                            break;
                    }
                    switch (elems[4])
                    {
                        case "1"://正常
                            sql += string.Format(" and t2.ORDER_STAT = '0'");
                            break;
                        case "2"://撤销
                            sql += string.Format(" and t2.ORDER_STAT = '1'");
                            break;
                    }
                    if (elems[5] != "")
                    {
                        sql += string.Format(" and t2.ORDER_DATE >= '{0}'", elems[5]);
                    }
                    if (elems[6] != "")
                    {
                        sql += string.Format(" and t2.ORDER_DATE <= '{0}'", elems[6]);
                    }
                    if (elems[7] != "")
                    {
                        sql += string.Format(" and t2.PRO_END_DATE >= '{0}'", elems[7]);
                    }
                    if (elems[8] != "")
                    {
                        sql += string.Format(" and t2.PRO_END_DATE <= '{0}'", elems[8]);
                    }
                    if (elems[9] != "")
                    {
                        sql += string.Format(" and t2.ALL_END_DATE >= '{0}'", elems[9]);
                    }
                    if (elems[10] != "")
                    {
                        sql += string.Format(" and t2.ALL_END_DATE <= '{0}'", elems[10]);
                    }
                    break;
                default:
                    return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTable dt = DataTableTool.SelectTableInfo(sql);
            if (dt == null)
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            DataTableTool.SetDataTable(dataHelper, dt);
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET CancelOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = string.Format("update DEMO_ORDER_INFO set ORDER_STAT = '1' where ORDER_ID = '{0}';", elems[0]);
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET RedoOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = string.Format("update DEMO_ORDER_INFO set ORDER_STAT = '0' where ORDER_ID = '{0}';", elems[0]);
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET UnfinishOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = string.Format("update DEMO_ORDER_INFO set ALL_END_DATE = '' where ORDER_ID = '{0}';", elems[0]);
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }

        internal static CommonDef.COM_RET DelOrderInfo(DataHelper dataHelper)
        {
            string message = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_MSG);
            string[] elems = message.Split('\n');
            string sql = string.Format("delete from DEMO_ORDER_INFO where ORDER_ID = '{0}';", elems[0]);
            sql += string.Format("delete from DEMO_ORDER_DETAIL_INFO where ORDER_ID = '{0}';", elems[0]);
            sql += string.Format("delete from DEMO_STOCK_INFO where ORDER_ID = '{0}';", elems[0]);
            sql += string.Format("delete from DEMO_STOCK_DETAIL where ORDER_ID = '{0}';", elems[0]);
            if (!DataTableTool.ActionTableFunc(sql))
            {
                return CommonDef.COM_RET.DATABASE_ERROR;
            }
            return CommonDef.COM_RET.RET_OK;
        }
    }
}

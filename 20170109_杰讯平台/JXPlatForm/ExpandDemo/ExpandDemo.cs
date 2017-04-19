using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientControls;
using ClientLibrary;
using DataBaseManager;
using ExpandDemo.DataBase;
using CommonLib;

namespace ExpandDemo
{
    class ExpandDemoClient : ExpandBaseClient
    {
        private Dictionary<string, string> dicPageClassName = new Dictionary<string, string>()
        {
            {"客户信息管理", "ExpandDemo.Pages.ClientInfoManage"},
            {"新增订单", "ExpandDemo.Pages.AddNewOrder"},
            {"未完成订单维护", "ExpandDemo.Pages.UnfinishOrder"},
            {"已完成订单维护", "ExpandDemo.Pages.FinishedOrder"},
            {"商品价格表", "ExpandDemo.Pages.OrderPriceList"},
            {"订单查询", "ExpandDemo.Pages.OrderQuery"},
            {"新增下货单", "ExpandDemo.Pages.AddNewStock"},
            {"货单查询", "ExpandDemo.Pages.StockQuery"},
            //{"进货价格表", "ExpandDemo.Pages.StockPriceList"},
        };

        /// <summary>
        /// 获取扩展目录
        /// </summary>
        /// <returns></returns>
        override public List<MenuTree> GetExpandTree()
        {
            return new List<MenuTree>()
            {//初始值
                 new MenuTree(){
                     MenuName = "客户管理",
                     ListChildMenu = new List<MenuTree>()
                     {
                        new MenuTree(){ MenuName = "客户信息管理" },//客户信息增、删、改、查
                     }
                 },
                 new MenuTree(){
                     MenuName = "订单管理",
                     ListChildMenu = new List<MenuTree>()
                     {
                        new MenuTree(){ MenuName = "新增订单" },//录入新订单，打印订单
                        new MenuTree(){ MenuName = "未完成订单维护" },//未完成订单信息、进度变更维护、打印
                        new MenuTree(){ MenuName = "已完成订单维护" },//已完成订单信息、进度变更维护、打印
                        new MenuTree(){ MenuName = "商品价格表" },//订单价格表
                        new MenuTree(){ MenuName = "订单查询" },//历史订单查询、汇总
                     }
                 },
                 new MenuTree(){
                     MenuName = "订货管理",
                     ListChildMenu = new List<MenuTree>()
                     {
                        new MenuTree(){ MenuName = "新增下货单" },//生成、录入下货单信息、打印下货单
                        new MenuTree(){ MenuName = "货单查询" },//历史订货查询、汇总
                        //new MenuTree(){ MenuName = "进货价格表" },//进货价格表
                     }
                 }
            };
        }


        /// <summary>
        /// 获取目录和页面类名对应关系
        /// </summary>
        /// <returns></returns>
        override public string GetPageFile(string menuName)
        {
            return dicPageClassName[menuName];
        }
    }

    class ExpandDemoServer : ExpandBaseServer
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        public override bool InitDataBase()
        {
            if (!DEMO_CLIENT_INFO.InitTable()) return false;
            if (!DEMO_ORDER_INFO.InitTable()) return false;
            if (!DEMO_ORDER_DETAIL_INFO.InitTable()) return false;
            if (!DEMO_SALE_PRICE_LIST.InitTable()) return false;
            if (!DEMO_STOCK_INFO.InitTable()) return false;
            if (!DEMO_STOCK_DETAIL.InitTable()) return false;
            return true;
        }

        public override CommonDef.COM_RET HandleData(DataHelper dataHelper)
        {
            CommonDef.COM_RET ret = CommonDef.COM_RET.RET_OK;
            ExpandDemoCommon.FUNC_NO funNo = (ExpandDemoCommon.FUNC_NO)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.PLATFORM_FUNCNO));
            switch (funNo)
            {
                case ExpandDemoCommon.FUNC_NO.ADD_CLIENT_INFO:
                    ret = DEMO_CLIENT_INFO.AddClientInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.MOD_CLIENT_INFO:
                    ret = DEMO_CLIENT_INFO.ModClientInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.GET_CLIENT_INFO:
                    ret = DEMO_CLIENT_INFO.GetClientInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.DEL_CLIENT:
                    ret = DEMO_CLIENT_INFO.DelClientInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.ADD_ORDER_INFO:
                    ret = DEMO_ORDER_INFO.AddOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.MOD_ORDER_INFO:
                    ret = DEMO_ORDER_INFO.ModOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.CANCEL_ORDER:
                    ret = DEMO_ORDER_INFO.CancelOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.REDO_ORDER:
                    ret = DEMO_ORDER_INFO.RedoOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.UNFINISH_ORDER:
                    ret = DEMO_ORDER_INFO.UnfinishOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.DEL_ORDER:
                    ret = DEMO_ORDER_INFO.DelOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.GET_ORDER_INFO:
                    ret = DEMO_ORDER_INFO.GetOrderInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.GET_ORDER_DETAIL:
                    ret = DEMO_ORDER_DETAIL_INFO.GetOrderDetail(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.GET_SALE_PRICE_INFO:
                    ret = DEMO_SALE_PRICE_LIST.GelPriceInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.DEL_SALE_PRICE_INFO:
                    ret = DEMO_SALE_PRICE_LIST.DelPriceInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.ADD_MOD_SALE_PRICE_INFO:
                    ret = DEMO_SALE_PRICE_LIST.AddModPriceInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.ADD_STOCK_INFO:
                    ret = DEMO_STOCK_INFO.AddStockInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.GET_STOCK_INFO:
                    ret = DEMO_STOCK_INFO.GetStockInfo(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.GET_STOCK_DETAIL:
                    ret = DEMO_STOCK_DETAIL.GetStockDetail(dataHelper);
                    break;
                case ExpandDemoCommon.FUNC_NO.DEL_STOCK:
                    ret = DEMO_STOCK_INFO.DelStockInfo(dataHelper);
                    break;
                default:
                    ret = CommonDef.COM_RET.FUNC_NO_ERROR;
                    break;
            }
            return ret;
        }
    }

    class ExpandDemoCommon
    {
        internal enum FUNC_NO
        { 
            DEL_CLIENT,
            ADD_CLIENT_INFO,
            MOD_CLIENT_INFO,
            GET_CLIENT_INFO,

            ADD_ORDER_INFO,
            MOD_ORDER_INFO,
            CANCEL_ORDER,
            REDO_ORDER,
            UNFINISH_ORDER,
            DEL_ORDER,
            GET_ORDER_INFO,
            GET_ORDER_DETAIL,

            GET_SALE_PRICE_INFO,
            DEL_SALE_PRICE_INFO,
            ADD_MOD_SALE_PRICE_INFO,

            ADD_STOCK_INFO,
            GET_STOCK_INFO,
            GET_STOCK_DETAIL,
            DEL_STOCK,

            TOTAL_COUNT,
        };
    }
}

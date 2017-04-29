using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientCommunication;
using System.Data;
using CommonLib;
using ClientLibrary;
using ClientControls;
using ExpandDemo.DataBase;
using ClientControls.Dialogs;

namespace ExpandDemo.Pages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class OrderQuery
    {
        Dictionary<string, string> dicOrderColumns = new Dictionary<string, string>()
        {
            {"客户名称","CLIENT_NAME"},
            {"客户电话","CLIENT_TEL"},
            {"客户地址","CLIENT_ADDR"},
            {"订单日期","ORDER_DATE"},
            {"订单金额","ORDER_AMT"},
            {"已付金额","DEPOSIT_AMT"},
            {"预计完结日期","EXPECT_DAYS"},
            {"安装完成日期","PRO_END_DATE"},
            {"订单完结日期","ALL_END_DATE"},
            {"订单状态","ORDER_STAT"},
            {"工人","WORKER"},
            {"备注","REMARK"},
        };

        Dictionary<string, string> dicDetailColumns = new Dictionary<string, string>() 
        {
            {"商品种类","KIND"},
            {"品名","NAME"},
            {"型号(规格)","MODEL"},
            {"尺寸","SIZE"},
            {"套线类型(开向)","STYLE_KIND"},
            {"颜色","COLOR"},
            {"数量","COUNT"},
            {"单价","PRICE"},
            {"小计","TOTAL_PRICE"},
            {"备注","REMARK"},
        };
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderQuery()
        {
            InitializeComponent();
            dgUnfinishGrid.Height = Global.WorkAreaHeight - 70;
            dbeUnFinishOrderDetail.SetSelectBarInUse(false);
            dbeUnFinishOrder.DataBoxSelectionChangeds += dbeUnfinishOrder_SelectionChanged;
            //tbFinishDateStart.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            //tbFinishDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        private void dbeUnfinishOrder_SelectionChanged(DataRow dr)
        {
            DataTable dtOrderDetail = null;
            string message = dr["ORDER_ID"].ToString();//0查未完成。 1、查已完成。 2、查全部加查询条件
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_ORDER_DETAIL, ref dtOrderDetail, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            dtOrderDetail.Columns.Add("TOTAL_PRICE");
            foreach (DataRow drDetail in dtOrderDetail.Rows)
            {
                drDetail["TOTAL_PRICE"] = "¥" + (double.Parse(drDetail["COUNT"].ToString()) *
                    double.Parse(drDetail["PRICE"].ToString())).ToString("N2").Replace(",", "");
                drDetail["COUNT"] = drDetail["COUNT"].ToString() + dicUnit[drDetail["KIND"].ToString()];
                drDetail["PRICE"] = "¥" + double.Parse(drDetail["PRICE"].ToString()).ToString("N2").Replace(",", "");
            }
            dbeUnFinishOrderDetail.InitDataBox(dicDetailColumns, dtOrderDetail, false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            DataTable dtUnfinishOrder = null;
            string message = "2\n";//0查未完成。 1、查已完成。 2、查全部加查询条件
            message += tbClientName.Text + "\n" + tbWorkerName.Text + "\n";
            message += cbOrderRate.SelectedIndex.ToString() + "\n" + cbOrderState.SelectedIndex.ToString() + "\n";
            message += tbOrderDateStart.Text + "\n" + tbOrderDateEnd.Text + "\n";
            message += tbInstallDateStart.Text + "\n" + tbInstallDateEnd.Text + "\n";
            message += tbFinishDateStart.Text + "\n" + tbFinishDateEnd.Text + "\n";
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_ORDER_INFO, ref dtUnfinishOrder, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            foreach (DataRow dr in dtUnfinishOrder.Rows)
            {
                if (dr["ORDER_STAT"].ToString() == "1")
                {
                    dr["ORDER_STAT"] = "已撤销";
                }
                else
                {
                    dr["ORDER_STAT"] = "正常";
                }
            }
            dbeUnFinishOrder.InitDataBox(dicOrderColumns, dtUnfinishOrder, false);
            dbeUnFinishOrderDetail.InitDataBox(dicDetailColumns, null, false);
            return true;
        }

        private void btnSelectClient_Click(object sender, RoutedEventArgs e)
        {
            DataTable tbAllClient = null;
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_CLIENT_INFO, ref tbAllClient);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            Dictionary<string, string> dicClientColumns = new Dictionary<string, string>() 
            {
                {"客户姓名", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_NAME.ToString()},
                {"电话号码", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_TEL.ToString()},
                {"地址", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_ADDR.ToString()},
                {"备注", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_REMARK.ToString()},
            };
            DataRow select = SelectDataBoxDialog.PopSelectTable("请选择客户", dicClientColumns, tbAllClient, false);
            if (select == null)
            {
                return;
            }
            tbClientName.Text = select[1].ToString();
        }

        Dictionary<string, string> dicUnit = new Dictionary<string, string>()
        {
            {"木门", "(套)"},
            {"合金门", "(㎡)"},
            {"垭口窗套", "(m)"},
        };

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

    }
}

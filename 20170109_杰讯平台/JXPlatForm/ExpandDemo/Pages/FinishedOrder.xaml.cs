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
using ExpandDemo.Pages.Prints;

namespace ExpandDemo.Pages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class FinishedOrder
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
        public FinishedOrder()
        {
            InitializeComponent();
            if (Global.WorkAreaHeight - ADJUST_HEIGHT > 100)
            {
                gdGoodsInfo.Height = Global.WorkAreaHeight - ADJUST_HEIGHT;
            }
            else
            {
                gdGoodsInfo.Height = 100;
            }

            gdGoodsInfo.SetSelectBarInUse(false);
            gdGoodsInfo.SetExportInUse(false);
            gdGoodsInfo.DataBoxDoubleClicks += gdGoodsInfo_DoubleClick;

            dgUnfinishGrid.Height = Global.WorkAreaHeight - 60;
            dbeUnFinishOrderDetail.SetSelectBarInUse(false);
            dbeUnFinishOrder.DataBoxDoubleClicks += dbeUnfinishOrder_DoubleClick;
            dbeUnFinishOrder.DataBoxSelectionChangeds += dbeUnfinishOrder_SelectionChanged;

            tbFinishDateStart.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            tbFinishDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        private string modOrderId = "";

        private void dbeUnfinishOrder_DoubleClick(DataRow dr)
        {
            if (dr["ORDER_STAT"].ToString() == "已撤销")
            {
                ShowMassageBox.JXMassageBox("已撤销订单不可以修改！");
                return;
            }
            modOrderId = dr["ORDER_ID"].ToString();
            tbClientName.Text = dr["CLIENT_NAME"].ToString();
            tbClientTel.Text = dr["CLIENT_TEL"].ToString();
            tbClientAddr.Text = dr["CLIENT_ADDR"].ToString();
            tbOrderDate.Text = dr["ORDER_DATE"].ToString();
            tbEndDay.Text = dr["EXPECT_DAYS"].ToString(); 
            if (tbEndDay.Text == "")
            {
                tbEndDay.Text = DateTime.Now.ToString("yyyyMMdd");
            }
            tbExpectDays.Text = (DateTime.ParseExact(tbEndDay.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                - DateTime.ParseExact(tbOrderDate.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)).Days.ToString();
            tbOrderAmt.Text = dr["ORDER_AMT"].ToString();
            tbDepositAmt.Text = dr["DEPOSIT_AMT"].ToString();
            tbRemark.Text = dr["REMARK"].ToString();
            tbInstallDate.Text = dr["PRO_END_DATE"].ToString();
            tbInstallWorker.Text = dr["WORKER"].ToString();
            tbAllEndDate.Text = dr["ALL_END_DATE"].ToString();
            RefreshAllEnd();

            DataTable dtDetailInfo = dbeUnFinishOrderDetail.GetBindTable().Copy();
            gdGoodsInfo.InitDataBox(dicDetailColumns, dtDetailInfo, false);

            spSelectOrder.Visibility = Visibility.Collapsed;
            spDefaultShow.Visibility = Visibility.Visible;
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

        private void gdGoodsInfo_DoubleClick(DataRow dr)
        {
            string kind = dr["KIND"].ToString();
            cbGoodsKind.Text = kind;
            if (kind == "垭口窗套")
            {
                cbGoodsName.Text = dr["NAME"].ToString();
                cbGoodsModel.Text = dr["MODEL"].ToString();
            }
            else
            {
                tbGoodsName.Text = dr["NAME"].ToString();
                tbGoodsModel.Text = dr["MODEL"].ToString();
            }
            tbSize.Text = dr["SIZE"].ToString();
            cbStyleKinds.Text = dr["STYLE_KIND"].ToString();
            tbColor.Text = dr["COLOR"].ToString();
            tbCount.Text = dr["COUNT"].ToString();
            tbPrice.Text = dr["PRICE"].ToString();
            tbTotalPrice.Text = dr["TOTAL_PRICE"].ToString();
            tbDetialRemark.Text = dr["REMARK"].ToString();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            if (cbStyleKinds.ItemsSource == null)
            {
                cbGoodsKind_SelectionChanged(null, null);
            }
            DateTime checkDay;
            if (!DateTime.TryParseExact(tbFinishDateStart.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkDay))
            {
                ShowMassageBox.JXMassageBox("请输入正确的日期，格式（YYYYMMDD）！");
                return false;
            }
            if (!DateTime.TryParseExact(tbFinishDateEnd.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkDay))
            {
                ShowMassageBox.JXMassageBox("请输入正确的日期，格式（YYYYMMDD）！");
                return false;
            }
            DataTable dtUnfinishOrder = null;
            string message = "1\n";//0查未完成。 1、查已完成。 2、查全部加查询条件
            message += tbFinishDateStart.Text + "\n" + tbFinishDateEnd.Text + "\n";
            message += (bool)cbxShowCancel.IsChecked ? "1" : "0";
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
                    dr["ORDER_STAT"] = "已完成";
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
            tbClientTel.Text = select[2].ToString();
            tbClientAddr.Text = select[3].ToString();
        }

        Dictionary<string, string> dicUnit = new Dictionary<string, string>()
        {
            {"木门", "(套)"},
            {"合金门", "(㎡)"},
            {"垭口窗套", "(m)"},
        };

        private void cbGoodsKind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbChangeForward == null)
            {
                return;
            }
            string kind = (cbGoodsKind.SelectedItem as ComboBoxItem).Content.ToString();
            tbkCountTag.Text = "数量" + dicUnit[kind];

            if (kind == "木门" || kind == "垭口窗套")
            {
                tbChangeForward.Text = "套线类型";
                cbStyleKinds.ItemsSource = new List<string>() 
                {
                    "平板", "斜边", "欧式"
                };
            }
            else
            {
                tbChangeForward.Text = "开  向";
                cbStyleKinds.ItemsSource = new List<string>() 
                {
                    "左开", "右开", "左前右后", "左后右前", "中开"
                };
            }
            cbStyleKinds.SelectedIndex = 0;

            tbGoodsName.Visibility = Visibility.Collapsed;
            cbAlloyGoodsName.Visibility = Visibility.Collapsed;
            cbGoodsName.Visibility = Visibility.Collapsed;
            tbGoodsModel.Visibility = Visibility.Collapsed;
            cbGoodsModel.Visibility = Visibility.Collapsed;

            switch (kind)
            {
                case "垭口窗套":
                    cbGoodsName.Visibility = Visibility.Visible;
                    cbGoodsModel.Visibility = Visibility.Visible;
                    break;
                case "合金门":
                    cbAlloyGoodsName.Visibility = Visibility.Visible;
                    tbGoodsModel.Visibility = Visibility.Visible;
                    break;
                case "木门":
                    tbGoodsName.Visibility = Visibility.Visible;
                    tbGoodsModel.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            RefreshCount();
        }
        /// <summary>
        /// 修整高度
        /// </summary>
        private double ADJUST_HEIGHT = 480;

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtGoodsInfo = gdGoodsInfo.GetBindTable();
            string goodsName = "";
            string goodsModel = "";
            switch (cbGoodsKind.Text)
            {
                case "木门":
                    goodsName = tbGoodsName.Text;
                    goodsModel = tbGoodsModel.Text;
                    break;
                case "合金门":
                    goodsName = cbAlloyGoodsName.Text;
                    goodsModel = tbGoodsModel.Text;
                    break;
                case "垭口窗套":
                    goodsName = cbGoodsName.Text;
                    goodsModel = cbGoodsModel.Text;
                    break;
            }
            if (goodsName.Trim() == "")
            {
                ShowMassageBox.JXMassageBox("请输入品名！");
                return;
            }
            if (goodsModel.Trim() == "")
            {
                ShowMassageBox.JXMassageBox("请输入型号！");
                return;
            }
            if (!tbSize.Text.Contains("*"))
            {
                ShowMassageBox.JXMassageBox("正确的尺寸！");
                return;
            }
            int check = 0;
            string[] sizes = tbSize.Text.Split('*');
            foreach (string size in sizes)
            {
                if (!int.TryParse(size, out check))
                {
                    ShowMassageBox.JXMassageBox("正确的尺寸！");
                    return;
                }
            }
            List<string> newRow = new List<string>() 
            {
                modOrderId,
                cbGoodsKind.Text,//{"商品种类","KIND"},
                goodsName.Trim(),//{"品名","NAME"},
                goodsModel.Trim(),//{"型号","MODEL"},
                tbSize.Text,//{"尺寸","SIZE"},
                cbStyleKinds.Text,//{"套线类型(开向)","STYLE_KIND"},
                tbColor.Text,//{"颜色","COLOR"},
                tbCount.Text + dicUnit[cbGoodsKind.Text],//{"数量","COUNT"},
                tbPrice.Text,//{"单价","PRICE"},
                tbDetialRemark.Text,//{"备注","REMARK"},
                tbTotalPrice.Text,//{"小计","TOTAL_PRICE"},
            };
            dtGoodsInfo.Rows.Add(newRow.ToArray());
            gdGoodsInfo.UpdateAlert();
            UpdateTotalAmt();
        }

        private void UpdateTotalAmt()
        {
            DataTable dtGoodsInfo = gdGoodsInfo.GetBindTable();
            double total = 0;
            foreach (DataRow dr in dtGoodsInfo.Rows)
            {
                total += double.Parse(dr["TOTAL_PRICE"].ToString().Substring(1));
            }
            tbOrderAmt.Text = total.ToString("N2");
        }

        private void btnModDetail_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = gdGoodsInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择所要修改的行！");
                return;
            }
            string goodsName = "";
            string goodsModel = "";
            switch (cbGoodsKind.Text)
            {
                case "木门":
                    goodsName = tbGoodsName.Text;
                    goodsModel = tbGoodsModel.Text;
                    break;
                case "合金门":
                    goodsName = cbAlloyGoodsName.Text;
                    goodsModel = tbGoodsModel.Text;
                    break;
                case "垭口窗套":
                    goodsName = cbGoodsName.Text;
                    goodsModel = cbGoodsModel.Text;
                    break;
            }
            if (goodsName.Trim() == "")
            {
                ShowMassageBox.JXMassageBox("请输入品名！");
                return;
            }
            if (goodsModel.Trim() == "")
            {
                ShowMassageBox.JXMassageBox("请输入型号！");
                return;
            }
            if (!tbSize.Text.Contains("*"))
            {
                ShowMassageBox.JXMassageBox("正确的尺寸！");
                return;
            }
            int check = 0;
            string[] sizes = tbSize.Text.Split('*');
            foreach (string size in sizes)
            {
                if (!int.TryParse(size, out check))
                {
                    ShowMassageBox.JXMassageBox("正确的尺寸！");
                    return;
                }
            }
            List<string> newRow = new List<string>() 
            {
                modOrderId,
                cbGoodsKind.Text,//{"商品种类","KIND"},
                goodsName.Trim(),//{"品名","NAME"},
                goodsModel.Trim(),//{"型号","MODEL"},
                tbSize.Text,//{"尺寸","SIZE"},
                cbStyleKinds.Text,//{"套线类型(开向)","STYLE_KIND"},
                tbColor.Text,//{"颜色","COLOR"},
                tbCount.Text + dicUnit[cbGoodsKind.Text],//{"数量","COUNT"},
                tbPrice.Text,//{"单价","PRICE"},
                tbDetialRemark.Text,//{"备注","REMARK"},
                tbTotalPrice.Text,//{"小计","TOTAL_PRICE"},
            };
            for (int i = 0; i < newRow.Count; i++)
            {
                dr[i] = newRow[i];
            }
            UpdateTotalAmt();
        }

        private void btnDelDetail_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = gdGoodsInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择所要删除的行！");
                return;
            }
            DataTable dtGoodsInfo = gdGoodsInfo.GetBindTable();
            dtGoodsInfo.Rows.Remove(dr);
            gdGoodsInfo.UpdateAlert();
            UpdateTotalAmt();
        }

        private void tbSize_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshCount();
        }

        private void RefreshCount()
        {
            string size = tbSize.Text.Trim();
            if (size == "" || !size.Contains('*'))
            {
                return;
            }
            string[] sizes = size.Split('*');
            int width = 0;
            int height = 0;
            if (!int.TryParse(sizes[1], out width))
            {
                return;
            }
            if (!int.TryParse(sizes[0], out height))
            {
                return;
            }
            string kind = (cbGoodsKind.SelectedItem as ComboBoxItem).Content.ToString();
            string model = (cbGoodsModel.SelectedItem as ComboBoxItem).Content.ToString();
            switch (kind)
            {
                case "木门":
                    tbCount.Text = "1";
                    break;
                case "合金门":
                    tbCount.Text = ((double)width * height / 1000000).ToString("N2");
                    break;
                case "垭口窗套":
                    switch (model)
                    {
                        case "两高一宽":
                            tbCount.Text = (((double)width + (double)height * 2) / 1000).ToString("N2");
                            break;
                        case "两宽一高":
                            tbCount.Text = (((double)width * 2 + (double)height) / 1000).ToString("N2");
                            break;
                        case "一宽一高":
                            tbCount.Text = (((double)width + (double)height) / 1000).ToString("N2");
                            break;
                        case "两宽两高":
                            tbCount.Text = (((double)width * 2 + (double)height * 2) / 1000).ToString("N2");
                            break;
                    }
                    break;
            }
        }

        private void cbGoodsModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbCount == null)
            {
                return;
            }
            RefreshCount();
        }

        private void tbCount_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshTotalAmt();
        }

        private void RefreshTotalAmt()
        {
            if (tbCount.Text == "")
            {
                return;
            }
            double count = double.Parse(tbCount.Text);
            double amt = double.Parse(tbPrice.Text.Substring(1));
            double total = count * amt;
            tbTotalPrice.Text = total.ToString();
        }

        private void tbPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshTotalAmt();
        }

        private void tbOrderDate_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshEndDay();
        }

        private void tbExpectDays_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshEndDay();
        }

        private void RefreshEndDay()
        {
            DateTime orderDay;
            if (!DateTime.TryParseExact(tbOrderDate.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out orderDay))
            {
                return;
            }
            int days;
            if (!int.TryParse(tbExpectDays.Text, out days))
            {
                return;
            }
            tbEndDay.Text = orderDay.AddDays(int.Parse(tbExpectDays.Text)).ToString("yyyyMMdd");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckElments())
            {
                return;
            }
            if (!SaveOrderInfo())
            {
                return;
            }
            PrintPreView();
        }

        private void btnPrintView_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckElments())
            {
                return;
            }
            PrintPreView();
            if (ShowMassageBox.JXMassageBox("是否保存订单？", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) == ShowMassageBox.SHOW_RES.SELECT_CLOSE)
            {
                return;
            }
            SaveOrderInfo();
        }

        private bool SaveOrderInfo()
        {
            //客户信息0-2
            string message = tbClientName.Text.Trim() + "\n" + tbClientTel.Text.Trim() + "\n";
            message += tbClientAddr.Text.Trim() + "\n";

            //商品信息3
            DataTable dtGoodsInfo = gdGoodsInfo.GetBindTable();
            foreach (DataRow dr in dtGoodsInfo.Rows)
            {
                //                cbGoodsKind.Text,//{"商品种类","KIND"},
                //goodsName.Trim(),//{"品名","NAME"},
                //goodsModel.Trim(),//{"型号","MODEL"},
                //tbSize.Text,//{"尺寸","SIZE"},
                //cbStyleKinds.Text,//{"套线类型(开向)","STYLE_KIND"},
                //tbColor.Text,//{"颜色","COLOR"},
                //tbCount.Text + dicUnit[cbGoodsKind.Text],//{"数量","COUNT"},
                //tbPrice.Text,//{"单价","PRICE"},
                //tbTotalPrice.Text,//{"小计","TOTAL_PRICE"},
                //tbRemark.Text//{"备注","REMARK"},
                string count = dr["COUNT"].ToString();
                count = count.Substring(0, count.IndexOf('('));
                message += dr["KIND"].ToString() + "\t" + dr["NAME"].ToString() + "\t" + dr["MODEL"].ToString() + "\t";
                message += dr["SIZE"].ToString() + "\t" + dr["STYLE_KIND"].ToString() + "\t" + dr["COLOR"].ToString() + "\t";
                message += count + "\t" + dr["PRICE"].ToString().Substring(1) + "\t" + dr["REMARK"].ToString() + "\r";
            }
            message += "\n";

            //订单基础信息4-8
            message += modOrderId + "\n";
            message += tbOrderDate.Text + "\n";
            message += tbInstallDate.Text.Trim() + "\n";
            message += tbAllEndDate.Text.Trim() + "\n";
            message += tbEndDay.Text + "\n";
            //9-12
            message += tbOrderAmt.Text.Substring(1) + "\n";
            message += tbDepositAmt.Text.Substring(1) + "\n";
            message += tbInstallWorker.Text.Trim() + "\n";
            message += tbRemark.Text.Trim() + "\n";
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
            (int)ExpandDemoCommon.FUNC_NO.MOD_ORDER_INFO, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            ShowMassageBox.JXMassageBox("保存成功");
            return true;
        }

        private void PrintPreView()
        {
            List<UserControl> prints = OrderPrint.SetPrintInfo(tbClientName.Text, tbClientTel.Text, tbOrderDate.Text,
                tbClientAddr.Text, tbOrderAmt.Text, tbDepositAmt.Text, gdGoodsInfo.GetBindTable());
            ClientPrintDialog.ShowPrintPreView(prints, 21, 29.7);
        }

        private bool CheckElments()
        {
            if (tbClientName.Text == "")
            {
                ShowMassageBox.JXMassageBox("请输入客户姓名！");
                return false;
            }
            if (tbClientTel.Text != "")
            {
                if (!InputTypeControl.IsTelephone(tbClientTel.Text))
                {
                    ShowMassageBox.JXMassageBox("请输入正确的手机号码！");
                    return false;
                }
            }
            if (double.Parse(tbDepositAmt.Text.Substring(1)) > double.Parse(tbOrderAmt.Text.Substring(1)))
            {
                ShowMassageBox.JXMassageBox("已付金额不能大于订单金额！");
                return false;
            }
            DataTable dtGoodsInfo = gdGoodsInfo.GetBindTable();
            if (dtGoodsInfo.Rows.Count == 0)
            {
                ShowMassageBox.JXMassageBox("请添加商品信息！");
                return false;
            }
            DateTime orderDay;
            if (!DateTime.TryParseExact(tbOrderDate.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out orderDay))
            {
                ShowMassageBox.JXMassageBox("请输入正确的日期，格式（YYYYMMDD）！");
                return false;
            }
            if (tbExpectDays.Text == "")
            {
                tbExpectDays.Text = "0";
            }
            return true;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
            spDefaultShow.Visibility = Visibility.Collapsed;
            spSelectOrder.Visibility = Visibility.Visible;
        }

        private void tbDepositAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshAllEnd();
        }

        private void RefreshAllEnd()
        {
            if (tbDepositAmt.Text == tbOrderAmt.Text &&
                tbInstallDate.Text.Length == 8)
            {
                tbAllEndDate.Text = DateTime.Now.ToString("yyyyMMdd");
                tbxAllEnd.Visibility = Visibility.Visible;
                tbAllEndDate.Visibility = Visibility.Visible;
            }
            else
            {
                tbAllEndDate.Text = "";
                tbxAllEnd.Visibility = Visibility.Collapsed;
                tbAllEndDate.Visibility = Visibility.Collapsed;
            }
        }

        private void tbInstallDate_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshAllEnd();
        }

        private void btnModOrder_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dbeUnFinishOrder.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择要修改的订单！");
                return;
            }
            dbeUnfinishOrder_DoubleClick(dr);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void btnReDoOrder_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dbeUnFinishOrder.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择要恢复的订单！");
                return;
            }
            if (dr["ORDER_STAT"].ToString() != "已撤销")
            {
                ShowMassageBox.JXMassageBox("请选择已撤销订单！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("确定要恢复所选订单！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            string orderId = dr["ORDER_ID"].ToString();
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.REDO_ORDER, orderId);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }

        private void btnCancelFinish_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dbeUnFinishOrder.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择要撤销完成状态的订单！");
                return;
            }
            if (dr["ORDER_STAT"].ToString() != "已完成")
            {
                ShowMassageBox.JXMassageBox("请选择已完成订单！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("确定要撤销所选订单的完成状态！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            string orderId = dr["ORDER_ID"].ToString();
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.UNFINISH_ORDER, orderId);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void CheckPrices(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            switch (cbGoodsKind.Text)
            {
                case "木门":
                case "合金门":
                    price = CommonFunctions.GetGoodsPrice(cbGoodsKind.Text, tbGoodsName.Text, tbGoodsModel.Text);
                    break;
                case "垭口窗套":
                    price = CommonFunctions.GetGoodsPrice(cbGoodsKind.Text, cbGoodsName.Text, cbGoodsModel.Text);
                    break;
            }
            tbPrice.Text = price.ToString();
            RefreshTotalAmt();
        }

        private void btnDelOrder_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dbeUnFinishOrder.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择要删除的订单！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("删除订单会连同删除该订单的货单信息，确定要删除所选订单！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            string orderId = dr["ORDER_ID"].ToString();
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.DEL_ORDER, orderId);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }
    }
}

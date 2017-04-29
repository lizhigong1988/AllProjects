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
    public partial class AddNewOrder
    {

        Dictionary<string, string> dicColumns = new Dictionary<string, string>() 
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
        public AddNewOrder()
        {
            InitializeComponent();
            DataTable dtBind = new DataTable();
            dtBind.Columns.Add("KIND");
            dtBind.Columns.Add("NAME");
            dtBind.Columns.Add("MODEL");
            dtBind.Columns.Add("SIZE");
            dtBind.Columns.Add("STYLE_KIND");
            dtBind.Columns.Add("COLOR");
            dtBind.Columns.Add("COUNT");
            dtBind.Columns.Add("PRICE");
            dtBind.Columns.Add("TOTAL_PRICE");
            dtBind.Columns.Add("REMARK");
            gdGoodsInfo.SetSelectBarInUse(false);
            gdGoodsInfo.SetExportInUse(false);
            gdGoodsInfo.DataBoxDoubleClicks += gdGoodsInfo_DoubleClick;
            gdGoodsInfo.InitDataBox(dicColumns, dtBind, false);

            tbOrderDate.Text = DateTime.Now.ToString("yyyyMMdd");
            tbEndDay.Text = DateTime.Now.ToString("yyyyMMdd");
            if (Global.WorkAreaHeight - ADJUST_HEIGHT > 100)
            {
                gdGoodsInfo.Height = Global.WorkAreaHeight - ADJUST_HEIGHT;
            }
            else
            {
                gdGoodsInfo.Height = 100;
            }
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
        private double ADJUST_HEIGHT = 450;

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
                cbGoodsKind.Text,//{"商品种类","KIND"},
                goodsName.Trim(),//{"品名","NAME"},
                goodsModel.Trim(),//{"型号","MODEL"},
                tbSize.Text,//{"尺寸","SIZE"},
                cbStyleKinds.Text,//{"套线类型(开向)","STYLE_KIND"},
                tbColor.Text,//{"颜色","COLOR"},
                tbCount.Text + dicUnit[cbGoodsKind.Text],//{"数量","COUNT"},
                tbPrice.Text,//{"单价","PRICE"},
                tbTotalPrice.Text,//{"小计","TOTAL_PRICE"},
                tbDetialRemark.Text//{"备注","REMARK"},
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
                cbGoodsKind.Text,//{"商品种类","KIND"},
                goodsName.Trim(),//{"品名","NAME"},
                goodsModel.Trim(),//{"型号","MODEL"},
                tbSize.Text,//{"尺寸","SIZE"},
                cbStyleKinds.Text,//{"套线类型(开向)","STYLE_KIND"},
                tbColor.Text,//{"颜色","COLOR"},
                tbCount.Text + dicUnit[cbGoodsKind.Text],//{"数量","COUNT"},
                tbPrice.Text,//{"单价","PRICE"},
                tbTotalPrice.Text,//{"小计","TOTAL_PRICE"},
                tbDetialRemark.Text//{"备注","REMARK"},
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
            //客户信息
            string message = tbClientName.Text.Trim() + "\n" + tbClientTel.Text.Trim() + "\n";
            message += tbClientAddr.Text.Trim() + "\n";

            //商品信息
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
            
            //订单基础信息
            message += tbOrderDate.Text + "\n";
            message += tbEndDay.Text + "\n";
            message += tbOrderAmt.Text.Substring(1) + "\n";
            message += tbDepositAmt.Text.Substring(1) + "\n";
            message += tbRemark.Text.Trim();
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
            (int)ExpandDemoCommon.FUNC_NO.ADD_ORDER_INFO, message);
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
                ShowMassageBox.JXMassageBox("定金金额不能大于订单金额！");
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

    }
}

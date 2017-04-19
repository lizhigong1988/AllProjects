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

namespace ExpandDemo.Pages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class OrderPriceList
    {
        /// <summary>
        /// 修整高度
        /// </summary>
        private double ADJUST_HEIGHT = 160;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderPriceList()
        {
            InitializeComponent();
            dgGoodsPrices.Height = Global.WorkAreaHeight - ADJUST_HEIGHT;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            DataTable tbAllPrice = CommonFunctions.GetAllGoodsPrice();
            if (tbAllPrice == null)
            {
                ShowMassageBox.JXMassageBox("获取价格表信息失败！");
                return false;
            }
            Dictionary<string, string> dicColumns = new Dictionary<string, string>() 
            {
                {"商品种类", DEMO_SALE_PRICE_LIST.TABLE_COLUMS.KIND.ToString()},
                {"品名", DEMO_SALE_PRICE_LIST.TABLE_COLUMS.NAME.ToString()},
                {"型号", DEMO_SALE_PRICE_LIST.TABLE_COLUMS.MODEL.ToString()},
                {"单价", DEMO_SALE_PRICE_LIST.TABLE_COLUMS.PRICE.ToString()},
            };
            dgGoodsPrices.InitDataBox(dicColumns, tbAllPrice, false);
            spDefaultShow.Visibility = Visibility.Visible;
            gbAddMod.Visibility = Visibility.Collapsed;
            
            return true;
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string message = cbGoodsKind.Text.Trim() + "\n";

            switch (cbGoodsKind.Text)
            {
                case "木门":
                case "合金门":
                    message += tbGoodsName.Text + "\n";
                    message += tbGoodsModel.Text + "\n";
                    break;
                case "垭口窗套":
                    message += cbGoodsName.Text + "\n";
                    message += cbGoodsModel.Text + "\n";
                    break;
                default:
                    return;
            }
            message += tbPrice.Text.Substring(1);
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
            (int)ExpandDemoCommon.FUNC_NO.ADD_MOD_SALE_PRICE_INFO, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }

        private void btnAddPrice_Click(object sender, RoutedEventArgs e)
        {
            gbAddMod.Header = "增加价格信息";
            cbGoodsKind.SelectedIndex = 0;
            tbGoodsName.Text = "";
            tbGoodsModel.Text = "";
            tbPrice.Text = "";
            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        private void btnModPrice_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dgGoodsPrices.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择一行数据！");
                return;
            }

            gbAddMod.Header = "修改价格信息";
            cbGoodsKind.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.KIND.ToString()].ToString();
            switch(cbGoodsKind.Text)
            {
                case "木门":
                case "合金门":
                    tbGoodsName.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.NAME.ToString()].ToString();
                    tbGoodsModel.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.MODEL.ToString()].ToString();
                    tbPrice.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.PRICE.ToString()].ToString();
                    break;
                case "垭口窗套":
                    cbGoodsName.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.NAME.ToString()].ToString();
                    cbGoodsModel.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.MODEL.ToString()].ToString();
                    tbPrice.Text = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.PRICE.ToString()].ToString();
                    break;
                default:
                    return;
            }
            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        private void btnDelPrice_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dgGoodsPrices.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择所要删除的数据！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("确定要删除所选数据！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            string delInfo = dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.KIND.ToString()].ToString() + "\n";
            delInfo += dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.NAME.ToString()].ToString() + "\n";
            delInfo += dr[DEMO_SALE_PRICE_LIST.TABLE_COLUMS.MODEL.ToString()].ToString();
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.DEL_SALE_PRICE_INFO, delInfo);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        Dictionary<string, string> dicUnit = new Dictionary<string, string>()
        {
            {"木门", "(套)"},
            {"合金门", "(㎡)"},
            {"垭口窗套", "(m)"},
        };

        private void cbGoodsKind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string kind = (cbGoodsKind.SelectedItem as ComboBoxItem).Content.ToString();
            tbUnit.Text = "单位：" + dicUnit[kind];
            if (kind == "垭口窗套")
            {
                tbGoodsName.Visibility = Visibility.Collapsed;
                cbGoodsName.Visibility = Visibility.Visible;
                tbGoodsModel.Visibility = Visibility.Collapsed;
                cbGoodsModel.Visibility = Visibility.Visible;
            }
            else
            {
                cbGoodsName.Visibility = Visibility.Collapsed;
                tbGoodsName.Visibility = Visibility.Visible;
                cbGoodsModel.Visibility = Visibility.Collapsed;
                tbGoodsModel.Visibility = Visibility.Visible;
            }
        }
    }
}

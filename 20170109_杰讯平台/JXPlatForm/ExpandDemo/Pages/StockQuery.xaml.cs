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
    public partial class StockQuery
    {
        Dictionary<string, string> dicOrderColumns = new Dictionary<string, string>() 
        {
            {"货单日期","STOCK_DATE"},
            {"货单金额","TOTAL_AMT"},
            {"木门货单备注","REMARK1"},
            {"合金货单备注","REMARK2"},
        };

        Dictionary<string, string> dicWoodenStockColumns = new Dictionary<string, string>()
        {
            {"序号","COUNT_INDEX"},
            {"产品型号","MODEL"},
            {"材质","TEXTURE"},
            {"线条","LINE"},
            {"套板","PLATE"},

            {"颜色","COLOR"},
            {"尺寸（高*宽*厚）","SIZE"},
            {"数量","COUNT"},
            {"单价","PRICE"},
            {"合计","TOTAL_PRICE"},

            {"备注","REMARK"},
        };
        Dictionary<string, string> dicAlloyStockColumns = new Dictionary<string, string>()
        {
            {"编号","COUNT_INDEX"},
            {"材料","TEXTURE"},
            {"颜色","COLOR"},
            {"品名","NAME"},
            {"规格（高*宽）","SIZE"},

            {"套数","COUNT"},
            {"扇数","DOOR_COUNT"},
            {"百叶","SHUTTER"},
            {"吊脚","SUSPEND"},
            {"玻璃型号","GLASS_MODEL"},

            {"开向","FORWORD"},
            {"单价","PRICE"},
            {"合计","TOTAL_PRICE"},
            {"备注","REMARK"},
        };
        /// <summary>
        /// 构造函数
        /// </summary>
        public StockQuery()
        {
            InitializeComponent();
            dgStockGrid.Height = Global.WorkAreaHeight - 70;
            dbeStockInfo.DataBoxSelectionChangeds += dbeStockInfo_SelectionChanged;
            tbStockDateStart.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            tbStockDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        private void dbeStockInfo_SelectionChanged(DataRow dr)
        {
            DataTable dtStockDetail = null;
            string message = dr["STOCK_ID"].ToString() + "\n" + dr["ORDER_ID"].ToString() + "\n";
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_STOCK_DETAIL, ref dtStockDetail, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            DataTable dtWoodenStock = dtStockDetail.Clone();
            DataTable dtAlloyStock = dtStockDetail.Clone();
            foreach (DataRow drDetail in dtStockDetail.Rows)
            {
                if (drDetail["KIND"].ToString() == "0")
                {
                    dtWoodenStock.Rows.Add(drDetail.ItemArray);
                }
                else
                {
                    dtAlloyStock.Rows.Add(drDetail.ItemArray);
                }
            }
            InitDataBox(dicWoodenStockColumns, dtWoodenStock, dgWoodenNote);
            InitDataBox(dicAlloyStockColumns, dtAlloyStock, dgAlloyNote);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            DataTable dtStockInfo = null;
            string message = tbStockDateStart.Text + "\n" + tbStockDateEnd.Text + "\n";
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_STOCK_INFO, ref dtStockInfo, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            dbeStockInfo.InitDataBox(dicOrderColumns, dtStockInfo, false);
            InitDataBox(dicWoodenStockColumns, null, dgWoodenNote);
            InitDataBox(dicAlloyStockColumns, null, dgAlloyNote);
            return true;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        /// <param name="dicColumns">key:显示列名;value:绑定列名</param>
        /// <param name="bindData">数据</param>
        private void InitDataBox(Dictionary<string, string> dicColumns, DataTable bindData, DataGrid dgShowData)
        {
            dgShowData.Columns.Clear();
            foreach (var dic in dicColumns)
            {
                DataGridTextColumn dgc = new DataGridTextColumn();
                switch (dic.Value)
                {
                    case "COUNT_INDEX":
                        dgc.IsReadOnly = true;
                        break;
                    case "TOTAL_PRICE":
                        dgc.IsReadOnly = true;
                        break;
                    default:
                        break;
                }
                dgc.MaxWidth = 400;
                dgc.Header = dic.Key;
                System.Windows.Data.Binding binding = null;
                binding = new System.Windows.Data.Binding(dic.Value);
                binding.Mode = System.Windows.Data.BindingMode.TwoWay;
                dgc.Binding = binding;
                dgShowData.Columns.Add(dgc);
            }
            dgShowData.DataContext = bindData;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dbeStockInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择要删除的货单！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("确定要删除所选货单！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            string stockId = dr["STOCK_ID"].ToString();
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.DEL_STOCK, stockId);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }
    }
}

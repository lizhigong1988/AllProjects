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
    public partial class AddNewStock
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
        public AddNewStock()
        {
            InitializeComponent();
            tbStockDate.Text = DateTime.Now.ToString("yyyyMMdd");
            gridStockNote.Height = Global.WorkAreaHeight - 260;

            dgUnfinishGrid.Height = Global.WorkAreaHeight - 60;
            dbeUnFinishOrderDetail.SetSelectBarInUse(false);
            dbeUnFinishOrder.DataBoxDoubleClicks += dbeUnfinishOrder_DoubleClick;
            dbeUnFinishOrder.DataBoxSelectionChangeds += dbeUnfinishOrder_SelectionChanged;
        }

        private string modOrderId = "";

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
            string message = "0";//0查未完成。 1、查已完成。 2、查全部加查询条件
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_ORDER_INFO, ref dtUnfinishOrder, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            dbeUnFinishOrder.InitDataBox(dicOrderColumns, dtUnfinishOrder, false);
            dbeUnFinishOrderDetail.InitDataBox(dicDetailColumns, null, false);
            return true;
        }

        DataRow modDr;

        private void dbeUnfinishOrder_DoubleClick(DataRow dr)
        {
            modOrderId = dr["ORDER_ID"].ToString();
            DataTable dtWoodenStock = new DataTable();
            foreach (var dic in dicWoodenStockColumns)
            {
                switch (dic.Value)
                {
                    case "COUNT_INDEX":
                        dtWoodenStock.Columns.Add(dic.Value, typeof(int));
                        break;
                    case "COUNT":
                        dtWoodenStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    case "PRICE":
                        dtWoodenStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    case "DOOR_COUNT":
                        dtWoodenStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    case "TOTAL_PRICE":
                        dtWoodenStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    default:
                        dtWoodenStock.Columns.Add(dic.Value);
                        break;
                }
            }
            DataTable dtAlloyStock = new DataTable();
            foreach (var dic in dicAlloyStockColumns)
            {
                switch (dic.Value)
                {
                    case "COUNT_INDEX":
                        dtAlloyStock.Columns.Add(dic.Value, typeof(int));
                        break;
                    case "COUNT":
                        dtAlloyStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    case "DOOR_COUNT":
                        dtAlloyStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    case "PRICE":
                        dtAlloyStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    case "TOTAL_PRICE":
                        dtAlloyStock.Columns.Add(dic.Value, typeof(double));
                        break;
                    default:
                        dtAlloyStock.Columns.Add(dic.Value);
                        break;
                }
            }
            DataTable dtOrderDetail = null;
            string message = dr["ORDER_ID"].ToString();//0查未完成。 1、查已完成。 2、查全部加查询条件
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_ORDER_DETAIL, ref dtOrderDetail, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            if (dtOrderDetail == null)
            {
                return;
            }
            int woodenIndex = 1;
            int alloyIndex = 1;
            foreach (DataRow drDetail in dtOrderDetail.Rows)
            {
                if (drDetail["KIND"].ToString() == "木门")
                {
                    List<object> newRow = new List<object>() 
                    {
                        woodenIndex.ToString(),
                        drDetail["MODEL"].ToString(),
                        "",
                        "",
                        "",
                        
                        drDetail["COLOR"].ToString(),
                        drDetail["SIZE"].ToString(),
                        drDetail["COUNT"].ToString(),
                        "0",
                        "0",

                        drDetail["REMARK"].ToString(),
                    };
                    dtWoodenStock.Rows.Add(newRow.ToArray());
                    woodenIndex++;
                }
                else if(drDetail["KIND"].ToString() == "垭口窗套")
                {
                    List<object> newRow;
                    string[] sizes = drDetail["SIZE"].ToString().Split('*');
                    #region 单包套
                    if (drDetail["NAME"].ToString() == "单包套")
                    {
                        switch (drDetail["MODEL"].ToString())
                        {
                            #region 两高一宽
                            case "两高一宽":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + (int.Parse(sizes[2]) * 2).ToString(),//高
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "单包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + sizes[2],//宽
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                            #region 两宽一高
                            case "两宽一高":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "单包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + sizes[2],//高
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + (int.Parse(sizes[2]) * 2).ToString(),//宽
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                            #region 一宽一高
                            case "一宽一高":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "单包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + sizes[2],//高
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "单包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + sizes[2],//宽
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                            #region 两宽两高
                            case "两宽两高":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + (int.Parse(sizes[2]) * 2).ToString(),//高
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + (int.Parse(sizes[2]) * 2).ToString(),//宽
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                        }
                    }
                    if (drDetail["NAME"].ToString() == "双包套")
                    {
                        switch (drDetail["MODEL"].ToString())
                        {
                            #region 两高一宽
                            case "两高一宽":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + sizes[2],//高
                                        "2",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + sizes[2],//宽
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                            #region 两宽一高
                            case "两宽一高":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + sizes[2],//高
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + sizes[2],//宽
                                        "2",
                                        "0",
                                        "0",

                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                            #region 一宽一高
                            case "一宽一高":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + sizes[2],//高
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + sizes[2],//宽
                                        "1",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                            #region 两宽两高
                            case "两宽两高":
                                if (sizes[0] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[0] + "*" + sizes[2],//高
                                        "2",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                if (sizes[1] != "0")
                                {
                                    newRow = new List<object>() 
                                    {
                                        woodenIndex.ToString(),
                                        "双包套",
                                        "",
                                        "",
                                        "",
                        
                                        drDetail["COLOR"].ToString(),
                                        sizes[1] + "*" + sizes[2],//宽
                                        "2",
                                        "0",
                                        "0",
                                    
                                        drDetail["REMARK"].ToString(),
                                    };
                                    dtWoodenStock.Rows.Add(newRow.ToArray());
                                    woodenIndex++;
                                }
                                break;
                            #endregion
                        }
                    }
                    #endregion
                }
                else
                {
                    List<object> newRow = new List<object>() 
                    {
                        alloyIndex,
                        "",
                        drDetail["COLOR"].ToString(),
                        drDetail["NAME"].ToString(),
                        drDetail["SIZE"].ToString(),
                        
                        double.Parse(drDetail["COUNT"].ToString()),
                        1,
                        "",
                        "",
                        "",
                        
                        drDetail["STYLE_KIND"].ToString(),
                        0,
                        0,
                        drDetail["REMARK"].ToString(),
                    };
                    dtAlloyStock.Rows.Add(newRow.ToArray());
                    alloyIndex++;
                }
            }
            InitDataBox(dicWoodenStockColumns, dtWoodenStock, dgWoodenNote);
            InitDataBox(dicAlloyStockColumns, dtAlloyStock, dgAlloyNote);
            RefreshTotalAmt();
            modDr = dr;
            spSelectOrder.Visibility = Visibility.Collapsed;
            spDefaultShow.Visibility = Visibility.Visible;
        }

        Dictionary<string, string> dicUnit = new Dictionary<string, string>()
        {
            {"木门", "(套)"},
            {"合金门", "(㎡)"},
            {"垭口窗套", "(m)"},
        };

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

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

        private void btnCreateStock_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dbeUnFinishOrder.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择要修改的订单！");
                return;
            }
            dbeUnfinishOrder_DoubleClick(dr);
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

        private void btnPrintView_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckElments())
            {
                return;
            }
            PrintPreView();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckElments())
            {
                return;
            }
            if (!SaveStockInfo())
            {
                return;
            }
            PrintPreView();
        }

        private void PrintPreView()
        {
            List<UserControl> prints;
            if ((dgWoodenNote.DataContext as DataTable).Rows.Count > 0)
            {
                prints = WoodenStockPrint.SetPrintInfo(modDr, dgWoodenNote.DataContext as DataTable,
                    tbStockDate.Text, tbWoodenRemark.Text);
                ClientPrintDialog.ShowPrintPreView(prints, 21, 29.7);
            }
            if ((dgAlloyNote.DataContext as DataTable).Rows.Count > 0)
            {
                prints = AlloyStockPrint.SetPrintInfo(modDr, dgAlloyNote.DataContext as DataTable,
                    tbStockDate.Text, tbAlloyRemark.Text);
                ClientPrintDialog.ShowPrintPreView(prints, 21, 29.7);
            }
        }

        private bool SaveStockInfo()
        {
            DataTable dtAlloyStock = dgAlloyNote.DataContext as DataTable;
            DataTable dtWoodenStock = dgWoodenNote.DataContext as DataTable;
            string newStockId = Guid.NewGuid().ToString();
            //基础信息01
            string message = newStockId + "\n" + modOrderId + "\n";

            //木门货单2
            foreach (DataRow dr in dtWoodenStock.Rows)
            {
                message += newStockId + "\t" + modOrderId + "\t" + dr["COUNT_INDEX"].ToString() + "\t";
                message += dr["MODEL"].ToString() + "\t" + dr["TEXTURE"].ToString() + "\t" + dr["LINE"].ToString() + "\t";
                message += dr["PLATE"].ToString() + "\t" + dr["COLOR"].ToString() + "\t" + dr["SIZE"].ToString() + "\t";
                message += dr["COUNT"].ToString() + "\t" + dr["PRICE"].ToString() + "\t" + dr["REMARK"].ToString() + "\t";
                message += "\t\t\t\t\t\t0\r";
            }
            message += "\n";
            //合金门信息3
            foreach (DataRow dr in dtAlloyStock.Rows)
            {
                message += newStockId + "\t" + modOrderId + "\t" + dr["COUNT_INDEX"].ToString() + "\t";
                message += "\t" + dr["TEXTURE"].ToString() + "\t\t";
                message += "\t" + dr["COLOR"].ToString() + "\t" + dr["SIZE"].ToString() + "\t";
                message += dr["COUNT"].ToString() + "\t" + dr["PRICE"].ToString() + "\t" + dr["REMARK"].ToString() + "\t";
                message += dr["NAME"].ToString() + "\t" + dr["DOOR_COUNT"].ToString() + "\t" + dr["SHUTTER"].ToString() + "\t";
                message += dr["SUSPEND"].ToString() + "\t" + dr["GLASS_MODEL"].ToString() + "\t" + dr["FORWORD"].ToString() + "\t1\r";
            }
            message += "\n";
            //其他信息456
            message += tbStockDate.Text + "\n";
            message += tbStockAmt.Text.Substring(1) + "\n";
            message += tbWoodenRemark.Text.Trim() + "\n";
            message += tbAlloyRemark.Text.Trim() + "\n";
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
            (int)ExpandDemoCommon.FUNC_NO.ADD_STOCK_INFO, message);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            ShowMassageBox.JXMassageBox("保存成功");
            return true;
        }

        private bool CheckElments()
        {
            DateTime orderDay;
            if (!DateTime.TryParseExact(tbStockDate.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out orderDay))
            {
                ShowMassageBox.JXMassageBox("请输入正确的日期，格式（YYYYMMDD）！");
                return false;
            }
            return true;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
            spDefaultShow.Visibility = Visibility.Collapsed;
            spSelectOrder.Visibility = Visibility.Visible;
        }

        private void dgWoodenNote_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string editColum = e.Column.SortMemberPath;
            string value = (e.EditingElement as TextBox).Text;
            DataRow dr = (e.Row.Item as DataRowView).Row;
            double count = 0;
            double price = 0;
            if (editColum == "COUNT")
            {
                if (double.TryParse(value, out count) &&
                    double.TryParse(dr["PRICE"].ToString(), out price))
                {
                    dr["TOTAL_PRICE"] = (count * price).ToString("N2");
                }
            }
            if (editColum == "PRICE")
            {
                if (double.TryParse(dr["COUNT"].ToString(), out count) &&
                    double.TryParse(value, out price))
                {
                    dr["TOTAL_PRICE"] = (count * price).ToString("N2");
                }
            }
            RefreshTotalAmt();
        }

        private void dgAlloyNote_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string editColum = e.Column.SortMemberPath;
            string value = (e.EditingElement as TextBox).Text;
            DataRow dr = (e.Row.Item as DataRowView).Row;
            double count = 0;
            double price = 0;
            if (editColum == "COUNT")
            {
                if (double.TryParse(value, out count) &&
                    double.TryParse(dr["PRICE"].ToString(), out price))
                {
                    dr["TOTAL_PRICE"] = (count * price).ToString("N2");
                }
            }
            if (editColum == "PRICE")
            {
                if (double.TryParse(dr["COUNT"].ToString(), out count) &&
                    double.TryParse(value, out price))
                {
                    dr["TOTAL_PRICE"] = (count * price).ToString("N2");
                }
            }
            RefreshTotalAmt();
        }

        private void RefreshTotalAmt()
        {
            DataTable dtAlloyStock = dgAlloyNote.DataContext as DataTable;
            DataTable dtWoodenStock = dgWoodenNote.DataContext as DataTable;
            double totalAmt = 0;
            foreach (DataRow dr in dtWoodenStock.Rows)
            {
                totalAmt += double.Parse(dr["TOTAL_PRICE"].ToString());
            }
            foreach (DataRow dr in dtAlloyStock.Rows)
            {
                totalAmt += double.Parse(dr["TOTAL_PRICE"].ToString());
            }
            tbStockAmt.Text = totalAmt.ToString();
        }

        private void btnAddWoodenStock_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtWoodenStock = dgWoodenNote.DataContext as DataTable;
            List<object> newRow = new List<object>() 
                    {
                        (dtWoodenStock.Rows.Count + 1).ToString(),
                        "",
                        "",
                        "",
                        "",
                        
                        "",
                        "",
                        "0",
                        "0",
                        "0",

                        "",
                    };
            dtWoodenStock.Rows.Add(newRow.ToArray());
        }

        private void btnAddAlloyStock_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtAlloyStock = dgAlloyNote.DataContext as DataTable;
            List<object> newRow = new List<object>() 
                    {
                        (dtAlloyStock.Rows.Count + 1).ToString(),
                        "",
                        "",
                        "",
                        "",
                        
                        0,
                        1,
                        "",
                        "",
                        "",
                        
                        "",
                        0,
                        0,
                        "",
                    };
            dtAlloyStock.Rows.Add(newRow.ToArray());
        }

        private void btnDelWoodenStock_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgWoodenNote.SelectedItem as DataRowView;
            if (drv == null)
            {
                ShowMassageBox.JXMassageBox("请选择所要删除的行！");
                return;
            }
            DataTable dtWoodenStock = dgWoodenNote.DataContext as DataTable;
            dtWoodenStock.Rows.Remove(drv.Row);
            int index = 1;
            foreach (DataRow dr in dtWoodenStock.Rows)
            {
                dr["COUNT_INDEX"] = index.ToString();
                index++;
            }
            RefreshTotalAmt();
        }

        private void btnDelAlloyStock_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgAlloyNote.SelectedItem as DataRowView;
            if (drv == null)
            {
                ShowMassageBox.JXMassageBox("请选择所要删除的行！");
                return;
            }
            DataTable dtAlloyStock = dgAlloyNote.DataContext as DataTable;
            dtAlloyStock.Rows.Remove(drv.Row);
            int index = 1;
            foreach (DataRow dr in dtAlloyStock.Rows)
            {
                dr["COUNT_INDEX"] = index.ToString();
                index++;
            }
            RefreshTotalAmt();
        }
    }
}

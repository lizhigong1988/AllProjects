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
using System.Data;
using System.IO;

namespace ClientControls.Controls
{
    /// <summary>
    /// DataBoxExt.xaml 的交互逻辑
    /// </summary>
    public partial class DataBoxExt : UserControl
    {
        public DataBoxExt()
        {
            InitializeComponent();
        }

        DataTable dtBindTable = null;

        Dictionary<string, string> dicBindTableColums = null;

        /// <summary>
        /// 初始化表格
        /// </summary>
        /// <param name="dicColumns">key:显示列名;value:绑定列名</param>
        /// <param name="bindData">数据</param>
        public void InitDataBox(Dictionary<string, string> dicColumns, DataTable bindData, bool mutiSelect = false)
        {
            dgShowData.Columns.Clear();
            cbSelectAll.Visibility = Visibility.Collapsed;
            if (mutiSelect && bindData != null)
            {
                if (!bindData.Columns.Contains("SELECT"))
                {
                    bindData.Columns.Add("SELECT", typeof(bool));
                }
                foreach (DataRow dr in bindData.Rows)
                {
                    dr["SELECT"] = false;
                }
                DataGridCheckBoxColumn dgc = new DataGridCheckBoxColumn();
                dgc.Header = "选择";
                System.Windows.Data.Binding binding = null;
                binding = new System.Windows.Data.Binding("SELECT");
                binding.Mode = System.Windows.Data.BindingMode.TwoWay;
                dgc.Binding = binding;
                dgShowData.Columns.Add(dgc);
                cbSelectAll.Visibility = Visibility.Visible;
            }
            foreach (var dic in dicColumns)
            {
                DataGridTextColumn dgc = new DataGridTextColumn();
                dgc.IsReadOnly = true;
                dgc.MaxWidth = 400;
                dgc.Header = dic.Key;
                System.Windows.Data.Binding binding = null;
                binding = new System.Windows.Data.Binding(dic.Value);
                binding.Mode = System.Windows.Data.BindingMode.OneWay;
                dgc.Binding = binding;
                dgShowData.Columns.Add(dgc);
            }
            dgShowData.DataContext = bindData;
            dicBindTableColums = dicColumns;
            dtBindTable = bindData;
            UpdateAlert();
        }

        public DataRow GetSelectSingleRow()
        {
            DataRowView drv = dgShowData.SelectedItem as DataRowView;
            if (drv == null)
            {
                return null;
            }
            return drv.Row;
        }

        public DataRow[] GetSelectMultiRows()
        {
            DataTable dt = dgShowData.DataContext as DataTable;
            if(!dt.Columns.Contains("SELECT"))
            {
                return null;
            }
            List<DataRow> listDr = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SELECT"].ToString().ToUpper() == "TRUE")
                {
                    listDr.Add(dr);
                }
            }
            if (listDr.Count == 0)
            {
                DataRow single = GetSelectSingleRow();
                if (single != null)
                {
                    listDr.Add(single);
                }
            }
            return listDr.ToArray();
        }

        public void SetSelectBarInUse(bool isEnable)
        {
            if (isEnable)
            {
                gdSelectBar.Height = 40;
            }
            else
            {
                gdSelectBar.Height = 0;
            }
        }

        public void SetExportInUse(bool isEnable)
        {
            if (isEnable)
            {
                btnExport.Visibility = Visibility.Visible;
            }
            else
            {
                btnExport.Visibility = Visibility.Collapsed;
            }
        }

        public DataTable GetBindTable()
        {
            return dtBindTable;
        }

        public delegate void DataBoxDoubleClick(DataRow dr);
        public DataBoxDoubleClick DataBoxDoubleClicks;
        public delegate void DataBoxSelectionChanged(DataRow dr);
        public DataBoxSelectionChanged DataBoxSelectionChangeds;

        private void dgShowData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView drv = dgShowData.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            if (DataBoxDoubleClicks != null)
            {
                DataBoxDoubleClicks(drv.Row);
            }
        }

        public void UpdateAlert()
        {
            DataTable dt = dgShowData.DataContext as DataTable;
            if (dt != null)
            {
                tbAlert.Text = string.Format("共【{0}】条数据", dt.Rows.Count.ToString());
            }
        }

        private void dgShowData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgShowData.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            if (DataBoxSelectionChangeds != null)
            {
                DataBoxSelectionChangeds(drv.Row);
            }
        }



        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (tbSearchInfo.Text == "" || dtBindTable == null)
            {
                dgShowData.DataContext = dtBindTable;
                return;
            }
            DataTable queryTable = dtBindTable.Clone();
            foreach (DataRow dr in dtBindTable.Rows)
            {
                object[] rowItems = dr.ItemArray;
                foreach (object obj in rowItems)
                {
                    if (!(obj is bool))
                    {
                        if (-1 != obj.ToString().IndexOf(tbSearchInfo.Text.Trim()))
                        {
                            queryTable.Rows.Add(dr.ItemArray);
                            break;
                        }
                    }
                }
            }
            dgShowData.DataContext = queryTable;
            UpdateAlert();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = dgShowData.DataContext as DataTable;
            if (dt == null)
            {
                MessageBox.Show("没有要导出的内容！");
                return;
            }
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("没有要导出的内容！");
                return;
            }
            string exportInfo = "";
            foreach (var dic in dicBindTableColums)
            {
                exportInfo += "\"" + dic.Key + "\",";
            }
            exportInfo += "\n";
            foreach (DataRow dr in dt.Rows)
            {
                foreach (var dic in dicBindTableColums)
                {
                    exportInfo += "\"" + dr[dic.Value].ToString() + "\",";
                }
                exportInfo += "\n";
            }

            var saveDialog = new System.Windows.Forms.SaveFileDialog { FileName = "导出文件.csv" };
            saveDialog.ShowDialog();
            var outputFile = saveDialog.FileName;
            if (outputFile.IndexOf(":", System.StringComparison.Ordinal) < 0)
                return; //被点了取消   
            //File.Copy(DataBaseTool_SQLite3.DATABASE_FILE, outputFile);
            File.WriteAllText(outputFile, exportInfo, Encoding.Default);
            MessageBox.Show("导出完成！");
        }
    }
}

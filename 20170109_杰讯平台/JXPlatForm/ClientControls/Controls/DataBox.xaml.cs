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

namespace ClientControls.Controls
{
    /// <summary>
    /// DataBox.xaml 的交互逻辑
    /// </summary>
    public partial class DataBox : UserControl
    {
        /// <summary>
        /// 表格备份
        /// </summary>
        private DataTable table;

        /// <summary>
        /// 隐藏的列标题
        /// </summary>
        List<string> listHideColumns = new List<string>();

        /// <summary>
        /// 修改了隐藏状态
        /// </summary>
        bool hideStatChanged = false;

        /// <summary>
        /// checkbox增加事件
        /// </summary>
        bool bCheckBoxUpdated = false;

        public DataBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置表格
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mutiSelect"></param>
        public void SetTable(DataTable data, bool mutiSelect = false)
        {
            if (null != data)
            {
                btnSelectAll.Visibility = Visibility.Collapsed;
                table = new DataTable();
                hideStatChanged = true;
                bCheckBoxUpdated = false;
                listHideColumns.Clear();

                if (mutiSelect)
                {
                    btnSelectAll.Visibility = Visibility.Visible;
                        DataColumn dcColumn = new DataColumn("选择", typeof(bool)) { ReadOnly = false };
                        table.Columns.Add(dcColumn);
                    bCheckBoxUpdated = true;
                }
                foreach (DataColumn column in data.Columns)
                {
                    if (column.ColumnName != "选择")
                    {
                        table.Columns.Add(new DataColumn(column.ColumnName, typeof(string)) { ReadOnly = true });
                    }
                }
                foreach (DataRow dr in data.Rows)
                {
                    List<object> listObj = dr.ItemArray.ToList();
                    if (data.Columns[0].ColumnName != "选择")
                    {
                        if (mutiSelect)
                        {
                            listObj.Insert(0, false);
                        }
                    }
                    table.Rows.Add(listObj.ToArray());
                }
                dataGrid.ItemsSource = table.DefaultView;
                tbAlert.Text = "共【" + table.Rows.Count.ToString() + "】条数据";
            }
        }

        /// <summary>
        /// 隐藏列
        /// </summary>
        /// <param name="column"></param>
        public void SetColumnVisible(string columnName, bool visibility)
        {
            if (visibility)
            {
                if (listHideColumns.Contains(columnName))
                {
                    listHideColumns.Remove(columnName);
                    hideStatChanged = true;
                }
            }
            else
            {
                if (!listHideColumns.Contains(columnName))
                {
                    listHideColumns.Add(columnName);
                    hideStatChanged = true;
                }
            }
            dataGrid_LayoutUpdated(null, null);
        }

        private void dataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (hideStatChanged)
            {
                if (dataGrid.Columns.Count > 0)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        int index = table.Columns.IndexOf(column);
                        if (listHideColumns.Contains(column.ColumnName))
                        {
                            dataGrid.Columns[index].Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            dataGrid.Columns[index].Visibility = Visibility.Visible;
                        }
                    }
                    hideStatChanged = false;
                }
            }
            if (bCheckBoxUpdated)
            {
                if (dataGrid.Columns.Count > 0)
                {
                    foreach (DataRowView drv in dataGrid.Items)
                    {
                        object content = dataGrid.Columns[0].GetCellContent(drv);
                        if (content is CheckBox)
                        {
                            CheckBox cb = content as CheckBox;
                            cb.Tag = drv;
                            cb.Margin = new Thickness(3);
                            cb.Checked -= cb_Click;
                            cb.Checked += cb_Click;
                            cb.Unchecked -= cb_Click;
                            cb.Unchecked += cb_Click;
                            bCheckBoxUpdated = false;
                        }
                    }
                    cb_Click(null, null);
                }

            }
        }
        /// <summary>
        /// 翻译列
        /// </summary>
        /// <param name="p"></param>
        /// <param name="dicTansRole"></param>
        public void TranslateColumn(string columnName, Dictionary<string, string> dicTansRole)
        {
            int index = table.Columns.IndexOf(columnName);
            if (index == -1)
            {
                return;
            }
            table.Columns[index].ReadOnly = false;
            foreach (DataRow dr in table.Rows)
            { 
                if(dicTansRole.ContainsKey(dr[columnName].ToString()))
                {
                    dr[columnName] = dicTansRole[dr[columnName].ToString()];
                }
            }
            table.Columns[index].ReadOnly = true;
        }
        /// <summary>
        /// 获取当前选择行
        /// </summary>
        /// <returns></returns>
        public DataRow GetSelectSingleRow()
        {
            DataRowView drv = dataGrid.SelectedItem as DataRowView;
            if (null == drv)
            {
                return null;
            }
            return drv.Row;
        }

        /// <summary>
        /// 获取当前选择多行
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetSelectMultiRows()
        {
            List<DataRow> list = new List<DataRow>();
            foreach (DataRow dr in table.Rows)
            {
                if (dr[0].ToString().ToUpper() == "TRUE")
                {
                    list.Add(dr);
                }
            }
            if (list.Count == 0)
            {
                DataRowView drv = dataGrid.SelectedItem as DataRowView;
                if (null != drv)
                {
                    list.Add(drv.Row);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 全选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool select = (bool)btnSelectAll.IsChecked;
            foreach (DataRow dr in table.Rows)
            {
                dr[0] = select;
            }
        }

        /// <summary>
        /// 选择多行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            if (sender is CheckBox)
            {
                ((sender as CheckBox).Tag as DataRowView).Row[0] = (bool)(sender as CheckBox).IsChecked;
            }
            foreach (DataRow dr in table.Rows)
            {
                
                if (dr[0].ToString().ToUpper() == "TRUE")
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            if (i == table.Rows.Count)
            {
                btnSelectAll.IsChecked = true;
            }
            else
            {
                btnSelectAll.IsChecked = false;
            }
        }
    }
}

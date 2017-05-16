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
using 项目管理.Connect;

namespace 项目管理.Pages
{
    /// <summary>
    /// SystemMange.xaml 的交互逻辑
    /// </summary>
    public partial class SystemMange : UserControl
    {
        public SystemMange()
        {
            InitializeComponent();
            RefreshTable();
        }

        private void RefreshTable()
        {
            DataTable dt = CommunicationHelper.GetSystemInfo();
            if (dt == null)
            {
                MessageBox.Show("查询系统信息失败");
            }
            dgSystemsInfo.DataContext = dt;
        }

        private void dgSystemsInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgSystemsInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            tbSysName.Text = drv.Row["SYS_NAME"].ToString();
            tbSysManager1.Text = drv.Row["USER_NAME1"].ToString();
            tbSysManager2.Text = drv.Row["USER_NAME2"].ToString();
            tbSysRemark.Text = drv.Row["REMARK"].ToString();
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (tbSysName.Text.Trim() == "")
            {
                MessageBox.Show("请输入系统名称");
                return;
            }
            if (!CommunicationHelper.AddNewSystem(tbSysName.Text.Trim(), tbSysManager1.Text.Trim(),
                tbSysManager2.Text.Trim(), tbSysRemark.Text.Trim()))
            {
                MessageBox.Show("添加系统失败！");
                return;
            }
            RefreshTable();
        }

        private void btnMod_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgSystemsInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择需要修改的系统");
                return;
            }

            if (tbSysName.Text.Trim() == "")
            {
                MessageBox.Show("请输入系统名称");
                return;
            }
            if (!CommunicationHelper.ModSystem(drv.Row[0].ToString(), tbSysName.Text.Trim(), tbSysManager1.Text.Trim(),
                tbSysManager2.Text.Trim(), tbSysRemark.Text.Trim()))
            {
                MessageBox.Show("修改系统失败！");
                return;
            }
            RefreshTable();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgSystemsInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择需要删除的系统");
                return;
            }
            if (MessageBox.Show("确认删除所选择的系统？已有项目或人员信息的系统将不能被删除！", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            if (!CommunicationHelper.DelSystem(drv.Row[0].ToString()))
            {
                MessageBox.Show("删除系统失败！");
                return;
            }
            RefreshTable();
        }
    }
}

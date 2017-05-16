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
using 项目管理.Tools;
using 项目管理.Connect;

namespace 项目管理.Pages
{
    /// <summary>
    /// UserMange.xaml 的交互逻辑
    /// </summary>
    public partial class UserMange : UserControl
    {
        public UserMange()
        {
            InitializeComponent();

            Dictionary<string,string> dicSysInfo = CommunicationHelper.GetAllSysDic();
            dicSysInfo.Add("", "无");
            cbSystem.ItemsSource = dicSysInfo;
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            if (GlobalFuns.LoginSysId != "")
            {
                cbSystem.SelectedValue = GlobalFuns.LoginSysId;
                cbSystem.IsEnabled = false;
            }
            RefreshTable();
        }

        private void dgUserInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataRowView drv = dgUserInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            tbUserName.Text = drv.Row["USER_NAME"].ToString();
            cbSystem.Text = drv.Row["SYS_NAME"].ToString();
            cbRole.Text = drv.Row["USER_ROLE"].ToString();
            tbUserCompany.Text = drv.Row["COMPANY"].ToString();
            tbRemark.Text = drv.Row["REMARK"].ToString();
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text.Trim() == "")
            {
                MessageBox.Show("请输入人员名称");
                return;
            }
            if (!CommunicationHelper.AddNewUser(tbUserName.Text.Trim(), cbSystem.SelectedValue.ToString(),
                cbRole.Text.Trim(), tbUserCompany.Text.Trim(), tbRemark.Text.Trim()))
            {
                MessageBox.Show("添加系统失败！");
                return;
            }
            RefreshTable();
        }

        private void RefreshTable()
        {
            DataTable dt = CommunicationHelper.GetUserInfo();
            if (dt == null)
            {
                MessageBox.Show("查询人员信息失败");
            }
            dgUserInfo.DataContext = dt;
        }

        private void cbSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSystem.SelectedValue == null)
            {
                cbRole.ItemsSource = new List<string>() { "项目经理", "开发人员" };
                cbRole.SelectedIndex = 0;
                return;
            }
            string sysId = cbSystem.SelectedValue.ToString();
            if (sysId == "")
            {
                cbRole.ItemsSource = new List<string>() { "部门领导" };
            }
            else
            {
                cbRole.ItemsSource = new List<string>() { "项目经理", "开发人员" };
            }
            cbRole.SelectedIndex = 0;
        }

        private void cbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRole.SelectedItem == null)
            {
                return;
            }
            switch (cbRole.SelectedItem.ToString())
            {
                case "部门领导":
                case "项目经理":
                    tbUserCompany.Text = "焦作中旅银行股份有限公司";
                    tbUserCompany.IsEnabled = false;
                    break;
                case "开发人员":
                    tbUserCompany.IsEnabled = true;
                    break;
            }
        }

        private void btnMod_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgUserInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要修改的人员");
                return;
            }
            if (GlobalFuns.LoginSysId != "")
            {
                if (GlobalFuns.LoginSysId != drv.Row["SYS_ID"].ToString())
                {
                    MessageBox.Show("只能修改本系统的人员信息");
                    return;
                }
            }
            if (tbUserName.Text.Trim() != drv.Row["USER_NAME"].ToString())
            {
                MessageBox.Show("用户名不能修改");
                return;
            }
            if (!CommunicationHelper.ModUserInfo(tbUserName.Text.Trim(), drv.Row["USER_PSW"].ToString(), cbSystem.SelectedValue.ToString(),
                cbRole.Text.Trim(), tbUserCompany.Text.Trim(), tbRemark.Text.Trim()))
            {
                MessageBox.Show("修改人员信息失败！");
                return;
            }
            RefreshTable();
        }
    }
}

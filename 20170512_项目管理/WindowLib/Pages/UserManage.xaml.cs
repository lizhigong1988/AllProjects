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
using WindowLib.Tools;
using WindowLib.Connect;

namespace WindowLib.Pages
{
    /// <summary>
    /// UserManage.xaml 的交互逻辑
    /// </summary>
    public partial class UserManage : UserControl
    {
        public UserManage()
        {
            InitializeComponent();

            cbRole.ItemsSource = new List<string>() { "部门领导", "PMO", "项目经理", "开发人员" };
            cbRole.SelectedIndex = 0;

            Dictionary<string,string> dicSysInfo = CommunicationHelper.GetAllSysDic();
            cbSystem.ItemsSource = dicSysInfo;
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            cbSystem.SelectedIndex = 0;
            Dictionary<string, string> dicQuerySysInfo = new Dictionary<string, string>() 
            {
                {"0", "全部"},
                {"-1", "无"}
            };
            foreach (var dic in dicSysInfo)
            {
                dicQuerySysInfo.Add(dic.Key, dic.Value);
            }
            cbQuerySystem.ItemsSource = dicQuerySysInfo;
            cbQuerySystem.SelectedValuePath = "Key";
            cbQuerySystem.DisplayMemberPath = "Value";
            if (GlobalFuns.LoginSysId != "")
            {
                cbQuerySystem.SelectedValue = GlobalFuns.LoginSysId;
            }
            else
            {
                cbQuerySystem.SelectedIndex = 0;
            }
        }

        private void dgUserInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgUserInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            tbUserName.Text = drv.Row["USER_NAME"].ToString();
            tbEmail.Text = drv.Row["EMAIL"].ToString();
            cbRole.Text = drv.Row["USER_ROLE"].ToString();
            tbUserCompany.Text = drv.Row["COMPANY"].ToString();
            tbRemark.Text = drv.Row["REMARK"].ToString();
            DataTable dtSys = CommunicationHelper.GetUserSysInfo(tbUserName.Text);
            dgUserSys.DataContext = dtSys;
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text.Trim() == "")
            {
                MessageBox.Show("请输入人员名称");
                return;
            }
            string userSysInfo = "";
            DataTable dt = dgUserSys.DataContext as DataTable;
            if (cbRole.Text != "部门领导" && cbRole.Text != "PMO")
            {
                if(dt == null)
                {
                    MessageBox.Show("请添加人员所属系统");
                    return;
                }
                if(dt.Rows.Count == 0)
                {
                    MessageBox.Show("请添加人员所属系统");
                    return;
                }
                foreach(DataRow dr in dt.Rows)
                {
                    userSysInfo += dr["SYS_ID"].ToString() + "\r";
                }
            }
            if (!CommunicationHelper.AddNewUser(tbUserName.Text.Trim(), tbEmail.Text,
                cbRole.Text.Trim(), tbUserCompany.Text.Trim(), tbRemark.Text.Trim(), userSysInfo))
            {
                MessageBox.Show("添加人员信息失败！");
                return;
            }
            RefreshTable();
        }

        DataTable allDt = null;

        private void RefreshTable()
        {
            if (cbQuerySystem.SelectedValue == null)
            {
                return;
            }
            allDt = CommunicationHelper.GetUserInfo("", cbQuerySystem.SelectedValue.ToString());
            btnSelectQuery_Click(null, null);
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
                case "PMO":
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
            if (tbUserName.Text.Trim() != drv.Row["USER_NAME"].ToString())
            {
                MessageBox.Show("用户名不能修改");
                return;
            }
            string userSysInfo = "";
            DataTable dt = dgUserSys.DataContext as DataTable;
            if (cbRole.Text != "部门领导" && cbRole.Text != "PMO")
            {
                if (dt == null)
                {
                    MessageBox.Show("请添加人员所属系统");
                    return;
                }
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("请添加人员所属系统");
                    return;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    userSysInfo += dr["SYS_ID"].ToString() + "\r";
                }
            }
            if (!CommunicationHelper.ModUserInfo(tbUserName.Text.Trim(), tbEmail.Text, 
                drv.Row["USER_PSW"].ToString(),
                cbRole.Text.Trim(), tbUserCompany.Text.Trim(), tbRemark.Text.Trim(), userSysInfo))
            {
                MessageBox.Show("修改人员信息失败！");
                return;
            }
            RefreshTable();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgUserInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要删除的人员");
                return;
            }
            if (MessageBox.Show("确定要删除所选人员？", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            if (!CommunicationHelper.DelUserInfo(drv.Row["USER_NAME"].ToString()))
            {
                MessageBox.Show("删除人员信息失败！");
                return;
            }
            RefreshTable();
        }

        private void btnSelectQuery_Click(object sender, RoutedEventArgs e)
        {
            if (allDt == null)
            {
                //RefreshTable();
                return;
            }
            if (tbSelectKey.Text.Trim() == "")
            {
                dgUserInfo.DataContext = allDt;
                return;
            }
            DataTable queryDt = allDt.Clone();
            foreach (DataRow dr in allDt.Rows)
            {
                for (int i = 0; i < allDt.Columns.Count; i++ )
                {
                    if (dr[i].ToString().Contains(tbSelectKey.Text))
                    {
                        queryDt.Rows.Add(dr.ItemArray);
                        break;
                    }
                }
            }
            dgUserInfo.DataContext = queryDt;
        }

        private void btnAddSys_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = dgUserSys.DataContext as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("USER_NAME");
                dt.Columns.Add("SYS_ID");
                dt.Columns.Add("SYS_ID1");
                dt.Columns.Add("SYS_NAME");
                dt.Columns.Add("USER_NAME1");
                dt.Columns.Add("USER_NAME2");
                dt.Columns.Add("REMARK");
            }
            string selectSys = cbSystem.Text;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SYS_NAME"].ToString() == selectSys)
                {
                    MessageBox.Show("不能重复添加");
                    return;
                }
            }
            string sysId = cbSystem.SelectedValue.ToString();
            DataTable dtSysInfo = CommunicationHelper.GetSystemInfo(sysId);
            if (dtSysInfo == null)
            {
                MessageBox.Show("获取系统信息失败");
                return;
            }
            DataRow sysDr = dtSysInfo.Rows[0];
            dt.Rows.Add(new string[] { 
                tbUserName.Text,
                sysDr[0].ToString(),
                sysDr[0].ToString(),
                sysDr[1].ToString(), 
                sysDr[2].ToString(), 
                sysDr[3].ToString(), 
                sysDr[4].ToString()});
        }

        private void btnDelSys_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgUserSys.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请先选择所要删除的系统");
                return;
            }
            drv.Row.Table.Rows.Remove(drv.Row);
        }

        private void cbQuerySystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTable();
        }
    }
}

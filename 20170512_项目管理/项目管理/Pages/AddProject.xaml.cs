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
using 项目管理.DataBases;
using System.IO;

namespace 项目管理.Pages
{
    /// <summary>
    /// AddEngineerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AddProject : UserControl
    {
        string CurDirectory = "";

        public AddProject()
        {
            InitializeComponent();
            Refresh();
            CurDirectory = Guid.NewGuid().ToString().Replace("-","");
            if (GlobalFuns.LoginSysId != "")
            {
                cbIsMain.Visibility = Visibility.Collapsed;
            }
        }

        private void Refresh()
        {
            cbDemandDepart.ItemsSource = DataBaseManager.GetHisDeparts();
            tbDemandDate.Text = DateTime.Now.ToString("yyyyMMdd");

            cbProKinds.ItemsSource = new List<string>() { "新项目", "功能优化" };
            cbProKinds.SelectedIndex = 0;

            cbProStage.ItemsSource = new List<string>() { 
                "软需编写及评审", "系统开发/单元测试" , "集成测试", "SIT测试", "UAT测试"
            };
            cbProStage.SelectedIndex = 1;

            cbProState.ItemsSource = new List<string>() { 
                "正常", "延迟" , "关闭", "暂停"
            };
            cbProState.SelectedIndex = 0;

            cbSystem.ItemsSource = DataBaseManager.GetAllSysDic();
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            cbSystem.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbDemandName.Text == "")
            {
                MessageBox.Show("请输入项目名称！");
                return;
            }
            DataTable dt = dgProSysInfo.DataContext as DataTable;
            bool hasMain = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["IS_MAIN"].ToString() == "是")
                {
                    hasMain = true;
                    break;
                }
            }
            if (!hasMain)
            {
                MessageBox.Show("请添加一个主系统！");
                return;
            }
            if (!DataBaseManager.AddNewProject(tbDemandName.Text, cbDemandDepart.Text, tbDemandDate.Text,
                tbExpectDate.Text, cbProKinds.Text, cbProStage.Text, cbProState.Text,
                tbProgressNote.Text, tbTestPerson.Text, tbBusinessPerson.Text, tbRemark.Text, dt))
            {
                MessageBox.Show("保存项目失败！");
                return;
            }
            MessageBox.Show("保存成功！");
        }

        private void btnAddNewSys_Click(object sender, RoutedEventArgs e)
        {
            float days = 0;
            if (!float.TryParse(tbSysEstimatedDays.Text, out days))
            {
                MessageBox.Show("请输入正确的预计工作量，单位天！");
                tbSysEstimatedDays.Text = "0";
                return;
            }
            DataTable dt = DataBaseManager.GetSystemInfo(cbSystem.SelectedValue.ToString());
            if (dt == null)
            {
                MessageBox.Show("获取系统信息失败");
                return;
            }
            DataTable dtSysInfo = dgProSysInfo.DataContext as DataTable;
            if (dtSysInfo == null)
            {
                dtSysInfo = new DataTable();
                dtSysInfo.Columns.Add("SYS_ID");
                dtSysInfo.Columns.Add("SYS_NAME");
                dtSysInfo.Columns.Add("MANAGER1");
                dtSysInfo.Columns.Add("MANAGER2");
                dtSysInfo.Columns.Add("IS_MAIN");
                dtSysInfo.Columns.Add("ESTIMATE_DAYS");
                dtSysInfo.Columns.Add("REMARK");
                dgProSysInfo.DataContext = dtSysInfo;
            }
            bool isMain = false;
            if (GlobalFuns.LoginSysId != "")
            {
                if (GlobalFuns.LoginSysId == cbSystem.SelectedValue.ToString())
                {
                    isMain = true;
                }
            }
            else
            {
                isMain = (bool)cbIsMain.IsChecked;
                if (isMain)
                {
                    foreach (DataRow dr in dtSysInfo.Rows)
                    {
                        if (dr["IS_MAIN"].ToString() == "是")
                        {
                            MessageBox.Show("已经存在主导系统！");
                            return;
                        }
                    }
                }
            }

            foreach (DataRow dr in dtSysInfo.Rows)
            {
                if (dr["SYS_ID"].ToString() == cbSystem.SelectedValue.ToString())
                {
                    MessageBox.Show("系统不能重复添加！");
                    return;
                }
            }
            List<string> newRow = new List<string>()
            {
                dt.Rows[0]["SYS_ID"].ToString(),
                dt.Rows[0]["SYS_NAME"].ToString(),
                dt.Rows[0]["USER_NAME1"].ToString(),
                dt.Rows[0]["USER_NAME2"].ToString(),
                (bool)isMain ? "是" : "否",
                days.ToString(),
                tbSysRemark.Text,
            };
            dtSysInfo.Rows.Add(newRow.ToArray());
            cbIsMain.IsChecked = false;
            UpdateDays();
        }

        private void UpdateDays()
        {
            DataTable dtSysInfo = dgProSysInfo.DataContext as DataTable;
            float days = 0;
            foreach (DataRow dr in dtSysInfo.Rows)
            {
                days += float.Parse(dr["ESTIMATE_DAYS"].ToString());
            }
            tbEstimatedDays.Text = days.ToString();
        }

        private void btnModSys_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgProSysInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择修改的系统！");
                return;
            }
            if (cbSystem.SelectedValue.ToString() != drv.Row["SYS_ID"].ToString())
            {
                MessageBox.Show("系统不能修改！");
                return;
            }
            float days = 0;
            if (!float.TryParse(tbSysEstimatedDays.Text, out days))
            {
                MessageBox.Show("请输入正确的预计工作量，单位天！");
                tbSysEstimatedDays.Text = "0";
                return;
            }
            DataTable dt = DataBaseManager.GetSystemInfo(cbSystem.SelectedValue.ToString());
            if (dt == null)
            {
                MessageBox.Show("获取系统信息失败");
                return;
            }
            DataTable dtSysInfo = dgProSysInfo.DataContext as DataTable;
            if (dtSysInfo == null)
            {
                MessageBox.Show("获取系统信息失败");
                return;
            }
            bool isMain = false;
            if (GlobalFuns.LoginSysId != "")
            {
                if (GlobalFuns.LoginSysId == cbSystem.SelectedValue.ToString())
                {
                    isMain = true;
                }
            }
            else
            {
                isMain = (bool)cbIsMain.IsChecked;
                if (isMain)
                {
                    foreach (DataRow dr in dtSysInfo.Rows)
                    {
                        if (dr["IS_MAIN"].ToString() == "是" && 
                            dr["SYS_ID"].ToString() != drv.Row["SYS_ID"].ToString())
                        {
                            MessageBox.Show("已经存在主导系统！");
                            return;
                        }
                    }
                }
            }

            foreach (DataRow dr in dtSysInfo.Rows)
            {
                if (dr["SYS_ID"].ToString() == cbSystem.SelectedValue.ToString() &&
                    dr["SYS_ID"].ToString() != drv.Row["SYS_ID"].ToString())
                {
                    MessageBox.Show("系统不能重复添加！");
                    return;
                }
            }
            drv.Row["SYS_ID"] = dt.Rows[0]["SYS_ID"].ToString();
            drv.Row["SYS_NAME"] = dt.Rows[0]["SYS_NAME"].ToString();
            drv.Row["USER_NAME1"] = dt.Rows[0]["USER_NAME1"].ToString();
            drv.Row["USER_NAME2"] = dt.Rows[0]["USER_NAME2"].ToString();
            drv.Row["IS_MAIN"] = (bool)isMain ? "是" : "否";
            drv.Row["ESTIMATE_DAYS"] = days.ToString();
            drv.Row["REMARK"] = tbSysRemark.Text;
            cbIsMain.IsChecked = false;
            UpdateDays();
        }

        private void btnDelSys_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgProSysInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择删除的系统！");
                return;
            }
            drv.Row.Table.Rows.Remove(drv.Row);
        }

    }
}

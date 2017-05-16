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
using System.IO;
using 项目管理.Connect;

namespace 项目管理.Pages
{
    /// <summary>
    /// AddEngineerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ModProject : UserControl
    {
        public ModProject()
        {
            InitializeComponent();
            Refresh();
            if (GlobalFuns.LoginSysId != "")//项目经理
            {
                cbIsMain.Visibility = Visibility.Collapsed;
            }
            else//管理人员
            {
                btnAddTrade.IsEnabled = false;
                btnDelTrade.IsEnabled = false;
                dgDevelopmentInfo.IsReadOnly = true;
            }
        }

        private void Refresh()
        {
            cbDemandDepart.ItemsSource = CommunicationHelper.GetHisDeparts();

            cbProKinds.ItemsSource = new List<string>() { "新项目", "功能优化" };

            cbProStage.ItemsSource = new List<string>() { 
                "软需编写及评审", "系统开发/单元测试" , "集成测试", "SIT测试", 
                "UAT测试", "投产实施" , "已上线"
            };

            cbProState.ItemsSource = new List<string>() { 
                "正常", "延迟" , "关闭", "暂停", "完成"
            };

            cbSystem.ItemsSource = CommunicationHelper.GetAllSysDic();
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            cbSystem.SelectedIndex = 0;

            List<string> proNames = CommunicationHelper.GetCurProNames(GlobalFuns.LoginSysId, false);
            cbDemandName.ItemsSource = proNames;
            if (proNames.Count == 0)
            {
                MessageBox.Show("当前无项目！");
                return;
            }
            cbDemandName.SelectedIndex = 0;
        }

        private void btnAddTrade_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = dgDevelopmentInfo.DataContext as DataTable;
            string[] newRow = new string[dt.Columns.Count];
            dt.Rows.Add(newRow);
        }

        private void btnDelTrade_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgDevelopmentInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要删除的行！");
                return;
            }
            DataTable dt = dgDevelopmentInfo.DataContext as DataTable;
            dt.Rows.Remove(drv.Row);
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog imageFileDialog = new System.Windows.Forms.OpenFileDialog();
            imageFileDialog.Multiselect = true;
            imageFileDialog.Title = "请选择文件";
            imageFileDialog.Multiselect = false;
            imageFileDialog.Filter = "文件(*.*)|*.*";
            if (imageFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = System.IO.Path.GetFileName(imageFileDialog.FileName);
                DataTable dtFiles = dgFiles.DataContext as DataTable;
                foreach (DataRow dr in dtFiles.Rows)
                {
                    if (dr["FILE_NAME"].ToString() == fileName)
                    {
                        MessageBox.Show("文件名重复！");
                        return;
                    }
                }
                if (!Directory.Exists(curFilePath))
                {
                    Directory.CreateDirectory(curFilePath);
                }
                File.Copy(imageFileDialog.FileName, curFilePath + "/" + fileName);
                dtFiles.Rows.Add(new string[]{fileName,"是","是"});
            }
        }

        private void btnDelFile_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgFiles.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要删除的文件");
                return;
            }
            string select = drv.Row["FILE_NAME"].ToString();
            if (select != null)
            {
                drv.Row.Table.Rows.Remove(drv.Row);
                string fileName = curFilePath + "/" + select;
                File.Delete(fileName);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.ModProject(curProId, GlobalFuns.LoginSysId, cbDemandName.Text, cbDemandDepart.Text, tbDemandDate.Text,
                tbExpectDate.Text, cbProKinds.Text, cbProStage.Text, cbProState.Text, tbFinishDate.Text,
                tbProgressNote.Text, tbTestPerson.Text, tbBusinessPerson.Text, tbRemark.Text,
                dgProSysInfo.DataContext as DataTable, dgDevelopmentInfo.DataContext as DataTable))
            {
                MessageBox.Show("保存项目失败！");
                return;
            }
            MessageBox.Show("保存成功！");
        }

        string curProId = "";
        string curFilePath = "";

        private void cbDemandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string select = cbDemandName.SelectedItem as string;
            DataTable dt = CommunicationHelper.GetProInfoFromName(select);
            if (dt == null)
            {
                MessageBox.Show("查找项目信息失败！");
                return;
            }
            DataRow dr = dt.Rows[0];
            curProId = dr["DEMAND_ID"].ToString();
            cbDemandDepart.Text = dr["DEMAND_DEPART"].ToString();
            tbDemandDate.Text = dr["DEMAND_DATE"].ToString();
            tbExpectDate.Text = dr["EXPECT_DATE"].ToString();
            cbProKinds.Text = dr["PRO_KIND"].ToString();
            cbProStage.Text = dr["PRO_STAGE"].ToString();
            cbProState.Text = dr["PRO_STATE"].ToString();
            tbProgressNote.Text = dr["PRO_NOTE"].ToString();
            tbTestPerson.Text = dr["TEST_PERSON"].ToString();
            tbBusinessPerson.Text = dr["BUSINESS_PERSON"].ToString();
            tbRemark.Text = dr["REMARK"].ToString();
            tbFinishDate.Text = dr["FINISH_DATE"].ToString();
            tbLastTime.Text = dr["LAST_MOD_TIME"].ToString();

            DataTable dtSystems = CommunicationHelper.GetProSystemInfo(curProId);
            dgProSysInfo.DataContext = dtSystems;
            DataTable dtTrades = CommunicationHelper.GetTradesInfo(curProId, GlobalFuns.LoginSysId);
            dgDevelopmentInfo.DataContext = dtTrades;
            curFilePath = "projects/" + tbDemandDate.Text + "_" + select;
            DataTable dtFile = new DataTable();
            dtFile.Columns.Add("FILE_NAME");
            dtFile.Columns.Add("IS_DOWNLOAD");
            dtFile.Columns.Add("IS_RENEW");
            if (Directory.Exists(curFilePath))
            {
                string[] files = Directory.GetFiles(curFilePath);
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    dtFile.Rows.Add(new string[] { fileName, "是", "是" });
                }
            }
            else
            {
                Directory.CreateDirectory(curFilePath);  
            }
            dgFiles.DataContext = dtFile;
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            List<string> proNames = CommunicationHelper.GetCurProNames(GlobalFuns.LoginSysId, true);
            cbDemandName.ItemsSource = proNames;
            if (proNames.Count == 0)
            {
                MessageBox.Show("当前无项目！");
                return;
            }
            cbDemandName.SelectedIndex = 0;
        }

        private void btnOpenDir_Click(object sender, RoutedEventArgs e)
        {
            if (curFilePath != "")
            {
                System.Diagnostics.Process.Start("Explorer.exe", AppDomain.CurrentDomain.BaseDirectory + curFilePath.Replace("/", "\\"));
            }
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
            DataTable dt = CommunicationHelper.GetSystemInfo(cbSystem.SelectedValue.ToString());
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
            DataRow drNew = dtSysInfo.NewRow();
            drNew["SYS_ID"] = dt.Rows[0]["SYS_ID"].ToString();
            drNew["DEMAND_ID"] = curProId;
            drNew["SYS_NAME"] = dt.Rows[0]["SYS_NAME"].ToString();
            drNew["USER_NAME1"] = dt.Rows[0]["USER_NAME1"].ToString();
            drNew["USER_NAME2"] = dt.Rows[0]["USER_NAME2"].ToString();
            drNew["IS_MAIN"] = (bool)isMain ? "是" : "否";
            drNew["ESTIMATE_DAYS"] = days.ToString();
            drNew["USED_DAYS"] = "0";
            drNew["REMARK"] = tbSysRemark.Text;
            dtSysInfo.Rows.Add(drNew);
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
            DataTable dt = CommunicationHelper.GetSystemInfo(cbSystem.SelectedValue.ToString());
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
            if (MessageBox.Show("确定删除所选系统，该系统已有工时，则无法删除！", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            if (drv.Row["USED_DAYS"].ToString()!="0")
            {
                MessageBox.Show("删除失败！");
                return;
            }
            drv.Row.Table.Rows.Remove(drv.Row);
        }

        private void btnDownLoad_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}

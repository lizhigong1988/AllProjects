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
using System.IO;
using WindowLib.Connect;

namespace WindowLib.Pages
{
    /// <summary>
    /// AddEngineerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class QueryProject : UserControl
    {
        public QueryProject()
        {
            InitializeComponent();


            cbProStage.ItemsSource = new List<string>() { 
                "全部", "软需编写及评审", "系统开发/单元测试" , "集成测试", "SIT测试", 
                "UAT测试", "投产实施" , "已上线"
            };
            cbProStage.SelectedIndex = 0;

            cbProState.ItemsSource = new List<string>() { 
                "全部", "正常", "延迟" , "关闭", "暂停", "全部未完成", "完成"
            };
            cbProState.SelectedIndex = 0;

            cbSystem.ItemsSource = CommunicationHelper.GetAllSysDic();
            if (cbSystem.ItemsSource == null)
            {
                MessageBox.Show("获取系统信息失败");
                return;
            }
            (cbSystem.ItemsSource as Dictionary<string, string>).Add("", "全部");
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            cbSystem.SelectedIndex = (cbSystem.ItemsSource as Dictionary<string, string>).Count - 1;
            if (GlobalFuns.LoginSysId != "")
            {
                cbSystem.SelectedValue = GlobalFuns.LoginSysId;
            }
            else
            {
                btnDelete.Visibility = Visibility.Visible;
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = CommunicationHelper.QueryProInfo(cbProStage.Text, cbProState.Text, tbProDate.Text,
                cbSystem.SelectedValue.ToString());
            if (dt == null)
            {
                MessageBox.Show("获取项目信息失败");
                return;
            }
            dgProInfo.DataContext = dt;
        }

        string curFilePath = "";
        private void dgProInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgProInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            DataTable dtSystems = CommunicationHelper.GetProSystemInfo(drv.Row["DEMAND_ID"].ToString());
            if (dtSystems == null)
            {
                MessageBox.Show("获取项目子单信息失败！");
                return;
            }
            dgProSysInfo.DataContext = dtSystems;
            DataTable dtTrades = CommunicationHelper.GetTradesInfo(drv.Row["DEMAND_ID"].ToString(), GlobalFuns.LoginSysId);
            if (dtTrades == null)
            {
                MessageBox.Show("获取开发信息失败！");
                return;
            }
            dtTrades.Columns.Add("DIFF");
            dgDevelopmentInfo.DataContext = dtTrades;
            curFilePath = "projects/" + drv.Row["DEMAND_DATE"].ToString() + "_" + drv.Row["DEMAND_NAME"].ToString();

            DataTable dtFiles = CommunicationHelper.GetProFileInfo(drv.Row["DEMAND_ID"].ToString());
            if (dtFiles == null)
            {
                MessageBox.Show("获取项目文件信息失败！");
                return;
            }
            DataTable dtFile = new DataTable();
            dtFile.Columns.Add("FILE_ALL_NAME");
            dtFile.Columns.Add("FILE_NAME");
            dtFile.Columns.Add("IS_DOWNLOAD");
            dtFile.Columns.Add("IS_RENEW");
            if (Directory.Exists(curFilePath))
            {
                foreach (DataRow drFile in dtFiles.Rows)
                {
                    string fileAllName = curFilePath + "/" + drFile["FILE_NAME"].ToString();
                    if (File.Exists(fileAllName))
                    {
                        string fileName = System.IO.Path.GetFileName(fileAllName);
                        string time = File.GetLastWriteTime(fileAllName).ToString("yyyyMMddHHmmss");
                        if (time.CompareTo(drFile["FILE_TIME"].ToString()) >= 0) // 本地的日期大 本地较新
                        {
                            dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "是", "是" });
                        }
                        else
                        {
                            dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "是", "否" });
                        }
                    }
                    else
                    {
                        dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "否", "否" });
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(curFilePath);
                foreach (DataRow drFile in dtFiles.Rows)
                {
                    string fileAllName = curFilePath + "/" + drFile["FILE_NAME"].ToString();
                    dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "否", "否" });
                }
            }
            dgFiles.DataContext = dtFile;
        }

        private void btnOpenDir_Click(object sender, RoutedEventArgs e)
        {
            if (curFilePath != "")
            {
                System.Diagnostics.Process.Start("Explorer.exe", AppDomain.CurrentDomain.BaseDirectory + curFilePath.Replace("/","\\"));
            }
        }

        private void btnDiff_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtTrades = dgDevelopmentInfo.DataContext as DataTable;
            foreach (DataRow dr in dtTrades.Rows)
            {
                List<string> difDemandNames = CommunicationHelper.DiffTrade(dr["DEMAND_ID"].ToString(), dr["SYS_ID"].ToString(), dr["TRADE_CODE"].ToString());
                if (difDemandNames == null)
                {
                    MessageBox.Show("查询失败");
                    return;
                }
                dr["DIFF"] = "";
                if (difDemandNames.Count != 0)
                {
                    //dr["DIFF"] = "冲突";
                    foreach(string name in difDemandNames)
                    {
                        dr["DIFF"] += name + ",";
                    }
                    dr["DIFF"] = dr["DIFF"].ToString().TrimEnd(',');
                }
                else
                {
                    dr["DIFF"] = "无";
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtTrades = dgDevelopmentInfo.DataContext as DataTable;
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog { FileName = "交易列表.xls" };
            saveDialog.ShowDialog();
            string outputFile = saveDialog.FileName;
            if (outputFile.IndexOf(":", System.StringComparison.Ordinal) < 0)
                return; //被点了取消   
            ExcelOperation.ExportSimpleExcel(dtTrades, outputFile);
            MessageBox.Show("导出完成！");
        }

        private void btnExportMain_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtTrades = dgProInfo.DataContext as DataTable;
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog { FileName = "项目列表.xls" };
            saveDialog.ShowDialog();
            string outputFile = saveDialog.FileName;
            if (outputFile.IndexOf(":", System.StringComparison.Ordinal) < 0)
                return; //被点了取消   
            ExcelOperation.ExportSimpleExcel(dtTrades, outputFile);
            MessageBox.Show("导出完成！");
        }

        private void btnDownLoad_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgFiles.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要下载的文件！");
                return;
            }
            if (CommunicationHelper.DownloadFile(drv.Row["FILE_ALL_NAME"].ToString()))
            {
                drv.Row["IS_DOWNLOAD"] = "是";
                drv.Row["IS_RENEW"] = "是";
            }
            else
            {
                MessageBox.Show("下载失败！");
                return;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgProInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择需要删除的项目！");
                return;
            }
            if (MessageBox.Show("确定删除所选项目？", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (!CommunicationHelper.DelProject(drv.Row["DEMAND_ID"].ToString()))
            {
                MessageBox.Show("删除项目失败！");
                return;
            }
            btnQuery_Click(null, null);
        }

    }
}

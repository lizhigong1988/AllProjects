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
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = DataBaseManager.QueryProInfo(cbProStage.Text, cbProState.Text, tbProDate.Text);
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
            DataTable dtSystems = DataBaseManager.GetProSystemInfo(drv.Row["DEMAND_ID"].ToString());
            dgProSysInfo.DataContext = dtSystems;
            DataTable dtTrades = DataBaseManager.GetTradesInfo(drv.Row["DEMAND_ID"].ToString(), GlobalFuns.LoginSysId);
            dtTrades.Columns.Add("DIFF");
            dgDevelopmentInfo.DataContext = dtTrades;
            curFilePath = "projects/" + drv.Row["DEMAND_DATE"].ToString() + "_" + drv.Row["DEMAND_NAME"].ToString();

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
                List<string> difDemandNames = DataBaseManager.DiffTrade(dr["DEMAND_ID"].ToString(), dr["SYS_ID"].ToString(), dr["TRADE_CODE"].ToString());
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

        }

    }
}

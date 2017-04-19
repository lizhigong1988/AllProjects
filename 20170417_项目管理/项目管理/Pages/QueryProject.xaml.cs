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
            DataTable dtTrades = DataBaseManager.GetTradesInfo(drv.Row["DEMAND_ID"].ToString());
            dtTrades.Columns.Add("DIFF");
            dgDevelopmentInfo.DataContext = dtTrades;
            lbFiles.Items.Clear();
            curFilePath = "projects/" + drv.Row["DEMAND_DATE"].ToString() + "_" + drv.Row["DEMAND_NAME"].ToString();
            if (!Directory.Exists(curFilePath))
            {
                Directory.CreateDirectory(curFilePath);
            }
            else
            {
                string[] files = Directory.GetFiles(curFilePath);
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    lbFiles.Items.Add(fileName);
                }
            }
            decimal work = 0;
            foreach (DataRow dr in dtTrades.Rows)
            { 
                if(dr["WORKLOAD"].ToString() != "")
                {
                    work += decimal.Parse(dr["WORKLOAD"].ToString());
                }
            }
            tbUsedWork.Text = string.Format("已用工作量{0}人日", work.ToString());
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
                bool res = false;
                if (!DataBaseManager.DiffTrade(dr["DEMAND_ID"].ToString(), dr["TRADE_CODE"].ToString(), ref res))
                {
                    MessageBox.Show("查询失败");
                    return;
                }
                if (res)
                {
                    dr["DIFF"] = "冲突";
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

    }
}

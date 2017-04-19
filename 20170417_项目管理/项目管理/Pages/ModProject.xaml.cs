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
    public partial class ModProject : UserControl
    {
        public ModProject()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            cbDemandDepart.ItemsSource = DataBaseManager.GetHisDeparts();

            cbProKinds.ItemsSource = new List<string>() { "新项目", "功能优化" };

            cbProStage.ItemsSource = new List<string>() { 
                "软需编写及评审", "系统开发/单元测试" , "集成测试", "SIT测试", 
                "UAT测试", "投产实施" , "已上线"
            };

            cbProState.ItemsSource = new List<string>() { 
                "正常", "延迟" , "关闭", "暂停", "完成"
            };

            cbSystem.ItemsSource = DataBaseManager.GetHisSystem();

            List<string> proNames = DataBaseManager.GetCurProNames();
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
                foreach (string item in lbFiles.Items)
                {
                    if (item == fileName)
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
                lbFiles.Items.Add(fileName);
            }
        }

        private void btnDelFile_Click(object sender, RoutedEventArgs e)
        {
            string select = lbFiles.SelectedItem as string;
            if (select != null)
            {
                lbFiles.Items.Remove(select);
                string fileName = curFilePath + "/" + select;
                File.Delete(fileName);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!DataBaseManager.ModProject(curProId, cbDemandName.Text, cbDemandDepart.Text, tbDemandDate.Text,
                tbExpectDate.Text, cbProKinds.Text, cbProStage.Text, cbProState.Text, tbEstimatedDays.Text,
                tbProgressNote.Text, cbSystem.Text, tbRelationSystem.Text, tbFirstPerson.Text, tbSecondPerson.Text,
                tbTestPerson.Text, tbBusinessPerson.Text, tbRemark.Text, dgDevelopmentInfo.DataContext as DataTable))
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
            DataTable dt = DataBaseManager.GetProInfoFromName(select);
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
            tbEstimatedDays.Text = dr["ESTIMATE_DAYS"].ToString();
            tbProgressNote.Text = dr["PRO_NOTE"].ToString();
            cbSystem.Text = dr["SYSTEM"].ToString();
            tbRelationSystem.Text = dr["RELA_SYSTEMS"].ToString();
            tbFirstPerson.Text = dr["FIRST_PERSON"].ToString();
            tbSecondPerson.Text = dr["SECOND_PERSON"].ToString();
            tbTestPerson.Text = dr["TEST_PERSON"].ToString();
            tbBusinessPerson.Text = dr["BUSINESS_PERSON"].ToString();
            tbRemark.Text = dr["REMARK"].ToString();

            DataTable dtTrades = DataBaseManager.GetTradesInfo(curProId);
            dgDevelopmentInfo.DataContext = dtTrades;
            curFilePath = "projects/" + tbDemandDate.Text + "_" + select;
            lbFiles.Items.Clear();
            if (Directory.Exists(curFilePath))
            {
                string[] files = Directory.GetFiles(curFilePath);
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    lbFiles.Items.Add(fileName);
                }
            }
            else
            {
                Directory.CreateDirectory(curFilePath);  
            }
        }

    }
}

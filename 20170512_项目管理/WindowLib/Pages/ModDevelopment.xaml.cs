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
using WindowLib.PopWindows;

namespace WindowLib.Pages
{
    /// <summary>
    /// AddEngineerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ModDevelopment : UserControl
    {
        public ModDevelopment()
        {
            InitializeComponent();
            Refresh();
            if (GlobalFuns.LoginSysId == "")
            {
                btnAddTrade.IsEnabled = false;
                btnDelTrade.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
        }

        private void Refresh()
        {
            Dictionary<string, string> proNames = CommunicationHelper.GetCurProNames(GlobalFuns.LoginSysId, false);
            if (proNames == null)
            {
                MessageBox.Show("获取项目信息失败！");
                return;
            } 
            cbDemandName.ItemsSource = proNames;
            if (proNames.Count == 0)
            {
                MessageBox.Show("当前无项目！");
                return;
            }
            cbDemandName.SelectedValuePath = "Key";
            cbDemandName.DisplayMemberPath = "Value";
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

                if (!Directory.Exists(curFilePath))
                {
                    Directory.CreateDirectory(curFilePath);
                }
                string fileAllName = curFilePath + "/" + fileName;
                File.Copy(imageFileDialog.FileName, fileAllName + "new");
                if (File.Exists(fileAllName))
                {
                    File.Delete(fileAllName);
                }
                File.Move(fileAllName + "new", fileAllName);
                File.SetLastWriteTime(fileAllName, DateTime.Now.AddMinutes(5));
                if (CommunicationHelper.UploadFile(fileAllName))
                {
                    bool has = false;
                    foreach (DataRow dr in dtFiles.Rows)
                    {
                        if (dr["FILE_NAME"].ToString() == fileName)
                        {
                            dr["IS_DOWNLOAD"] = "是";
                            dr["IS_RENEW"] = "是";
                            dr["NEW_TIME"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            has = true;
                            break;
                        }
                    }
                    if (!has)
                    {
                        dtFiles.Rows.Add(new string[] { fileAllName, fileName, "是", "是", DateTime.Now.ToString("yyyyMMddHHmmss") });
                    }
                }
                else
                {
                    MessageBox.Show("上传失败！");
                    return;
                }
            }
        }

        //private void btnDelFile_Click(object sender, RoutedEventArgs e)
        //{
        //    DataRowView drv = dgFiles.SelectedItem as DataRowView;
        //    if (drv == null)
        //    {
        //        MessageBox.Show("请选择要删除的文件");
        //        return;
        //    }
        //    string select = drv.Row["FILE_NAME"].ToString();
        //    if (select != null)
        //    {
        //        drv.Row.Table.Rows.Remove(drv.Row);
        //        string fileName = curFilePath + "/" + select;
        //        if (CommunicationHelper.DelFile(fileName))
        //        {
        //            File.Delete(fileName);
        //        }
        //        else
        //        {
        //            MessageBox.Show("删除失败");
        //            return;
        //        }
        //    }
        //}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.ModDevelopment(curProId, GlobalFuns.LoginSysId, 
                dgDevelopmentInfo.DataContext as DataTable))
            {
                MessageBox.Show("保存开发信息失败！");
                return;
            }
            MessageBox.Show("保存成功！");
        }

        string curProId = "";
        string curFilePath = "";

        private void cbDemandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string select = cbDemandName.SelectedValue as string;
            DataTable dt = CommunicationHelper.GetProInfo(select);
            if (dt == null)
            {
                MessageBox.Show("查找项目信息失败！");
                return;
            }
            DataRow dr = dt.Rows[0];
            curProId = dr["DEMAND_ID"].ToString();
            string demandDate = dr["DEMAND_DATE"].ToString();

            DataTable dtSystems = CommunicationHelper.GetProSystemInfo(curProId);
            if (dtSystems == null)
            {
                MessageBox.Show("获取项目子单信息失败！");
                return;
            }
            dgProSysInfo.DataContext = dtSystems;

            DataTable dtTrades = CommunicationHelper.GetTradesInfo(curProId, GlobalFuns.LoginSysId);
            if (dtTrades == null)
            {
                MessageBox.Show("获取开发信息失败！");
                return;
            }
            dgDevelopmentInfo.DataContext = dtTrades;

            DataTable dtFiles = CommunicationHelper.GetProFileInfo(curProId);
            if (dtFiles == null)
            {
                MessageBox.Show("获取项目文件信息失败！");
                return;
            }
            curFilePath = "projects/" + demandDate + "_" +
                (cbDemandName.ItemsSource as Dictionary<string, string>).ElementAt(cbDemandName.SelectedIndex).Value;
            DataTable dtFile = new DataTable();
            dtFile.Columns.Add("FILE_ALL_NAME");
            dtFile.Columns.Add("FILE_NAME");
            dtFile.Columns.Add("IS_DOWNLOAD");
            dtFile.Columns.Add("IS_RENEW");
            dtFile.Columns.Add("NEW_TIME");
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
                            dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "是", "是", drFile["FILE_TIME"].ToString() });
                        }
                        else
                        {
                            dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "是", "否", drFile["FILE_TIME"].ToString() });
                        }
                    }
                    else
                    {
                        dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "否", "否", drFile["FILE_TIME"].ToString() });
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(curFilePath);
                foreach (DataRow drFile in dtFiles.Rows)
                {
                    string fileAllName = curFilePath + "/" + drFile["FILE_NAME"].ToString();
                    dtFile.Rows.Add(new string[] { fileAllName, drFile["FILE_NAME"].ToString(), "否", "否", drFile["FILE_TIME"].ToString() });
                }
            }
            dgFiles.DataContext = dtFile;
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> proNames = CommunicationHelper.GetCurProNames(GlobalFuns.LoginSysId, true);
            if (proNames == null)
            {
                MessageBox.Show("获取项目信息失败！");
                return;
            }
            cbDemandName.ItemsSource = proNames;
            if (proNames.Count == 0)
            {
                MessageBox.Show("当前无项目！");
                return;
            }
            cbDemandName.SelectedValuePath = "Key";
            cbDemandName.DisplayMemberPath = "Value";
            cbDemandName.SelectedIndex = 0;
        }

        private void btnOpenDir_Click(object sender, RoutedEventArgs e)
        {
            if (curFilePath != "")
            {
                System.Diagnostics.Process.Start("Explorer.exe", AppDomain.CurrentDomain.BaseDirectory + curFilePath.Replace("/", "\\"));
            }
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}

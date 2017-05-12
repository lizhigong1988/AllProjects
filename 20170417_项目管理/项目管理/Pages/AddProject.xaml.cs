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
                string filePath = "TEMP/" + CurDirectory;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                File.Copy(imageFileDialog.FileName, filePath + "/" + fileName);
                lbFiles.Items.Add(fileName);
            }
        }

        private void btnDelFile_Click(object sender, RoutedEventArgs e)
        {
            string select = lbFiles.SelectedItem as string;
            if (select != null)
            {
                lbFiles.Items.Remove(select);
                string fileName = "TEMP/" + CurDirectory + "/" + select;
                File.Delete(fileName);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbDemandName.Text == "")
            {
                MessageBox.Show("请输入项目名称！");
                return;
            }
            //if (!DataBaseManager.AddNewProject(tbDemandName.Text, cbDemandDepart.Text, tbDemandDate.Text,
            //    tbExpectDate.Text, cbProKinds.Text, cbProStage.Text, cbProState.Text, tbEstimatedDays.Text,
            //    tbProgressNote.Text, cbSystem.Text, tbRelationSystem.Text, tbFirstPerson.Text, tbSecondPerson.Text,
            //    tbTestPerson.Text, tbBusinessPerson.Text, tbRemark.Text, tbFinishDate.Text, dgDevelopmentInfo.DataContext as DataTable))
            //{
            //    MessageBox.Show("保存项目失败！");
            //    return;
            //}
            string filePath = "TEMP/" + CurDirectory;
            string mvFilePath = "projects/" + tbDemandDate.Text + "_" + tbDemandName.Text;
            try
            {
                if (Directory.Exists(filePath))
                {
                    Directory.Move(filePath, mvFilePath);
                }
                else
                {
                    Directory.CreateDirectory(mvFilePath);
                }
            }
            catch
            {
                MessageBox.Show("项目保存成功，文件保存失败，请手工保存！");
                return;
            }
            MessageBox.Show("保存成功！");
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}

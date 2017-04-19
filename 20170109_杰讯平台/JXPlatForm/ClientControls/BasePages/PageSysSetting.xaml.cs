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
using ClientCommunication;
using System.Data;
using CommonLib;
using ClientLibrary;

namespace ClientControls.BasePages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PageSysSetting
    {
        /// <summary>
        /// 默认目录
        /// </summary>
        private string defaultPath = "Images";
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageSysSetting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            tbLoginBak.Text = ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.LOGIN_BAK);
            tbLoginLogo.Text = ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.LOGIN_LOGO);
            tbMainBak.Text = ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.MAIN_BAK);
            tbMainLogo.Text = ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.MAIN_LOGO);
            return true;
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbLoginBak.Text == "")
            {
                ShowMassageBox.JXMassageBox("请选择登陆背景图！");
                return;
            }
            if (tbLoginBak.Text.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            {
                tbLoginBak.Text = tbLoginBak.Text.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);
            }
            if (tbLoginLogo.Text == "")
            {
                ShowMassageBox.JXMassageBox("请选择登陆LOGO！");
                return;
            }
            if (tbLoginLogo.Text.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            {
                tbLoginLogo.Text = tbLoginLogo.Text.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);
            }
            if (tbMainBak.Text == "")
            {
                ShowMassageBox.JXMassageBox("请选择主背景图！");
                return;
            }
            if (tbMainBak.Text.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            {
                tbMainBak.Text = tbMainBak.Text.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);
            }
            if (tbMainLogo.Text == "")
            {
                ShowMassageBox.JXMassageBox("请选择主LOGO！");
                return;
            }
            if (tbMainLogo.Text.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            {
                tbMainLogo.Text = tbMainLogo.Text.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);
            }
            ClientConfigHeper.SetConfig(ClientConfigHeper.CONFIG_KEYS.LOGIN_BAK, tbLoginBak.Text);
            ClientConfigHeper.SetConfig(ClientConfigHeper.CONFIG_KEYS.LOGIN_LOGO, tbLoginLogo.Text);
            ClientConfigHeper.SetConfig(ClientConfigHeper.CONFIG_KEYS.MAIN_BAK, tbMainBak.Text);
            ClientConfigHeper.SetConfig(ClientConfigHeper.CONFIG_KEYS.MAIN_LOGO, tbMainLogo.Text);
            RefreshPage();
            MessageBox.Show("保存成功！");
        }

        private void btnLoginBak_Click(object sender, RoutedEventArgs e)
        {
            tbLoginBak.Text = GetSelectFile(tbLoginBak.Text);
        }

        private void btnLoginLogo_Click(object sender, RoutedEventArgs e)
        {
            tbLoginLogo.Text = GetSelectFile(tbLoginLogo.Text);
        }

        private void btnMainBak_Click(object sender, RoutedEventArgs e)
        {
            tbMainBak.Text = GetSelectFile(tbMainBak.Text);
        }

        private void btnMainLogo_Click(object sender, RoutedEventArgs e)
        {
            tbMainLogo.Text = GetSelectFile(tbMainLogo.Text);
        }

        private string GetSelectFile(string orgName)
        {
            string initPath = AppDomain.CurrentDomain.BaseDirectory + defaultPath + "\\";
            System.Windows.Forms.OpenFileDialog imageFileDialog = new System.Windows.Forms.OpenFileDialog();
            imageFileDialog.InitialDirectory = initPath;
            imageFileDialog.Title = "请选择图片";
            imageFileDialog.Multiselect = false;
            //imageFileDialog.Filter = "jpg文件(*.jpg)|*.jpg";
            //imageFileDialog.Filter = "图片文件 jpg|*.jpg|bmp|*.bmp|gif|*.gif|png|*.png";
            imageFileDialog.Filter = "图片文件|*.jpg;*.bmp;*.png";
            if (imageFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return imageFileDialog.FileName;
            }
            return orgName;
        }
    }
}

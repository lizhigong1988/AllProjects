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
using 项目管理.Pages;
using System.IO;
using 项目管理.Tools;
using System.Data;
using CommonLib;
using 项目管理.Connect;

namespace 项目管理
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tbVersionClient.Text = "客户端版本：" + CommonDef.VERSION_NUM;
            string serverVersion = CommunicationHelper.GetServerVersion();
            tbVersionServer.Text = "服务端版本：" + serverVersion;
            if (CommonDef.VERSION_NUM.Split('.')[0] != serverVersion.Split('.')[0])
            {
                MessageBox.Show("版本过旧！");
                this.Close();
                return;
            }
            Tools.GlobalFuns.MainWind = this;
            if (Directory.Exists("TEMP"))
            {
                Directory.Delete("TEMP", true);
            }

            if (GlobalFuns.LoginUser != "")
            {
                if (GlobalFuns.LoginSysName != "")
                {
                    tbAlert.Text += "->" + GlobalFuns.LoginSysName;
                }
                if (GlobalFuns.LoginRole != "")
                {
                    tbAlert.Text += "->" + GlobalFuns.LoginRole;
                }
                if (GlobalFuns.LoginUser != "")
                {
                    tbAlert.Text += "->" + GlobalFuns.LoginUser;
                }
            }
            else
            {
                tbAlert.Text += "->系统管理员";
            }

            switch (GlobalFuns.LoginRole)
            { 
                case ""://系统管理员
                    listMenu.Items.RemoveAt(6);//修改密码
                    break;
                case "部门领导":
                    break;
                case "项目经理":
                    listMenu.Items.RemoveAt(4);//系统信息管理
                    break;
                default:
                    listMenu.Items.RemoveAt(5);//人员管理
                    listMenu.Items.RemoveAt(4);//系统信息管理
                    listMenu.Items.RemoveAt(3);//工作量统计
                    listMenu.Items.RemoveAt(2);//项目统计
                    listMenu.Items.RemoveAt(0);//新增项目
                    break;
            }
        }

        private void Item_Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string header = btn.Tag.ToString();
            foreach (TabItem item in tabPageBox.Items)
            {
                if (item.Header.ToString() == header)
                {
                    tabPageBox.Items.Remove(item);
                    break;
                }
            }
        }
        
        private void listMenu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = listMenu.SelectedItem as ListBoxItem;
            if (item == null)
            {
                return;
            }
            //判断为已打开的页面
            foreach (TabItem tabItem in tabPageBox.Items)
            {
                if (tabItem.Header.ToString() == item.Content.ToString())
                {
                    tabPageBox.SelectedItem = tabItem;
                    return;
                }
            }

            //新增页面
            TabItem newItem = new TabItem();
            newItem.Header = item.Content.ToString();
            ScrollViewer scrl = new ScrollViewer();
            newItem.Content = scrl;
            switch (item.Content.ToString())
            {
                case "新增项目":
                    scrl.Content = new AddProject();
                    break;
                case "项目维护":
                    scrl.Content = new ModProject();
                    break;
                case "项目统计":
                    scrl.Content = new QueryProject();
                    break;
                case "工作量统计":
                    scrl.Content = new QueryWorkDays();
                    break;
                case "系统信息管理":
                    scrl.Content = new SystemMange();
                    break;
                case "人员管理":
                    scrl.Content = new UserMange();
                    break;
                case "修改密码":
                    scrl.Content = new ModPassword();
                    break;
                default:
                    return;
            }
            tabPageBox.Items.Add(newItem);
            tabPageBox.SelectedItem = newItem;
        }

        public void CloseThisPage(string header)
        {
            foreach (TabItem item in tabPageBox.Items)
            {
                if (item.Header.ToString() == header)
                {
                    tabPageBox.Items.Remove(item);
                    return;
                }
            }
        }
    }
}

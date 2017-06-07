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
using WindowLib.Pages;
using System.IO;
using WindowLib.Tools;
using System.Data;
using CommonLib;
using WindowLib.Connect;

namespace WindowLib
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Tools.GlobalFuns.MainWind = this;
            if (GlobalFuns.DtLoginUserSysInfo.Rows.Count > 1)
            {
                Dictionary<string, string> manageDic = new Dictionary<string, string>();
                foreach (DataRow dic in GlobalFuns.DtLoginUserSysInfo.Rows)
                {
                    string sysId = dic["SYS_ID"].ToString();
                    if (sysId == "")
                    {
                        continue;
                    }
                    if (!manageDic.ContainsKey(sysId))
                    {
                        manageDic.Add(sysId, dic["SYS_NAME"].ToString());
                    }
                }
                cbSelectSys.ItemsSource = manageDic;
                cbSelectSys.SelectedValuePath = "Key";
                cbSelectSys.DisplayMemberPath = "Value";
                cbSelectSys.SelectedIndex = 0;
                spSelectSys.Visibility = Visibility.Visible;
            }
            else
            {
                spSelectSys.Visibility = Visibility.Collapsed;
            }
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            if (GlobalFuns.LoginUser != "")
            {
                tbAlert.Text = "焦作中旅银行->信息技术部";
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
            listMenu.Items.Clear();
            switch (GlobalFuns.LoginRole)
            {
                case ""://系统管理员
                    listMenu.Items.Add(new ListBoxItem() { Content = "新增项目" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "项目维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "开发信息维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "进度录入" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "项目统计" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "工作量统计" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "系统信息管理" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "人员管理" });
                    //listMenu.Items.Add(new ListBoxItem() { Content = "修改密码" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "系统设置" });
                    break;
                case "部门领导":
                case "PMO":
                    listMenu.Items.Add(new ListBoxItem() { Content = "新增项目" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "项目维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "开发信息维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "进度录入" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "项目统计" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "工作量统计" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "系统信息管理" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "人员管理" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "修改密码" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "系统设置" });
                    break;
                case "项目经理":
                    listMenu.Items.Add(new ListBoxItem() { Content = "新增项目" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "项目维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "开发信息维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "进度录入" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "项目统计" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "工作量统计" });
                    //listMenu.Items.Add(new ListBoxItem() { Content = "系统信息管理" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "人员管理" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "修改密码" });
                    //listMenu.Items.Add(new ListBoxItem() { Content = "系统设置" });
                    break;
                case "开发人员":
                    listMenu.Items.Add(new ListBoxItem() { Content = "开发信息维护" });
                    listMenu.Items.Add(new ListBoxItem() { Content = "修改密码" });
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
                case "开发信息维护":
                    scrl.Content = new ModDevelopment();
                    break;
                case "进度录入":
                    scrl.Content = new ProRateEntry();
                    break;
                case "项目统计":
                    scrl.Content = new QueryProject();
                    break;
                case "工作量统计":
                    scrl.Content = new QueryWorkDays();
                    break;
                case "系统信息管理":
                    scrl.Content = new SystemManage();
                    break;
                case "人员管理":
                    scrl.Content = new UserManage();
                    break;
                case "修改密码":
                    scrl.Content = new ModPassword();
                    break;
                case "系统设置":
                    scrl.Content = new SysConfig();
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

        private void cbSelectSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSelectSys.SelectedValue == null)
            {
                return;
            }
            if (tabPageBox.Items.Count != 0)
            {
                if (MessageBox.Show("切换系统后将关闭已打开页面，前请先保存当前内容，确认切换？", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
                tabPageBox.Items.Clear();
            }
            GlobalFuns.LoginSysId = cbSelectSys.SelectedValue.ToString();
            GlobalFuns.LoginSysName = (cbSelectSys.ItemsSource as Dictionary<string, string>)[GlobalFuns.LoginSysId];
            RefreshMenu();
        }
    }
}

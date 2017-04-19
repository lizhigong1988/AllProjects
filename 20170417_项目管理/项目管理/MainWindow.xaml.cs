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
using 项目管理.DataBases;
using 项目管理.Pages;
using System.IO;

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
            Tools.GlobalFuns.MainWind = this;
            if (Directory.Exists("TEMP"))
            {
                Directory.Delete("TEMP", true);
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!DataBaseManager.InitDataBases())
            {
                MessageBox.Show("初始化数据库失败！");
                this.Close();
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

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
using ClientControls;

namespace JXPlatForm.Client.MainPage
{
    /// <summary>
    /// DisplayArea.xaml 的交互逻辑
    /// </summary>
    public partial class DisplayArea : UserControl
    {
        Dictionary<string, double> dicPageItemActWidth = new Dictionary<string, double>();

        public Window MainWind = null;

        public DisplayArea()
        {
            InitializeComponent();
        }

        private void tabPageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Item_Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Button select = sender as Button;
            foreach (TabItem item in tabPageBox.Items)
            {
                if (item.Header.ToString() == select.Tag.ToString())
                {//存在
                    tabPageBox.Items.Remove(item);
                    dicPageItemActWidth.Remove(item.Header.ToString());
                    return;
                }
            }
            ResizeTabItem();
        }

        internal void SetCurrentPage(string pageTag)
        {
            foreach (TabItem item in tabPageBox.Items)
            {
                if (item.Header.ToString() == pageTag)
                {//存在
                    tabPageBox.SelectedItem = item;
                    return;
                }
            }
            PageBase newPage = PageManager.GetNewPage(pageTag);
            if (newPage == null)
            {
                ShowMassageBox.JXMassageBox("打开页面失败！");
                return;
            }
            if (!newPage.RefreshPage())
            {
                return;
            }

            foreach (TabItem item in tabPageBox.Items)
            {
                item.Width = 0;
            }
            TabItem newItem = new TabItem();
            newItem.Header = pageTag;
            newItem.Content = newPage;
            tabPageBox.Items.Add(newItem);
            tabPageBox.SelectedItem = newItem;
            tabPageBox.LayoutUpdated += tabPageBox_LayoutUpdated;
        }

        private void ResizeTabItem()
        {
            double total = 0;
            foreach (TabItem item in tabPageBox.Items)
            {
                if (!dicPageItemActWidth.ContainsKey(item.Header.ToString()))
                {
                    dicPageItemActWidth.Add(item.Header.ToString(), item.ActualWidth);
                }
                total += dicPageItemActWidth[item.Header.ToString()];
            }
            double tableWidth = tabPageBox.ActualWidth - 10;
            if (total > tableWidth)
            {
                foreach (TabItem item in tabPageBox.Items)
                {
                    item.Width = dicPageItemActWidth[item.Header.ToString()] * tableWidth / total;
                }
            }
            else
            {
                foreach (TabItem item in tabPageBox.Items)
                {
                    item.Width = dicPageItemActWidth[item.Header.ToString()];
                }
            }
        }

        private void tabPageBox_LayoutUpdated(object sender, EventArgs e)
        {
            tabPageBox.LayoutUpdated -= tabPageBox_LayoutUpdated;
            ResizeTabItem();
        }
    }
}

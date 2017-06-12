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
using WindowLib.Connect;
using System.Net.Mail;
using CommonLib;
using WindowLib.PopWindows;

namespace WindowLib.Pages
{
    /// <summary>
    /// UserManage.xaml 的交互逻辑
    /// </summary>
    public partial class NoticeMange : UserControl
    {
        public NoticeMange()
        {
            InitializeComponent();
            tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            tbEndDate.Text = DateTime.Now.ToString("yyyyMMdd"); 
            switch (GlobalFuns.LoginRole)
            { 
                case "部门领导":
                case "PMO":
                case ""://系统管理员
                    cbTop.Visibility = Visibility.Visible;
                    break;
                default:
                    cbTop.Visibility = Visibility.Collapsed;
                    break;
            }
            if (RefreshNotice())
            {
                GlobalFuns.OpenFlag = true;
            }
        }

        private bool RefreshNotice()
        {
            DataTable dtNotice = CommunicationHelper.QueryNoticeInfo(tbStartDate.Text, tbEndDate.Text);
            if (dtNotice == null)
            {
                MessageBox.Show("获取公告失败");
                return false;
            }
            dgNoticeInfo.DataContext = dtNotice;
            return true;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            RefreshNotice();
        }

        private void dgNoticeInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgNoticeInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            tbNoticeTitle.Text = drv.Row["NOTICE_TITLE"].ToString();
            tbNoticeContent.Text = drv.Row["NOTICE_CONTENT"].ToString().Replace("<br/>", "\r\n");
        }

        private void tbStartDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbStartDate);
        }

        private void tbEndDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbEndDate);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbNoticeTitle.Text == "")
            {
                MessageBox.Show("请输入公告标题！");
                return;
            }
            if (tbNoticeContent.Text == "")
            {
                MessageBox.Show("请输入公告内容！");
                return;
            }
            int lvl = (bool)cbTop.IsChecked? 0 : 1;
            string name = GlobalFuns.LoginUser;
            if (name == "")
            {
                name = "系统管理员";
            }
            if ((bool)cbNoName.IsChecked)
            {
                name = "[匿名]";
            }
            if (!CommunicationHelper.AddNewNotice(name, tbNoticeTitle.Text,
                tbNoticeContent.Text.Replace("\r\n", "<br/>"), lvl))
            {
                MessageBox.Show("新增公告失败！");
                return;
            }
            RefreshNotice();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgNoticeInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要删除的公告！");
                return;
            }
            switch (GlobalFuns.LoginRole)
            {
                case "部门领导":
                case "PMO":
                case ""://系统管理员
                    break;
                default:
                    if (drv.Row["NOTICE_MAN"].ToString() != GlobalFuns.LoginUser)
                    {
                        MessageBox.Show("只能删除自己的公告！");
                        return;
                    }
                    break;
            }
            if (MessageBox.Show("确定删除所选公告？", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            if (!CommunicationHelper.DelNotice(drv.Row["NOTICE_ID"].ToString()))
            {
                MessageBox.Show("删除公告失败！");
                return;
            }
            RefreshNotice();
        }

    }
}

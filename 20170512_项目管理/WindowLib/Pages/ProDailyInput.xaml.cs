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
using WindowLib.Tools;
using System.Data;
using WindowLib.Connect;
using WindowLib.PopWindows;

namespace WindowLib.Pages
{
    /// <summary>
    /// ProDailyInput.xaml 的交互逻辑
    /// </summary>
    public partial class ProDailyInput : UserControl
    {
        Dictionary<string, string> dicWorkType = new Dictionary<string, string>() 
        {
            {"1", "开发"},
            {"2", "会议"},
            {"3", "文档整理"},
            {"4", "协助运维"},
            {"Z", "其他"},
        };

        public ProDailyInput()
        {
            InitializeComponent();
            tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            tbEndDate.Text = DateTime.Now.ToString("yyyyMMdd");

            tbUserName.Text = GlobalFuns.LoginUser;
            switch (GlobalFuns.LoginRole)
            {
                case "开发人员":
                    tbUserName.IsEnabled = false;
                    tbDate.IsEnabled = false;
                    tbSignInTime.IsEnabled = false;
                    tbSignOutTime.IsEnabled = false;
                    btnQuery_Click(null, null);
                    RefreshCurDayInfo();
                    break;
                default:
                    tbUserName.IsEnabled = true;
                    tbDate.IsEnabled = true;
                    tbSignInTime.IsEnabled = true;
                    tbSignOutTime.IsEnabled = true;
                    tbDate.Text = DateTime.Now.ToString("yyyyMMdd");
                    break;
            
            }
            cbWorkType.SelectedItem = new Dictionary<string, string>(dicWorkType);
            cbWorkType.DisplayMemberPath = "Value";
            cbWorkType.SelectedValuePath = "Key";
            cbWorkType.SelectedIndex = 0;
            GlobalFuns.OpenFlag = true;
        }

        private void RefreshCurDayInfo()
        {
            string date = "";
            string signIn = "";
            string signOut = "";
            DataTable dt = CommunicationHelper.GetCurDailyInfo(tbUserName.Text, ref date, out signIn, out signOut);
            if (dt == null)
            {
                MessageBox.Show("获取当前日志失败");
                return;
            }
            dgCurDailyDetail.DataContext = dt;
            tbDate.Text = date;
            tbSignInTime.Text = signIn;
            if (signIn != "")
            {
                btnSignIn.IsEnabled = false;
            }
            tbSignOutTime.Text = signOut;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                MessageBox.Show("请输入查询姓名");
                return;
            }
            DataTable dt = CommunicationHelper.QueryDailyInfo(tbUserName.Text, tbStartDate.Text, tbEndDate.Text);
            if (dt == null)
            {
                MessageBox.Show("查询签到信息失败");
                return;
            }
            dgHisDailyInfo.DataContext = dt;
        }

        private void dgHisDailyInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgHisDailyInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            DataTable dt = CommunicationHelper.QueryDailyDetail(drv.Row["DAILY_ID"].ToString());
            if (dt == null)
            {
                MessageBox.Show("查询签到详细信息失败");
                return;
            }
            dgDailyDetail.DataContext = dt;
        }

        private void tbDate_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void dgCurDailyDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbStartDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbStartDate);
        }

        private void tbEndDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbEndDate);
        }

        private void tbDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbDate);
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                MessageBox.Show("请输入姓名");
                return;
            }
            if (tbDate.Text == "")
            {
                MessageBox.Show("请输入日期");
                return;
            }
            string signInTime = tbSignInTime.Text;
            string signDate = tbDate.Text;
            string harddeskId = "";
            switch (GlobalFuns.LoginRole)
            {
                case "开发人员":
                    signInTime = "";
                    signDate = "";
                    harddeskId = GlobalFuns.GetHardDiskID();
                    break;
                default:
                    break;
            }
            if (!CommunicationHelper.DailySignIn(tbUserName.Text, signDate, signInTime, harddeskId))
            {
                MessageBox.Show("签到失败");
                return;
            }
            RefreshCurDayInfo();
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                MessageBox.Show("请输入姓名");
                return;
            }
            if (tbDate.Text == "")
            {
                MessageBox.Show("请输入日期");
                return;
            }
            string signOutTime = tbSignOutTime.Text;
            string signDate = tbDate.Text;
            string harddeskId = "";
            switch (GlobalFuns.LoginRole)
            {
                case "开发人员":
                    signOutTime = "";
                    signDate = "";
                    harddeskId = GlobalFuns.GetHardDiskID();
                    break;
                default:
                    break;
            }
            if (!CommunicationHelper.DailySignOut(tbUserName.Text, signDate, signOutTime, harddeskId))
            {
                MessageBox.Show("签退失败");
                return;
            }
            RefreshCurDayInfo();
        }
    }
}

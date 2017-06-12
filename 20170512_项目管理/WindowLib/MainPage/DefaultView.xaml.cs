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
using System.IO;
using System.Data;
using WindowLib.PopWindows;
using WindowLib.Connect;

namespace WindowLib.MainPage
{
    /// <summary>
    /// DefaultView.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultView : UserControl
    {
        public DefaultView()
        {
            InitializeComponent();
            if (File.Exists(WORK_NOTE_PATH))
            {
                tbWorkNote.Text = File.ReadAllText(WORK_NOTE_PATH);
            }
            tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            tbEndDate.Text = DateTime.Now.ToString("yyyyMMdd"); 
            //RefreshNotice();
        }

        public void RefreshNotice()
        {
            DataTable dtNotice = CommunicationHelper.QueryNoticeInfo(tbStartDate.Text, tbEndDate.Text);
            if (dtNotice == null)
            {
                MessageBox.Show("获取公告失败");
                return;
            }
            dgNoticeInfo.DataContext = dtNotice;
            if (dtNotice.Rows.Count != 0)
            {
                tbNoticeContent.Text = dtNotice.Rows[0]["NOTICE_CONTENT"].ToString().Replace("<br/>", "\r\n");
            }
        }

        private string WORK_NOTE_PATH = "WORK_NOTE";

        private void tbWorkNote_LostFocus(object sender, RoutedEventArgs e)
        {
            try{
                File.WriteAllText(WORK_NOTE_PATH, tbWorkNote.Text);
            }
            catch
            {
                MessageBox.Show("保存便签失败");
            }
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
            tbNoticeContent.Text = drv.Row["NOTICE_CONTENT"].ToString().Replace("<br/>", "\r\n") ;
        }

        private void tbStartDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbStartDate);
        }

        private void tbEndDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbEndDate);
        }
    }
}

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
using System.Windows.Interop;
using WindowLib.Tools;

namespace WindowLib.PopWindows
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class CalendarPop
    {
        public TextBox TbDate;
        public CalendarPop()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
        }

        public static void ShowCalendarWind(TextBox tbDate)
        {
            Window wind = GlobalFuns.MainWind;
            CalendarPop calendar = new CalendarPop();
            //HwndSource winformWindow = (HwndSource.FromDependencyObject(wind) as HwndSource);
            //if (winformWindow != null)
            //{
            //    new WindowInteropHelper(calendar)
            //    {
            //        Owner = winformWindow.Handle
            //    };
            //}
            Point point = tbDate.TransformToAncestor(wind).Transform(new Point(0, 0));
            calendar.Left = point.X + wind.Left;
            calendar.Top = point.Y + wind.Top;
            calendar.TbDate = tbDate;
            calendar.Show();
        }

        /// <summary>
        /// 选择日期事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (null != calendar.SelectedDate)
            {
                TbDate.Text = calendar.SelectedDate.Value.ToString("yyyyMMdd");
                isClosed = true;
                this.Close();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            TbDate.Text = "";
            isClosed = true;
            this.Close();
        }

        bool isClosed = true;

        private void btnCancelMod_Click(object sender, RoutedEventArgs e)
        {
            isClosed = true;
            this.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!isClosed)
            {
                this.Close();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Activate();
            isClosed = false;
        }
    }
}

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

namespace WindowLib.PopWindows
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class CalendarPop
    {
        public string date = "";
        public CalendarPop()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
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
                date = calendar.SelectedDate.Value.ToString("yyyyMMdd");
                this.Close();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            date = "";
            this.Close();
        }
    }
}

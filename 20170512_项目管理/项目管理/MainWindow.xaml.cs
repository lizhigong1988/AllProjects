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
            if (File.Exists(IP_CONFIG_FILE))
            {
                string[] fileInfo = File.ReadAllText(IP_CONFIG_FILE).Split('\n');
                tbIPAddr.Text = fileInfo[0];
            }
        }
        static string IP_CONFIG_FILE = "CONFIG_IP_ADDR";

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.AppConnectInit(tbIPAddr.Text))
            {
                MessageBox.Show("网络连接失败！");
                return;
            }
        }
    }
}

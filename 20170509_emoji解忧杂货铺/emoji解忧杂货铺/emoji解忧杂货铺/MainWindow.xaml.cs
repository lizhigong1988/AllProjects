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

namespace emoji解忧杂货铺
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void bd1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton1Cover.png", UriKind.Relative)));
        }

        private void bd1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton1Normal.png", UriKind.Relative)));
        }

        private void bd2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton2Cover.png", UriKind.Relative)));
        }

        private void bd2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton2Normal.png", UriKind.Relative)));
        }

        private void bd3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd3.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton3Cover.png", UriKind.Relative)));
        }

        private void bd3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd3.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton3Normal.png", UriKind.Relative)));
        }

        private void bd4_MouseEnter(object sender, MouseEventArgs e)
        {
            bd4.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton4Cover.png", UriKind.Relative)));
        }

        private void bd4_MouseLeave(object sender, MouseEventArgs e)
        {
            bd4.Background = new ImageBrush(new BitmapImage(new Uri("Images/FirstFloorButton4Normal.png", UriKind.Relative)));
        }

        private void bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new SecondFloor().Show();
            this.Close();
        }
    }
}

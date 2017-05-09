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
using System.Windows.Shapes;

namespace emoji解忧杂货铺
{
    /// <summary>
    /// SecondFloor.xaml 的交互逻辑
    /// </summary>
    public partial class SecondFloor : Window
    {
        public SecondFloor()
        {
            InitializeComponent();
        }

        private void bdBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void bd1_1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_1.Background = Brushes.Transparent;
            bdShadow1_1.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadowCorver.png", UriKind.Relative)));
        }

        private void bd1_1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_1.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButton1_1.png", UriKind.Relative)));
            bdShadow1_1.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadow.png", UriKind.Relative)));
        }

        private void bd1_2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_2.Background = Brushes.Transparent;
            bdShadow1_2.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadowCorver.png", UriKind.Relative)));
        }

        private void bd1_2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_2.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButton1_2.png", UriKind.Relative)));
            bdShadow1_2.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadow.png", UriKind.Relative)));
        }

        private void bd1_3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_3.Background = Brushes.Transparent;
            bdShadow1_3.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadowCorver.png", UriKind.Relative)));
        }

        private void bd1_3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_3.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButton1_3.png", UriKind.Relative)));
            bdShadow1_3.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadow.png", UriKind.Relative)));
        }


        private void bd2_1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_1.Background = Brushes.Transparent;
            bdShadow2_1.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadowCorver.png", UriKind.Relative)));
        }

        private void bd2_1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_1.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButton2_1.png", UriKind.Relative)));
            bdShadow2_1.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadow.png", UriKind.Relative)));
        }

        private void bd2_2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_2.Background = Brushes.Transparent;
            bdShadow2_2.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadowCorver.png", UriKind.Relative)));
        }

        private void bd2_2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_2.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButton2_2.png", UriKind.Relative)));
            bdShadow2_2.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadow.png", UriKind.Relative)));
        }

        private void bd2_3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_3.Background = Brushes.Transparent;
            bdShadow2_3.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadowCorver.png", UriKind.Relative)));
        }

        private void bd2_3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_3.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButton2_3.png", UriKind.Relative)));
            bdShadow2_3.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorButtonShadow.png", UriKind.Relative)));
        }


        private void bd_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

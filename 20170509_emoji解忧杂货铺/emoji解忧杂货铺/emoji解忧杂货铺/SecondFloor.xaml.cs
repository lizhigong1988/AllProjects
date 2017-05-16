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
using System.Drawing.Printing;
using System.Drawing;
using System.IO;

namespace emoji解忧杂货铺
{
    /// <summary>
    /// SecondFloor.xaml 的交互逻辑
    /// </summary>
    public partial class SecondFloor 
    {
        public SecondFloor()
        {
            InitializeComponent();
        }

        public static RoutedEvent ImageClickEvent =
            EventManager.RegisterRoutedEvent("ImageClick",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SecondFloor));

        public event RoutedEventHandler ImageClick
        {
            add { this.AddHandler(ImageClickEvent, value); }
            remove { this.RemoveHandler(ImageClickEvent, value); }
        }


        private void bdBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ImageClickEvent, Source = (sender as Border).Tag });
        }

        private void bd1_1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_1_Up.png", UriKind.Relative)));
        }

        private void bd1_1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_1.png", UriKind.Relative)));
        }

        private void bd1_2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_2_Up.png", UriKind.Relative)));
        }

        private void bd1_2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_2.png", UriKind.Relative)));
        }

        private void bd1_3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_3_Up.png", UriKind.Relative)));
        }

        private void bd1_3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_3.png", UriKind.Relative)));
        }


        private void bd2_1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_1_Up.png", UriKind.Relative)));
        }

        private void bd2_1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_1.png", UriKind.Relative)));
        }

        private void bd2_2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_2_Up.png", UriKind.Relative)));
        }

        private void bd2_2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_2.png", UriKind.Relative)));
        }

        private void bd2_3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_3_Up.png", UriKind.Relative)));
        }

        private void bd2_3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_3.png", UriKind.Relative)));
        }

        private void bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ImageClickEvent, Source = (sender as Border).Tag });
        }

    }
}

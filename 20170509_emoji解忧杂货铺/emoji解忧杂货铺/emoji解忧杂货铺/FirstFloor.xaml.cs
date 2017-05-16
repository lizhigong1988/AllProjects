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
    /// FirstFloor.xaml 的交互逻辑
    /// </summary>
    public partial class FirstFloor : UserControl
    {
        public FirstFloor()
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


        public static RoutedEvent ImageClickEvent =
            EventManager.RegisterRoutedEvent("ImageClick",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FirstFloor));

        public event RoutedEventHandler ImageClick
        {
            add { this.AddHandler(ImageClickEvent, value); }
            remove { this.RemoveHandler(ImageClickEvent, value); }
        }

        private void bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ImageClickEvent, Source = (sender as Border).Tag });
        }
    }
}

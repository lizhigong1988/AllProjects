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
using CommonLib;
using System.Timers;
using ClientControls;

namespace JXPlatForm.Client.MainPage
{
    /// <summary>
    /// MainToolbar.xaml 的交互逻辑
    /// </summary>
    public partial class MainToolbar : UserControl
    {
        /// <summary>
        /// 定时器
        /// </summary>
        Timer timer = new Timer();

        public static RoutedEvent ButtonCloseClickEvent =
            EventManager.RegisterRoutedEvent("ButtonCloseClick",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MainToolbar));

        public event RoutedEventHandler ButtonCloseClick
        {
            add { this.AddHandler(ButtonCloseClickEvent, value); }
            remove { this.RemoveHandler(ButtonCloseClickEvent, value); }
        }

        public static RoutedEvent ButtonMinClickEvent =
            EventManager.RegisterRoutedEvent("ButtonMinClick",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MainToolbar));

        public event RoutedEventHandler ButtonMinClick
        {
            add { this.AddHandler(ButtonMinClickEvent, value); }
            remove { this.RemoveHandler(ButtonMinClickEvent, value); }
        }

        public static RoutedEvent TopButtonClickEvent =
            EventManager.RegisterRoutedEvent("TopButtonClick",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MainToolbar));

        public event RoutedEventHandler TopButtonClick
        {
            add { this.AddHandler(TopButtonClickEvent, value); }
            remove { this.RemoveHandler(TopButtonClickEvent, value); }
        }

        public MainToolbar()
        {
            InitializeComponent();
            tbTimer.Text = DateTime.Now.ToString();
            timer.Elapsed += new ElapsedEventHandler(TimerRefresh);
            //5分钟
            timer.Interval = 1000;
            timer.Start();
        }

        public void TimerRefresh(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    tbTimer.Text = DateTime.Now.ToString();
                }),
                null);
        }
        public void Init()
        {
            bdLogo.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(
                    ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.MAIN_LOGO),
                    UriKind.Relative))
            };
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ButtonCloseClickEvent, Source = "" });
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ButtonMinClickEvent, Source = "" });
        }

        private void btnCalculator_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("calc.exe");
        }

        private void btnModPsw_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = TopButtonClickEvent, Source = "MOD_PSW" });
        }

        private void btnReLogin_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = TopButtonClickEvent, Source = "RE_LOGIN" });
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            new HelpDocments().Show();
        }

        internal void ShowIndexDoc()
        {
            btnHelp_Click(null, null);
        }
    }
}

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
using ClientCommunication;

namespace ClientControls
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShowMassageBox 
    {
        public enum SHOW_RES
        {
            SELECT_OK,
            SELECT_CLOSE
        };

        public enum SHOW_TYPE
        {
            SHOW_ALERT,
            SHOW_QUEST
        };

        public SHOW_RES select = SHOW_RES.SELECT_CLOSE;

        public ShowMassageBox()
        {
            InitializeComponent();
        }

        public static SHOW_RES JXMassageBox(string massage, SHOW_TYPE type = SHOW_TYPE.SHOW_ALERT, string title = "")
        {
            ShowMassageBox box = new ShowMassageBox();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(LoadingWorker.MainWind) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(box)
                {
                    Owner = winformWindow.Handle
                };
            }
            box.tbTitle.Text = title;
            box.tbContent.Text = massage;
            if (type == SHOW_TYPE.SHOW_ALERT)
            {
                box.AlertMassage();
            }
            else
            {
                box.QuestionMassage();
            }
            box.ShowDialog();
            return box.select;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void AlertMassage() {
            btnYes.Visibility = Visibility.Hidden;
            btnNo.Visibility = Visibility.Hidden;
            btnOk.Focus();
        }

        public void QuestionMassage()
        {
            btnOk.Visibility = Visibility.Hidden;
            btnYes.Focus();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            select = SHOW_RES.SELECT_OK;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bder_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

    }
}

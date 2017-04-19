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
using ClientControls;
using ClientLibrary;
using CommonLib;
using ClientCommunication;

namespace JXPlatForm.Client.Login
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadingWorker.MainWind = this;
            this.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(
                    ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.LOGIN_BAK),
                    UriKind.Relative))
            };
            bdLogo.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(
                    ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.LOGIN_LOGO),
                    UriKind.Relative))
            };
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        } 
        
        //enter键拥有tab键的功能
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // MoveFocus takes a TraveralReqest as its argument.
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                // Gets the element with keyboard focus.
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                // Change keyboard focus. 
                if (elementWithFocus != null)
                {
                    elementWithFocus.MoveFocus(request);
                }
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                ShowMassageBox.JXMassageBox("请输入用户名！");
                return;
            }
            if (pbPassword.Password == "")
            {
                ShowMassageBox.JXMassageBox("请输入用户密码！");
                return;
            }

            CommonDef.COM_RET ret = CommunicationHelper.GetUserInfo(tbUserName.Text, Global.LoginUser);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            if (pbPassword.Password != Global.LoginUser.UserPsw)
            {
                ShowMassageBox.JXMassageBox("密码不正确，请重试！");
                return;
            }
            new MainWindow().Show();
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CommonDef.COM_RET ret = CommunicationHelper.InitCommunication();
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                this.Close();
            }

            int countUser = 0;
            ret = CommunicationHelper.CountUserInfo(ref countUser);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            if (0 == countUser)
            {
                if (ShowMassageBox.JXMassageBox("当前系统无用户，点击确定直接登录！", 
                    ShowMassageBox.SHOW_TYPE.SHOW_QUEST)
                    == ShowMassageBox.SHOW_RES.SELECT_OK)
                {
                    Global.LoginUser.SetDefalt();
                    new MainWindow().Show();
                }
                this.Close();
                return;
            }
        }
    }
}

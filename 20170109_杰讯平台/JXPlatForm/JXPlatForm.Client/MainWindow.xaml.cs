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
using ClientControls;
using ClientCommunication;
using JXPlatForm.Client.Login;
using ClientLibrary;
using System.Data;

namespace JXPlatForm.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Rect rc = SystemParameters.WorkArea;
            this.Width = rc.Width;
            this.Height = rc.Height;

            LoadingWorker.MainWind = this;
            this.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(
                    ClientConfigHeper.ReadConfig(ClientConfigHeper.CONFIG_KEYS.MAIN_BAK),
                    UriKind.Relative))
            };
            mainToolbal.Init();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            mainMenu.MenuInit();

            string roleName = "系统管理员";
            if (Global.LoginUser.RoleId != "0")
            {
                DataTable dtAllRole = new DataTable();
                CommonDef.COM_RET ret = CommunicationHelper.GetAllRoleInfo(ref dtAllRole);
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                    return;
                }
                foreach (DataRow dr in dtAllRole.Rows)
                {
                    if (Global.LoginUser.RoleId == dr[0].ToString())
                    {
                        roleName = dr[1].ToString();
                        break;
                    }
                }
            }
            Global.WorkAreaHeight = mainArea.ActualHeight;
            mainAlert.Text = "登录用户【" + Global.LoginUser.UserName + "】 用户角色【" + roleName + "】";
            //if (Global.LoginUser.UserCode == "")
            //{
            //    mainToolbal.ShowIndexDoc();
            //}
        }

        private void mainToolbal_ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mainToolbal_ButtonMinClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void mainMenu_MenuItemClick(object sender, RoutedEventArgs e)
        {
            mainArea.SetCurrentPage(e.OriginalSource as string);
        }

        private void mainToolbal_TopButtonClick(object sender, RoutedEventArgs e)
        {
            string operation = e.OriginalSource as string;
            switch (operation)
            {
                case "MOD_PSW":
                    mainArea.SetCurrentPage("修改密码");
                    break;
                case "RE_LOGIN":
                    new LoginWindow().Show();
                    this.Close();
                    break;
            }
        }
    }
}

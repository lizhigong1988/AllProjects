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
using ClientCommunication;
using System.Data;
using CommonLib;
using ClientLibrary;

namespace ClientControls.BasePages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PageModUserPsw
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageModUserPsw()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            if ("" == Global.LoginUser.UserCode)
            {
                ShowMassageBox.JXMassageBox("该用户不支持修改密码！");
                return false;
            }
            tbUserCode.Text = Global.LoginUser.UserCode;
            pbUserPsw.Password = "";
            pbNewUserPsw.Password = "";
            pbNewUserPsw2.Password = "";
            return true;
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (pbUserPsw.Password != Global.LoginUser.UserPsw)
            {
                ShowMassageBox.JXMassageBox("登录密码输入错误！");
                return;
            }
            if (pbUserPsw.Password == "")
            {
                ShowMassageBox.JXMassageBox("密码不能为空！");
                return;
            }
            if (pbNewUserPsw.Password != pbNewUserPsw2.Password)
            {
                ShowMassageBox.JXMassageBox("新密码与密码确认不一致！");
                return;
            }
            CommonDef.COM_RET ret = CommunicationHelper.ModUserPsw(tbUserCode.Text, pbNewUserPsw.Password);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }

            ShowMassageBox.JXMassageBox("修改密码成功！");
            RefreshPage();
        }
    }
}

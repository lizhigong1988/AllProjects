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
using WindowLib.Tools;
using System.Data;
using WindowLib.Connect;

namespace WindowLib.Pages
{
    /// <summary>
    /// UserManage.xaml 的交互逻辑
    /// </summary>
    public partial class ModPassword : UserControl
    {
        public ModPassword()
        {
            InitializeComponent();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (pbNewPsw.Password != pbNewPsw2.Password)
            {
                MessageBox.Show("两次密码输入不一致！");
                return;
            }
            DataTable dt = CommunicationHelper.GetUserInfo(GlobalFuns.LoginUser);
            if (dt == null)
            {
                MessageBox.Show("数据查询错误");
                return;
            }
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("没有此用户");
                return;
            }
            DataRow dr = dt.Rows[0];
            if (dr["USER_PSW"].ToString() != pbCurPsw.Password)
            {
                MessageBox.Show("密码不正确");
                return;
            }
            if (!CommunicationHelper.ModPassword(GlobalFuns.LoginUser, pbNewPsw.Password))
            {
                MessageBox.Show("修改密码失败！");
                return;
            }

            MessageBox.Show("修改密码成功！");
        }
    }
}

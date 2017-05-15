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
using System.Data;
using 项目管理.DataBases;
using 项目管理.Tools;

namespace 项目管理
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            if (!DataBaseManager.InitDataBases())
            {
                MessageBox.Show("初始化数据库失败！");
                this.Close();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == GlobalFuns.ADMIN_USER && tbUserPsw.Password == GlobalFuns.ADMIN_PASSWORD)
            {
                new MainWindow().Show();
                this.Close();
                return;
            }
            DataTable dt = DataBaseManager.GetUserInfo(tbUserName.Text);
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
            if (dr["USER_PSW"].ToString() != tbUserPsw.Password)
            {
                MessageBox.Show("密码不正确");
                return;
            }
            GlobalFuns.LoginUser = tbUserName.Text;
            GlobalFuns.LoginSysId = dr["SYS_ID"].ToString();
            GlobalFuns.LoginSysName = dr["SYS_NAME"].ToString();
            GlobalFuns.LoginRole = dr["USER_ROLE"].ToString();
            new MainWindow().Show();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbUserPsw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(null, null);
            }
        }
    }
}

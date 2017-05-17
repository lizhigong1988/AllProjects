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
using 项目管理.Tools;
using 项目管理.Connect;
using System.IO;

namespace 项目管理
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        static string IP_CONFIG_FILE = "CONFIG_IP_ADDR";
        public LoginWindow()
        {
            InitializeComponent();
            if (File.Exists(IP_CONFIG_FILE))
            {
                tbIPAddr.Text = File.ReadAllText(IP_CONFIG_FILE);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.AppConnectInit(tbIPAddr.Text))
            {
                MessageBox.Show("网络连接失败！");
                return;
            }
            File.WriteAllText(IP_CONFIG_FILE, tbIPAddr.Text);

            if (tbUserName.Text == GlobalFuns.ADMIN_USER && tbUserPsw.Password == GlobalFuns.ADMIN_PASSWORD)
            {
                new MainWindow().Show();
                this.Close();
                return;
            }
            dt = CommunicationHelper.GetUserInfo(tbUserName.Text);
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
            if (dt.Rows.Count == 1)
            {
                GlobalFuns.LoginUser = tbUserName.Text;
                GlobalFuns.LoginSysId = dr["SYS_ID"].ToString();
                GlobalFuns.LoginSysName = dr["SYS_NAME"].ToString();
                GlobalFuns.LoginRole = dr["USER_ROLE"].ToString();
                new MainWindow().Show();
                this.Close();
            }
            else
            {
                Dictionary<string, string> dicSys = CommunicationHelper.GetAllSysDic();
                Dictionary<string, string> manageDic = new Dictionary<string, string>();
                foreach (DataRow dic in dt.Rows)
                {
                    string sysId = dic["SYS_ID"].ToString();
                    if (sysId == "")
                    {
                        continue;
                    }
                    if (dicSys.ContainsKey(sysId))
                    {
                        manageDic.Add(sysId, dicSys[sysId]);
                    }
                }
                cbSelectSys.ItemsSource = manageDic;
                cbSelectSys.SelectedValuePath = "Key";
                cbSelectSys.DisplayMemberPath = "Value";
                cbSelectSys.SelectedIndex = 0;
                spLogin.Visibility = Visibility.Collapsed;
                spSelectSys.Visibility = Visibility.Visible;
            }
        }

        DataTable dt;
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

        private void btnSelectSys_Click(object sender, RoutedEventArgs e)
        {
            GlobalFuns.LoginUser = tbUserName.Text;
            GlobalFuns.LoginSysId = cbSelectSys.SelectedValue.ToString();
            GlobalFuns.LoginSysName = cbSelectSys.Text;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SYS_ID"].ToString() == GlobalFuns.LoginSysId)
                {
                    GlobalFuns.LoginRole = dr["USER_ROLE"].ToString();
                    break;
                }
            }
            new MainWindow().Show();
            this.Close();
        }
    }
}

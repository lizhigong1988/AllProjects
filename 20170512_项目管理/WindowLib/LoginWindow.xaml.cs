﻿using System;
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
using WindowLib.Tools;
using WindowLib.Connect;
using System.IO;

namespace WindowLib
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        static string IP_CONFIG_FILE = "CONFIG_IP_ADDR";
        string curIpAddr = "";
        public LoginWindow()
        {
            InitializeComponent();
            if (File.Exists(IP_CONFIG_FILE))
            {
                string[] fileInfo = File.ReadAllText(IP_CONFIG_FILE).Split('\n');
                curIpAddr = fileInfo[0];
                if (fileInfo.Length > 1)
                {
                    tbUserName.Text = fileInfo[1];
                }
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.AppConnectInit(curIpAddr))
            {
                MessageBox.Show("网络连接失败！");
                return;
            }

            if (tbUserName.Text == GlobalFuns.ADMIN_USER && tbUserPsw.Password == GlobalFuns.ADMIN_PASSWORD)
            {
                GlobalFuns.DtLoginUserSysInfo = new DataTable();
                new MainWindow().Show();
                this.Close();
                File.WriteAllText(IP_CONFIG_FILE, curIpAddr + "\n" + tbUserName.Text);
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
                MessageBox.Show("密码错误");
                return;
            }

            File.WriteAllText(IP_CONFIG_FILE, curIpAddr + "\n" + tbUserName.Text);
            GlobalFuns.LoginUser = tbUserName.Text;
            GlobalFuns.LoginRole = dr["USER_ROLE"].ToString();

            GlobalFuns.DtLoginUserSysInfo = CommunicationHelper.GetUserSysInfo(tbUserName.Text);

            if (GlobalFuns.DtLoginUserSysInfo == null)
            {
                MessageBox.Show("数据查询错误");
                return;
            }
            if (GlobalFuns.LoginRole == "部门领导" || GlobalFuns.LoginRole == "PMO")
            {
                GlobalFuns.LoginSysId = "";
                GlobalFuns.LoginSysName = "";
                new MainWindow().Show();
                this.Close();
                return;
            }
            if (GlobalFuns.DtLoginUserSysInfo.Rows.Count == 0)
            {
                MessageBox.Show("请联系管理员添加所属系统");
                return;
            }
            if (GlobalFuns.DtLoginUserSysInfo.Rows.Count >= 1)
            {
                GlobalFuns.LoginSysId = GlobalFuns.DtLoginUserSysInfo.Rows[0]["SYS_ID"].ToString();
                GlobalFuns.LoginSysName = GlobalFuns.DtLoginUserSysInfo.Rows[0]["SYS_NAME"].ToString();
                new MainWindow().Show();
                this.Close();
                return;
            }
            //if (dtUserSys.Rows.Count > 1)
            //{
            //    Dictionary<string, string> manageDic = new Dictionary<string, string>();
            //    foreach (DataRow dic in dtUserSys.Rows)
            //    {
            //        string sysId = dic["SYS_ID"].ToString();
            //        if (sysId == "")
            //        {
            //            continue;
            //        }
            //        if (!manageDic.ContainsKey(sysId))
            //        {
            //            manageDic.Add(sysId, dic["SYS_NAME"].ToString());
            //        }
            //    }
            //    cbSelectSys.ItemsSource = manageDic;
            //    cbSelectSys.SelectedValuePath = "Key";
            //    cbSelectSys.DisplayMemberPath = "Value";
            //    cbSelectSys.SelectedIndex = 0;
            //    spLogin.Visibility = Visibility.Collapsed;
            //    spSelectSys.Visibility = Visibility.Visible;
            //}
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

        //private void btnSelectSys_Click(object sender, RoutedEventArgs e)
        //{
        //    GlobalFuns.LoginSysId = cbSelectSys.SelectedValue.ToString();
        //    GlobalFuns.LoginSysName = cbSelectSys.Text;
        //    new MainWindow().Show();
        //    this.Close();
        //}
    }
}

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
using System.Data;
using WindowLib.Tools;
using WindowLib.Connect;
using System.Net.Mail;
using CommonLib;

namespace WindowLib.Pages
{
    /// <summary>
    /// UserManage.xaml 的交互逻辑
    /// </summary>
    public partial class SysConfig : UserControl
    {
        public SysConfig()
        {
            InitializeComponent();
            if (Refresh())
            {
                GlobalFuns.OpenFlag = true;
            }
        }

        private bool Refresh()
        {
            DataTable dt = CommunicationHelper.GetSysConfig();
            if (dt == null)
            {
                MessageBox.Show("获取配置信息失败");
                return false;
            }
            tbSenderName.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_NAME) ;
            tbSenderEmail.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_ADDR);
            pbSenderPsw.Password = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_PASSWORD);
            tbSenderServer.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_HOST);
            tbSendPMTime.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_PM_TIME);
            tbSendAllTime.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_ALL_TIME);
            tbLastSendPMDate.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_PM);
            tbLastSendAllDate.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_ALL);
            cbAutoSend.IsChecked = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_FLAG) == "1";
            return true;
        }

        private string GetConfig(DataTable dt, CommonDef.CONFIG_KEYS key)
        {
            string keyIndex = ((int)key).ToString();
            foreach (DataRow dr in dt.Rows)
            {
                if (keyIndex == dr["KEY"].ToString())
                { 
                    return dr["VALUE"].ToString();
                }
            }
            return "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.SaveSysConfig(tbSenderName.Text, tbSenderEmail.Text,
                pbSenderPsw.Password, tbSenderServer.Text, tbLastSendPMDate.Text, tbLastSendAllDate.Text,
                tbSendPMTime.Text, tbSendAllTime.Text, (bool)cbAutoSend.IsChecked ? "1":"0"))
            {
                MessageBox.Show("保存失败");
                return;
            }
            MessageBox.Show("保存成功");
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.TestEmail(tbSenderName.Text, tbSenderEmail.Text,
                pbSenderPsw.Password, tbSenderServer.Text))
            {
                MessageBox.Show("测试失败");
                return;
            }
            MessageBox.Show("测试成功，请查收邮件！");
        }

    }
}

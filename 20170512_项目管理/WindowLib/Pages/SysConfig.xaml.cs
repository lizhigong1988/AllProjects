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
            Refresh();
        }

        private void Refresh()
        {
            DataTable dt = CommunicationHelper.GetSysConfig();
            if (dt == null)
            {
                MessageBox.Show("获取配置信息失败");
                return;
            }
            tbSenderName.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_NAME) ;
            tbSenderEmail.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_ADDR);
            pbSenderPsw.Password = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_PASSWORD);
            tbSenderServer.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_HOST);
            tbSendPMTime.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_PM_TIME);
            tbSendAllTime.Text = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_ALL_TIME);
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
                pbSenderPsw.Password, tbSenderServer.Text,
                tbSendPMTime.Text, tbSendAllTime.Text))
            {
                MessageBox.Show("保存失败");
                return;
            }
            MessageBox.Show("保存成功");
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(tbTestRcv.Text);//收件人邮箱   
            msg.From = new MailAddress(tbSenderEmail.Text, tbSenderName.Text, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "[测试邮件]";//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = "TEST";//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = false;//是否是HTML邮件    
            //msg.Priority = MailPriority.High;//邮件优先级    
            msg.CC.Add(tbSenderEmail.Text);//李志功<lizhigong@hkcts.com>
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(tbSenderEmail.Text, pbSenderPsw.Password);
            client.Host = tbSenderServer.Text;//smtp.hkcts.com
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                //简单一点儿可以client.Send(msg);    
                tbTestAlert.Text = "发送成功";
                MessageBox.Show("发送成功");
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                tbTestAlert.Text = "发送邮件出错";
                MessageBox.Show("发送邮件出错");
            }
        }

    }
}

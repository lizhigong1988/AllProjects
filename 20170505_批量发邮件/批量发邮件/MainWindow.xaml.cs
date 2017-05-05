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
using System.Windows.Threading;
using System.Net.Mail;

namespace 批量发邮件
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog imageFileDialog = new System.Windows.Forms.OpenFileDialog();
            imageFileDialog.Multiselect = true;
            imageFileDialog.Title = "请选择导入文件";
            imageFileDialog.Multiselect = false;
            imageFileDialog.Filter = "电子表格文件(*.xls,*.xlsx)|*.xls;*.xlsx";
            if (imageFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFile.Text = imageFileDialog.FileName;
                DataTable dt = ExcelOperation.ImportSimpleExcel(tbFile.Text);
                databox.ItemsSource = dt.DefaultView;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = (databox.ItemsSource as DataView).Table;
            foreach (DataRow dr in dt.Rows)
            {
                //DoEvents();
                if (dr[16].ToString() == "邮箱"|| dr[16].ToString() == "")
                    continue;
                if (!IsEmail(dr[16].ToString()))
                {
                    if (MessageBox.Show(dr[16].ToString()+"邮箱格式不正确，发送失败！\n是否继续？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                { 
                    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                    msg.To.Add(dr[16].ToString());//收件人邮箱   
                    msg.From = new MailAddress("yudandan@hkcts.com", "于丹丹", System.Text.Encoding.UTF8);    
                        /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/    
                    msg.Subject = "2017年4月小微条线工资明细";//邮件标题    
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
                    msg.Body = "经营单位：" + dr[0].ToString()
                        + "\n岗位：" + dr[1].ToString()
                        + "\n客户经理："+dr[2].ToString()
                        + "\n工号：" + dr[3].ToString() 
                        + "\n折算后考核贷款发放量：" + dr[4].ToString()
                        + "\n贷款发放量绩效（元）：6元/万元：" + dr[5].ToString()
                        + "\nFTP利润：" + dr[6].ToString()
                        + "\nFTP利润绩效（元）：200元/万元：" + dr[7].ToString() 
                        + "\n中间业务收入："+dr[8].ToString()
                        + "\n中间业务收入绩效（元）：500元/万元：" + dr[9].ToString() 
                        + "\n存量日均存款："+dr[10].ToString()
                        + "\n存量日均存款绩效（元）：6元/万元：：" + dr[11].ToString()
                        + "\n增量日均存款：" + dr[12].ToString()
                        + "\n增量日均存款绩效（元）：12元/万元：：" + dr[13].ToString()
                        + "\n虚拟行员绩效二次分配：" + dr[14].ToString()
                        + "\n绩效工资合计（元）：日均存款绩效按1/4计入：" + dr[15].ToString()
                        + "\n本月实发工资=绩效合计金额*80% -  一月、二月预发绩效"
                        + "\n根据《商业银行稳健薪酬监管指引》要求，对全行小微客户经理的绩效薪酬实行延期支付制度，延期支付方式为绩效工资当年季度考核发放60%，其余40%采取延期支付方式，递延期限为3年,递延比例分别为30%、40%、30%。若当年如出现资产质量风险的，则相应调高递延发放比例，具体根据相关制度执行。";//邮件内容    
                    msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
                    msg.IsBodyHtml = false;//是否是HTML邮件    
                    //msg.Priority = MailPriority.High;//邮件优先级    
                    msg.CC.Add("yudandan@hkcts.com");
                    SmtpClient client = new SmtpClient();
                    client.Credentials = new System.Net.NetworkCredential("yudandan@hkcts.com", "DANdan521");
                    client.Host = "smtp.hkcts.com";    
                    object userState = msg;    
                    try    
                    {    
                        client.SendAsync(msg, userState);    
                        //简单一点儿可以client.Send(msg);    
                        MessageBox.Show(dr[2].ToString() + "发送成功"); 
                    }    
                    catch (System.Net.Mail.SmtpException ex)    
                    {    
                        MessageBox.Show(ex.Message, "发送邮件出错");    
                    }    
                }
            }
         
        }

        //验证是否一致
        private bool IsEmail(string vEmail)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(vEmail,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        //public void DoEvents()
        //{
        //    DispatcherFrame frame = new DispatcherFrame();
        //    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
        //        new DispatcherOperationCallback(delegate(object f)
        //        {
        //            ((DispatcherFrame)f).Continue = false;

        //            return null;
        //        }
        //            ), frame);
        //    Dispatcher.PushFrame(frame);
        //} 
    }
}

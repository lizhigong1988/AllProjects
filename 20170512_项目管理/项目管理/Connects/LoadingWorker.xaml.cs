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
using System.Threading;

namespace 项目管理.Connect
{
    /// <summary>
    /// LoadingWorker.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWorker
    {
        public static Window MainWind;

        public LoadingWorker()
        {
            InitializeComponent();
        }

        public string MsgData  = "";
        public string RecvData = "";
        public bool ret = false;

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Thread newThread = new Thread(SendData);
            newThread.Start();
        }

        private void SendData()
        {
            if (MsgData != "")
            {
                ret = SendAndRecv();
            }
            this.Dispatcher.BeginInvoke(new Action(() => 
            {
                this.Close();
            }), null);
        }


        private bool SendAndRecv()
        {
            return CommunicationHelper.SendAndRcv(MsgData, out RecvData);
        }
    }
}

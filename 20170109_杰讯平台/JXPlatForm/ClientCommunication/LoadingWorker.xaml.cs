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
using ServerImage;
using System.Threading;

namespace ClientCommunication
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Thread newThread = new Thread(SendData);
            newThread.Start();
        }

        private void SendData()
        {
            if (MsgData != "")
            {
                if (CommonDef.CLIENT_TYPE == "Single")
                {
                    ServerImageHelper.ProcessFile(ref MsgData);
                }
                else
                {

                }
            }
            this.Dispatcher.BeginInvoke(new Action(() => 
            {
                this.Close();
            }), null);
        }
    }
}

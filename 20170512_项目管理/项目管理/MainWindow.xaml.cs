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
using System.IO;
using 项目管理.Connect;
using System.Data;

namespace 项目管理
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string curUser = "";

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(IP_CONFIG_FILE))
            {
                string[] fileInfo = File.ReadAllText(IP_CONFIG_FILE).Split('\n');
                tbIPAddr.Text = fileInfo[0];
                if (fileInfo.Length > 1)
                {
                    curUser = fileInfo[1];
                }
            }
        }
        static string IP_CONFIG_FILE = "CONFIG_IP_ADDR";

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!CommunicationHelper.AppConnectInit(tbIPAddr.Text))
            {
                MessageBox.Show("网络连接失败！");
                return;
            }

            DataTable dtProgramFiles = CommunicationHelper.GetProgramFiles();
            if (dtProgramFiles != null)
            {
                foreach (DataRow dr in dtProgramFiles.Rows)
                {
                    string file = dr["FILE_NAME"].ToString();
                    if (File.Exists(file))
                    {
                        string time = File.GetLastWriteTime(file).ToString("yyyyMMddHHmmss");
                        if (time.CompareTo(dr["FILE_DATE"].ToString()) < 0)//本地较早
                        {
                            if (!CommunicationHelper.DownloadFile("program/" + file, file))
                            {
                                MessageBox.Show("更新文件失败！");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (!CommunicationHelper.DownloadFile("program/" + file, file))
                        {
                            MessageBox.Show("更新文件失败！");
                            return;
                        }
                    }
                }
            }
            File.WriteAllText(IP_CONFIG_FILE, tbIPAddr.Text + "\n" + curUser);

            //string dllFile = System.Environment.CurrentDirectory + "\\WindowLib.dll";
            //string className = "WindowLib.LoginWindow";
            //System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom(dllFile);
            //Type tp = ass.GetType(className);
            //Object obj = Activator.CreateInstance(tp);
            //Window wind = obj as Window;
            //wind.Show();

            new WindowLib.LoginWindow().Show();

            CommunicationHelper.CloseConnect();
            this.Close();
        }
    }
}

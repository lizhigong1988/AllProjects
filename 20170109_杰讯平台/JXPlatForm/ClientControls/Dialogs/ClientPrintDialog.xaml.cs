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
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Xps;
using System.Windows.Interop;
using ClientCommunication;

namespace ClientControls.Dialogs
{
    /// <summary>
    /// PrintDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ClientPrintDialog : Window
    {
        List<UserControl> Prints;

        public ClientPrintDialog()
        {
            InitializeComponent();
        }

        private double width = 0;
        private double height = 0;
        /// <summary>
        /// 设置打印尺寸cm
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetPaperSize(double _width, double _height)
        {
            width = _width;
            height = _height;
        }

        public static void ShowPrintPreView(List<UserControl> print, double width, double height)
        {
            ClientPrintDialog dialog = new ClientPrintDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(LoadingWorker.MainWind) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(dialog)
                {
                    Owner = winformWindow.Handle
                };
            }
            dialog.Prints = print;
            dialog.SetPaperSize(width, height);
            dialog.ShowDialog();

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (Prints == null)
            {
                return;
            }
            string tempPath = "CommonPrint";
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            //FixedDocument设置页大小
            FixedDocument fd = new FixedDocument();
            fd.DocumentPaginator.PageSize = new Size(96 * width, 96 * height);

            foreach (UserControl print in Prints)
            {
                //将读取的文件转换成页
                FixedPage fp = new FixedPage();
                fp.Children.Add(print);

                //将页加载到PageContent中
                PageContent pc = new PageContent();
                pc.Child = fp;
                fd.Pages.Add(pc);
            }
            //将XAML保存成XPS文件
            XpsDocument doc = new XpsDocument(tempPath, FileAccess.Write);
            XpsDocumentWriter docWriter = XpsDocument.CreateXpsDocumentWriter(doc);
            docWriter.Write(fd);
            doc.Close();
            XpsDocument xpsdoc = new XpsDocument(tempPath, FileAccess.Read);
            docViewer.Document = xpsdoc.GetFixedDocumentSequence() as IDocumentPaginatorSource;
            xpsdoc.Close();
        }
    }
}

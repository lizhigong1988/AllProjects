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
using System.Drawing;
using System.IO;
using System.Drawing.Printing;

namespace emoji解忧杂货铺
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

        private void firstFloor_ImageClick(object sender, RoutedEventArgs e)
        {
            string type = e.OriginalSource.ToString();
            switch (type)
            { 
                case "感情":
                    secondFloor.cvMain.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorBackgroundGQ.png", UriKind.Relative)));
                    break;
                case "工作":
                    secondFloor.cvMain.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorBackgroundGZ.png", UriKind.Relative)));
                    break;
                case "学习":
                    secondFloor.cvMain.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorBackgroundXX.png", UriKind.Relative)));
                    break;
                case "生活":
                    secondFloor.cvMain.Background = new ImageBrush(new BitmapImage(new Uri("Images/SecondFloorBackgroundSH.png", UriKind.Relative)));
                    break;
            }
            secondFloor.Visibility = Visibility.Visible;
            firstFloor.Visibility = Visibility.Collapsed;
        }

        private void secondFloor_ImageClick(object sender, RoutedEventArgs e)
        {
            string type = e.OriginalSource.ToString();
            if (type == "返回")
            {
                secondFloor.Visibility = Visibility.Collapsed;
                firstFloor.Visibility = Visibility.Visible;
            }
            else
            {
                secondFloor.Visibility = Visibility.Collapsed;
                mediaElement.Visibility = Visibility.Visible;
                mediaElement.Source = new Uri("Videos/PreviewPrint.mp4", UriKind.Relative);
                mediaElement.Play();
                curPlayVideo = 2;
                playing = true;
            }
        }

        /// <summary>
        /// 标记当前播放的视频 0 1 2
        /// </summary>
        int curPlayVideo = 0;
        /// <summary>
        /// 播放标识
        /// </summary>
        bool playing = false;

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            mediaElement.Source = new Uri("Videos/FirstVideo.mp4", UriKind.Relative);
            mediaElement.Play();
            curPlayVideo = 0;
            playing = true;
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (curPlayVideo)
            {
                case 0:
                    mediaElement.Position = new TimeSpan(0);
                    mediaElement.Play();
                    break;
                case 1:
                    mediaElement.Visibility = Visibility.Collapsed;
                    firstFloor.Visibility = Visibility.Visible;
                    playing = false;
                    break;
                case 2:
                    Print();
                    mediaElement.Visibility = Visibility.Collapsed;
                    firstFloor.Visibility = Visibility.Visible;
                    mediaElement.Source = new Uri("Videos/FirstVideo.mp4", UriKind.Relative);
                    mediaElement.Play();
                    curPlayVideo = 0;
                    playing = true;
                    break;
            }
        }

        private void mediaElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (playing)
            {
                if (curPlayVideo == 0)
                {
                    mediaElement.Source = new Uri("Videos/MoveMouse.mp4", UriKind.Relative);
                    mediaElement.Play();
                    curPlayVideo = 1;
                }
            }
        }


        private enum DrawKinds
        {
            DRAW_KINDS_STRING,
            DRAW_KINDS_LINE,
        }
        private class DrawElement
        {
            public string drawText { get; set; }
            public DrawKinds drawKind { get; set; }
            public Font font { get; set; }
            public RectangleF rect { get; set; }
            public StringFormat sf { get; set; }
        };
        List<List<DrawElement>> drawPageList = new List<List<DrawElement>>();

        float _width = 5.6F * 39.36F;
        float _height = 9F * 39.36F;
        float _left = 0.3F * 39.36F;
        float _top = 0.3F * 39.36F;
        float _right = 0.3F * 39.36F;
        float _buttom = 0.3F * 39.36F;
        private void Print()
        {
            if (File.Exists("SizeConfig.txt"))
            {
                string[] configs = File.ReadAllText("SizeConfig.txt").Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if (configs.Length > 5)
                {
                    if (configs[0].Contains(':'))
                    {
                        if (!float.TryParse(configs[0].Split(':')[1], out _width))
                        {
                            MessageBox.Show("配置文件损坏！");
                        }
                        else
                        {
                            _width *= 39.36F;
                        }
                    }
                    else
                    {
                        MessageBox.Show("配置文件损坏！");
                    }
                    if (configs[1].Contains(':'))
                    {
                        if (!float.TryParse(configs[1].Split(':')[1], out _height))
                        {
                            MessageBox.Show("配置文件损坏！");
                        }
                        else
                        {
                            _height *= 39.36F;
                        }
                    }
                    else
                    {
                        MessageBox.Show("配置文件损坏！");
                    }
                    if (configs[2].Contains(':'))
                    {
                        if (!float.TryParse(configs[2].Split(':')[1], out _left))
                        {
                            MessageBox.Show("配置文件损坏！");
                        }
                        else
                        {
                            _left *= 39.36F;
                        }
                    }
                    else
                    {
                        MessageBox.Show("配置文件损坏！");
                    }
                    if (configs[3].Contains(':'))
                    {
                        if (!float.TryParse(configs[3].Split(':')[1], out _top))
                        {
                            MessageBox.Show("配置文件损坏！");
                        }
                        else
                        {
                            _top *= 39.36F;
                        }
                    }
                    else
                    {
                        MessageBox.Show("配置文件损坏！");
                    }
                    if (configs[4].Contains(':'))
                    {
                        if (!float.TryParse(configs[4].Split(':')[1], out _right))
                        {
                            MessageBox.Show("配置文件损坏！");
                        }
                        else
                        {
                            _right *= 39.36F;
                        }
                    }
                    else
                    {
                        MessageBox.Show("配置文件损坏！");
                    }
                    if (configs[5].Contains(':'))
                    {
                        if (!float.TryParse(configs[5].Split(':')[1], out _buttom))
                        {
                            MessageBox.Show("配置文件损坏！");
                        }
                        else
                        {
                            _buttom *= 39.36F;
                        }
                    }
                    else
                    {
                        MessageBox.Show("配置文件损坏！");
                    }
                }
            }
            PrintDocument printDoc = new PrintDocument();
            //设置打印用的纸张 当设置为Custom的时候，可以自定义纸张的大小，还可以选择A4,A5等常用纸型
            //printDoc.DefaultPageSettings.PaperSize = new PaperSize("Custum", 500, 300);
            PaperSize paperSize = new PaperSize("Custum", (int)_width, (int)_height);
            printDoc.DefaultPageSettings.PaperSize = paperSize;
            printDoc.DocumentName = "打印报表";
            drawPageList.Clear();
            bool isHLandscape = false;
            printDoc.DefaultPageSettings.Landscape = isHLandscape;//true横向 false纵向
            printDoc.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
            printDoc.Print();

            //System.Windows.Forms.PrintPreviewDialog printPreDlg = new System.Windows.Forms.PrintPreviewDialog();
            ////将写好的格式给打印预览控件以便预览
            //printPreDlg.Document = printDoc;
            //(printPreDlg as System.Windows.Forms.Form).WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //printPreDlg.PrintPreviewControl.Zoom = 1.0;
            ////显示打印预览
            //printPreDlg.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string[] files = Directory.GetFiles(System.Environment.CurrentDirectory + "\\PrintLibs");
            int index = new Random().Next(0, files.Length);
            System.Drawing.Image printImage = System.Drawing.Image.FromFile(files[index]);
            e.Graphics.DrawImage(printImage, _left, _top, _width - _left - _right, _height - _top - _buttom);
            e.HasMorePages = false;
        }
    }
}

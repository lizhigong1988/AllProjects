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
using System.Drawing.Printing;
using System.Drawing;
using System.IO;

namespace emoji解忧杂货铺
{
    /// <summary>
    /// SecondFloor.xaml 的交互逻辑
    /// </summary>
    public partial class SecondFloor : Window
    {
        public SecondFloor()
        {
            InitializeComponent();
        }

        private void bdBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void bd1_1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_1_Up.png", UriKind.Relative)));
        }

        private void bd1_1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_1.png", UriKind.Relative)));
        }

        private void bd1_2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_2_Up.png", UriKind.Relative)));
        }

        private void bd1_2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_2.png", UriKind.Relative)));
        }

        private void bd1_3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd1_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_3_Up.png", UriKind.Relative)));
        }

        private void bd1_3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd1_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton1_3.png", UriKind.Relative)));
        }


        private void bd2_1_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_1_Up.png", UriKind.Relative)));
        }

        private void bd2_1_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_1.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_1.png", UriKind.Relative)));
        }

        private void bd2_2_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_2_Up.png", UriKind.Relative)));
        }

        private void bd2_2_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_2.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_2.png", UriKind.Relative)));
        }

        private void bd2_3_MouseEnter(object sender, MouseEventArgs e)
        {
            bd2_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_3_Up.png", UriKind.Relative)));
        }

        private void bd2_3_MouseLeave(object sender, MouseEventArgs e)
        {
            bd2_3.Background = new ImageBrush(new BitmapImage(
                new Uri("Images/SecondFloorButton2_3.png", UriKind.Relative)));
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
        private void bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists("SizeConfig.txt"))
            {
                string[] configs = File.ReadAllText("SizeConfig.txt").Split(new string[]{"\r\n"}, StringSplitOptions.None);
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

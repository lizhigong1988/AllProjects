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
using WPFMediaKit.DirectShow.Controls;
using System.IO;

namespace TakePhotos
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MultimediaUtil.VideoInputNames.Length > 0)
            {
                int index = 0;
                vce.VideoCaptureSource = MultimediaUtil.VideoInputNames[index];
            }
            else
            {
                MessageBox.Show("未检测到任何可用摄像头!");
            }
        }

        private string fileName = "";
        //拍照并保存图片
        private void btnCap_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)vce.ActualWidth, (int)vce.ActualHeight, 96, 96, PixelFormats.Default);
            bmp.Render(vce);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                byte[] captureData = ms.ToArray();
                fileName = "photos\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                File.WriteAllBytes(fileName, captureData);
            }
            string path = System.AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (System.IO.File.Exists(path))
            {
                img.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                MessageBox.Show("照片保存在Debug目录下photos文件夹内。");
                img.Source = null;
            }
        }



    }
}

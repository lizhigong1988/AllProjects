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
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Threading;
using System.Threading;

namespace 图片识别
{
    /// <summary>
    /// PageDistinguish.xaml 的交互逻辑
    /// </summary>
    public partial class PageDistinguish : System.Windows.Controls.UserControl
    {
        public PageDistinguish()
        {
            InitializeComponent();
        }

        private void lbBigImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectPath == "")
            {
                return;
            }
            string select = lbBigImages.SelectedItem.ToString();
            string file = selectPath + "\\" + select;
            image.Source = new BitmapImage(new Uri(file));
        }

        private string selectPath = "";

        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string[] programFiles = Directory.GetFiles(dialog.SelectedPath);
            selectPath = dialog.SelectedPath;
            List<string> fileNames = new List<string>();
            foreach (string file in programFiles)
            {
                if (!file.ToUpper().EndsWith(".PNG") &&
                   !file.ToUpper().EndsWith(".JPG"))
                {
                    continue;
                }
                string imageName = System.IO.Path.GetFileName(file);
                fileNames.Add(imageName);
            }
            lbBigImages.ItemsSource = fileNames;
        }

        private void btnDelImage_Click(object sender, RoutedEventArgs e)
        {
            if (lbBigImages.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("请选择要删除的图片");
                return;
            }
            lbBigImages.Items.Remove(lbBigImages.SelectedItem);
        }

        private void btnDistinguish_Click(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                System.Windows.MessageBox.Show("正在识别，请稍等");
                return;
            }
            new Thread(DisTinguishThread).Start();

            //DisTinguishThreadDebug();
        }

        bool running = false;
        int middle = 0;
        private void DisTinguishThread()
        {
            running = true;
            DataTable dtSource = DataBases.DataBaseManager.GetImages();
            if (dtSource == null)
            {
                System.Windows.MessageBox.Show("获取原图信息失败");
                return;
            }
            Dictionary<string, byte[]> dicSource = new Dictionary<string, byte[]>();
            List<byte> listFirstByte = new List<byte>();
            foreach (DataRow dr in dtSource.Rows)
            {
                dicSource.Add(dr["IMAGE_NAME"].ToString(), Convert.FromBase64String(dr["IMAGE_TAG"].ToString()));
                listFirstByte.Add(dicSource[dr["IMAGE_NAME"].ToString()][0]);
            }
            string message = "";
            List<Rect> listIgnore = new List<Rect>();
            middle = CommonDef.IMAGE_DEFAULT_SIZE / CommonDef.IMAGE_TAG_LENGTH;
            foreach (var item in lbBigImages.Items)
            {
                listIgnore.Clear();
                string filePath = item.ToString();
                message += filePath + ":";
                Bitmap bp = new Bitmap(selectPath + "\\" + filePath);
                int width = bp.Width;
                int height = bp.Height;
                for (int j = 0; j < height - CommonDef.IMAGE_DEFAULT_SIZE; j++)
                {
                    for (int i = 0; i < width - CommonDef.IMAGE_DEFAULT_SIZE; i++)
                    {//遍历大图的每一个像素点
                        foreach (Rect rect in listIgnore)
                        {//跳过已识别的区域
                            if (i >= rect.X && i <= rect.X + rect.Width &&
                                j >= rect.Y && j <= rect.Y + rect.Height)
                            {
                                i = (int)rect.X + (int)rect.Width;
                                if (i >= width - CommonDef.IMAGE_DEFAULT_SIZE)
                                {
                                    i = 0;
                                    j = (int)rect.Y + (int)rect.Height;
                                }
                            }
                        }
                        if (j >= height - CommonDef.IMAGE_DEFAULT_SIZE)
                        {
                            break;
                        }
                        System.Drawing.Color c = bp.GetPixel(i, j);
                        foreach(byte b in listFirstByte)
                        {
                            if (ColorCompare(b, c.R))
                            {
                                if (!running)
                                {
                                    return;
                                }
                                if (ImageCompare(bp, i, j, dicSource, ref message, filePath))
                                {
                                    listIgnore.Add(new Rect()
                                    {   X = i,
                                        Y = j,
                                        Width = CommonDef.IMAGE_DEFAULT_SIZE,
                                        Height = CommonDef.IMAGE_DEFAULT_SIZE });
                                    break;
                                }
                            }
                        }

                    }
                }

                message += "\r\n";

                tbResault.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        tbResault.Text = message;
                    }));
            }
            running = false;
        }

        private bool ImageCompare(Bitmap bp, int x, int y, Dictionary<string, byte[]> dicSource,
            ref string message, string filePath)
        {
            bool has = false;
            foreach (var dic in dicSource)
            {
                has = true;
                for (int i = 0; i < CommonDef.IMAGE_TAG_LENGTH; i++)
                {
                    for (int j = 0; j < CommonDef.IMAGE_TAG_LENGTH; j++)
                    {
                        if (!running)
                        {
                            return false;
                        }
                        System.Drawing.Color col = bp.GetPixel(x + i * middle, y + j * middle);
                        if (!ColorCompare(col.R, dic.Value[(i * CommonDef.IMAGE_TAG_LENGTH + j) * 3]))
                        {
                            has = false;
                            break;
                        }
                        if (!ColorCompare(col.G, dic.Value[(i * CommonDef.IMAGE_TAG_LENGTH + j) * 3 + 1]))
                        {
                            has = false;
                            break;
                        }
                        if (!ColorCompare(col.B, dic.Value[(i * CommonDef.IMAGE_TAG_LENGTH + j) * 3 + 2]))
                        {
                            has = false;
                            break;
                        }
                    }
                    if (!has)
                    {//不是此图
                        break;
                    }
                }
                if (has)
                { //匹配成功
                    for (int k = 0; k < 5; k++)
                    {
                        for (int l = 0; l < 5; l++)
                        {
                            bp.SetPixel(x + k, y + l, System.Drawing.Color.White);
                        }
                    }
                    bp.Save(filePath +  "_" + x.ToString() + "_" + y.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    message += dic.Key + ",";
                    return true; ;
                }
            }
            return false;
        }

        private bool ColorCompare(byte a, byte b)
        {
            int ab = a - b;
            if (ab < 0)
            {
                ab = ab * -1;
            }
            if (ab > CommonDef.COLOR_TOLERANCE)
            {
                return false;
            }
            return true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            running = false;
        }
    }
}
